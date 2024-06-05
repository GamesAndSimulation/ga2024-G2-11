using System;
using System.Collections;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [Header("References")]
    public PlayerCam playerCam;
    public Transform orientation;
    public Camera cam;
    private InputManager _inputManager;
    public Rigidbody rb;
    
    

    [Header("Movement")]
    public float walkSpeed;
    public float sprintSpeed;
    private float moveSpeed;
    private float initialWalkSpeed;
    public float maxYSpeed;
    public float groundDrag;
    public bool canMove = true;

    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;

    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        dashing,
        air
    }

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchHeight;
    public float standingHeight;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded { get; private set; }
    public Transform groundCheck;
    public float groundDistance = 0.4f;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    
    [Header("Sound")]
    public AudioClip[] footstepSounds;
    public float footstepSoundCooldownTime = 0.43f;
    private float footstepSoundCooldownTimer;

    [Header("Quake Camera Rolling")]
    public float rollSpeed;
    public float maxRoll;
    public float tiltAmount = 5.0f;
    public float currentTilt { get; private set; }
    public float currentRoll { get; private set; }
    public bool otherside = false;

    [Header("Miscellaneous")]
    
    // Used to align the ray used to check if the player is in the line of sight of an enemy
    // to the middle of the enemy's body
    public float yOffsetRaycast = 30f;

 
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        _inputManager = InputManager.Instance;
        rb.freezeRotation = true;
        initialWalkSpeed = walkSpeed;
        standingHeight = transform.localScale.y;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }
 
    void Update()
    {
        
        grounded = Physics.CheckSphere(groundCheck.position, groundDistance, whatIsGround);
        Debug.DrawRay(transform.position, Vector3.down * (playerHeight / 2 + 0.1f), Color.red);
        
        HandleInputs();
        CameraTilting(); 
        StateHandler();
        SpeedControl();
        
        if (state == MovementState.walking || state == MovementState.sprinting || state == MovementState.crouching)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

    private void CameraTilting()
{
    float horizontalInput = Input.GetAxisRaw("Horizontal");
    int direction = otherside ? -1 : 1;
    float rollThisFrame = direction * horizontalInput * rollSpeed * Time.deltaTime;
    currentRoll = Mathf.Clamp(currentRoll + rollThisFrame, -maxRoll, maxRoll);
    
    if (horizontalInput == 0 && currentRoll != 0)
    {
        currentRoll = Mathf.MoveTowards(currentRoll, 0, Time.deltaTime * rollSpeed);
    }

    if (!grounded)
    {
        float normalizedVelocityY = Mathf.Clamp(rb.velocity.y / 3.0f, -1f, 1f);
        currentTilt = Mathf.Lerp(currentTilt, tiltAmount * normalizedVelocityY, Time.deltaTime * rollSpeed);
    }
    else
    {
        currentTilt = Mathf.Lerp(currentTilt, 0, Time.deltaTime * rollSpeed * 5);
    }

    Camera.main.transform.localEulerAngles = new Vector3(currentTilt, Camera.main.transform.localEulerAngles.y, Camera.main.transform.localEulerAngles.z);
    //weaponCamera.transform.localEulerAngles = new Vector3(currentTilt, weaponCamera.transform.localEulerAngles.y, weaponCamera.transform.localEulerAngles.z);
}

    private void FixedUpdate()
    {
        MovePlayer();
    }

    void ApplyAdditionalGravityForce(){
        float additionalGravityForce = 9.81f;
        float additionalGravityFactor = 2f;
        rb.AddForce(Vector3.down * additionalGravityForce * additionalGravityFactor, ForceMode.Acceleration);
    }
    
    void HandleInputs(){
        
        if(GameManager.Instance.gamePaused || GameManager.Instance.gameLoading || !canMove) return;
        Vector2 movement = _inputManager.GetPlayerMovement();
        horizontalInput = movement.x;
        verticalInput = movement.y;
        
        footstepSoundCooldownTimer -= Time.deltaTime;
        
        if(grounded && movement != Vector2.zero && footstepSoundCooldownTimer <= 0){
            AudioManager.Instance.PlaySound(footstepSounds[UnityEngine.Random.Range(0, footstepSounds.Length)]);
            footstepSoundCooldownTimer = footstepSoundCooldownTime;
        }

        // TODO change this to new input system
        if(Input.GetKeyUp(KeyCode.Space)){
            walkSpeed = initialWalkSpeed;
        }

        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    _GameFocused = !_GameFocused;
        //    var camPov = GameObject.FindWithTag("MainVirtualCamera").GetComponent<CinemachineVirtualCamera>()
        //        .GetCinemachineComponent<CinemachinePOV>();
        //}

        if (_inputManager.PlayerJumpedNow())
        {
            walkSpeed = Mathf.Clamp(walkSpeed + 0.1f, initialWalkSpeed, sprintSpeed);
            if(grounded){
                Jump();
            }
        }

        if (_inputManager.PlayerJustCrouched())
        {
            state = MovementState.crouching;
            transform.localScale = new Vector3(transform.localScale.x, crouchHeight, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
            walkSpeed = crouchSpeed;
        }
        
        else if(_inputManager.PlayerCrouchReleased()) 
        {
            state = MovementState.walking;
            transform.localScale = new Vector3(transform.localScale.x, standingHeight, transform.localScale.z);
            walkSpeed = initialWalkSpeed;
        }
        
    }

    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;
    private MovementState lastState;
    private bool keepMomentum;
    private void StateHandler()
    {
        // Mode - Crouching
        
        
        // Mode - Sprinting
        if (grounded && Input.GetKey(KeyCode.LeftShift))
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;
        }

        // Mode - Walking
        else if (grounded)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }

        // Mode - Air
        else
        {
            state = MovementState.air;

            if (desiredMoveSpeed < sprintSpeed)
                desiredMoveSpeed = walkSpeed;
            else
                desiredMoveSpeed = sprintSpeed;
        }

        bool desiredMoveSpeedHasChanged = desiredMoveSpeed != lastDesiredMoveSpeed;
        if (lastState == MovementState.dashing) keepMomentum = true;

        if (desiredMoveSpeedHasChanged)
        {
            if (keepMomentum)
            {
                StopAllCoroutines();
                StartCoroutine(SmoothlyLerpMoveSpeed());
            }
            else
            {
                StopAllCoroutines();
                moveSpeed = desiredMoveSpeed;
            }
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
        lastState = state;

    }

    private float speedChangeFactor;
    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        // smoothly lerp movementSpeed to desired value
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        float boostFactor = speedChangeFactor;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            time += Time.deltaTime * boostFactor;

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
        speedChangeFactor = 1f;
        keepMomentum = false;
    }

    private void MovePlayer()
    {
        if (state == MovementState.dashing) return;

        // calculate movement direction
        orientation.rotation = Quaternion.Euler(0f, Camera.main.transform.eulerAngles.y, 0f);
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (grounded){
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if (!grounded){
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }

    private void SpeedControl()
    {
        // limiting speed on ground or in air
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }

        // limit y vel
        if (maxYSpeed != 0 && rb.velocity.y > maxYSpeed)
            rb.velocity = new Vector3(rb.velocity.x, maxYSpeed, rb.velocity.z);
    }

    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        UpwardsForce(jumpForce);
    }

    public void UpwardsForce(float force){

        rb.AddForce(transform.up * force , ForceMode.Impulse);
    }

    
    //private void OnTriggerEnter(Collider other)
    //{
    //    Debug.Log($"Sight trigger: {other.tag}");
    //    if (other.CompareTag("EnemySight"))
    //    {
    //        Vector3 enemyPosition = other.transform.parent.Find("Body").position + new Vector3(0f, yOffsetRaycast, 0f);
    //    
    //        Vector3 directionToEnemy = enemyPosition - transform.position;
    //        
    //        RaycastHit hit;
    //        if (Physics.Raycast(transform.position,  directionToEnemy, out hit, 300f))
    //        {
    //            Debug.Log($"raycast tag {hit.transform.tag}");
    //            if (hit.transform.CompareTag("Enemy"))
    //            {
    //                hit.transform.GetComponent<Enemy>().currentEnemyState = Enemy.EnemyState.Chase;
    //            }
    //        }
    //        else
    //        {
    //            Debug.Log("No hit");
    //        }
    //    }
    //    
    //}
}