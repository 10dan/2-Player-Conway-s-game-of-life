using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SettingsHolder {

    private static string path = @"Assets/settings.txt";

    public static bool patternSelected = false;
    public static int[,] patternData;

    public static bool AIEnabled = true;
    public static int AIDifficulty = 10;
    public static float TimeBetweenCycles = 0.1f;
    public static int NumberOfCycles = 30;
    public static int BoardHeight = 10;
    public static bool WrapAround = true;

    public static Cell[] cells;

    public static void UpdateSettingsBoard(Cell[,] cellBoard) {
        int[,] cells = BoardConverter.ConvertToInt(cellBoard);
        List<string> lines = ReadSettings();
        List<string> importantLines = new List<string>();
        //Go through all the lines in the settings file, only copy parameters, not board.
        foreach (string l in lines) {
            if (l[0].Equals('-')) { //Important settings are pre marked with "-".
                importantLines.Add(l);
            }
        }
        //Convert the int[,] into a list of strings.
        for (int y = cells.GetLength(1) - 1; y >= 0; y--) {
            string line = "";
            for (int x = 0; x < cells.GetLength(0); x++) {
                line += cells[x, y];
            }
            importantLines.Add(line);
        }
        //Write lines to settings file.
        System.IO.File.WriteAllLines(path, importantLines);
    }


    public static List<string> ReadSettings() {
        string line;
        List<string> lines = new List<string>();
        System.IO.StreamReader file = new System.IO.StreamReader(path);
        while ((line = file.ReadLine()) != null) {
            lines.Add(line);
        }
        file.Close();
        return lines;
    }







    //Legacy method for loading settings.
    public static int GetSetting(string settingName) {
        string stringValue = "";
        int value = 0;
        List<string> settingList = ReadSettings();
        bool foundSetting = false;
        foreach (string line in settingList) {
            string[] splitLine = line.Split(':');
            if (splitLine[0] == settingName) {
                stringValue = splitLine[1];
                foundSetting = true;
            }
        }
        if (!foundSetting) { //If we can't find the named setting in the file.
            Debug.LogError("Setting not found!");
            return -1;
        } else {
            try {
                value = int.Parse(stringValue);
            } catch (Exception e) {
                Debug.LogError("Unable to convert setting value to int. " + e);
            }
        }
        return value;
    }
}
