using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardBehavior : MonoBehaviour {
    [SerializeField] GameObject cell = null; //Cell prefab.
    [SerializeField] GameObject loadingScreen = null;

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
        CheckForInputs();
        if (SettingsHolder.boardSizeChanged) PlaceCells();
        if (SettingsHolder.gameState == SettingsHolder.GameStates.Planning) {
            CheckForCellSelection();
        }
        CheckLoading();
    }

    IEnumerator ExecuteSimulations() {
        SettingsHolder.gameState = SettingsHolder.GameStates.Playing;
        for (int i = 0; i < SettingsHolder.NumberOfCycles; i++) {
            yield return new WaitForSeconds(SettingsHolder.TimeBetweenCycles);
            BoardConverter.UpdateCells(cells);
        }
        SettingsHolder.gameState = SettingsHolder.GameStates.Planning;
    }

    private void CheckForInputs() {
        if (Input.GetKeyDown(KeyCode.E)) {
            StartCoroutine(ExecuteSimulations());
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            SettingsHolder.gameState = SettingsHolder.GameStates.Planning;
        }
        //Press C to clear.
        if (Input.GetKeyDown(KeyCode.C)) {
            SettingsHolder.gameState = SettingsHolder.GameStates.Planning;
            foreach (Cell c in cells) {
                c.state = Cell.CellState.Dead;
            }
        }
        if (SettingsHolder.playButtonPressed) {
            SettingsHolder.playButtonPressed = false;
            StartCoroutine(ExecuteSimulations());
        }
    }

    public void LoadCellsFromSettings() {
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
        if (numCells > 1 || SettingsHolder.BoardHeight > 11 || SettingsHolder.AIDifficulty > 20) {
            shouldShow_loading = true;
        }
        while (shouldShow_loading == true && isShow_loading == false) {
            yield return new WaitForSeconds(0.1f);
            SendToAi(numCells);
        }
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
                                    SettingsHolder.playerOneTurn = false;
                                }
                            }
                        } else {//Player 2 turn.
                            if ((endX <= SettingsHolder.BoardHeight * 2) && (endY >= 0)) {
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
                GameObject createdCell = Instantiate(cell, new Vector3(d / 2 + x * d, d / 2 + y * d, 0f), Quaternion.identity);
                cells[x, y] = createdCell.GetComponent<Cell>();
                cells[x, y].pos = new Vector2Int(x, y);
            }
        }
    }

    private GameObject RayCastScreen() {
        Ray ray;
        RaycastHit hit;
        GameObject objectHit = null;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit)) {
            objectHit = hit.collider.gameObject;
        }
        return objectHit;
    }
}
