﻿using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [Header("Game Objects")]
    public Rigidbody2D rb;

    [Header("Multipliers")]
    public Vector2 movesMultiplier;
    public float airControl = 0.5f;

    [Header("Attributes")]
    public bool isAlive = true;

    JumpController jc;
	AudioSource audioSource;
    Animator animator;
    Vector2 lookDirection = new Vector2(1,0);
	
    // Start is called before the first frame update
    void Start()
    {
        if(rb==null)
            rb = GetComponent<Rigidbody2D>();
        jc = GetComponentInChildren<JumpController>();
		audioSource = GetComponent<AudioSource>();
        // timer = changeTime;
        animator = GetComponent<Animator>();
    }

    void Update() {
        if (rb.velocity.x > 0) {
            animator.SetFloat("Move X", (float)0.5);
        } else if (rb.velocity.x < 0) {
            animator.SetFloat("Move X", (float)-0.5);
        } else {
            animator.SetFloat("Move X", (float)0.0);
        }
        if (rb.velocity.y > 0) {
            animator.SetFloat("Move Y", (float)0.5);
        } else if (rb.velocity.y < 0) {
            animator.SetFloat("Move Y", (float)-0.5);
        } else {
            animator.SetFloat("Move Y", (float)0.0);
        }
        // Debug.Log(rb.velocity);
    }

    // Called every physics frame
    private void FixedUpdate()
    {
        rb.AddForce(movesMultiplier * new Vector2(Input.GetAxis("Horizontal") * (jc.onGround ? 1 : airControl), 0));
		float tileNumber = Mathf.FloorToInt(transform.position.x);
		//Debug.Log(tileNumber);
    }
	
    void Death() {
        if (this.isAlive) {
            this.isAlive = false;
            Debug.Log("Dead !");
			audioSource.Play();
        }
    }

    // Manages whether the character is on the groud or not
    private void OnTriggerEnter2D(Collider2D collision)
    {
		if (collision.tag == "Spike") {
			this.Death();
		}
    }
	
}
