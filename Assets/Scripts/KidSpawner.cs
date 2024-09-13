using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KidSpawner : MonoBehaviour
{
    [SerializeField] GameObject kidPrefab;

    int spawnCount = 5;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            for (int i = 0; i < spawnCount; ++i)
            {
                Vector2 position = Vector2.left * 10 + Vector2.up * UnityEngine.Random.Range(-4f, 3f);
                Instantiate(kidPrefab, position, Quaternion.identity);
            }
        }
    }
}
