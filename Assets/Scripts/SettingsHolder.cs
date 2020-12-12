using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SettingsHolder {
    public static List<string> ReadSettings() {
        string line;
        List<string> lines = new List<string>();
        System.IO.StreamReader file = new System.IO.StreamReader(@"Assets/settings.txt");
        while ((line = file.ReadLine()) != null) {
            lines.Add(line);
        }
        file.Close();
        return lines;
    }

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
            }catch(Exception e) {
                Debug.LogError("Unable to convert setting value to int. " + e);
            }
        }
        return value;
    }
}
