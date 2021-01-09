using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class DisplayOneShotSentence : MonoBehaviour
{
    [SerializeField] private string sentence;
    [SerializeField] private GameObject speechManagerGameObject;

    private SpeechManager speechManager;

    private void Awake() => speechManager = speechManagerGameObject.GetComponent<SpeechManager>();

    public void TellSentence() => speechManager.TellOneSentence(sentence);

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) TellSentence();
    }
}