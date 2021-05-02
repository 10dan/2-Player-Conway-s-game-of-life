using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BoardBehavior : MonoBehaviour {
    //Links to unity objects.
    [SerializeField] GameObject cell = null; //Cell prefab.
    [SerializeField] GameObject loadingScreen = null;
    [SerializeField] GameObject winnerText = null;
    [SerializeField] GameObject[] winFXs = new GameObject[4];

    //Structure that will store cells.
    public static Cell[,] cells;

    //Initiate AI.
    AIScript ai;

    void Start() {
        InitVariables();
        PlaceCells();
        LoadCellsFromSettings();
    }

    private void InitVariables() {
        ai = new AIScript();
    }

    private void Update() {
        //Check if player places a cell
        CheckForInputs();
        //If they change the board size, update visual display to match new size.
        if (SettingsHolder.boardSizeChanged) PlaceCells();
        if (SettingsHolder.gameState == SettingsHolder.GameStates.Planning) {
            CheckForCellSelection();
        }
        CheckLoading();
    }

    IEnumerator ExecuteSimulations() {
        SettingsHolder.gameState = SettingsHolder.GameStates.Playing;
        //Update the board x number of times according to set rules.
        for (int i = 0; i < SettingsHolder.NumberOfCycles; i++) {
            yield return new WaitForSeconds(SettingsHolder.TimeBetweenCycles);
            BoardConverter.UpdateCells(cells);
        }
        //Now we pick a winner and show on screen
        PickWinner();

        SettingsHolder.gameState = SettingsHolder.GameStates.Planning;
    }
    private void PickWinner() {
        //Now all the generations are over, lets decide a winner.
        int num_p1 = BoardConverter.CountCells(BoardConverter.ConvertToInt(cells), 1);
        int num_p2 = BoardConverter.CountCells(BoardConverter.ConvertToInt(cells), 2);
        if (num_p1 == num_p2) { //Draw
            winnerText.SetActive(true);
            winnerText.GetComponent<TextMeshProUGUI>().color = Color.yellow;
            winnerText.GetComponent<TextMeshProUGUI>().text = "ITS A DRAW!!!";
        } else if (num_p1 > num_p2) { //Player 1 wins
            winnerText.SetActive(true);
            winnerText.GetComponent<TextMeshProUGUI>().color = Color.blue;
            winnerText.GetComponent<TextMeshProUGUI>().text = "PLAYER ONE WINS!!";
            winFXs[2].GetComponent<ParticleSystem>().Play();
            winFXs[3].GetComponent<ParticleSystem>().Play();
        } else { //Player 2 wins
            winnerText.SetActive(true);
            winnerText.GetComponent<TextMeshProUGUI>().color = Color.red;
            winnerText.GetComponent<TextMeshProUGUI>().text = "PLAYER TWO WINS!!";
            winFXs[0].GetComponent<ParticleSystem>().Play();
            winFXs[1].GetComponent<ParticleSystem>().Play();
        }
        //Hide the win text after a number of seconds.
        StartCoroutine(HideWinScreen(3));
    }

    IEnumerator HideWinScreen(int waitTime) {
        //Hides the win screen after "waitTime" seconds.
        yield return new WaitForSeconds(waitTime);
        winnerText.SetActive(false);
    }

    private void CheckForInputs() {
        //Keyboard shortcuts:
        if (Input.GetKeyDown(KeyCode.E)) {//Start game.
            StartCoroutine(ExecuteSimulations());
        }
        if (Input.GetKeyDown(KeyCode.R)) {//Stop game, allow placement of cells.
            SettingsHolder.gameState = SettingsHolder.GameStates.Planning;
        }
        //Press C to clear.
        if (Input.GetKeyDown(KeyCode.C)) {
            SettingsHolder.gameState = SettingsHolder.GameStates.Planning;
            foreach (Cell c in cells) {
                c.state = Cell.CellState.Dead;
            }
        }
        if (SettingsHolder.playButtonPressed) { //If play button pressed.
            SettingsHolder.playButtonPressed = false;
            StartCoroutine(ExecuteSimulations());
        }
    }

    public void LoadCellsFromSettings() {
        //If they player is continuing an old game, load it from settings.
        List<string> settings = SettingsHolder.ReadSettings();
        List<string> boardLines = new List<string>();
        foreach (string line in settings) {
            if (line[0] != '-') { // Then it's board information.
                boardLines.Add(line);
            }
        }
        //Now extract the data and apply to cell list.
        for (int y = 0; y < boardLines.Count; y++) {
            for (int x = 0; x < boardLines[0].Length; x++) {
                int pos = int.Parse(boardLines[y][x].ToString());
                if (pos == 1) {
                    cells[x, (boardLines.Count - 1) - y].state = Cell.CellState.Alive1;
                } else if (pos == 2) {
                    cells[x, (boardLines.Count - 1) - y].state = Cell.CellState.Alive2;
                } else {
                    cells[x, (boardLines.Count - 1) - y].state = Cell.CellState.Dead;
                }
            }
        }
    }

    private void CheckForCellSelection() {
        //If AI is not enabled, we assume it is in HOTSEAT mode.
        if (SettingsHolder.AIEnabled == false) {
            RunHotseatSelection();
        } else { //AI is enabled.
            SettingsHolder.playerOneTurn = true; //Make sure it's always player ones turn if vs ai.
            RunAISelection();
        }

    }

    private bool shouldShow_loading = false;
    private bool isShow_loading = false;
    private void CheckLoading() {
        //Check if it should be showing the "ai loading screen"
        if (shouldShow_loading) {
            loadingScreen.SetActive(true);
            isShow_loading = true;
        } else {
            loadingScreen.SetActive(false);
            isShow_loading = false;
        }
    }

    private void RunAISelection() {
        if (SettingsHolder.gameState == SettingsHolder.GameStates.Planning) {
            GameObject selected = RayCastScreen();
            if (selected != null) {
                if (selected.GetComponent<Cell>() != null) { //If they mouse over a valid cell.
                    Vector2Int p = selected.gameObject.GetComponent<Cell>().pos;
                    Cell c = cells[p.x, p.y];

                    if (SettingsHolder.patternSelected) { //If they have just selected a pattern from list.
                        int[,] pattern = SettingsHolder.patternData;
                        int endX = p.x + pattern.GetLength(0) - 1; //The positions of the end dimensions of pattern.
                        int endY = p.y - pattern.GetLength(1) + 1;

                        if ((endX < SettingsHolder.BoardHeight) && (endY >= 0)) {
                            for (int x = p.x; x <= endX; x++) {
                                for (int y = p.y; y >= endY; y--) {
                                    cells[x, y].selected = true;
                                }
                            }
                            if (Input.GetMouseButtonDown(0)) {
                                SettingsHolder.patternSelected = false;
                                int livingCellsInPattern = 0; //Give AI this many times to have their go.
                                for (int x = 0; x < pattern.GetLength(0); x++) { //Count how many living cells in selected pattern.
                                    for (int y = pattern.GetLength(1) - 1; y >= 0; y--) {
                                        int cx = p.x + x; //Current x 
                                        int cy = p.y - y; //Current y
                                        if (pattern[x, y] == 1) {
                                            cells[cx, cy].state = Cell.CellState.Alive1;
                                            livingCellsInPattern++;
                                        } else {
                                            cells[cx, cy].state = Cell.CellState.Dead;
                                        }
                                    }
                                }
                                if (SettingsHolder.AIEnabled) {
                                    StartCoroutine(SendToAi(livingCellsInPattern));
                                }
                            }
                        }
                        if (Input.GetMouseButtonDown(1)) {
                            SettingsHolder.patternSelected = false;
                        }

                    } else { //No pattern selected, only placing one cell.
                        if (p.x < SettingsHolder.BoardHeight && SettingsHolder.gameState == SettingsHolder.GameStates.Planning) {
                            if (Input.GetMouseButtonDown(0)) {
                                if (c.state != Cell.CellState.Dead) {
                                    c.state = Cell.CellState.Dead;
                                } else {
                                    c.state = Cell.CellState.Alive1;
                                }
                                if (SettingsHolder.AIEnabled) {
                                    StartCoroutine(SendToAi(1));
                                }
                            }
                            c.selected = true; //Allows cell to change colour to "selected color".
                        }
                    }
                }
            }
        }
    }
    IEnumerator SendToAi(int numCells) {
        //Using magic numbers to fine tune when the "loading" screen should show.
        if (numCells > 1 || SettingsHolder.BoardHeight > 11 || SettingsHolder.AIDifficulty > 20) {
            shouldShow_loading = true;
        }
        //Show the "AI IS THINKING" screen while ai thinks.
        while (shouldShow_loading == true && isShow_loading == false) {
            yield return new WaitForSeconds(0.1f);
            SendToAi(numCells);
        }
        //Give ai as many turns to place cells as are living cells in the pattern just placed by player 1.
        for (int i = 0; i < numCells; i++) {
            ai.PredictNextMove(cells);
        }
        shouldShow_loading = false;
    }
    private void RunHotseatSelection() {
        if (SettingsHolder.gameState == SettingsHolder.GameStates.Planning) {
            GameObject selected = RayCastScreen();
            if (selected != null) {
                if (selected.GetComponent<Cell>() != null) { //If they mouse over a valid cell.

                    Vector2Int p = selected.gameObject.GetComponent<Cell>().pos;
                    Cell c = cells[p.x, p.y];

                    //Assign the enum cellstate to whoevers turn it is.
                    Cell.CellState activePlayer = Cell.CellState.Dead;
                    if (SettingsHolder.playerOneTurn) {
                        activePlayer = Cell.CellState.Alive1;
                    } else {
                        activePlayer = Cell.CellState.Alive2;
                    }


                    if (SettingsHolder.patternSelected) { //If they have just selected a pattern from list.
                        int[,] pattern = SettingsHolder.patternData;
                        int endX = p.x + pattern.GetLength(0) - 1; //The positions of the end dimensions of pattern.
                        int endY = p.y - pattern.GetLength(1) + 1;

                        if (SettingsHolder.playerOneTurn) { //Player ones turn, must restrict to left.
                            if ((endX < SettingsHolder.BoardHeight) && (endY >= 0)) {
                                for (int x = p.x; x <= endX; x++) {
                                    for (int y = p.y; y >= endY; y--) {
                                        cells[x, y].selected = true;
                                    }
                                }
                                if (Input.GetMouseButtonDown(0)) { //If they click.
                                    SettingsHolder.patternSelected = false; //Pattern is about to be palced, so toggle this off.
                                    //Go through each cell starting from the mouse location and overwrite with pattern.
                                    for (int x = 0; x < pattern.GetLength(0); x++) {
                                        for (int y = pattern.GetLength(1) - 1; y >= 0; y--) {
                                            int cx = p.x + x; //Current x 
                                            int cy = p.y - y; //Current y
                                            if (pattern[x, y] == 1) {
                                                cells[cx, cy].state = activePlayer; //Give cell to player who's turn it is.
                                            } else {
                                                cells[cx, cy].state = Cell.CellState.Dead;
                                            }
                                        }
                                    }
                                    SettingsHolder.playerOneTurn = false;
                                }
                            }
                        } else {//Player 2 turn.
                            //Force them to place on right hand side.
                            if ((endX <= SettingsHolder.BoardHeight * 2) && (endX > SettingsHolder.BoardHeight) && (endY >= 0)) {
                                //Highlight all cells that will be changed if they place pattern.
                                for (int x = p.x; x <= endX; x++) {
                                    for (int y = p.y; y >= endY; y--) {
                                        cells[x, y].selected = true;
                                    }
                                }
                                if (Input.GetMouseButtonDown(0)) {
                                    SettingsHolder.patternSelected = false;
                                    for (int x = 0; x < pattern.GetLength(0); x++) {
                                        for (int y = pattern.GetLength(1) - 1; y >= 0; y--) {
                                            int cx = p.x + x; //Current x 
                                            int cy = p.y - y; //Current y
                                            if (pattern[x, y] == 1) {
                                                cells[cx, cy].state = activePlayer;
                                            } else {
                                                cells[cx, cy].state = Cell.CellState.Dead;
                                            }
                                        }
                                    }
                                    SettingsHolder.playerOneTurn = true;
                                }
                            }

                        }

                        if (Input.GetMouseButtonDown(1)) { //They decide not to place a cell.
                            SettingsHolder.patternSelected = false;
                        }

                    } else { //No pattern selected, only placing one cell.
                        if (SettingsHolder.playerOneTurn) { //Player ones turn.
                            if (p.x < SettingsHolder.BoardHeight) {
                                if (Input.GetMouseButtonDown(0)) {
                                    if (c.state != Cell.CellState.Dead) {
                                        c.state = Cell.CellState.Dead;
                                    } else {
                                        c.state = activePlayer;
                                    }
                                    SettingsHolder.playerOneTurn = false;
                                }
                                c.selected = true; //Allows cell to change colour to "selected color".
                            }
                        } else { //Player 2 turn.
                            if (p.x >= SettingsHolder.BoardHeight) {
                                if (Input.GetMouseButtonDown(0)) {
                                    if (c.state != Cell.CellState.Dead) {
                                        c.state = Cell.CellState.Dead;
                                    } else {
                                        c.state = activePlayer;
                                    }
                                    SettingsHolder.playerOneTurn = true;
                                }
                                c.selected = true; //Allows cell to change colour to "selected color".
                            }
                        }
                    }
                }
            }
        }
    }

    public void PlaceCells() {
        //Kill old cells.
        cells = new Cell[SettingsHolder.BoardHeight * 2, SettingsHolder.BoardHeight]; //2x as long as it is tall.
        SettingsHolder.boardSizeChanged = false;
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("cell")) {
            Destroy(g);
        }

        //Calc the neccessary size for the cells.
        Vector3 boardSize = gameObject.transform.localScale;
        float newCellScale = (boardSize.y / SettingsHolder.BoardHeight) / 1.2f;
        Vector3 newCellScaleVector = new Vector3(newCellScale, newCellScale, 1f);
        cell.transform.localScale = newCellScaleVector;

        //Place the cells onto the board
        float d = boardSize.y / SettingsHolder.BoardHeight; //Distance between cells
        for (int x = 0; x < SettingsHolder.BoardHeight * 2; x++) {
            for (int y = 0; y < SettingsHolder.BoardHeight; y++) {
                GameObject createdCell = Instantiate(cell, new Vector3
                    (d / 2 + x * d, d / 2 + y * d, 0f), Quaternion.identity);
                cells[x, y] = createdCell.GetComponent<Cell>();
                cells[x, y].pos = new Vector2Int(x, y);
            }
        }
    }

    private GameObject RayCastScreen() {
        Ray ray;
        RaycastHit hit;
        GameObject objectHit = null; //To store object hit.
        //Set ray to come from camera towards mouse pointer.
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit)) { //if we hit somthing.
            //Get the game object connected to collider hit.
            objectHit = hit.collider.gameObject;
        }
        return objectHit;
    }
}
