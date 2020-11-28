using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour {

    enum CellState { Alive1, Alive2, Dead } //Potential states of cells. Alive1 means owned by player 1.
    CellState state = CellState.Dead; 

    public bool selected = false; //Is the player hovering this cell?
    public Vector2Int pos; //Holds the x,y index of the cell.
    [SerializeField] Material[] cols; //Holds the possible colours of cells.

    private void Update() {
        UpdateColour();
    }

    private void UpdateColour() {
        Material m;
        switch (state) {
            case CellState.Alive1:
                if (selected) {
                    m = cols[0];
                } else {
                    m = cols[1];
                }
                gameObject.GetComponent<MeshRenderer>().material = m;
                break;
            case CellState.Alive2:
                if (selected) {
                    m = cols[2];
                } else {
                    m = cols[3];
                }
                gameObject.GetComponent<MeshRenderer>().material = m;
                break;
            case CellState.Dead:
                if (selected) {
                    m = cols[4];
                } else {
                    m = cols[5];
                }
                gameObject.GetComponent<MeshRenderer>().material = m;
                break;
            default:
                print("Unknown cell state!");
                break;
        }
        selected = false; //Reset selected state.
    }
}

