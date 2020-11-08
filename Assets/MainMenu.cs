using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    [SerializeField] GameObject mainMenu, howToPage;
    public void ShowHowTo() {
        mainMenu.active = false;
        howToPage.active = true;
    }

    public void PlayGame() {
        SceneManager.LoadScene(1);
    }

    public void ReturnToMainMenu() {
        mainMenu.active = true;
        howToPage.active = false;
    }
}
