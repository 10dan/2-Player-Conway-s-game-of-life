using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PatternSelection : MonoBehaviour {
    [SerializeField] string path = null; //To where the patterns are stored.
    [SerializeField] GameObject button;
    void Start() {
        foreach (string file in System.IO.Directory.GetFiles(path)) {
            if (file.EndsWith(".cells")) {
                string[] s = file.Split('/');
                string withDotCells = s[2];
                string without = withDotCells.Split('.')[0];
                GameObject b = Instantiate(button, this.gameObject.transform);
                b.GetComponent<ButtonInfo>().SetPath(file);
                b.GetComponentInChildren<Text>().text = without;
            }
        }
    }

    public void SetPattern(string patternPath) {
        var sr = new StreamReader(patternPath);
        var fileContents = sr.ReadToEnd();
        sr.Close();

        var lines = fileContents.Split("\n"[0]);
        foreach(string line in lines) {
            print(line);
        }
    }
}
