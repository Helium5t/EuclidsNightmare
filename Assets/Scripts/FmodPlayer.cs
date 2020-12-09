using FMOD.Studio;
using UnityEngine;

/// <summary>
/// For now, this class is responsible for playing the sounds via FMOD Studio.
/// </summary>
public class FmodPlayer : MonoBehaviour
{
    private const float _rayCastLength = 0.1f;
    private float _material;

    private void FixedUpdate()
    {
        GroundTypeCheck();
        Debug.DrawRay(transform.position, Vector3.down, Color.blue); //just here to check collisions in the editor
    }

    private void GroundTypeCheck()
    {
        const int layerMask = 1 << 31;
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo, _rayCastLength, layerMask))
        {
            switch (hitInfo.collider.tag)
            {
                case "Material:Floor":
                    _material = 1.0f;
                    break;
                case "Material:Stone":
                    _material = 2.0f;
                    break;
                default:
                    _material = 1.0f;
                    break;
            }
        }
    }

    public void PlayFootstepsSound(string eventPath)
    {
        EventInstance footstepsInstance = FMODUnity.RuntimeManager.CreateInstance(eventPath);
        footstepsInstance.setParameterByName("Material", _material);
        footstepsInstance.start();
        footstepsInstance.release();
    }
    
    
}