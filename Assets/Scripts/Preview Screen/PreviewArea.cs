using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewArea : MonoBehaviour {

    [SerializeField] GameObject cellPrefab = null;
    Cell[,] preview;
    void Start() {
        preview = new Cell[5, 10];
        int w = preview.GetLength(0);
        int h = preview.GetLength(1);
        Transform t = gameObject.transform;
        Vector3 boardScale = t.localScale;
        float cellScale = (boardScale.y / h) / 1.2f;
        Vector3 cellScaleVector = new Vector3(cellScale, cellScale, 1f);
        float d = boardScale.y / h; //Distance between cells
        for (int x = 0; x < preview.GetLength(0); x++) {
            for (int y = 0; y < preview.GetLength(1); y++) {
                float xpos = d / 2 + x * d + t.localPosition.x/2 + 1.5f*d;
                float ypos = d / 2 + y * d - boardScale.y/2;
                Vector3 pos = new Vector3(xpos,ypos, -1);
                GameObject c = Instantiate(cellPrefab, pos, Quaternion.identity);
                c.transform.localScale = cellScaleVector;
                preview[x, y] = c.GetComponent<Cell>();
            }
        }
    }
}