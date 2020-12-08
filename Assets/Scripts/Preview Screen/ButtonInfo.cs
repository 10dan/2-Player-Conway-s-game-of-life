using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInfo : MonoBehaviour {
    private string path = "";
    public void SetPath(string p) {
        path = p;
    }

    public void ButtonPressed() {
        GetComponentInParent<PatternSelection>().SetPattern(path);
    }
}
