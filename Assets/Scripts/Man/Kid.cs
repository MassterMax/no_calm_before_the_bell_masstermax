using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kid : MonoBehaviour
{
    [SerializeField] List<GameObject> heads;
    [SerializeField] List<GameObject> bodies;
    [SerializeField] List<GameObject> legs;

    Rigidbody2D rb;

    float speed = 5f;
    bool win = false;
    void Start()
    {
        int i;
        int activated;

        i = 0;
        activated = UnityEngine.Random.Range(0, heads.Count);
        foreach (GameObject head in heads)
        {
            head.SetActive(i == activated);
            i += 1;
        }

        i = 0;
        activated = UnityEngine.Random.Range(0, bodies.Count);
        foreach (GameObject body in bodies)
        {
            body.SetActive(i == activated);
            i += 1;
        }

        i = 0;
        activated = UnityEngine.Random.Range(0, legs.Count);
        foreach (GameObject leg in legs)
        {
            leg.SetActive(i == activated);
            i += 1;
        }

        // set speed
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.right * speed * UnityEngine.Random.Range(1f, 1.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (win) {
            if (0 <= transform.position.x) {
                rb.velocity = Vector2.zero;
            }
            return;
        }
        if (transform.position.x > 10) {
            Destroy(gameObject);  // destroy kid
        }
    }

    public void SetWin() {
        win = true;
    }
}
