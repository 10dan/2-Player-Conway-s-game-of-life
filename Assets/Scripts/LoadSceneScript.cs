using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneScript : MonoBehaviour {
    [SerializeField] int sceneIndex = 0;
    public void LoadScene() {
        SceneManager.LoadScene(sceneIndex);
    }
}
