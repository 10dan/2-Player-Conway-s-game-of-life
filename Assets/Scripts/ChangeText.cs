using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChangeText : MonoBehaviour {
    [SerializeField] GameObject teamOneText;
    [SerializeField] GameObject teamTwoText;
    [SerializeField] GameObject gameState;
    void Update() {
        int numCells_team1 = BoardConverter.CountCells(BoardConverter.ConvertToInt(BoardBehavior.cells), 1);
        int numCells_team2 = BoardConverter.CountCells(BoardConverter.ConvertToInt(BoardBehavior.cells), 2);
        teamOneText.GetComponent<TextMeshProUGUI>().text = ("Team 1:"  + numCells_team1);
        teamTwoText.GetComponent<TextMeshProUGUI>().text = ("Team 2:"  + numCells_team2);
        if (SettingsHolder.gameState == SettingsHolder.GameStates.Planning) {
            gameState.GetComponent<TextMeshProUGUI>().text = "Planning phase";
        } else if(SettingsHolder.gameState == SettingsHolder.GameStates.Playing) {
            gameState.GetComponent<TextMeshProUGUI>().text = "Game is playing";
        } else {
            gameState.GetComponent<TextMeshProUGUI>().text = "Game over!";
        }
    }
}
