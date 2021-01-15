using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour {

    public enum CellState { Alive1, Alive2, Dead } //Potential states of cells. Alive1 means owned by player 1.
    public CellState state = CellState.Dead;

    public bool selected = false; //Is the player hovering this cell?
    public Vector2Int pos; //Holds the x,y index of the cell.
    [SerializeField] Material[] cols = null; //Holds the possible colours of cells.

    private void Update() {
        UpdateColour();
    }

    public void PlayDeathFX(CellState prevState) {
        float scalingFactor = 0.5f; //How many times smaller the particles will be compared to parent.
        if(prevState == CellState.Alive1) {
            GetComponentInChildren<ParticleSystemRenderer>().material = cols[6];
        } else {
            GetComponentInChildren<ParticleSystemRenderer>().material = cols[7];
        }
        GetComponentInChildren<ParticleSystem>().transform.localScale = new Vector3
            (this.transform.localScale.x * scalingFactor, this.transform.localScale.y * scalingFactor, this.transform.localScale.z * scalingFactor);
        GetComponentInChildren<ParticleSystem>().Play();
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

