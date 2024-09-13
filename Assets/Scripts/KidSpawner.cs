using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KidSpawner : MonoBehaviour
{
    [SerializeField] GameObject kidPrefab;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.K))
        // {
        //     for (int i = 0; i < spawnCount; ++i)
        //     {
        //         Vector2 position = Vector2.left * 10 + Vector2.up * UnityEngine.Random.Range(-4f, 3f) + Vector2.left * UnityEngine.Random.Range(1, 4);
        //         Instantiate(kidPrefab, position, Quaternion.identity);
        //     }
        // }
    }

    public void SpawnKids(int count) {
        for (int i = 0; i < count; ++i)
            {
                Vector2 position = Vector2.left * 10 + Vector2.up * UnityEngine.Random.Range(-4f, 3f) + Vector2.left * UnityEngine.Random.Range(1, 4);
                Instantiate(kidPrefab, position, Quaternion.identity);
            }
    }
}
