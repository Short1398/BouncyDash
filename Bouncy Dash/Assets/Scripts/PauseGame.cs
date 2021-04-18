using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Basic functionality for pausing

public class PauseGame : MonoBehaviour {

    public GameObject PausePanel;
    bool isPaused = false;

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            isPaused = !isPaused;
        }

        if (isPaused) {
            Time.timeScale = 0;
            PausePanel.SetActive(true);
        } else if (isPaused == false) {
            Time.timeScale = 1;
            PausePanel.SetActive(false);
        }
    }
}
