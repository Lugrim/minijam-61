﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [Header("Game Objects")]
    public Rigidbody2D rb;

    [Header("Multipliers")]
    public float moveMultiplier = 7f;
    public float jumpVelocity = 5f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    [Header("Attributes")]
    public int maxJumpNumber = 1;
    public bool isAlive = true;

	AudioSource audioSource;
    Animator animator;
    bool jumpRequest;
    int jumpNb;

    float endOfCooldownTime = 0.0f;
    LeverScript leverToInteract = null;

    GameManager meneger = null;

    bool isPressedFire2 = false;
    // Rigidbody2D rbFeet;
    bool onPlant;

    // Start is called before the first frame update
    void Start()
    {
        if(rb==null)
            rb = GetComponent<Rigidbody2D>();
		audioSource = GetComponent<AudioSource>();
        // timer = changeTime;
        animator = GetComponent<Animator>();
        jumpRequest = false;
        jumpNb = 0;
        // rbFeet = transform.GetChild(0).GetComponent<Rigidbody2D>();

        meneger = GameManager.Instance;
        onPlant = false;
    }

    private void Update() {
        if (GameManager.Instance.Interacting)
            return; 

        if ((rb.velocity.y < 0.01f) && (rb.velocity.y > -0.01f)) {
            animator.SetFloat("SpeedY", 0.0f);
        } else if (rb.velocity.y > 0) {
            animator.SetFloat("SpeedY", 1.0f);
        } else {
            animator.SetFloat("SpeedY", -1.0f);
        }
        if ((Input.GetAxis("Horizontal") < 0.01f) && (Input.GetAxis("Horizontal") > -0.01f)) {
            animator.SetFloat("SpeedX", 0.0f);
        } else if (Input.GetAxis("Horizontal") > 0) {
            animator.SetFloat("SpeedX", 1.0f);
        } else {
            animator.SetFloat("SpeedX", -1.0f);
        }

        if (Input.GetButtonDown ("Jump") && jumpNb < maxJumpNumber)
        {
            jumpRequest = true;
            jumpNb++;
        }
        if (!isPressedFire2) {
            if (Input.GetButton("Fire2")) {
                isPressedFire2 = true;
                meneger.blocPosition = !meneger.blocPosition;
                // Debug.Log("Fire !");
            }
        } else {
            if (!Input.GetButton("Fire2")) {
                isPressedFire2 = false;
                // Debug.Log("End");
            }
        }
        if (endOfCooldownTime < Time.time) {
            if (leverToInteract != null) {
                if (Input.GetButton("Fire1")){
                    // Debug.Log("Hep !");
                    endOfCooldownTime = Time.time + 0.5f;
                    leverToInteract.Interact();
                }
            }
        }
        // Debug.Log(rb.velocity);
    }

    // Called every physics frame
    private void FixedUpdate()
    {
        // var epsilon = 0.00000000000001;
        if (GameManager.Instance.Interacting)
            return;

        if (jumpRequest) //Jump impulsion
        {
            // Debug.Log(rb.velocity.y);
            // NOTE : The line below has weird behavior when standing on the limits of platforms (rb.volicity < 0)
            // As it was designed for double-jump, and we don't use it, we'll use a simpler version
            //rb.AddForce(Vector2.up * jumpVelocity * (rb.velocity.y < -0.01 ? 2 : 1), ForceMode2D.Impulse);
            // if (rb.velocity.y>-1 * epsilon) {
                rb.AddForce(Vector2.up * jumpVelocity, ForceMode2D.Impulse);
            // }
            jumpRequest = false;
        }
        if (!onPlant)
        {
            if (rb.velocity.y < 0) // Falling down
            {
                rb.gravityScale = fallMultiplier;
            }
            else if (rb.velocity.y > 0 && !Input.GetButtonDown("Jump")) // Jump gradient
            {
                rb.gravityScale = lowJumpMultiplier;
            }
            else // Reset gravity
            {
                rb.gravityScale = 1f;
            }
        }
        else
        {
            rb.gravityScale = 0f;
        }
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), onPlant ? Input.GetAxis("Vertical") : 0f, 0f);
        transform.position += movement * Time.deltaTime * moveMultiplier;
        //float tileNumber = Mathf.FloorToInt(transform.position.x);
        //Debug.Log(tileNumber);
    }

    void Death() {
        audioSource.Play();
        GameManager.Instance.blocPosition = false;
        try {
            GameManager.Instance.TriggerPrefabsReset();
        } catch (NullReferenceException) {}
        Camera.main.transform.position = GameManager.Instance.respawnCamera.transform.position;
        transform.position = GameManager.Instance.respawnLocation.transform.position;
    }

    // Manages whether the character is on the groud or not
    private void OnTriggerEnter2D(Collider2D collision)
    {
		if (collision.tag == "Spike") {
			this.Death();
		} else if (collision.tag == "Lever") {
            leverToInteract = collision.gameObject.GetComponent<LeverScript>();
        } else if (collision.tag == "Plant") {
            onPlant = true;
            rb.velocity = new Vector2(rb.velocity.x, 0f);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Plant")
        {
            onPlant = false;
        }
        leverToInteract = null;
    }

    public void FeetCollision(bool activate = true)
    {
        // if (jumpNb != 0) {
        //     Debug.Log("Reset !");
        // }
        if (activate) {
            jumpNb = 0;
        } else {
            jumpNb = 42;
        }
    }

    // private void OnTriggerStay2D(Collider2D other){
    //     if (endOfCooldownTime < Time.time) {
    //         if (other.tag == "Lever") {
    //             if (Input.GetButton("Fire3") || Input.GetButton("Fire2") || Input.GetButton("Fire1")){
    //                 Debug.Log("Hep !");
    //                 endOfCooldownTime = Time.time + 1;
    //                 // LeverScript machin = other.gameObject.GetComponent<LeverScript>();
    //                 // machin.Interact();
    //             }
    //         }
    //     }
    // }
}
