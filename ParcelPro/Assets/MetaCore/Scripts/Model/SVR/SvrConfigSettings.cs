using UnityEngine;
using System.Collections;

public class SvrConfigSettings : MonoBehaviour
{
    public Transform reticle;

    void Awake()
    {
    }

    IEnumerator Start()
    {
        if (SvrManager.Instance == null || SvrConfigOptions.Instance == null)
        {
            yield break;
        }

        if (SvrConfigOptions.Instance.TrackEyesEnabled.HasValue)
        {
            SvrManager.Instance.settings.trackEyes = SvrConfigOptions.Instance.TrackEyesEnabled.Value;
        }

        if (SvrConfigOptions.Instance.TrackPositionEnabled.HasValue)
        {
            SvrManager.Instance.settings.trackPosition = SvrConfigOptions.Instance.TrackPositionEnabled.Value;
        }

        if (SvrConfigOptions.Instance.EyeResolutionScale.HasValue)
        {
            SvrManager.Instance.settings.eyeResolutionScaleFactor = SvrConfigOptions.Instance.EyeResolutionScale.Value;
        }
        if (SvrConfigOptions.Instance.OverlayResolutionScale.HasValue)
        {
            SvrManager.Instance.settings.overlayResolutionScaleFactor = SvrConfigOptions.Instance.OverlayResolutionScale.Value;
        }

        if (SvrConfigOptions.Instance.FoveationLevel.HasValue)
        {
            SvrManager.Instance.settings.foveationLevel = (SvrManager.SvrSettings.eFoveationLevel)SvrConfigOptions.Instance.FoveationLevel;
        }
        else if (SvrConfigOptions.Instance.FoveationEnabled.HasValue)
        {
            if (SvrConfigOptions.Instance.FoveationEnabled.Value)
                SetFoveatedRendering(SvrConfigOptions.Instance.FoveationGain, SvrConfigOptions.Instance.FoveationArea, SvrConfigOptions.Instance.FoveationMinimum);
            else
                SetFoveatedRendering(Vector2.zero, 0, 0);
        }
        if (SvrConfigOptions.Instance.FoveationSubsampled.HasValue)
        {
            if (SvrConfigOptions.Instance.FoveationSubsampled.Value)
                SvrManager.Instance.settings.optionFlags |= SvrManager.SvrSettings.eOptionFlags.FoveationSubsampled;
            else
                SvrManager.Instance.settings.optionFlags &= ~SvrManager.SvrSettings.eOptionFlags.FoveationSubsampled;
        }

        if (SvrManager.Instance.gaze != null && SvrConfigOptions.Instance.GazeReticleEnabled.HasValue)
        {
            SvrManager.Instance.gaze.gameObject.SetActive(SvrConfigOptions.Instance.GazeReticleEnabled.Value);
        }

        if (SvrDebugHud.Instance != null && SvrConfigOptions.Instance.DebugHudEnabled.HasValue)
        {
            SvrDebugHud.Instance.gameObject.SetActive(SvrConfigOptions.Instance.DebugHudEnabled.Value);
        }

        if (SvrConfigOptions.Instance.OverrideRenderTextureMSAA != 0)
        {
            SetSvrRenderTextureAntialiasing(SvrConfigOptions.Instance.OverrideRenderTextureMSAA);
        }

        if (reticle == null && SvrManager.Instance.reticleOverlay != null)
        {
            reticle = SvrManager.Instance.reticleOverlay.transform;
        }

        if (reticle != null)
        {
            if (SvrConfigOptions.Instance.GazeReticleEnabled.HasValue)
                reticle.gameObject.SetActive(SvrConfigOptions.Instance.GazeReticleEnabled.Value);
        }

        if (SvrConfigOptions.Instance.VideoSeeThruEnabled.HasValue)
        {
            SvrManager.Instance.settings.cameraPassThruVideo = SvrConfigOptions.Instance.VideoSeeThruEnabled.Value ? SvrManager.SvrSettings.eCameraPassThruVideo.Enabled : SvrManager.SvrSettings.eCameraPassThruVideo.Disabled;
        }

        yield return new WaitUntil(() => SvrManager.Instance.Initialized);

        if (SvrConfigOptions.Instance.StartPosition.HasValue)
        {
            SvrManager.Instance.transform.localPosition = SvrConfigOptions.Instance.StartPosition.Value;
        }
        if (SvrConfigOptions.Instance.StartRotation.HasValue)
        {
            SvrManager.Instance.transform.localRotation = Quaternion.Euler(SvrConfigOptions.Instance.StartRotation.Value);
        }

        if (SvrConfigOptions.Instance.UseFixedViewport)
        {
            DisableSvrInput();
            SetSvrCameraView(SvrConfigOptions.Instance.FixedViewportPosition, SvrConfigOptions.Instance.FixedViewportEulerAnglesRotation);
        }

        if (SvrConfigOptions.Instance.FreezeAnimations)
        {
            FreezeAllAnimationsAtTime(Mathf.Max(0, SvrConfigOptions.Instance.FreezeAnimationsAtTimeInSecs));
        }

        if (SvrConfigOptions.Instance.DisableAudio)
        {
            DisableAudio();
        }
    }

    void Update()
    {
        if (!SvrManager.Instance)
        {
            return;
        }
    }

    private void FreezeAllAnimationsAtTime(float timeInSec)
    {
        Animator[] animators = GameObject.FindObjectsOfType<Animator>();
        foreach (Animator animator in animators)
        {
            animator.updateMode = AnimatorUpdateMode.Normal;
            animator.Update(timeInSec);
        }

        Time.timeScale = 0;
    }

    private void DisableSvrInput()
    {
        if (!SvrManager.Instance)
        {
            return;
        }

        SvrManager.Instance.DisableInput = true;
    }

    private void SetSvrCameraView(Vector3 position, Vector3 eulerAnglesRotation)
    {
        if(!SvrManager.Instance)
        {
            return;
        }

        SvrManager.Instance.transform.position = position;
        SvrManager.Instance.transform.eulerAngles = eulerAnglesRotation;
    }

    private void SetSvrRenderTextureAntialiasing(int mode)
    {
        if (!SvrManager.Instance)
        {
            return;
        }

        switch (mode)
        {
            case 1:
                SvrManager.Instance.settings.eyeAntiAliasing = SvrManager.SvrSettings.eAntiAliasing.k1;
                break;
            case 2:
                SvrManager.Instance.settings.eyeAntiAliasing = SvrManager.SvrSettings.eAntiAliasing.k2;
                break;
            case 4:
                SvrManager.Instance.settings.eyeAntiAliasing = SvrManager.SvrSettings.eAntiAliasing.k4;
                break;

            default:
                Debug.LogError("Antialiasing: " + mode + " not supported!");
                break;
        }
    }

    private void DisableAudio()
    {

        AudioSource [] audioSources = GameObject.FindObjectsOfType<AudioSource>();
        foreach(AudioSource audio in audioSources)
        {
            audio.enabled = false;
        }
    }

    private void SetFoveatedRendering(Vector2 gain, float area, float minimum)
    {
        SvrManager.Instance.settings.foveationGain = gain;
        SvrManager.Instance.settings.foveationArea = area;
        SvrManager.Instance.settings.foveationMinimum = minimum;
    }

}
