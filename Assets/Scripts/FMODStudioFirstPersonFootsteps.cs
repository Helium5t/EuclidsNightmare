using FMOD.Studio;
using FMODUnity;
using GameManagement;
using UnityEngine;
using Utility;
using Random = UnityEngine.Random;

/*
 * Attach this script to the Game Object you're using as your First Person Controller.
 * Only include one instance of this script as a component in each scene of the game.
 */
public class FMODStudioFirstPersonFootsteps : MonoBehaviour
{
    /*
     * These variables will all be set in the Inspector tab of Unity's Editor by either us,
     * or the 'FMODStudioFootstepsEditor' script.
     */

    #region ExposedVariables

    [Header("FMOD Settings")]

    /* Use this in the Editor to write the name of the parameter that controls
     which material the player is currently walking on.*/
    [SerializeField] private string _materialParameterName;

    /*
     * Use this in the Editor to write the name of the parameter that controls which footstep speed needs to be heard.
     */
    [SerializeField] private string _speedParameterName;

    /*
     * Use this in the Editor to write the name of the parameter that controls
     * whether or not a jumping or a landing sound needs to be heard.
     */
    [SerializeField] private string _jumpOrLandParameterName;

    [Header("Playback Settings")]
    // Select how far the player must travel before they hear a footstep.
    [SerializeField]
    private float _stepDistance = 1.5f;

    // Select how far the raycast will travel down to when checking for a floor.
    [SerializeField] private float _rayDistance = 1.2f;

    /* Set a time.
     * If the time between each step the player takes is less than this value,
     * the player will start to hear running footsteps.
     */
    [SerializeField] private float _startRunningTime = 0.3f;

    /* In Unity, go Edit -> Project Settings -> Input Manager.
     * Then find the name of the input that controls which key/button the player must press in order to jump
     * (it's probably called "Jump").
     * Then once you know it's name, write it into this variable in the Inspector tab. */
    [SerializeField] private string _jumpInputName;

    [Space] [Header("Walking material settings")]
    /*
     * In the inspector we can decide how many Material types we have in FMOD by setting the size of this array.
     * Depending on the size,
     * the array will then create a certain amount of strings for us to fill in with the name of each of our footstep
     * materials for our scripts to use.
     */
    public string[] MaterialTypes;

    #endregion

    // This will be told by the 'FMODStudioFootstepsEditor' script which Material has been set as the default.
    // It will then store the value of that Material for this script to use.
    // This cannot be changed in the Editor, but a drop down menu created by the 'FMODStudioFootstepsEditor' script can.
    [HideInInspector] public int DefaultMaterialValue;

    #region PrivateVariables

    //These variables are used to control when the player executes a footstep.

    /* This will be set as random number,
     * which will later be added to the StepDistance to add a little variation to the length in steps.
     * */
    private float _stepRandom;

    // This will hold the co-ordinates of the previous position the player was in during the last frame.
    private Vector3 _prevPos;

    // This will hold a value that how represent how far the player has travelled since they last took a step.
    private float _distanceTravelled;

    //These variables are used when checking the Material type the player is on top of.
    private RaycastHit _hit;
    private int _fmodMaterialValue; // We'll use this to set the value of our FMOD Material Parameter.

    /* These booleans will hold values that tell us if the player is touching the ground currently and
     * if they were touching it during the last frame.
     */
    private bool _isPlayerTouchingGround;

    // This will hold a value to represent whether or not the player was touching the ground during the last frame.
    private bool _isPreviouslyTouchingGround;

    //These floats help us determine when the player should be running.
    private float _timeTakenSinceStep; // We'll use this as a timer, to track the time it takes between each step.
    private int _fmodPlayerRunning; // We'll use to set the value of our FMOD Switch Speed Parameter.

    private Transform _transform; //Transform component caching
    private EventInstance _breathingEventInstance;
    private float _breathingEventValue;

    #endregion

    private void Awake() => _transform = GetComponent<Transform>();

    private void Start()
    {
        _stepRandom = Random.Range(0f, 0.3f);
        // 'PrevPos' now holds the location that the player is starting at as co-ordinates (x, y, z).
        _prevPos = _transform.position;
        _breathingEventInstance = RuntimeManager.CreateInstance(GameSoundPaths.BreathingEventPath);
        SetBreathingSoundParameter();
        _breathingEventInstance.start();
    }

    private void SetBreathingSoundParameter()
    {
        _breathingEventValue = _fmodPlayerRunning == 1 ? 1f : 0f;
        _breathingEventInstance.setParameterByName("BreathingFade", _breathingEventValue);
    }


    private void Update()
    {
        if (PauseMenu.GameIsPaused)
        {
            _breathingEventInstance.setPaused(true);
            return;
        }

        _breathingEventInstance.setPaused(false);
        Debug.DrawRay(_transform.position, Vector3.down * _rayDistance, Color.blue);

        /*
         * This section checks to see if the player is touching the ground or not,
         * so that a footstep is played whilst the player is in the air.
         * It also determines when the JumpOrLand method should be told to trigger,
         * thus playing the Jump & Land event in FMOD.
         */

        #region GroundAndSoundCheck

        GroundedCheck();
        // If the player is touching the ground AND the player presses the button to jump at the same time,
        // we know that the player character is about to jump, therefore we perform our method to play a sound.
        if (_isPlayerTouchingGround && Input.GetButtonDown(_jumpInputName))
        {
            // Before we play a jumping sound, we need to know what material the player has jumped off of.
            MaterialCheck();
            // The 'PlayJumpOrLand' method is performed, triggering our 'Jump & Land' event in FMOD to play.
            // We also pass through it's parameter brackets the 'true' boolean value for our method to store inside
            // a variable and use to play a jump sound with.
            PlayJumpOrLand(true);
        }

        // If the player wasn't touching the ground during the last frame,
        // but is touching the ground during the current frame,
        // then that means they must have just landed on the ground, therefore we perform out methods and play a sound.
        if (!_isPreviouslyTouchingGround && _isPlayerTouchingGround)
        {
            MaterialCheck();
            // The 'PlayJumpOrLand' method is performed, triggering our 'Jump & Land' event in FMOD to play. We also
            // pass through it's parameter brackets the 'false' boolean value
            // for our method to store inside a variable and use to play a landing sound with.
            PlayJumpOrLand(false);
        }

        // Finally, we update the 'PreviouslyTouchingGround' variable by setting it to the value of whatever our
        // 'PlayerTouchingGround' variable is. Then when the next frame is run, 'PlayerTouchingGround' will be updated,
        // allowing us to compare it with 'PreviouslyTouchingGround',
        // which still holds the value of it from the last frame.
        _isPreviouslyTouchingGround = _isPlayerTouchingGround;

        #endregion

        /*
         * This section determines when and how the PlayFootstep method should be told to trigger,
         * thus playing the footstep event in FMOD.
         */

        #region PlaySounds

        // Essentially turning 'TimeTakenToStep' into a running timer that starts at 0 seconds.
        // Remember that 'Time.deltaTime' counts how long in seconds the last frame lasted for.
        _timeTakenSinceStep += Time.deltaTime;
        // Every frame, the co-ordinates that the player is currently at is reduced by the value of the co-ordinates
        // they were at during the previous frame.
        // This gives us the length between the two points as a new set of co-ordinates (AKA a vector).
        // That length is then turned into a single number by '.magnitude' and then
        // finally added onto the value of the DistanceTravelled float.
        // Now we know how far the player has travelled!
        _distanceTravelled += (_transform.position - _prevPos).magnitude;
        // If the distance the player has travelled is greater than or equal to the StepDistance plus the StepRandom,

        // then we can perform our methods.
        if (_distanceTravelled >= _stepDistance + _stepRandom)
        {
            MaterialCheck();
            // The SpeedCheck method is performed. This checks for the time it took between
            // this step and the last tot hen update the 'F_PlayerRunning' variable with
            // for our 'PlayFootstep()' method to use.
            SpeedCheck();
            SetBreathingSoundParameter();
            PlayFootstep();
            // Now that our footstep has been played, this will reset 'StepRandom' and give it a new random value
            // between 0 and 0.3, in order to make the distance the player has to travel to hear a footstep
            // different from what it previously was.
            _stepRandom = Random.Range(0f, 0.3f);
            // Since the player has just taken a step, we need to set the 'DistanceTravelled' float back to 0.
            _distanceTravelled = 0f;
        }

        // To calculate where the player was during the last frame, we set 'PrevPos' to whatever position the player
        // is at during the end of the current frame, after the distance they've travelled has been calculated.
        // This means that when this frame ends and the new one begins,
        // the co-ordinates inside 'PrevPos' won't have changed,
        // but will now represent where the player was during our last frame.
        // 'Transform.position' will then be updated at the very start of the new frame to hold
        // whatever co-ordinates the player is now at.
        _prevPos = _transform.position;

        #endregion
    }

    /// <summary>
    /// This method when performed will find out what material our player is currently on top of
    /// and will update the value of 'F_MaterialValue' accordingly, to represent that value.
    /// </summary>
    private void MaterialCheck()
    {
        if (Physics.Raycast(_transform.position, Vector3.down, out _hit, _rayDistance))
        {
            _fmodMaterialValue = _hit.collider.gameObject.GetComponent<FMODStudioMaterialSetter>()
                ? _hit.collider.gameObject.GetComponent<FMODStudioMaterialSetter>().MaterialValue
                : DefaultMaterialValue;
        }
        else
            _fmodMaterialValue = DefaultMaterialValue;
    }

    /// <summary>
    /// This method when performed will update the 'F_PlayerRunning' variable,
    /// to find out if the player should be hearing heavy running footsteps.
    /// </summary>
    private void SpeedCheck()
    {
        // Remember that 'TimeTakenToStep' is a running timer.
        // This method will only be performed as the player takes a step.
        // So if this time is less than the value set to the 'StartRunningTime' variable,
        // we can assume the player is moving fast enough to warrant a heavy running footstep to be played.
        // aSo therefore..
        _fmodPlayerRunning = _timeTakenSinceStep < _startRunningTime ? 1 : 0;
        // Finally, now that the player has taken the correct step, we reset our timer 'TimeTakenToStep' back to 0.
        // Now th timer is tracking how much time has passed since the player took that last step. 
        _timeTakenSinceStep = 0f;
    }

    /// <summary>
    /// This method will update the 'PlayerTouchingGround' variable,
    /// to find out if the player is currently touching the ground or not.
    /// </summary>
    private void GroundedCheck()
    {
        Physics.Raycast(_transform.position, Vector3.down, out _hit, _rayDistance);
        _isPlayerTouchingGround = _hit.collider;
    }

    /// <summary>
    /// When this method is performed, our footsteps event in FMOD will be told to play.
    /// </summary>
    private void PlayFootstep()
    {
        if (_isPlayerTouchingGround)
        {
            // If they are, we create an FMOD event instance.
            // We use the event path inside the 'FootstepsEventPath' variable to find the event we want to play.
            EventInstance footstepEventInstance =
                RuntimeManager.CreateInstance(GameSoundPaths.PlayerFootstepsEventPath);
            RuntimeManager.AttachInstanceToGameObject(footstepEventInstance, _transform, GetComponent<Rigidbody>());
            footstepEventInstance.setParameterByName(_materialParameterName, _fmodMaterialValue);
            footstepEventInstance.setParameterByName(_speedParameterName, _fmodPlayerRunning);
            footstepEventInstance.start();
            // We also set our event instance to release straight after we tell it to play,
            // so that the instance is released once the event had finished playing.
            footstepEventInstance.release();
        }
    }

    /// <summary>
    /// // When this method is performed our Jumping And Landing event in FMOD will be told to play.
    /// A boolean variable is also created inside it's parameter brackets to be used inside this method.
    /// This variable will hold whichever value we gave this method when we called it in the Update function.
    /// </summary>
    /// <param name="fmodJumpOrLand"></param>
    private void PlayJumpOrLand(bool fmodJumpOrLand)
    {
        EventInstance jumpLandEvent = RuntimeManager.CreateInstance(GameSoundPaths.PlayerJumpLandSoundPath);
        RuntimeManager.AttachInstanceToGameObject(jumpLandEvent, _transform, GetComponent<Rigidbody>());
        jumpLandEvent.setParameterByName(_materialParameterName, _fmodMaterialValue);
        jumpLandEvent.setParameterByName(_jumpOrLandParameterName, fmodJumpOrLand ? 0f : 1f);
        jumpLandEvent.start();
        if (fmodJumpOrLand) PlayJumpGroanSound();
        jumpLandEvent.release();
    }

    private void PlayJumpGroanSound()
    {
        RuntimeManager.PlayOneShot("event:/Sounds/Player/Locomotion/JumpGroan/Groan");
    }
}