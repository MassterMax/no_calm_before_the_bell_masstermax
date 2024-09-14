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

    int currentWave = 0;
    float waveDuration = 15f;
    float currentWaveDuration = 0;
    [SerializeField] Slider waveBar;
    [SerializeField] GameObject clock;
    [SerializeField] GameObject bell;
    KidSpawner kidSpawner;
    Image fill;
    GameController gameController;
    PlayerControl playerControl; 
    void Start()
    {
        StartNewWave();
        kidSpawner = FindObjectOfType<KidSpawner>();
        playerControl = FindObjectOfType<PlayerControl>();
        fill = waveBar.fillRect.gameObject.GetComponent<Image>();
        gameController = FindObjectOfType<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
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
    }

    void UpdateCurrentWave()
    {
        if (currentWaveDuration == 0) return;
        currentWaveDuration = Mathf.Max(0, currentWaveDuration - Time.deltaTime);
        if (currentWaveDuration == 0)
        {
            // between waves
            playerControl.SetBetweenWaves(true);
            bell.SetActive(true);
            clock.SetActive(false);
            IEnumerator enumerator = SpawnKids(BETWEEN_KIDS_DELAY, BATCH_CNT, KIDS_IN_BATCH_CNT);
            StartCoroutine(enumerator);
            gameController.ClearAllItems(AFTER_WAVE_DELAY + BETWEEN_KIDS_DELAY * BATCH_CNT);
        }
        // todo after some time reset bar
        waveBar.value = currentWaveDuration / waveDuration;
        HandleWaveBarColor();
    }

    void HandleWaveBarColor() {
        if (waveBar.value >= 0.5) {
            fill.color = Color.white;
        } else if (waveBar.value >= 0.2) {
            fill.color = Color.yellow;
        } else {
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
        yield return new WaitForSeconds(AFTER_WAVE_DELAY);
        playerControl.SetBetweenWaves(false);
        StartNewWave();
    }
}
