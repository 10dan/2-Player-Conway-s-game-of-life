using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    [SerializeField] GameObject mainMenu = null;
    [SerializeField] GameObject howToPage = null;
    private AudioSource audio;

    private void Start() {
        audio = GetComponent<AudioSource>();
    }

    public void ShowHowTo() {
        mainMenu.SetActive(false);
        howToPage.SetActive(true);
        audio.Play();
    }

    public void PlayGame() {
        audio.Play();
        SceneManager.LoadScene(1);
    }

    public void ReturnToMainMenu() {
        audio.Play();
        mainMenu.SetActive(true);
        howToPage.SetActive(false);
    }
}
