using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class FPSController : PortalTraveller
{
    [Header("Main Settings")] public float walkSpeed = 3;
    public float runSpeed = 6;
    public float smoothMoveTime = 0.1f;

    public bool lockCursor;
    public float mouseSensitivity = 3;
    public Vector2 pitchBounds = new Vector2(-90, 90);
    public float rotationSmoothTime = 0f;

    [Header("Physics Settings")] public float jumpForce = 8;
    public float gravity = 18;

    [SerializeField] [Range(0.1f, 5f)] private float maxFallSpeed = 1.5f;


    CharacterController controller;
    Camera playerCamera;

    [Header("Advanced Settings")] public float rawYaw;
    public float rawPitch;
    float yaw;
    float pitch;

    float yawSmoothV;
    float pitchSmoothV;
    float verticalVelocity;
    Vector3 velocity;
    Vector3 smoothV;
    Vector3 rotationSmoothVelocity;
    Vector3 currentRotation;

    bool jumping;
    float lastGroundedTime;
    bool disabled;

    [Header("Audio Settings")] [SerializeField]
    private bool disableAudio;

    [SerializeField] [Range(0.3f, 3f)] private float stepCycle = 0.6f; // How long it takes to play two step sounds
    private AudioSource audioSource;

    [SerializeField] [Range(0.001f, 0.1f)] private float stopStepCycleThreshould = 0.01f;
    [SerializeField] [Range(1f, 8f)] private float walkFrequency = 2f;
    [SerializeField] [Range(1f, 16f)] private float runFrequency = 4f;
    float timeToNextStep; // Countdown time to playing next step sound

    [Header("Audio clips")] [SerializeField]
    private AudioClip[] footstepSounds; // an array of footstep sounds that will be randomly selected from.

    [SerializeField] private AudioClip jumpSound; // the sound played when character leaves the ground.
    [SerializeField] private AudioClip landSound; // the sound played when character touches back on ground.



    private void OnValidate()
    {
        if (!disableAudio && footstepSounds.Length == 0 || !jumpSound || !landSound)
        {
            Debug.LogError("No audio clips are set for " + gameObject.name + ", disabling audio");
            disableAudio = true;
        }
    }


    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        playerCamera = Camera.main;
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        controller = GetComponent<CharacterController>();

        rawYaw = transform.eulerAngles.y;
        rawPitch = playerCamera.transform.localEulerAngles.x;
        yaw = rawYaw;
        pitch = rawPitch;
        timeToNextStep = stepCycle / walkFrequency;
        QualitySettings.vSyncCount = 1;
        QualitySettings.antiAliasing = 2;
    }

    private void Update()
    {
        if (transform.Find("Canvas").Find("PauseMenu").gameObject.activeSelf)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            disabled = true;
        }
        else
        {
            if (disabled)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                disabled = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Debug.Break();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            disabled = !disabled;
        }

        if (disabled) return;

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        Vector3 inputDir = new Vector3(input.x, 0, input.y).normalized;
        Vector3 worldInputDir = transform.TransformDirection(inputDir);

        Vector3 targetVelocity = worldInputDir * ((Input.GetKey(KeyCode.LeftShift)) ? runSpeed : walkSpeed);
        velocity = Vector3.SmoothDamp(velocity, targetVelocity, ref smoothV, smoothMoveTime);
        verticalVelocity -= gravity * Time.deltaTime;
        if (verticalVelocity < 0f)
        {
            verticalVelocity = Mathf.Min(verticalVelocity, -maxFallSpeed);
        }

        velocity = new Vector3(velocity.x, verticalVelocity, velocity.z);

        var flags = controller.Move(velocity * Time.deltaTime);
        if (flags == CollisionFlags.Below)
        {
            if (jumping)
            {
                PlayLandingSound();
            }
            else if (Mathf.Abs(velocity.x) > stopStepCycleThreshould || Mathf.Abs(velocity.z) > stopStepCycleThreshould)
            {
                timeToNextStep -= Time.deltaTime;
                if (timeToNextStep <= 0f)
                {
                    PlayStepSound();
                }
            }
            else
            {
                timeToNextStep = stepCycle * getStepFrequency();
            }

            jumping = false;
            lastGroundedTime = Time.time;
            verticalVelocity = 0;
        }

        // Jumping
        if (Input.GetKeyDown(KeyCode.Space))
        {
            float timeSinceLastTouchedGround = Time.time - lastGroundedTime;
            if (controller.isGrounded || (!jumping && timeSinceLastTouchedGround < 0.15f))
            {
                jumping = true;
                verticalVelocity = jumpForce;
                PlayJumpSound();
            }
        }

        float mX = Input.GetAxisRaw("Mouse X");
        float mY = Input.GetAxisRaw("Mouse Y");

        // Verrrrrry gross hack to stop camera swinging down at start
        float mMag = Mathf.Sqrt(mX * mX + mY * mY);
        if (mMag > 5)
        {
            mX = 0;
            mY = 0;
        }

        rawYaw += mX * mouseSensitivity;
        rawPitch -= mY * mouseSensitivity;
        rawPitch = Mathf.Clamp(rawPitch, pitchBounds.x, pitchBounds.y);
        if (rotationSmoothTime > -1f)
        {
            // This should render the movement of the mouse smoother but it gives off a bad feeling during play.
            // It's better to keep this disabled
            pitch = Mathf.SmoothDampAngle(pitch, rawPitch, ref pitchSmoothV, rotationSmoothTime);
            yaw = Mathf.SmoothDampAngle(yaw, rawYaw, ref yawSmoothV, rotationSmoothTime);
        }
        else
        {
            pitch = rawPitch;
            yaw = rawYaw;
        }

        transform.eulerAngles = Vector3.up * yaw;
        playerCamera.transform.localEulerAngles = Vector3.right * pitch;
    }

    public override void Teleport(Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot)
    {
        transform.position = pos;
        Vector3 eulerRot = rot.eulerAngles;
        float delta = Mathf.DeltaAngle(yaw, eulerRot.y);
        rawYaw += delta;
        yaw += delta;
        transform.eulerAngles = Vector3.up * yaw;
        velocity = toPortal.TransformVector(fromPortal.InverseTransformVector(velocity));
        verticalVelocity = velocity.y;
        Physics.SyncTransforms();
    }

    private void PlayJumpSound()
    {
        //_fmodPlayer.PlayJumpSound(GameSoundPaths.PlayerJumpLandSoundPath);

        /*if (disableAudio) return;
        audioSource.clip = jumpSound;
        audioSource.Play();*/
    }

    private void PlayLandingSound()
    {
        /*
         * if (disableAudio) return;
        audioSource.clip = landSound;
        audioSource.Play();
         */
        //_fmodPlayer.PlayLandingSound(GameSoundPaths.PlayerJumpLandSoundPath);
        timeToNextStep = stepCycle / walkFrequency;
    }

    private void PlayStepSound()
    {
        //_fmodPlayer.PlayFootstepsSound(GameSoundPaths.FootstepsEventPath);

        // if (disableAudio) return;
        // timeToNextStep = stepCycle*getStepFrequency();
        // // pick & play a random footstep sound from the array,
        // // excluding sound at index 0
        //
        // int n = Random.Range(1, footstepSounds.Length);
        // audioSource.clip = footstepSounds[n];
        // audioSource.PlayOneShot(audioSource.clip);
        // // move picked sound to index 0 so it's not picked next time
        // footstepSounds[n] = footstepSounds[0];
        // footstepSounds[0] = audioSource.clip;
    }

    public float getStepFrequency()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            return 1 / runFrequency;
        }
        else return 1 / walkFrequency;
    }
}