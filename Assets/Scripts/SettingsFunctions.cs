using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsFunctions : MonoBehaviour {
    [SerializeField] GameObject[] settingsObjects;
    const int MAXDIFF = 30;
    const int MAXSIZE = 30;
    public void SettingButtonPressed() {
        gameObject.SetActive(true);
        //Get all toggle elements, check name and current setting. change accordingly.
        foreach (Toggle t in gameObject.GetComponentsInChildren<Toggle>()) {
            if (t.name == "WrapAround_toggle") {
                if (SettingsHolder.WrapAround) {
                    t.isOn = true;
                } else {
                    t.isOn = false;
                }
            }
            if (t.name == "AI_toggle") {
                if (SettingsHolder.AIEnabled) {
                    t.isOn = true;
                } else {
                    t.isOn = false;
                }
            }
        }

        //Do the same this but with text inputs.
        foreach (TMP_InputField t in gameObject.GetComponentsInChildren<TMP_InputField>()) {
            if (t.name == "diff_input") {
                t.text = SettingsHolder.AIDifficulty.ToString();
            }
            if (t.name == "time_input") {
                t.text = SettingsHolder.TimeBetweenCycles.ToString();
            }
            if (t.name == "cycles_input") {
                t.text = SettingsHolder.NumberOfCycles.ToString();
            }
            if (t.name == "height_input") {
                t.text = SettingsHolder.BoardHeight.ToString();
            }
        }
    }

    public void ApplyButtonPressed() {
        //Get all toggle elements, check name and current setting. change accordingly.
        foreach (Toggle t in gameObject.GetComponentsInChildren<Toggle>()) {
            if (t.name == "WrapAround_toggle") {
                if (t.isOn) {
                    SettingsHolder.WrapAround = true;
                } else {
                    SettingsHolder.WrapAround = false;
                }
            }
            if (t.name == "AI_toggle") {
                if (t.isOn) {
                    SettingsHolder.AIEnabled = true;
                } else {
                    SettingsHolder.AIEnabled = false;
                }
            }
        }

        //Do the same this but with text inputs.
        foreach (TMP_InputField t in gameObject.GetComponentsInChildren<TMP_InputField>()) {
            //make the settings screen reflect the current settings.
            if (t.name == "diff_input") {
                var isNumeric = int.TryParse(t.text, out int n) ? n : 3; //Try to get it, otherwise set to defult value of 3.
                if ((n > MAXDIFF)||(n <= 0)){
                    SettingsHolder.AIDifficulty = MAXDIFF;
                } else {
                    SettingsHolder.AIDifficulty = n;
                }
            }
            if (t.name == "time_input") {
                var isNumeric = double.TryParse(t.text, out double n) ? n : 0.1f;
                SettingsHolder.TimeBetweenCycles = (float) n;
            }
            if (t.name == "cycles_input") {
                var isNumeric = int.TryParse(t.text, out int n) ? n : 3;
                SettingsHolder.NumberOfCycles = n;
            }
            if (t.name == "height_input") {
                var isNumeric = int.TryParse(t.text, out int n) ? n : 2;
                if (n != SettingsHolder.BoardHeight) SettingsHolder.boardSizeChanged = true;

                if (n > MAXSIZE) { //If they try to enter a board size too big, might crash program, limit.
                    SettingsHolder.BoardHeight = MAXSIZE;
                } else {
                    SettingsHolder.BoardHeight = n;
                }
                if(n <= 0) { //Set to 3 if less than or = 0.
                    SettingsHolder.BoardHeight = 2;
                }
            }
        }
        gameObject.SetActive(false);
    }
}
