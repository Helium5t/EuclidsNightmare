using UnityEngine;
using UnityEngine.EventSystems;

namespace GameManagement
{
    public class ButtonSoundsPlayer : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
    {
        public void OnPointerEnter(PointerEventData eventData) => PlaySelectedButtonSound();

        public void OnPointerDown(PointerEventData eventData) => PlayPressedSound();

        public void PlayPressedSound() => FMODUnity.RuntimeManager.PlayOneShot("event:/Sounds/UI/ButtonPressed");

        public void PlaySelectedButtonSound() =>
            FMODUnity.RuntimeManager.PlayOneShot("event:/Sounds/UI/ButtonSelected");
    }
}