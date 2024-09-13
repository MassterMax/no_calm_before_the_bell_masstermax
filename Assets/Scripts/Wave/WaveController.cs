using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveController : MonoBehaviour
{
    int currentWave = 0;
    float waveDuration = 5f;
    float currentWaveDuration = 0;
    [SerializeField] Slider waveBar;
    KidSpawner kidSpawner;
    void Start()
    {
        StartNewWave();
        kidSpawner = FindObjectOfType<KidSpawner>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCurrentWave();

        if (Input.GetKeyDown(KeyCode.R)) {
            StartNewWave();
        }
    }

    void StartNewWave() {
        currentWaveDuration = waveDuration;
        currentWave += 1;
    }

    void UpdateCurrentWave() {
        if (currentWaveDuration == 0) return;
        currentWaveDuration = Mathf.Max(0, currentWaveDuration - Time.deltaTime);
        if (currentWaveDuration == 0) {
            IEnumerator enumerator = SpawnKids(0.5f, 5, 10);
            StartCoroutine(enumerator);
            // kidSpawner.SpawnKids(40);
        }
        // todo after some time reset bar
        waveBar.value = currentWaveDuration / waveDuration;
    }

    private IEnumerator SpawnKids(float delayTime, int batchCnt, int kidCnt)
    {
        for (int i = 0; i < batchCnt; ++i)
        {
            yield return new WaitForSeconds(delayTime);
            kidSpawner.SpawnKids(kidCnt);
        }
    }
}
