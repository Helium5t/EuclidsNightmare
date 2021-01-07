using UnityEngine;
using UnityEngine.EventSystems;
using Utility;

namespace GameManagement
{
    public class ButtonSoundsPlayer : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
    {
        public void OnPointerEnter(PointerEventData eventData) => PlaySelectedButtonSound();

        public void OnPointerDown(PointerEventData eventData) => PlayPressedSound();

        public void PlayPressedSound() => FMODUnity.RuntimeManager.PlayOneShot(GameSoundPaths.ButtonPressedSoundPath);

        public void PlaySelectedButtonSound() =>
            FMODUnity.RuntimeManager.PlayOneShot(GameSoundPaths.ButtonSelectedSoundPath);
    }
}