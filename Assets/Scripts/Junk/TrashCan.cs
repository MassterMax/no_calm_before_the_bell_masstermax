using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCan : MonoBehaviour
{
    [SerializeField] GameObject opened;
    [SerializeField] GameObject closed;
    void Start()
    {
        Close();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Open()
    {
        opened.SetActive(true);
        closed.SetActive(false);
    }

    void Close()
    {
        opened.SetActive(false);
        closed.SetActive(true);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "player")
        {
            Open();
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "player")
        {
            Close();
        }
    }
}
