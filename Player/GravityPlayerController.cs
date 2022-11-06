using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;
using System.Collections;

public class GravityPlayerController : MonoBehaviour,ISaveable
{
    #region Player variables
    private Rigidbody rb = null;
    private bool groundedPlayer;
    private bool isWalking;
    private bool isSprinting;
    private Vector3 move;
    private Vector3 slopeMoveDirection;
    private float movementMultiplier = 10f;
    private Animator animator = null;
    private const string RebindsKey = "rebinds";
    private RaycastHit slopeHit;
    #endregion

    [Header("References")]
    [SerializeField] private InputActionAsset playerInputAsset = null;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform start;
    [SerializeField] private Transform checkPoint;
    public Transform orientation;

    [Header("Sounds")]
    [SerializeField] private AudioSource _audioSource;
    public List<AudioClip> footstepSounds = new List<AudioClip>();

    [Header("Settings")]
    [SerializeField]
    private float playerSpeed = 4.0f;
    [SerializeField]
    private float runSpeed = 8.0f;
    [SerializeField]
    private float walkSpeed = 4.0f;
    public float gravityValue = -20f;
    [SerializeField]
    private float delayInDifferentFootSteps = 0.5f;
    [SerializeField]
    private float jumpHeight = 5f;
    [SerializeField]
    private float airMultiplier = 0.4f;
    [SerializeField]
    private float acceleration = 10f;

    private void Awake()
    {
        //Load keybinds on game start
        string rebinds = PlayerPrefs.GetString(RebindsKey, string.Empty);

        if (string.IsNullOrEmpty(rebinds)) { return; }

        playerInputAsset.LoadBindingOverridesFromJson(rebinds);
    }

    private void Start()
    {
        Physics.gravity = orientation.up.normalized * gravityValue;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        animator = gameObject.GetComponent<Animator>();
        Time.fixedDeltaTime = 0.02f;
        Time.maximumDeltaTime = 1 / 3;
        Time.maximumParticleDeltaTime = 0.03f;
    }

    private void FixedUpdate()
    {
        Movement();    
    }

    public void setCheckPoint(Transform spawnPoint)
    {
        checkPoint = spawnPoint;
    }

    public void playerOutOfBounds()
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;
        if (checkPoint != start)
        {
            transform.position = checkPoint.position;
            transform.rotation = checkPoint.rotation;
            Physics.gravity = orientation.up.normalized * gravityValue;
        }
        else
        {
            transform.position = start.position;
            transform.rotation = start.rotation;
            Physics.gravity = orientation.up.normalized * gravityValue;
        }
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void Movement()
    {
        if (groundedPlayer && !OnSlope())
        {
            rb.AddForce(move.normalized * playerSpeed * movementMultiplier, ForceMode.Acceleration);
        }
        else if(groundedPlayer && OnSlope())
        {
            rb.AddForce(slopeMoveDirection.normalized * playerSpeed * movementMultiplier, ForceMode.Acceleration);
        }
        else if(!groundedPlayer)
        {
            rb.AddForce(move.normalized * playerSpeed * movementMultiplier * airMultiplier, ForceMode.Acceleration);
        }
    }

    private void MyInput()
    {
        Vector2 movement = playerInput.actions["Move"].ReadValue<Vector2>();
        move = orientation.forward * movement.y + orientation.right * movement.x;

        animator.SetBool("IsWalking", move.magnitude > 0.2f);
        if (move.magnitude > 0.2f)
        {
            if (!isWalking)
            {
                PlayFootstepSounds(); //play the footstep sounds
            }
        }
    }

    private void ControlDrag()
    {
        if (groundedPlayer)
        {
            rb.drag = 6f;
        }
        else
        {
            rb.drag = 2f;
        }
    }



    private void Update()
    {
        //Physics.gravity = orientation.up.normalized * gravityValue;

        groundedPlayer = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        MyInput();
        ControlDrag();
        controlSpeed();

    }
   
    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, 1.75f / 2 + 0.5f))
        {
            if(slopeHit.normal != Vector3.up)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    public void Jump(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) { return; }

        if (groundedPlayer)
        {               
            rb.AddForce(orientation.up * jumpHeight, ForceMode.Impulse);
            animator.Play("Jump");
        }
    }

    public void Run(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) {
            isSprinting = false;
            delayInDifferentFootSteps = 0.5f; 
            return; 
        }

        if (groundedPlayer)
        {
            isSprinting = true;
            delayInDifferentFootSteps = 0.3f;
        }
    }

    private void controlSpeed()
    {
        if (groundedPlayer && isSprinting)
        {
            playerSpeed = Mathf.Lerp(playerSpeed, runSpeed, acceleration * Time.deltaTime);
        }
        else if(!isSprinting)
        {
            playerSpeed = Mathf.Lerp(playerSpeed, walkSpeed, acceleration * Time.deltaTime);
        }
    }


    private void PlayFootstepSounds()
    {
        StartCoroutine("PlayStepSound", delayInDifferentFootSteps);
    }

    IEnumerator PlayStepSound(float timer)     {

        _audioSource.clip = footstepSounds[UnityEngine.Random.Range(0, footstepSounds.Count)];
        _audioSource.Play();

        isWalking = true;

        yield return new WaitForSeconds(timer);

        isWalking = false;
    }

    [Serializable]
    private struct SaveData
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 checkPointPosition;
        public Quaternion checkPointRotation;
        public bool checkPointEmpty;
    }

    public object CaptureState()
    {
        if (checkPoint.position != start.position)
        {
            return new SaveData
            {
                position = transform.position,
                rotation = transform.rotation,
                checkPointPosition = checkPoint.position,
                checkPointRotation = checkPoint.rotation,
                checkPointEmpty = false
            };
        }
        else
        {
            return new SaveData
            {
                position = transform.position,
                rotation = transform.rotation,
                checkPointEmpty = true
            };
        }
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;

        transform.position = saveData.position;
        transform.rotation = saveData.rotation;
        if (!saveData.checkPointEmpty)
        {
            checkPoint.position = saveData.checkPointPosition;
            checkPoint.rotation = saveData.checkPointRotation;
        }
    }
}
