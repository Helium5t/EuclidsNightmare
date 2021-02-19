using System.Collections;
using Cinemachine;
using GameManagement;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Debugging
{
    public class FallingCam : MonoBehaviour
    {
        public bool fall = false;

        private CinemachineBrain cinemachineBrain;
        private PostProcessVolume postProcessVolume;
        private LensDistortion lensDistortionEffect;
        
        private const float GravitationalConstant = 9.8f;
        private const float LensDistortionLerpSpeedMultiplier = .5f;
        private float speed = 0f;

        [SerializeField] private Vector3 impactPoint;
        [SerializeField] private SpeechManager speechManager;
        [SerializeField] private LevelLoader loader;


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(transform.position + impactPoint, Vector3.one * 2f);
        }

        private void Start()
        {
            postProcessVolume = GetComponent<PostProcessVolume>();
            cinemachineBrain = GetComponent<CinemachineBrain>();

            postProcessVolume.profile.TryGetSettings(out lensDistortionEffect);
            lensDistortionEffect.intensity.value = 0.0f;
            lensDistortionEffect.active = false;

            StartCoroutine(nameof(ObserveDialogue));
            impactPoint = transform.position + impactPoint;
        }

        private void Update()
        {
            if (!fall) return;
            lensDistortionEffect.intensity.value =
                Mathf.Lerp(lensDistortionEffect.intensity.value, -100f,
                    LensDistortionLerpSpeedMultiplier * Time.deltaTime);
            
            speed += GravitationalConstant * Time.deltaTime;
            Vector3 nextPos = transform.position - Vector3.up * speed;
            nextPos.y = Mathf.Max(nextPos.y, impactPoint.y);
            transform.position = nextPos;
            if (transform.position.y <= impactPoint.y)
            {
                fall = false;
                lensDistortionEffect.intensity.value = 0.0f;
                lensDistortionEffect.active = false;
                loader.LoadNextLevel();
            }
        }

        private IEnumerator ObserveDialogue()
        {
            yield return null;
            while (!speechManager.spoken) yield return new WaitForEndOfFrame();
            fall = true;
            Debug.Log("Start to fall");
            lensDistortionEffect.active = true;
            cinemachineBrain.enabled = false;
            yield return null;
        }
    }
}