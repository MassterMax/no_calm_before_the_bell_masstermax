using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;


public class PlayerControl : MonoBehaviour
{
    const float PLAYER_SPEED = 4f;
    float playerSpeed;

    Dictionary<KeyCode, Vector2> keyToVector = new Dictionary<KeyCode, Vector2>() {
        { KeyCode.W, Vector2.up},
        { KeyCode.S, Vector2.down },
        { KeyCode.D, Vector2.right },
        { KeyCode.A, Vector2.left }
    };

    Vector2 direction;
    bool isWashing = false;
    Rigidbody2D rb;

    // Components
    SpriteRenderer playerSpriteRenderer;
    Animator animator;

    PuddleSpawner puddleSpawner;

    GameObject broom;
    GameObject trashBag;
    GameObject sweat;
    SpriteRenderer trashBagSpriteRenderer;
    int trashCount = 0;

    float trashBagScale = 1.2f;
    float playerSpeedScale = 0.75f;

    float playerSlowNearPuddle = 0.5f;

    Dictionary<Vector3, Collider2D> collidedJunk = new Dictionary<Vector3, Collider2D>();

    bool nearTrashCan = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        puddleSpawner = FindObjectOfType<PuddleSpawner>();
        puddleSpawner.SetPlayer(this);

        foreach (Transform child in GetComponentInChildren<Transform>())
        {
            if (child.name == "broom")
            {
                broom = child.gameObject;
                broom.SetActive(false);
            }
            else if (child.name == "trash-bag")
            {
                trashBag = child.gameObject;
                trashBagSpriteRenderer = trashBag.GetComponent<SpriteRenderer>();
            }
            else if (child.name == "sweat")
            {
                sweat = child.gameObject;
            }
        }

        sweat.SetActive(false);
        ClearTrashCount();
    }

    void Update()
    {
        Move();
        Rotate();
        HandleRunningAnimation();
        HandlePuddleWashing();
        HandleJunk();
    }

    void Move()
    {
        direction = Vector2.zero;

        if (isWashing) { }
        else
        {
            foreach (var pair in keyToVector)
            {
                if (Input.GetKey(pair.Key))
                {
                    direction += pair.Value;
                }
            }
        }
        direction = direction.normalized;

        if (puddleSpawner.NearPuddle())
        {
            rb.velocity = (Vector3)direction * playerSpeed * playerSlowNearPuddle;
        }
        else {
            rb.velocity = (Vector3)direction * playerSpeed;
        }
    }

    void Rotate()
    {
        if (direction.x == 0)
        {
            return;
        }
        if (playerSpriteRenderer.flipX != Mathf.Sign(direction.x) < 0)
        {
            playerSpriteRenderer.flipX = !playerSpriteRenderer.flipX;
            trashBagSpriteRenderer.flipX = playerSpriteRenderer.flipX;
            trashBag.transform.localPosition = new Vector3(-trashBag.transform.localPosition.x, trashBag.transform.localPosition.y, trashBag.transform.localPosition.z);
        }
    }

    void HandleRunningAnimation()
    {
        animator.SetBool("isRunning", direction.sqrMagnitude != 0);
    }

    void HandlePuddleWashing()
    {
        if (isWashing) return;

        if (puddleSpawner.NearPuddle())
        {
            broom.SetActive(true);
            sweat.SetActive(true);
            if (Input.GetKeyDown(KeyCode.Space) && puddleSpawner.TryToWashPuddle())
            {
                broom.SetActive(false);
                isWashing = true;
                animator.SetBool("isWashing", isWashing);
            }
        }
        else
        {
            sweat.SetActive(false);
            broom.SetActive(false);
        }
    }

    public void StopWashing()
    {
        isWashing = false;
        animator.SetBool("isWashing", isWashing);
    }



    void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("Enter " + collider.gameObject.tag);
        if (collider.gameObject.tag == "junk")
        {
            if (collidedJunk.ContainsKey(collider.transform.position))
            {
                throw new System.Exception();
            }
            collidedJunk.Add(collider.transform.position, collider);
        }
        else if (collider.gameObject.tag == "trash-can")
        {
            nearTrashCan = true;
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        Debug.Log("Exit " + collider.gameObject.tag);
        if (collider.gameObject.tag == "junk")
        {
            collidedJunk.Remove(collider.transform.position);
        }
        else if (collider.gameObject.tag == "trash-can")
        {
            nearTrashCan = false;
        }
    }

    void HandleJunk()
    {
        if (isWashing) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (collidedJunk.Count > 0)
            {
                Debug.Log("inside HandleJunk: try to destory first junk");
                float distance = -1;
                Vector3 preferedJunk = Vector3.zero;

                foreach (Vector3 pos in collidedJunk.Keys)
                {
                    if (distance == -1 || distance > (pos - transform.position).sqrMagnitude)
                    {
                        distance = (pos - transform.position).sqrMagnitude;
                        preferedJunk = pos;
                    }
                }

                GameObject junk = collidedJunk[preferedJunk].gameObject;
                collidedJunk.Remove(preferedJunk);
                Destroy(junk);
                IncreaseTrashCount();
            }
            else if (nearTrashCan)
            {
                Debug.Log("inside HandleJunk: try to drop junk to can");
                ClearTrashCount();
            }
        }
    }

    void IncreaseTrashCount()
    {
        trashCount += 1;
        trashBag.transform.localScale *= trashBagScale;
        playerSpeed *= playerSpeedScale;
        trashBag.SetActive(true);
    }

    void ClearTrashCount()
    {
        trashCount = 0;
        trashBag.transform.localScale = Vector3.one;
        playerSpeed = PLAYER_SPEED;
        trashBag.SetActive(false);
    }
}
