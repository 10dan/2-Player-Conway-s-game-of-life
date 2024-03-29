﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SettingsHolder {
    //Static class that remains active even through scene changes.
    private static string path = "Assets/Resources/settings.txt";

    //Holds all setting values.
    public static bool playButtonPressed = false;
    public static bool boardSizeChanged = false;
    public static bool patternSelected = false;
    public static int[,] patternData;

    public static bool AIEnabled = false;
    public static int AIDifficulty = 3;
    public static float TimeBetweenCycles = 0.3f;
    public static int NumberOfCycles = 20;
    public static int BoardHeight = 6;
    public static bool WrapAround = true;

    public static bool playerOneTurn = true;

    public enum GameStates { Planning, Playing, Over };
    public static GameStates gameState = GameStates.Planning;

    public static Cell[] cells;
    //Writes the current board state to a text file. used to save the game.
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
