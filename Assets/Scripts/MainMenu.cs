using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    [SerializeField] GameObject mainMenu, howToPage;
    public void ShowHowTo() {
        mainMenu.SetActive(false);
        howToPage.SetActive(true);
    }

    public void PlayGame() {
        SceneManager.LoadScene(1);
    }

    public void ReturnToMainMenu() {
        mainMenu.SetActive(true);
        howToPage.SetActive(false);
    }
}
