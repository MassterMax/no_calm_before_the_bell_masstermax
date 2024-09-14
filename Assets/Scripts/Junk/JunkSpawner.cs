using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JunkSpawner : MonoBehaviour
{
    [SerializeField] GameObject junkPrefab;
    float radius = 3f;

    HashSet<GameObject> junkSet = new HashSet<GameObject>();

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            SpawnJunk();
        }
    }

    public void SpawnJunk()
    {
        GameObject junk = Instantiate(junkPrefab, UnityEngine.Random.insideUnitCircle * radius, Quaternion.identity);
        int i = 0;
        int activated = UnityEngine.Random.Range(0, 3);
        foreach (Transform child in junk.GetComponentInChildren<Transform>())
        {
            child.gameObject.SetActive(i == activated);
            i += 1;
        }
        junkSet.Add(junk);
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

    public void ClearAllJunk() {
        foreach (GameObject junk in junkSet) {
            Destroy(junk);
        }
        junkSet.Clear();
    }
}
