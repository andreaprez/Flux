using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScenes : MonoBehaviour {

    private void Start() {
        if (Manager.instance) {
            Destroy(Manager.instance.gameObject);
        }
    }

    public void LoadScene(int i) {
        SceneManager.LoadScene(i);
    }

    public void ApplicationExit() {
        Application.Quit();
    }

    public void LoadLastScene() {
        SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings - 1);
    }
}
