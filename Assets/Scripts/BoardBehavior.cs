using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardBehavior : MonoBehaviour {
    [SerializeField] GameObject cell; //Cell prefab.
    [SerializeField] int numCellsVert = 20; //Number of cells vertically. (*2 for horizontal)
    int numCellsHorz;

    //Structure that will store cells.
    Cell[,] cells;

    void Start() {
        InitVariables();
        PlaceCells();
    }
    private void Update() {
        GameObject selected = RayCastScreen();
        if (selected != null) {
            if (selected.GetComponent<Cell>() != null) {
                Vector2Int p = selected.gameObject.GetComponent<Cell>().pos;
                cells[p.x, p.y].selected = true;
                print(cells[p.x, p.y].pos);
            }
        }
    }


    private void InitVariables() {
        //Init variables.
        numCellsHorz = numCellsVert * 2;
        cells = new Cell[numCellsHorz, numCellsVert];
    }

    private void PlaceCells() {
        //Calc the neccessary size for the cells.
        Vector3 boardSize = gameObject.transform.localScale;
        float newCellScale = (boardSize.y / numCellsVert) / 1.1f;
        Vector3 newCellScaleVector = new Vector3(newCellScale, newCellScale, 1f);
        cell.transform.localScale = newCellScaleVector;

        //Place the cells onto the board
        float d = boardSize.y / numCellsVert; //Distance between cells
        for (int x = 0; x < numCellsVert * 2; x++) {
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
