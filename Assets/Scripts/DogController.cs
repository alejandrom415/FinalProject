using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogController : MonoBehaviour
{
    public float speed;
    public float changeTime = 3.0f;
    Rigidbody2D rigidbody2D;
    float timer;
    int direction = 1;
    Animator animator;

    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        timer = changeTime;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            direction = -direction;
            timer = changeTime;
        }
    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2D.position;

        position.x = position.x + Time.deltaTime * speed * direction;
        animator.SetFloat("Move X", direction);
        rigidbody2D.MovePosition(position);
    }
}