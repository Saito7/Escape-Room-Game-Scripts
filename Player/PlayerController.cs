using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;
using System.Collections;

public class PlayerController : MonoBehaviour,ISaveable
{
    #region Player variables
    private CharacterController controller = null;
    private Transform cameraTransform;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private bool isWalking;
    private Animator animator = null;
    public float jumpHeight = 3f;
    #endregion

    [Header("References")]
    [SerializeField] private InputActionAsset playerInputAsset = null;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private MouseLook mouseLook;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Animator blackOut;

    [Header("Sounds")]
    [SerializeField] private AudioSource _audioSource;
    public List<AudioClip> footstepSounds = new List<AudioClip>();

    [Header("Settings")]
    [SerializeField]
    private float playerSpeed = 5.0f;
    [SerializeField]
    private float runSpeed = 10.0f;
    [SerializeField]
    private float walkSpeed = 5.0f;
    [SerializeField]
    private float gravityValue = -9.81f;
    [SerializeField]
    private float delayInDifferentFootSteps = 0.5f;

    private void Awake()
    {
        //Load keybinds on game start
        var rebinds = PlayerPrefs.GetString("rebinds");
        if (!string.IsNullOrEmpty(rebinds))
            playerInputAsset.LoadBindingOverridesFromJson(rebinds);
    }

    private void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        cameraTransform = mainCamera.transform;
        animator = gameObject.GetComponent<Animator>();
        mouseLook.mouseCanMove = true;
    }

    private void Update()
    {
        Move();   
    }

    private void Move()
    {
        Quaternion rotation = Quaternion.Euler(0f, cameraTransform.eulerAngles.y, 0f);     //make the body face which way the camera is facing
        transform.rotation = rotation;

        groundedPlayer = controller.isGrounded; //checks if the player is grounded
        if(groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0; //only sets player velocity to 0 once the player actually reaches the ground
        }

        Vector2 movement = playerInput.actions["Move"].ReadValue<Vector2>(); //gets the vector2 movement that a player needs to go
        Vector3 move = new Vector3(movement.x, 0f, movement.y);  //Creates a new vector3 to tell how a transform should be in 3d space
        //the move vector is set to be in relation to the forward direction of the camera
        //the vector is normalized to have a magnitude of 1 so the player moves a fixed distance in their desired direction each time
        move = cameraTransform.forward * move.z + cameraTransform.right * move.x; 
        move.y = 0f;
        move.Normalize();
        //multiplied by Time.deltaTime so framerate does not affect the distance that a player moves.
        //multiplied by playerSpeed so that when creating a sprint action playerSpeed will affect how fast a player traverses
        controller.Move(move * Time.deltaTime * playerSpeed);

        //if the player is moving then the playing animation is played
        animator.SetBool("IsWalking", move.magnitude > 0.2f);
        if(move.magnitude > 0.2f)
        {
            if (!isWalking)
            {
                PlayFootstepSounds(); //play the footstep sounds
            }
        }

        //makes sure that gravity affects the player so they fall down rather than floating
        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    public void lightPuzzleCompleteAnim()
    {
        mouseLook.mouseCanMove = false;
        animator.Play("Faint 1");
        blackOut.Play("lightPuzzleFaint");
        this.enabled = false;
    }

    public void datingPuzzleCompleteAnim()
    {
        mouseLook.mouseCanMove = false;
        animator.Play("Faint 2");
        blackOut.Play("datingPuzzleFaint");
        this.enabled = false;
    }

    public void Jump(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) { return; }

        if (groundedPlayer)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravityValue);
            if(jumpHeight != 0)
            {
                animator.Play("Jump");
            }
        }
    }

    public void Run(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) { playerSpeed = walkSpeed; delayInDifferentFootSteps = 0.5f; return; }

        playerSpeed = runSpeed;
        delayInDifferentFootSteps = 0.3f;
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
        public float transformRotY;
        public float cameraRotX;
        public float cameraRotY;
        public float cameraRotZ;
    }

    public object CaptureState()
    {
        return new SaveData
        {
            position = transform.position,
            transformRotY = transform.eulerAngles.y,
            cameraRotX = Camera.main.GetComponent<MouseLook>().xRotation,
            cameraRotY = Camera.main.transform.localEulerAngles.y,
            cameraRotZ = Camera.main.transform.localEulerAngles.z
        };
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;

        transform.position = saveData.position;
        transform.eulerAngles = new Vector3(0, saveData.transformRotY, 0);
        Camera.main.GetComponent<MouseLook>().xRotation = saveData.cameraRotX;
    }
}
