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
    [SerializeField] AudioClip gameOverClip;

    const int JUNK_REWARD = 1;
    const int JUNK_PENALTY = -2;
    const int PUDDLE_PENALTY = -4;
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

    [SerializeField] AudioSource backMusic;
    [SerializeField] GameObject winPane;

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
            else if (Input.GetKeyDown(KeyCode.M))
            {
                SceneManager.LoadScene("StartScene", LoadSceneMode.Single);
            }
            return;
        }

        if (paused && Input.GetKeyDown(KeyCode.M))
        {
            SceneManager.LoadScene("StartScene", LoadSceneMode.Single);
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
        backMusic.Stop();
        SoundFXManager.instance.PlaySoundFXClip(gameOverClip, transform, 0.15f);
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

    public void ClearAllItems(float timeToClear)
    {
        IEnumerator coroutine = IterateAllItems(timeToClear);
        StartCoroutine(coroutine);
    }

    IEnumerator IterateAllItems(float timeToClear)
    {
        int totalItemCount = junkSpawner.GetJunkSet().Count + puddleSpawner.GetPuddleList().Count;
        if (totalItemCount == 0)
        {
            yield break;
        }
        float delay = (timeToClear / (float)totalItemCount) * 0.4f;
        delay = Math.Min(delay, 0.5f);

        // also penalty for items
        moneyText.color = Color.red;

        // spawn warnings
        foreach (GameObject junk in junkSpawner.GetJunkSet())
        {
            // junkSpawner.RemoveJunk(junk);
            SpawnWarning(junk.transform.position + Vector3.up * 0.5f);
            ChangeMoney(JUNK_PENALTY);
            SoundFXManager.instance.PlaySoundFXClip(penaltyClip, junk.transform, 1f);
            yield return new WaitForSeconds(delay);
        }
        foreach (GameObject puddle in puddleSpawner.GetPuddleList())
        {
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
        foreach (GameObject warning in warnings)
        {
            Destroy(warning);
        }
        warnings.Clear();

        moneyText.color = originalMoneyColor;
    }


    void SpawnWarning(Vector3 pos)
    {
        GameObject warning = Instantiate(warningPrefab, pos, Quaternion.identity);
        warnings.Add(warning);
    }

    public void NewWave(int wave)
    {
        if (wave == WaveController.TOTAL_WAVES + 1) {
            return;
        }
        if (wave == 1)
        {
            IEnumerator coroutine = WaitForInitAndSpawn();
            StartCoroutine(coroutine);
        }
        else if (wave < 4)
        {
            junkSpawner.SpawnJunk(wave * 3);
        }
        else if (wave < 7)
        {
            junkSpawner.SpawnJunk((wave - 3) * 5);
            puddleSpawner.SpawnPuddle((int)(Mathf.Pow(2, (wave - 4)) + 0.01f));
        }
        else if (wave < 10)
        {
            junkSpawner.SpawnJunk(wave + 5);
            puddleSpawner.SpawnPuddle(wave);
        }
        else if (wave == 10)
        {
            junkSpawner.SpawnJunk(wave + 10);
            puddleSpawner.SpawnPuddle(wave + 5);
        }
        // int junkCnt = 1;
        // int puddleCnt = 1;
        // puddleSpawner.SpawnPuddle(puddleCnt);
        // junkSpawner.SpawnJunk(junkCnt);
    }

    IEnumerator WaitForInitAndSpawn()
    {
        while (puddleSpawner == null && junkSpawner == null)
        {
            yield return new WaitForSeconds(0.1f);
        }
        junkSpawner.SpawnJunk(3);
    }

    public bool FloorIsClean()
    {
        return puddleSpawner.GetPuddleList().Count + junkSpawner.GetJunkSet().Count == 0;
    }

    public void EndGame() {
        backMusic.Stop();
        // todo other staff
        // spawn kiddo
        // when kiddo on end - spawn pane
        StartCoroutine(Win());
    }

    IEnumerator Win()
    {
        GameObject kid = FindObjectOfType<KidSpawner>().SpawnWinKid();
        while (kid.transform.position.x < 0) {
            yield return new WaitForSeconds(0.2f);
        }
        winPane.SetActive(true);
        // kid.GetComponent<Animator>().speed = 0;
    }
}
