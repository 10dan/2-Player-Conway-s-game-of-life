using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardBehavior : MonoBehaviour {
    [SerializeField] GameObject cell; //Cell prefab.
    [SerializeField] int numCellsVert = 20; //Number of cells vertically.
    int numCellsHorz; //= NumCellsVert * 2;

    public static bool wrapAround = true;
    public enum GameStates { Planning, Playing, Over };
    public GameStates gameState = GameStates.Planning;

    //Structure that will store cells.
    Cell[,] cells;

    void Start() {
        InitVariables();
        PlaceCells();
    }

    private void InitVariables() {
        //Init variables.
        numCellsHorz = numCellsVert * 2;
        cells = new Cell[numCellsHorz, numCellsVert];
    }

    private void Update() {
        CheckForInputs();
        if (gameState == GameStates.Planning) {
            CheckForCellSelection();
        }
        if (gameState == GameStates.Playing) {
            if (Input.GetKeyDown(KeyCode.E)) {
                UpdateBoard();
            }
        }

    }

    private void CheckForInputs() {
        if (Input.GetKeyDown(KeyCode.E)) {
            gameState = GameStates.Playing;
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            gameState = GameStates.Planning;
        }
        //Press C to clear.
        if (Input.GetKeyDown(KeyCode.C)) {
            gameState = GameStates.Planning;
            foreach(Cell c in cells) {
                c.state = Cell.CellState.Dead;
                c.nextState = Cell.CellState.Dead;
            }
        }
    }

    private void UpdateBoard() {
        foreach(Cell c in cells) {
            c.CalcNextState(cells);
        }
        foreach (Cell c in cells) {
            c.state = c.nextState;
        }
    }

    private void CheckForCellSelection() {
        GameObject selected = RayCastScreen();
        if (selected != null) {
            if (selected.GetComponent<Cell>() != null) {
                Vector2Int p = selected.gameObject.GetComponent<Cell>().pos;
                Cell c = cells[p.x, p.y];
                if (Input.GetMouseButtonDown(0)) {
                    if (c.state != Cell.CellState.Dead) {
                        c.state = Cell.CellState.Dead;
                    } else {
                        c.state = Cell.CellState.Alive1;
                    }
                }
                if (Input.GetMouseButtonDown(1)) {
                    if (c.state != Cell.CellState.Dead) {
                        c.state = Cell.CellState.Dead;
                    } else {
                        c.state = Cell.CellState.Alive2;
                    }
                }
                c.selected = true;
            }
        }
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
