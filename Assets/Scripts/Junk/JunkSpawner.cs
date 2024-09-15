using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JunkSpawner : MonoBehaviour
{
    [SerializeField] GameObject junkPrefab;

    HashSet<GameObject> junkSet = new HashSet<GameObject>();

    void Start()
    {

    }

    void Update()
    {
    }

    public void SpawnJunk(int cnt)
    {
        for (int j = 0; j < cnt; ++j)
        {
            // -8 ... 8, -2 ... 2
            Vector2 pos = Vector2.right * Random.Range(-6f,6f) + Vector2.up * Random.Range(-3.5f, 2f);
            GameObject junk = Instantiate(junkPrefab, pos, Quaternion.identity);
            int i = 0;
            int activated = UnityEngine.Random.Range(0, 3);
            foreach (Transform child in junk.GetComponentInChildren<Transform>())
            {
                child.gameObject.SetActive(i == activated);
                i += 1;
            }
            junkSet.Add(junk);
        }
    }

    public void RemoveJunk(GameObject junk)
    {
        junkSet.Remove(junk);
        Destroy(junk);
    }

    public HashSet<GameObject> GetJunkSet()
    {
        return junkSet;
    }

    public void ClearAllJunk()
    {
        foreach (GameObject junk in junkSet)
        {
            Destroy(junk);
        }
        junkSet.Clear();
    }
}
