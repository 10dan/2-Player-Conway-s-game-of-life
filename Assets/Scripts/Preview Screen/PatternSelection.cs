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
        string desc = "";
        StreamReader sr = new StreamReader(patternPath);
        string fileContents = sr.ReadToEnd();
        sr.Close();
        string[] lines = fileContents.Split('\n');

        //We need to find the longest line to see how big to make preview.
        int maxLength = 0;
        int numLines = 0;
        //Go through each line to extract data we need to create cell array.
        foreach (string line in lines) {
            //If it's just description data then add to desc for later printing.
            if (line.Length > 0) {
                if (line.Substring(0, 1) == "!") {
                    desc += line + "\n";
                } else { //It is important cell data.
                    numLines++;
                    if (line.Length > maxLength) {
                        maxLength = line.Length - 1;
                    }
                    //print(line);
                }
            }
        }
       // print(numLines);
        //print(maxLength);
        int[,] patternDesc = new int[maxLength, numLines];
        //Go through again to populate cell array.
        int x = 0;
        int y = numLines - 1;
        foreach (string line in lines) {
            if (line.Length > 0) {
                if (line.Substring(0, 1) == "!") {
                    //Do nothing.
                } else {
                    foreach (char c in line) {
                        if (c == 'O') {
                            patternDesc[x, y] = 1;
                        }
                        x++;
                    }
                    x = 0;
                    y--;
                }
            }
        }
        //Send over data to be displayed in preview area.
        PreviewArea.SetPattern(patternDesc, desc);

        /* For debug, print the int array.
        for (int j = numLines - 1; j >= 0; j--) {
            string l = "";
            for (int i = 0; i < maxLength; i++) {
                l += patternDesc[i, j];
            }
            print(l);
        }*/
    }
}
