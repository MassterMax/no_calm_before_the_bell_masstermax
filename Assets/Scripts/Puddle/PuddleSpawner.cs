using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuddleSpawner : MonoBehaviour
{
    [SerializeField] GameObject puddlePrefab;

    List<GameObject> puddleList = new List<GameObject>();

    public const float WASHING_SPEED = 2.5f;


    float nearRadius = 0.66f;


    PlayerControl player;
    public void SetPlayer(PlayerControl player)
    {
        this.player = player;
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SpawnPuddle(int cnt)
    {
        for (int i = 0; i < cnt; ++i) {
            Vector2 pos = Vector2.right * UnityEngine.Random.Range(-8f, 8f) + Vector2.up * UnityEngine.Random.Range(-3.5f, 2f);
            GameObject newPuddle = Instantiate(puddlePrefab, pos, Quaternion.identity);
            puddleList.Add(newPuddle);
        }
    }


    public bool NearPuddle()
    {
        for (int i = 0; i < puddleList.Count; ++i)
        {
            GameObject puddle = puddleList[i];
            if ((player.transform.position - puddle.transform.position).sqrMagnitude < nearRadius * nearRadius)
            {
                return true;
            }
        }
        return false;
    }

    public bool TryToWashPuddle()
    {
        for (int i = 0; i < puddleList.Count; ++i)
        {
            GameObject puddle = puddleList[i];
            if ((player.transform.position - puddle.transform.position).sqrMagnitude < nearRadius * nearRadius)
            {
                IEnumerator coroutine = StartWashing(i);
                StartCoroutine(coroutine);
                return true;
            }
        }
        return false;
    }

    IEnumerator StartWashing(int puddleIndex)
    {
        yield return new WaitForSeconds(WASHING_SPEED);
        if (player.GetBetweenWaves())
        {
            // abort
            yield break;
        }
        RemovePuddle(puddleIndex);
        player.StopWashing();
    }

    void RemovePuddle(int index)
    {
        GameObject currentPuddle = puddleList[index];
        puddleList.RemoveAt(index);
        Destroy(currentPuddle);
    }

    public List<GameObject> GetPuddleList()
    {
        return puddleList;
    }

    public void ClearAllPuddles()
    {
        foreach (GameObject puddle in puddleList)
        {
            Destroy(puddle);
        }
        puddleList.Clear();
    }
}
