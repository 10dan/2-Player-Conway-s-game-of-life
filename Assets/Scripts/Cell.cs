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
    AudioSource audio;

    private void Start() {
        audio = GetComponent<AudioSource>();
    }

    private void Update() {
        UpdateColour();
    }

    public void PlayDeathFX(CellState prevState) {
        float scalingFactor = 0.5f; //How many times smaller the particles will be compared to parent.
        //Set audio souce to random pitch so it doesnt sound too samey.
        float shift = 0.8f;
        float shift2 = 0.5f;
        audio.pitch = 1 + (shift * (UnityEngine.Random.value) - shift / 2f);
        audio.volume = 0.5f + (shift2 * (UnityEngine.Random.value) - shift2 / 2f);

        if (prevState == CellState.Alive1) {
            GetComponentInChildren<ParticleSystemRenderer>().material = cols[6];
            audio.Play(); //Make death fx and noise.
        } else {
            GetComponentInChildren<ParticleSystemRenderer>().material = cols[7];
            audio.Play();
        }
        GetComponentInChildren<ParticleSystem>().transform.localScale = new Vector3
            (this.transform.localScale.x * scalingFactor, this.transform.localScale.y * scalingFactor, this.transform.localScale.z * scalingFactor);
        GetComponentInChildren<ParticleSystem>().Play();
    }

    private void UpdateColour() {
        Material m;
        switch (state) {
            //If the cell state means it belongs to player 1.
            case CellState.Alive1:
                //Check if it is being hovered over by the mouse.
                if (selected) {
                    m = cols[1]; //Set to a slightly lighter blue.
                } else {
                    m = cols[0]; //Otherwise leave it its default blue colour.
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

