using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneScript : MonoBehaviour {
    public void LoadScene(int sceneIndex) {
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
}
