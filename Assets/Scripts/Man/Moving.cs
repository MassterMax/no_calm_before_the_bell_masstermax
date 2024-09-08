using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Moving : MonoBehaviour
{
    [SerializeField] float playerSpeed;


    Dictionary<KeyCode, Vector2> keyToVector = new Dictionary<KeyCode, Vector2>() {
        { KeyCode.W, Vector2.up},
        { KeyCode.S, Vector2.down },
        { KeyCode.D, Vector2.right },
        { KeyCode.A, Vector2.left }
    };

    Vector2 direction;

    // Components
    SpriteRenderer playerSpriteRenderer;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Rotate();
        HandleAnimation();
    }

    void Move()
    {
        direction = Vector2.zero;

        foreach (var pair in keyToVector)
            if (Input.GetKey(pair.Key))
                direction += pair.Value;

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

    void HandleAnimation()
    {
        animator.SetBool("isRunning", direction.sqrMagnitude != 0);
    }

}
