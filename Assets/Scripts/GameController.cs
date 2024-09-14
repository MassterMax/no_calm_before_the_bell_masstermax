using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    const int JUNK_REWARD = 5;

    [SerializeField] GameObject pauseScreen;
    [SerializeField] GameObject gameOverScreen;
    [SerializeField] Text moneyText;
    int moneyBalance = 0;
    bool paused = false;
    bool gameOver = false;
    void Start()
    {
        pauseScreen.SetActive(false);
        gameOverScreen.SetActive(false);
        Time.timeScale = 1;
        ChangeMoney(0);
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
        // if (Input.GetKeyDown(KeyCode.O)) {
        //     // loose the game, remove after debug
        //     GameOver();
        // }
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

    public void RewardForJunk(int count) {
        ChangeMoney(count * JUNK_REWARD);
    }

    void ChangeMoney(int value) {
        moneyBalance += value;
        moneyText.text = moneyBalance.ToString();

        if (moneyBalance < 0) {
            moneyText.color = Color.red;
            GameOver();
        }
    }
}
