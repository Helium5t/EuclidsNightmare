using UnityEngine;

public class CubeSoundsManager : MonoBehaviour
{
    private void OnCollisionEnter(Collision other) => PlayCrateHitSound();

    private void PlayCrateHitSound()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Sounds/Crate/Fall", transform.position);
    }
}
