using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour {

    public enum CellState { Alive1, Alive2, Dead } //Potential states of cells. Alive1 means owned by player 1.
    public CellState state = CellState.Dead;
    public CellState nextState = CellState.Dead;

    public bool selected = false; //Is the player hovering this cell?
    public Vector2Int pos; //Holds the x,y index of the cell.
    [SerializeField] Material[] cols; //Holds the possible colours of cells.


    TextMesh text;
    private void Start() {
        text = GetComponentInChildren<TextMesh>();
    }

    private void Update() {
        UpdateColour();
    }
    public void CalcNextState(Cell[,] cells) {
        int x = pos.x;
        int y = pos.y;
        //Count the number of cells alive per team.
        int count1 = 0;
        int count2 = 0;
        for (int i = -1; i <= 1; i++) {
            int k = (x + i + cells.GetLength(0)) % cells.GetLength(0);
            for (int j = -1; j <= 1; j++) {
                int h = (y + j + cells.GetLength(1)) % cells.GetLength(1);
                //If wrap around is disabled, check if we are out of bounds.
                if (BoardBehavior.wrapAround == false) {
                    if ((x + i < 0) || (x + i >= cells.GetLength(0) ||
                        (y + j < 0) || (y + j >= cells.GetLength(1)))) {//Out of bounds.
                    } else {
                        if (cells[x + i, y + j].state == Cell.CellState.Alive1) {
                            count1++;
                        } else if (cells[x + i, y + j].state == Cell.CellState.Alive2) {
                            count2++;
                        }
                    }
                } else {
                    if (cells[k, h].state == Cell.CellState.Alive1) {
                        count1++;
                    } else if (cells[k, h].state == Cell.CellState.Alive2) {
                        count2++;
                    }
                }
            }
        }
        int totalCount = count1 + count2;
        //To avoid counting itself.
        if (cells[x, y].state != Cell.CellState.Dead) totalCount--;
        //Display text for debugging.
        text.text = totalCount.ToString();
        if (state != CellState.Dead) {
            if (totalCount > 3 || totalCount < 2) {
                nextState = CellState.Dead;
            } else if (totalCount == 2 || totalCount == 3) {
                nextState = state;
            }
        } else {
            if (totalCount == 3) {
                if (count1 > count2) {
                    nextState = CellState.Alive1;
                } else {
                    nextState = CellState.Alive2;
                }
            }
        }
    }

    private void UpdateColour() {
        Material m;
        switch (state) {
            case CellState.Alive1:
                if (selected) {
                    m = cols[1];
                } else {
                    m = cols[0];
                }
                gameObject.GetComponent<MeshRenderer>().material = m;
                break;
            case CellState.Alive2:
                if (selected) {
                    m = cols[3];
                } else {
                    m = cols[2];
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

