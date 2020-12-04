using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChangeText : MonoBehaviour {
    [SerializeField] int teamToCount = 0;
    void Update() {
        int numCells = BoardConverter.CountCells(BoardConverter.ConvertToInt(BoardBehavior.cells), teamToCount);
        gameObject.GetComponent<TextMeshProUGUI>().text = ("Team " + teamToCount + ": " + numCells);
    }
}
