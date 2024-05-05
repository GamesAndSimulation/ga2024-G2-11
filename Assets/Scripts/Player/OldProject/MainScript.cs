using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MainScript : MonoBehaviour
{
    public int bullets = 20;
 
    [Header("Gun")]
    public GameObject projectile;
    public float fireforce = 1000; 
 
    [Header("Jetpack")]
    public ParticleSystem jetpackSmoke;
 
    [Header("Audio")]
    public AudioClip jetpackSound;
    public AudioClip gunShotSound;
    public AudioClip emptuGunShotSound;
    public AudioClip reloadSound;
 
    [Header("Player Controller")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public bool isGrounded {get; private set;} 
 
    [Header("Movement Settings")]
    [SerializeField] private GameObject[] _dashIcons;
    [SerializeField] private bool noclip = false;
    public float moveSpeed = 8;
    public float mouseSpeed = 100;
    public float gravity = 9.8f;
    public float jumpForce = 20;
    public float dashForce = 750;
    public int maxDashes = 3;
    private int currentDashes;
    private float dashRecoverTime = 1.0f;
    private float dashRecoverTimeElapsed = 0.0f;
    
    [Header("Camera Settings")]
    private float pitch = 0.0f;
    private float yaw = 0.0f;
    private Vector3 velocity;
    public CharacterController characterController;
 
    [Header("Gun and Enemies")]
    public Animator gunAnimator;
    Rigidbody rb;
 
    void Start()
    {
        rb = GetComponent<Rigidbody>();
 
        /* Comment to unlock the mouse cursor */
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentDashes = maxDashes;
 
    }
 
    // Update is called once per frame
    void Update()
    {
        HandleMovementOld();
        HandleInputs();
    }

    /// <summary>
    /// pre: Requires a ridgid body component in player
    /// </summary>
    void HandleMovementRB(){
        
    }

    void HandleMovementOld(){
        float horizontalValue = Input.GetAxis("Horizontal");
        float verticalValue = Input.GetAxis("Vertical");
 
        float horizontalMouseValue = Input.GetAxis("Mouse X");
        float verticalMouseValue = Input.GetAxis("Mouse Y");
 
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if(isGrounded && velocity.y < 0){
            velocity.y = 0f;
        }
 
        if(currentDashes < maxDashes){
            dashRecoverTimeElapsed += Time.deltaTime;
            if(dashRecoverTimeElapsed >= dashRecoverTime && isGrounded){
                currentDashes++;
                _dashIcons[currentDashes - 1].SetActive(true);
                dashRecoverTimeElapsed = 0.0f;
            }
        }
        
        if(!noclip){
            Vector3 moveDirection = transform.forward * verticalValue + transform.right * horizontalValue;
            characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
        }
        else{
            characterController.Move(transform.forward * dashForce * Time.deltaTime);
        }
 
        if(!isGrounded){
            velocity.y -= gravity * Time.deltaTime;
        }
        if(!noclip)
            characterController.Move(velocity * Time.deltaTime);
 
        yaw += horizontalMouseValue * mouseSpeed * Time.deltaTime;
        pitch -= verticalMouseValue * mouseSpeed * Time.deltaTime;
 
        pitch = Mathf.Clamp(pitch, -90, 90);
 
        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
    }

    void HandleInputs(){
        if (Input.GetButtonUp("Fire1")){
        
            if(bullets <= 0){
                AudioManager.Instance.PlaySound(emptuGunShotSound);
            }
            else{
                gunAnimator.SetTrigger("Shoot");
                AudioManager.Instance.PlaySound(gunShotSound);
                GameObject instantiatedBullet =
                    Instantiate(projectile, transform.position + transform.forward, transform.rotation);
                instantiatedBullet.tag = "PlayerProjectile";
                instantiatedBullet.GetComponent<Rigidbody>().AddForce(transform.forward * fireforce);
                Destroy(instantiatedBullet, 5);
                bullets--;
            }
        }

        // Reload
        if(Input.GetKeyDown(KeyCode.R)){
            if(bullets < 30){
                gunAnimator.SetTrigger("Reload");
                AudioManager.Instance.PlaySound(reloadSound);
                bullets = 30; 
            }
        }

        if(Input.GetKeyDown(KeyCode.LeftShift)){
            Debug.Log("Dash");
            // If moving dash in the direction of the movement, otherwise dash in the direction of the camera
            if(currentDashes > 0){
                currentDashes--;
                dashRecoverTimeElapsed = 0.0f;
                _dashIcons[currentDashes].SetActive(false);
                Debug.Log("Dashes: " + currentDashes);
                StartCoroutine(Dash());
            }
        }
        // Press 1
        if(Input.GetKeyDown(KeyCode.Alpha1)){
            noclip = !noclip;
        }
    }

    IEnumerator Dash(){
        float dashDuration = 0.4f; 
        transform.position += transform.forward * 3.0f;

        float startTime = Time.time;
        while (dashDuration > 0)
        {
            characterController.Move(transform.forward * dashForce * Time.deltaTime);
            dashDuration -= Time.deltaTime; 
            yield return null; 
        }
    }
 
    public IEnumerator Jump(){
        velocity.y = jumpForce;
        yield return null;
    }
}