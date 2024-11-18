using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;

    public float groundDrag;

    [Header("Keybinds")]
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 movementDirection;

    Rigidbody rb;

    public MovementState state;

    public enum MovementState
    {
        walking,
        sprinting,
        crouching
    }

    [Header("Audio")]
    public AudioClip walkingSound; // Walking sound effect
    private AudioSource audioSource;
    private bool isPlayingWalkingSound = false;

    public AudioClip sprintingSound; // Sprinting sound effect
    private AudioSource sprintAudio;
    private bool isPlayingSprintingSound = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        startYScale = transform.localScale.y;

        // Initialize audio source
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = walkingSound;
        audioSource.loop = true; // Enable looping for walking sound
        audioSource.playOnAwake = false; // Don't play immediately

        sprintAudio = gameObject.AddComponent<AudioSource>();
        sprintAudio.clip = sprintingSound;
        sprintAudio.loop = true; // Enable looping for sprinting sound
        sprintAudio.playOnAwake = false; // Don't play immediately
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();

        // Control drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

        HandleWalkingSound();
        HandleSprintingSound();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Start crouching
        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        // Stop crouching
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    private void StateHandler()
    {
        // Mode - crouching
        if (Input.GetKeyDown(crouchKey))
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }

        // Mode - sprinting
        if (grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }

        // Mode - walking
        else if (grounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }
    }

    private void MovePlayer()
    {
        movementDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        rb.AddForce(movementDirection.normalized * moveSpeed * 10f, ForceMode.Force);
    }

    void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // Limit velocity if needed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void HandleWalkingSound()
    {
        // Play sound only in walking state and when moving
        if (state == MovementState.walking && (horizontalInput != 0 || verticalInput != 0))
        {
            if (!isPlayingWalkingSound)
            {
                audioSource.Play();
                isPlayingWalkingSound = true;
            }
        }
        else
        {
            if (isPlayingWalkingSound)
            {
                audioSource.Stop();
                isPlayingWalkingSound = false;
            }
        }
    }

    private void HandleSprintingSound()
    {
        // Play sound only in walking state and when moving
        if (state == MovementState.sprinting && (horizontalInput != 0 || verticalInput != 0))
        {
            if (!isPlayingSprintingSound)
            {
                sprintAudio.Play();
                isPlayingSprintingSound = true;
            }
        }
        else
        {
            if (isPlayingSprintingSound)
            {
                sprintAudio.Stop();
                isPlayingSprintingSound = false;
            }
        }
    }
}
