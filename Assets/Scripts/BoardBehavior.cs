using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardBehavior : MonoBehaviour {
    [SerializeField] GameObject cell = null; //Cell prefab.
    [SerializeField] int numCellsVert = 20; //Number of cells vertically.

    public enum GameStates { Planning, Playing, Over };
    public GameStates gameState = GameStates.Planning;

    //Game settings
    private int difficulty = 0;
    private float cycleTime = 0.1f;//time between each simulation.
    private int numCellsHorz; //= NumCellsVert * 2;
    private int numCycles = 40;
    private bool wrapAround = true; //Should the cells wrap around when on edge.
    private bool AiEnabled = true;

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
        //Get variables from settings file for continuity between scenes.
        difficulty = SettingsHolder.GetSetting("-AIDifficulty");
        cycleTime = ((float)SettingsHolder.GetSetting("-TimeBetweenCycle"))/1000f;//Given in ms in settings, so divide by 1000 to get s.
        numCellsVert = SettingsHolder.GetSetting("-BoardHeight");
        wrapAround = (SettingsHolder.GetSetting("-WrapAround") == 1) ? true : false;
        AiEnabled = (SettingsHolder.GetSetting("-AIEnabled") == 1) ? true : false;
        numCellsHorz = numCellsVert * 2; //We want 2x the number of cells horizontally.
        cells = new Cell[numCellsHorz, numCellsVert];
        ai = new AIScript();

    }

    private void Update() {
        CheckForInputs();
        if (gameState == GameStates.Planning) {
            bool hasPlaced = CheckForCellSelection();
            if (hasPlaced) {
                if (AiEnabled) {
                    ai.PredictNextMove(cells, difficulty);
                }
            }
            if (SettingsHolder.patternSelected) {
                //TODO: Place the pattern. So close now.
                print(SettingsHolder.patternData);
            }
        }

    }
    IEnumerator ExecuteSimulations(int numSimulations, float waitTime) {
        for (int i = 0; i < numSimulations; i++) {
            yield return new WaitForSeconds(waitTime);
            BoardConverter.UpdateCells(cells);
        }
        gameState = GameStates.Planning;
    }

    private void CheckForInputs() {
        if (Input.GetKeyDown(KeyCode.E)) {
            gameState = GameStates.Playing;
            StartCoroutine(ExecuteSimulations(numCycles, cycleTime));
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            gameState = GameStates.Planning;
        }
        //Press C to clear.
        if (Input.GetKeyDown(KeyCode.C)) {
            gameState = GameStates.Planning;
            foreach (Cell c in cells) {
                c.state = Cell.CellState.Dead;
            }
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
        for(int y = 0; y < boardLines.Count; y++) {
            for(int x = 0; x < boardLines[0].Length; x++) {
                int pos = int.Parse(boardLines[y][x].ToString());
                if(pos == 1) {
                    cells[x, (boardLines.Count - 1) - y].state = Cell.CellState.Alive1;
                }else if (pos == 2) {
                    cells[x, (boardLines.Count - 1) - y].state = Cell.CellState.Alive2;
                } else {
                    cells[x, (boardLines.Count-1)-y].state = Cell.CellState.Dead;
                }
            }
        }
    }

    private bool CheckForCellSelection() {
        bool hasPlaced = false;
        if (gameState == GameStates.Planning) {
            GameObject selected = RayCastScreen();
            if (selected != null) {
                if (selected.GetComponent<Cell>() != null) {
                    Vector2Int p = selected.gameObject.GetComponent<Cell>().pos;
                    Cell c = cells[p.x, p.y];
                    if (p.x < numCellsHorz / 2 && gameState == GameStates.Planning) {
                        if (Input.GetMouseButtonDown(0)) {
                            hasPlaced = true;
                            if (c.state != Cell.CellState.Dead) {
                                c.state = Cell.CellState.Dead;
                            } else {
                                c.state = Cell.CellState.Alive1;
                            }
                        }
                        c.selected = true; //Allows cell to change colour to "selected color".
                    }
                }
            }
        }
        return hasPlaced;
    }

    private void PlaceCells() {
        //Calc the neccessary size for the cells.
        Vector3 boardSize = gameObject.transform.localScale;
        float newCellScale = (boardSize.y / numCellsVert) / 1.2f;
        Vector3 newCellScaleVector = new Vector3(newCellScale, newCellScale, 1f);
        cell.transform.localScale = newCellScaleVector;

        //Place the cells onto the board
        float d = boardSize.y / numCellsVert; //Distance between cells
        for (int x = 0; x < numCellsHorz; x++) {
            for (int y = 0; y < numCellsVert; y++) {
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
