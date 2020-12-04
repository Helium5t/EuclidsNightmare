using System;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class DoorTrigger : Executor
{
    private Animator doorAnimator;
    private static readonly int Open = Animator.StringToHash("open");

    [SerializeField] private bool closeAutomatically = false;

    [SerializeField] private float timeToWait = 10f;

    private bool triggered = false;
    public bool startOpen = false;

    public bool stayOpen = false;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip openDoorClip;
    [SerializeField] private AudioClip closeDoorClip;
    [SerializeField] private bool disableAudio;
    private AudioSource audioSource;


    private void OnValidate() {
        audioSource = GetComponent<AudioSource>();
        if(!openDoorClip || !closeDoorClip){
            disableAudio =  true;
            Debug.LogError("No clips set for "+ gameObject.name + ", disabling audio");
        }
        if(audioSource.playOnAwake){
            audioSource.playOnAwake = false;
        }
    }

    private void Start()
    {
        if(audioSource.loop || audioSource.mute){
            audioSource.loop = false;
            audioSource.mute = false;
        }
        if (!doorAnimator)
        {
            doorAnimator = GetComponentInChildren<Animator>();
        }

        SetDoorAnimationBool(startOpen);
    }

    public override void deactivate()
    {
        if (!stayOpen) SetDoorAnimationBool(false);
    }

    public override void activate()
    {
        if(doorAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "IdleAnimation"){
            triggered = false;
        }
        if (!triggered )
        {
            ToggleUpTriggeredState();
            SetDoorAnimationBool(triggered);
            if(closeAutomatically){
                StartCoroutine(AnimationHelper());
            }
        }
        else
        {
            Debug.Log("I'm already triggered");
        }
    }

    private IEnumerator AnimationHelper()
    {
        yield return new WaitForSeconds(timeToWait);
        deactivate();
        yield return null;
    }

    public void ToggleDownTriggeredState()
    {
        if(triggered){
            triggered = false;
        }
        Debug.Log("Triggered state: " + triggered);
    }

    public void ToggleUpTriggeredState(){
        if(!triggered){
            triggered = true;
        }
    }

    public void PlayOpenSound(){
        if(disableAudio) return;
        audioSource.clip = openDoorClip;
        audioSource.Play();
    }

    public void PlayCloseSound(){
        if(disableAudio) return;
        audioSource.clip = closeDoorClip;
        audioSource.Play();

    }

    private void SetDoorAnimationBool(bool value) => doorAnimator.SetBool(Open, value);

}