using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Every button in the preview list has this script attached to it.
public class ButtonInfo : MonoBehaviour {
    private string path = ""; //It has a string pointing to the .txt file of the pattern it will load.
    public void SetPath(string p) { //Called when button is created.
        path = p;
    }

    public void ButtonPressed() {
        //When the button is pressed, tell the PatternSelection object to set the pattern to the "path"
        GetComponentInParent<PatternSelection>().SetPattern(path);
    }
}
