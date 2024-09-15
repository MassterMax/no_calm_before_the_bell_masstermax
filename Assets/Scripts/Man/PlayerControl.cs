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
    JunkSpawner junkSpawner;

    GameObject broom;
    GameObject trashBag;
    GameObject sweat;
    SpriteRenderer trashBagSpriteRenderer;
    int trashCount = 0;

    float trashBagScale = 1.2f;
    float playerSpeedScale = 0.85f;

    float playerSlowNearPuddle = 0.5f;

    // Dictionary<GameObject, Collider2D> collidedJunk = new Dictionary<GameObject, Collider2D>();
    HashSet<GameObject> collidedJunkSet = new HashSet<GameObject>();

    bool nearTrashCan = false;

    bool betweenWaves = false;

    [SerializeField] AudioClip cleaningClip;
    [SerializeField] AudioClip pickupClip;

    public void SetBetweenWaves(bool value)
    {
        betweenWaves = value;
        if (value)
        {
            playerSpeed = PLAYER_SPEED;
        }
        else
        {
            playerSpeed = PLAYER_SPEED * Mathf.Pow(playerSpeedScale, trashCount);
        }
    }

    public bool GetBetweenWaves()
    {
        return betweenWaves;
    }

    GameController gameController;

    // Start is called before the first frame update
    void Start()
    {
        gameController = FindObjectOfType<GameController>();
        rb = GetComponent<Rigidbody2D>();
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        puddleSpawner = FindObjectOfType<PuddleSpawner>();
        puddleSpawner.SetPlayer(this);
        junkSpawner = FindObjectOfType<JunkSpawner>();

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
        if (Time.timeScale == 0) return; // todo maybe remove
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
        else if (betweenWaves)
        {
            // todo meybe return 
            // direction = -transform.position;
        }
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

        if (puddleSpawner.NearPuddle() && !betweenWaves)
        {
            rb.velocity = (Vector3)direction * playerSpeed * playerSlowNearPuddle;
        }
        else
        {
            rb.velocity = (Vector3)direction * playerSpeed;
        }

        // todo meybe return
        // if (betweenWaves && transform.position.sqrMagnitude < 0.01)
        // {
        //     transform.position = Vector2.zero;
        // }
    }

    void Rotate()
    {
        if (direction.x == 0 || Time.timeScale == 0)
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
        if (betweenWaves)
        {
            broom.SetActive(false);
            sweat.SetActive(false);
            StopWashing();
            return;
        }
        if (isWashing)
        {
            return;
        }

        if (puddleSpawner.NearPuddle())
        {
            broom.SetActive(true);
            sweat.SetActive(true);
            if (Input.GetKeyDown(KeyCode.Space) && puddleSpawner.TryToWashPuddle())
            {
                broom.SetActive(false);
                isWashing = true;
                animator.SetBool("isWashing", isWashing);
                SoundFXManager.instance.PlaySoundFXClip(cleaningClip, transform, 1f, PuddleSpawner.WASHING_SPEED);
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
            collidedJunkSet.Add(collider.gameObject);
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
            collidedJunkSet.Remove(collider.gameObject);
        }
        else if (collider.gameObject.tag == "trash-can")
        {
            nearTrashCan = false;
        }
    }


    void HandleJunk()
    {
        if (isWashing || betweenWaves) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (collidedJunkSet.Count > 0)
            {
                Debug.Log("inside HandleJunk: try to destory first junk");
                float distance = -1;
                GameObject preferedJunk = null;

                foreach (GameObject collidedJunk in collidedJunkSet)
                {
                    if (distance == -1 || distance > (collidedJunk.transform.position - transform.position).sqrMagnitude)
                    {
                        distance = (collidedJunk.transform.position - transform.position).sqrMagnitude;
                        preferedJunk = collidedJunk;
                    }
                }
                collidedJunkSet.Remove(preferedJunk);
                junkSpawner.RemoveJunk(preferedJunk);
                IncreaseTrashCount();
                SoundFXManager.instance.PlaySoundFXClip(pickupClip, transform, 1f);
            }
            else if (nearTrashCan && trashCount > 0)
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
        gameController.RewardForJunk(trashCount);
        trashCount = 0;
        trashBag.transform.localScale = Vector3.one;
        playerSpeed = PLAYER_SPEED;
        trashBag.SetActive(false);
    }

    public int GetTrashCount() {
        return trashCount;
    }
}
