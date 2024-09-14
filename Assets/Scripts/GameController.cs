using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] AudioClip getMoneyClip;
    [SerializeField] AudioClip penaltyClip;

    const int JUNK_REWARD = 2;
    const int JUNK_PENALTY = -1;
    const int PUDDLE_PENALTY = -2;
    const int CHILD_ITEM_PENALTY = -5;

    [SerializeField] GameObject warningPrefab;
    [SerializeField] GameObject pauseScreen;
    [SerializeField] GameObject gameOverScreen;
    [SerializeField] Text moneyText;
    int moneyBalance = 0;
    bool paused = false;
    bool gameOver = false;
    PuddleSpawner puddleSpawner;
    JunkSpawner junkSpawner;

    List<GameObject> warnings = new List<GameObject>();

    Color originalMoneyColor;

    void Start()
    {
        pauseScreen.SetActive(false);
        gameOverScreen.SetActive(false);
        Time.timeScale = 1;
        ChangeMoney(0);
        puddleSpawner = FindObjectOfType<PuddleSpawner>();
        junkSpawner = FindObjectOfType<JunkSpawner>();

        originalMoneyColor = moneyText.color;
    }
    // Update is called once per frame
    void Update()
    {
        if (gameOver)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
            }
            return;
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            Pause();
        }
        // if (Input.GetKeyDown(KeyCode.O)) {
        //     // loose the game, remove after debug
        //     GameOver();
        // }
    }

    void Pause()
    {
        paused = !paused;
        Time.timeScale = paused ? 0 : 1;
        pauseScreen.SetActive(paused);
    }

    void GameOver()
    {
        gameOverScreen.SetActive(true);
        gameOver = true;
        Time.timeScale = 0;
    }

    public void RewardForJunk(int count)
    {
        if (count == 0) return;
        ChangeMoney(count * JUNK_REWARD);
        SoundFXManager.instance.PlaySoundFXClip(getMoneyClip, transform, 1f);
    }

    void ChangeMoney(int value)
    {
        moneyBalance += value;
        moneyText.text = moneyBalance.ToString();

        if (moneyBalance < 0)
        {
            moneyText.color = Color.red;
            GameOver();
        }
    }

    public void ClearAllItems(float timeToClear) {
        IEnumerator coroutine = IterateAllItems(timeToClear);
        StartCoroutine(coroutine);
    }

    IEnumerator IterateAllItems(float timeToClear)
    {
        int totalItemCount = junkSpawner.GetJunkSet().Count + puddleSpawner.GetPuddleList().Count;
        if (totalItemCount == 0) {
            yield break;
        }
        float delay = (timeToClear / (float)totalItemCount) * 0.4f;
        delay = Math.Min(delay, 0.5f);

        // also penalty for items
        moneyText.color = Color.red;

        // spawn warnings
        foreach (GameObject junk in junkSpawner.GetJunkSet()) {
            // junkSpawner.RemoveJunk(junk);
            SpawnWarning(junk.transform.position + Vector3.up * 0.5f);
            ChangeMoney(JUNK_PENALTY);
            SoundFXManager.instance.PlaySoundFXClip(penaltyClip, junk.transform, 1f);
            yield return new WaitForSeconds(delay);
        }
        foreach (GameObject puddle in puddleSpawner.GetPuddleList()) {
            // junkSpawner.RemoveJunk(junk);
            SpawnWarning(puddle.transform.position + Vector3.up * 0.5f);
            ChangeMoney(PUDDLE_PENALTY);
            SoundFXManager.instance.PlaySoundFXClip(penaltyClip, puddle.transform, 1f);
            yield return new WaitForSeconds(delay);
        }

        // remove items
        junkSpawner.ClearAllJunk();
        puddleSpawner.ClearAllPuddles();

        // remove warnings
        foreach (GameObject warning in warnings) {
            Destroy(warning);
        }
        warnings.Clear();

        moneyText.color =  originalMoneyColor;
    }


    void SpawnWarning(Vector3 pos)
    {
        GameObject warning = Instantiate(warningPrefab, pos, Quaternion.identity);
        warnings.Add(warning);
    }
}
