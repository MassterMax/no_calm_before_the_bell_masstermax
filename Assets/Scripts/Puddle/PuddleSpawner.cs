using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuddleSpawner : MonoBehaviour
{
    [SerializeField] GameObject puddlePrefab;

    List<GameObject> puddleList = new List<GameObject>();

    float washingSpeed = 2.5f;

    float radius = 3f;

    float nearRadius = 0.66f;

    
    PlayerControl player;
    public void SetPlayer(PlayerControl player) {
        this.player = player;
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // todo remove
        if (Input.GetKeyDown(KeyCode.L)) {
            SpawnPuddle();
        }
    }

    public void SpawnPuddle() {
        GameObject newPuddle = Instantiate(puddlePrefab, UnityEngine.Random.insideUnitCircle * radius, Quaternion.identity);
        puddleList.Add(newPuddle);
    }


    public bool NearPuddle() {
        for (int i = 0; i < puddleList.Count; ++i) {
            GameObject puddle = puddleList[i];
            if ((player.transform.position - puddle.transform.position).sqrMagnitude < nearRadius * nearRadius) {
                return true;
            }
        }
        return false;
    }

    public bool TryToWashPuddle() {
        for (int i = 0; i < puddleList.Count; ++i) {
            GameObject puddle = puddleList[i];
            if ((player.transform.position - puddle.transform.position).sqrMagnitude < nearRadius * nearRadius) {
                IEnumerator  coroutine = StartWashing(i);
                StartCoroutine(coroutine);
                return true;
            }
        }
        return false;
    }

    IEnumerator StartWashing(int puddleIndex)
    {
        yield return new WaitForSeconds(washingSpeed);
        if (player.GetBetweenWaves()) {
            // abort
            yield break;
        }
        RemovePuddle(puddleIndex);
        player.StopWashing();
    }

    void RemovePuddle(int index) {
        GameObject currentPuddle = puddleList[index];
        puddleList.RemoveAt(index);
        Destroy(currentPuddle);
    }

}
