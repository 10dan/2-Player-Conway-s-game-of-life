﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PreviewArea : MonoBehaviour {

    [SerializeField] GameObject cellPrefab = null;
    [SerializeField] GameObject descTextGO = null;
    Cell[,] cells;
    static int[,] cellData = new int[5, 5];
    static string descText = "";
    List<GameObject> cellsCreated;
    static bool boardUpdated = false;
    float bw, bh;
    Transform t;

    private void Start() {
        int[,] initPat = new int[5, 5];
        SetPattern(initPat, "***select a pattern from the list on the left to see a visual preview. ***");
        cellsCreated = new List<GameObject>();
        t = gameObject.transform;
        bw = t.localScale.x - t.localScale.x/15; //Board width
        bh = t.localScale.y - t.localScale.y / 15; //Board height
    }

    private void Update() {
        if (boardUpdated) {
            boardUpdated = false;
            //Update description.
            descText = descText.Replace("!", ""); //Remove annoying ! from start.
            descTextGO.GetComponent<TextMeshProUGUI>().ClearMesh();
            descTextGO.GetComponent<TextMeshProUGUI>().text=descText;
            //Remove old preview objects.
            foreach (GameObject g in cellsCreated) {
                Destroy(g);
            }
            int w = cellData.GetLength(0);
            int h = cellData.GetLength(1);
            cells = new Cell[w, h];
            float cellScaleY = (bh / h) / 1.2f;
            float cellScaleX = (bw / w) / 1.2f;
            Vector3 cellScaleVector = new Vector3(cellScaleX, cellScaleY, 1f);
            float dx = bw / w; //Distance between cells horz
            float dy = bh / h; //Dist vert.
            float bx = t.localPosition.x;
            float by = t.localPosition.y;
            for (int x = 0; x < w; x++) {
                for (int y = 0; y < h; y++) {
                    float xpos = (bx - bw / 2 + x * dx) + dx/2;
                    float ypos = (by - bh / 2 + y * dy) + dy/2;
                    Vector3 pos = new Vector3(xpos, ypos, -1);
                    GameObject c = Instantiate(cellPrefab, pos, Quaternion.identity);
                    cellsCreated.Add(c);
                    c.transform.localScale = cellScaleVector;
                    cells[x, y] = c.GetComponent<Cell>();
                    if (cellData[x, y] == 1) {
                        cells[x, y].state = Cell.CellState.Alive1;
                    }
                }
            }
        }
    }

    public static void SetPattern(int[,] pat, string desc) {
        cellData = pat;
        descText = desc;
        boardUpdated = true;
    }
}