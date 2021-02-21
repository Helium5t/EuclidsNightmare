using System.Collections;
using Cinemachine;
using FMOD.Studio;
using FMODUnity;
using GameManagement;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Utility;

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
        private EventInstance fallNoiseEventInstance;

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

            #region InitFallNoiseSoundInstance

            fallNoiseEventInstance = RuntimeManager.CreateInstance(GameSoundPaths.FallNoiseSoundPath);
            RuntimeManager.AttachInstanceToGameObject(fallNoiseEventInstance,
                GetComponent<Camera>().transform,
                GetComponent<Rigidbody>());

            #endregion

            #region InitLensDistortionEffect

            postProcessVolume.profile.TryGetSettings(out lensDistortionEffect);
            lensDistortionEffect.intensity.value = 0.0f;
            lensDistortionEffect.active = false;

            #endregion

            StartCoroutine(nameof(ObserveDialogue));
            impactPoint = transform.position + impactPoint;
        }

        private void Update()
        {
            if (!fall) return;

            #region LensDistortionParamModification

            lensDistortionEffect.intensity.value =
                Mathf.Lerp(lensDistortionEffect.intensity.value, -100f,
                    LensDistortionLerpSpeedMultiplier * Time.deltaTime);

            #endregion

            speed += GravitationalConstant * Time.deltaTime;
            Vector3 nextPos = transform.position - Vector3.up * speed;
            nextPos.y = Mathf.Max(nextPos.y, impactPoint.y);
            transform.position = nextPos;

            fallNoiseEventInstance.getPlaybackState(out PLAYBACK_STATE fallNoiseState);
            if (fallNoiseState != PLAYBACK_STATE.PLAYING) fallNoiseEventInstance.start();

            if (transform.position.y <= impactPoint.y)
            {
                fall = false;

                fallNoiseEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                fallNoiseEventInstance.release();

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