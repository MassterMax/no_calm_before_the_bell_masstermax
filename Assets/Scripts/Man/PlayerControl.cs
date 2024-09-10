using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerControl : MonoBehaviour
{
    [SerializeField] float playerSpeed;


    Dictionary<KeyCode, Vector2> keyToVector = new Dictionary<KeyCode, Vector2>() {
        { KeyCode.W, Vector2.up},
        { KeyCode.S, Vector2.down },
        { KeyCode.D, Vector2.right },
        { KeyCode.A, Vector2.left }
    };

    Vector2 direction;
    bool isWashing = false;

    // Components
    SpriteRenderer playerSpriteRenderer;
    Animator animator;

    PuddleSpawner puddleSpawner;

    GameObject broom;

    // Start is called before the first frame update
    void Start()
    {
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        puddleSpawner = FindObjectOfType<PuddleSpawner>();
        puddleSpawner.SetPlayer(this);
        broom = transform.GetChild(0).gameObject;
        broom.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Rotate();
        HandleRunningAnimation();
        HandlePuddleWashing();
    }

    void Move()
    {
        if (isWashing) return;

        direction = Vector2.zero;

        foreach (var pair in keyToVector)
            if (Input.GetKey(pair.Key)) {
                direction += pair.Value;
            }

        direction = direction.normalized;


        transform.Translate(direction * Time.deltaTime * playerSpeed);
    }

    void Rotate()
    {
        // skip if paused, todo -> make better!
        // if (Time.timeScale == 0f) return;

        if (direction.x == 0)
        {
            return;
        }
        if (playerSpriteRenderer.flipX != Mathf.Sign(direction.x) < 0)
        {
            playerSpriteRenderer.flipX = !playerSpriteRenderer.flipX;
        }
    }

    void HandleRunningAnimation()
    {
        animator.SetBool("isRunning", direction.sqrMagnitude != 0);
    }

    void HandlePuddleWashing()
    {
        if (isWashing) return;

        if (puddleSpawner.NearPuddle()) {
            broom.SetActive(true);
            if (Input.GetKeyDown(KeyCode.Space) && puddleSpawner.TryToWashPuddle()) {
                broom.SetActive(false);
                isWashing = true;
                animator.SetBool("isWashing", isWashing);
            }
        } else {
            broom.SetActive(false);
        }
    }

    public void StopWashing() 
    {
        isWashing = false;
        animator.SetBool("isWashing", isWashing);
    }

}
