using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animation : MonoBehaviour
{
    Animator animator;
    Player player;
    SpriteRenderer sr;

    public float horizontal;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        player = GetComponent<Player>();
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        if (player.moveDir.x != 0 || player.moveDir.y != 0)
        {
            animator.SetBool("Move", true);
            SpriteDirection();
        }
        else
        {
            animator.SetBool("Move", false);
        }
        //animator.SetFloat("Horizontal", horizontal);
    }

    private void SpriteDirection()
    {
        if (player._horizontalVector < 0)
        {
            sr.flipX = true;
        }
        else
        {
            sr.flipX = false;
        }
    }
}

