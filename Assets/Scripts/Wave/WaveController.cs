using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveController : MonoBehaviour
{
    const int BATCH_CNT = 10;
    const int KIDS_IN_BATCH_CNT = 20;
    const float BETWEEN_KIDS_DELAY = 0.5f;
    const float AFTER_WAVE_DELAY = 5f;
    const float BEFORE_SPAWN_DELAY = 0f;
    public const int TOTAL_WAVES = 2;
    const float CLEAN_FLOOR_BOOST = 10f;

    int currentWave = 0;
    float waveDuration = 30f; // 30f;
    float currentWaveDuration = 0;
    [SerializeField] Slider waveBar;
    [SerializeField] GameObject clock;
    [SerializeField] GameObject bell;
    KidSpawner kidSpawner;
    Image fill;
    GameController gameController;
    PlayerControl playerControl;
    [SerializeField] AudioClip bellClip;
    [SerializeField] Text currentWaveText;
    [SerializeField] AudioClip winClip;
    bool ended = false;

    void Start()
    {
        kidSpawner = FindObjectOfType<KidSpawner>();
        playerControl = FindObjectOfType<PlayerControl>();
        fill = waveBar.fillRect.gameObject.GetComponent<Image>();
        gameController = FindObjectOfType<GameController>();
        StartNewWave();
        gameController.NewWave(currentWave);
    }

    // Update is called once per frame
    void Update()
    {
        if (!ended)
            UpdateCurrentWave();

        // if (Input.GetKeyDown(KeyCode.R))
        // {
        //     StartNewWave();
        // }
    }

    void StartNewWave()
    {
        bell.SetActive(false);
        clock.SetActive(true);
        currentWaveDuration = waveDuration;
        currentWave += 1;
        // currentWaveText.text = "  lesson: " + currentWave.ToString() + "/" + TOTAL_WAVES.ToString();

        if (currentWave == TOTAL_WAVES + 1) {
            currentWaveText.text = "  lessons are over!";
            ended = true;
            SoundFXManager.instance.PlaySoundFXClip(winClip, transform, 0.15f);
            gameController.EndGame();
        } else {
            currentWaveText.text = "  lesson: " + currentWave.ToString() + "/" + TOTAL_WAVES.ToString();
        }
    }

    void UpdateCurrentWave()
    {
        if (currentWaveDuration == 0) return;
        if (playerControl.GetTrashCount() == 0 && gameController.FloorIsClean())
        {
            currentWaveDuration = Mathf.Max(0, currentWaveDuration - Time.deltaTime * CLEAN_FLOOR_BOOST);
        }
        else
        {
            currentWaveDuration = Mathf.Max(0, currentWaveDuration - Time.deltaTime);
        }
        if (currentWaveDuration == 0)
        {
            // SPAWN NEW WAVE
            playerControl.SetBetweenWaves(true);
            bell.SetActive(true);
            clock.SetActive(false);
            IEnumerator enumerator = SpawnKids(BETWEEN_KIDS_DELAY, BATCH_CNT, KIDS_IN_BATCH_CNT);
            StartCoroutine(enumerator);
            gameController.ClearAllItems(AFTER_WAVE_DELAY + BETWEEN_KIDS_DELAY * BATCH_CNT);
            SoundFXManager.instance.PlaySoundFXClip(bellClip, transform, 0.66f);
        }
        // todo after some time reset bar
        waveBar.value = currentWaveDuration / waveDuration;
        HandleWaveBarColor();
    }

    void HandleWaveBarColor()
    {
        if (waveBar.value >= 0.5)
        {
            fill.color = Color.white;
        }
        else if (waveBar.value >= 0.2)
        {
            fill.color = Color.yellow;
        }
        else
        {
            fill.color = Color.red;
        }
    }

    private IEnumerator SpawnKids(float delayTime, int batchCnt, int kidCnt)
    {
        for (int i = 0; i < batchCnt; ++i)
        {
            kidSpawner.SpawnKids(kidCnt);
            yield return new WaitForSeconds(delayTime);

        }
        yield return new WaitForSeconds(BEFORE_SPAWN_DELAY);
        gameController.NewWave(currentWave + 1);
        yield return new WaitForSeconds(AFTER_WAVE_DELAY - BEFORE_SPAWN_DELAY);
        StartNewWave();
        playerControl.SetBetweenWaves(false);
    }
}
