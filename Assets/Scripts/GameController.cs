using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField] GameObject pauseScreen;
    [SerializeField] GameObject gameOverScreen;
    bool paused = false;
    bool gameOver = false;
    void Start()
    {
        pauseScreen.SetActive(false);
        gameOverScreen.SetActive(false);
        Time.timeScale = 1;
    }
    // Update is called once per frame
    void Update()
    {
        if (gameOver) {
            if (Input.GetKeyDown(KeyCode.R)) {
                SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
            }
            return;
        }
        if (Input.GetKeyDown(KeyCode.P)) {
            Pause();
        }
        if (Input.GetKeyDown(KeyCode.O)) {
            // loose the game, remove after debug
            GameOver();
        }
    }

    void Pause() {
        paused = !paused;
        Time.timeScale = paused ? 0 : 1;
        pauseScreen.SetActive(paused);
    }

    void GameOver() {
        gameOverScreen.SetActive(true);
        gameOver = true;
        Time.timeScale = 0;
    }
}
