using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneScript : MonoBehaviour {
    //Handles button presses and scene loading.
    public void LoadScene(int sceneIndex) { //Load the unity scene with index "sceneIndex"
        SceneManager.LoadScene(sceneIndex);
    }

    public void PatternButtonBehavior() {
        SettingsHolder.UpdateSettingsBoard(BoardBehavior.cells);
        LoadScene(2);
    }

    public void PatturnScreenBackButtonBehavior() {
        LoadScene(1);
    }

    public void PatturnScreenPlaceButton() {
        int[,] pattern = PreviewArea.GetCellData(); //Pattern to place.
        SceneManager.LoadScene(1);
        SettingsHolder.patternSelected = true;
        SettingsHolder.patternData = pattern;
    }

    public void PlayButtonPressed() {
        SettingsHolder.playButtonPressed = true;
    }


}