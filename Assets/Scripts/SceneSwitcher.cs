using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void LoadSceneAR(){
        // SceneManager.LoadScene(1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex  + 1);
    }

    public void LoadSceneQR(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex  - 1);
        // SceneManager.LoadScene(0);
    }
}
