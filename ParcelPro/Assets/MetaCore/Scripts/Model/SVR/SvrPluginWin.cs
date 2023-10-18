using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;

class SvrPluginWin : SvrPlugin
{
	public static SvrPluginWin Create()
	{
		return new SvrPluginWin ();
	}

    private SvrPluginWin()
    {
        beginEyeCommandBuffer = new CommandBuffer();
        //beginEyeCommandBuffer.ClearRenderTarget(true, true, Color.red);
    }

    public override bool IsInitialized() { return svrCamera != null; }

    public override bool IsRunning() { return eyes != null; }

    public override IEnumerator Initialize()
	{
        yield return base.Initialize();

        deviceInfo = GetDeviceInfo();

        yield break;
	}

	public override IEnumerator BeginVr(int cpuPerfLevel, int gpuPerfLevel, int optionFlags)
	{
        yield return base.BeginVr(cpuPerfLevel, gpuPerfLevel, optionFlags);

        SvrManager.Instance.SubmitEvent(new SvrManager.SvrEvent { eventType = SvrManager.svrEventType.kEventVrModeStarted, deviceId = 0, eventData = { }, eventTimeStamp = 0.0f });

        yield break;
    }

    public override void EndVr()
    {
        base.EndVr();

        SvrManager.Instance.SubmitEvent(new SvrManager.SvrEvent { eventType = SvrManager.svrEventType.kEventVrModeStopped, deviceId = 0, eventData = { }, eventTimeStamp = 0.0f });
    }

    public override void SetVSyncCount(int vSyncCount)
    {
        QualitySettings.vSyncCount = vSyncCount;
    }

    public override int GetHeadPose(ref HeadPose headPose, int frameIndex)
    {
        int poseStatus = 0;

        //headPose.orientation = Quaternion.identity;
        //headPose.position = Vector3.zero;}

        if (Input.GetKey(KeyCode.LeftControl))
        {
            Vector2 mouseNDC = Vector2.zero;
            mouseNDC.x = 2 * (Input.mousePosition.x / Screen.width) - 1f;
            mouseNDC.y = 2 * (Input.mousePosition.y / Screen.height) - 1f;

            Vector3 eulers = Vector3.zero;
            eulers.y = mouseNDC.x * 90f;  // +/- degrees
            eulers.x = -mouseNDC.y * 45f;  // +/- degrees

            headPose.orientation.eulerAngles = eulers;

            poseStatus |= (int)TrackingMode.kTrackingOrientation;
        }

        if (Input.GetKey(KeyCode.LeftAlt))
        {
            Vector2 mouseNDC = Vector2.zero;
            mouseNDC.x = 2 * (Input.mousePosition.x / Screen.width) - 1f;
            mouseNDC.y = 2 * (Input.mousePosition.y / Screen.height) - 1f;

            Vector3 offset = new Vector3(mouseNDC.x, 0.0f, mouseNDC.y);
            headPose.position = headPose.orientation * offset;

            //headPose.position.x = mouseNDC.x;
            //headPose.position.z = mouseNDC.y;

            poseStatus |= (int)TrackingMode.kTrackingPosition;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            Vector3 offset = new Vector3(-1.0f, 0.0f, 0.0f) * Time.deltaTime;
            headPose.position += headPose.orientation * offset;

            poseStatus |= (int)TrackingMode.kTrackingPosition;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Vector3 offset = new Vector3(1.0f, 0.0f, 0.0f) * Time.deltaTime;
            headPose.position += headPose.orientation * offset;

            poseStatus |= (int)TrackingMode.kTrackingPosition;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            Vector3 offset = new Vector3(0.0f, 0.0f, -1.0f) * Time.deltaTime;
            headPose.position += headPose.orientation * offset;

            poseStatus |= (int)TrackingMode.kTrackingPosition;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            Vector3 offset = new Vector3(0.0f, 0.0f, 1.0f) * Time.deltaTime;
            headPose.position += headPose.orientation * offset;

            poseStatus |= (int)TrackingMode.kTrackingPosition;
        }

        return poseStatus;
    }

    public override DeviceInfo GetDeviceInfo()
	{
		DeviceInfo info 			= new DeviceInfo();

		info.displayWidthPixels 	= Screen.width;
		info.displayHeightPixels 	= Screen.height;
		info.displayRefreshRateHz 	= 60.0f;
		info.targetEyeWidthPixels 	= Screen.width / 2;
		info.targetEyeHeightPixels 	= Screen.height;
		info.targetFovXRad			= Mathf.Deg2Rad * 90;
		info.targetFovYRad			= Mathf.Deg2Rad * 90;
        info.targetFrustumLeft.left     = -0.0428f;
        info.targetFrustumLeft.right    = 0.0428f;
        info.targetFrustumLeft.top      = 0.0428f;
        info.targetFrustumLeft.bottom   = -0.0428f;
        info.targetFrustumLeft.near     = 0.0508f;
        info.targetFrustumLeft.far      = 100f;
        info.targetFrustumLeft.position.x = -0.032f;
        info.targetFrustumRight.left    = -0.0428f;
        info.targetFrustumRight.right   = 0.0428f;
        info.targetFrustumRight.top     = 0.0428f;
        info.targetFrustumRight.bottom  = -0.0428f;
        info.targetFrustumRight.near    = 0.0508f;
        info.targetFrustumRight.far     = 100f;
        info.targetFrustumRight.position.x = 0.032f;
        info.trackingCalibration = Matrix4x4.identity;
        info.trackingIntrinsics.FocalLength = Vector2.zero;
        info.trackingIntrinsics.PrincipalPoint = Vector2.zero;
        info.trackingIntrinsics.Distortion0 = 1.0f;
        return info;
	}

	public override void SubmitFrame(int frameIndex, float fieldOfView, int frameType)
	{
		RenderTexture.active = null;
		GL.Clear (false, true, Color.black);

		//float cameraFov = fieldOfView;
		//float fovMarginX = (cameraFov / deviceInfo.targetFovXRad) - 1;
		//float fovMarginY = (cameraFov / deviceInfo.targetFovYRad) - 1;
        //Rect textureRect = new Rect(fovMarginX * 0.5f, fovMarginY * 0.5f, 1 - fovMarginX, 1 - fovMarginY);
        Rect textureRect = new Rect(0, 0, 1, 1);

        Vector2 leftCenter = new Vector2(Screen.width * 0.25f, Screen.height * 0.5f);
		Vector2 rightCenter = new Vector2(Screen.width * 0.75f, Screen.height * 0.5f);
		Vector2 eyeExtent = new Vector3(Screen.width * 0.25f, Screen.height * 0.5f);
		eyeExtent.x -= 10.0f;
		eyeExtent.y -= 10.0f;

		Rect leftScreen = Rect.MinMaxRect(
            leftCenter.x - eyeExtent.x, 
            leftCenter.y - eyeExtent.y, 
            leftCenter.x + eyeExtent.x, 
            leftCenter.y + eyeExtent.y);
		Rect rightScreen = Rect.MinMaxRect(
            rightCenter.x - eyeExtent.x, 
            rightCenter.y - eyeExtent.y, 
            rightCenter.x + eyeExtent.x, 
            rightCenter.y + eyeExtent.y);

        if (eyes != null) for (int i = 0; i < eyes.Length; i++)
        {
            if (eyes[i].isActiveAndEnabled == false) continue;
            if (eyes[i].TexturePtr == null) continue;
            if (eyes[i].imageTransform != null && eyes[i].imageTransform.gameObject.activeSelf == false) continue;
            if (eyes[i].imageTransform != null && !eyes[i].imageTransform.IsChildOf(svrCamera.transform)) continue;   // svr only

            var eyeRectMin = eyes[i].clipLowerLeft; eyeRectMin /= eyeRectMin.w;
            var eyeRectMax = eyes[i].clipUpperRight; eyeRectMax /= eyeRectMax.w;

            if (eyes[i].Side == SvrPlugin.EyeMask.kLeft || eyes[i].Side == SvrPlugin.EyeMask.kBoth)
            {
                leftScreen = Rect.MinMaxRect(
                    leftCenter.x + eyeExtent.x * eyeRectMin.x, 
                    leftCenter.y + eyeExtent.y * eyeRectMin.y, 
                    leftCenter.x + eyeExtent.x * eyeRectMax.x, 
                    leftCenter.y + eyeExtent.y * eyeRectMax.y);
                Graphics.DrawTexture(leftScreen, eyes[i].TexturePtr, textureRect, 0, 0, 0, 0);
            }
            if (eyes[i].Side == SvrPlugin.EyeMask.kRight || eyes[i].Side == SvrPlugin.EyeMask.kBoth)
            {
                rightScreen = Rect.MinMaxRect(
                    rightCenter.x + eyeExtent.x * eyeRectMin.x,
                    rightCenter.y + eyeExtent.y * eyeRectMin.y,
                    rightCenter.x + eyeExtent.x * eyeRectMax.x,
                    rightCenter.y + eyeExtent.y * eyeRectMax.y);
                Graphics.DrawTexture(rightScreen, eyes[i].TexturePtr, textureRect, 0, 0, 0, 0);
            }
        }

        if (overlays != null) for (int i = 0; i < overlays.Length; i++)
        {
            if (overlays[i].isActiveAndEnabled == false) continue;
            if (overlays[i].TexturePtr == null) continue;
            if (overlays[i].imageTransform != null && overlays[i].imageTransform.gameObject.activeSelf == false) continue;
            if (overlays[i].imageTransform != null && !overlays[i].imageTransform.IsChildOf(svrCamera.transform)) continue;   // svr only

            var eyeRectMin = overlays[i].clipLowerLeft; eyeRectMin /= eyeRectMin.w;
            var eyeRectMax = overlays[i].clipUpperRight; eyeRectMax /= eyeRectMax.w;

            textureRect.Set(overlays[i].uvLowerLeft.x, overlays[i].uvLowerLeft.y,
                overlays[i].uvUpperRight.x - overlays[i].uvLowerLeft.x,
                overlays[i].uvUpperRight.y - overlays[i].uvLowerLeft.y);

            if (overlays[i].Side == SvrPlugin.EyeMask.kLeft || overlays[i].Side == SvrPlugin.EyeMask.kBoth)
            {
                leftScreen = Rect.MinMaxRect(
                    leftCenter.x + eyeExtent.x * eyeRectMin.x,
                    leftCenter.y + eyeExtent.y * eyeRectMin.y,
                    leftCenter.x + eyeExtent.x * eyeRectMax.x,
                    leftCenter.y + eyeExtent.y * eyeRectMax.y);
                Graphics.DrawTexture(leftScreen, overlays[i].TexturePtr, textureRect, 0, 0, 0, 0);
            }
            if (overlays[i].Side == SvrPlugin.EyeMask.kRight || overlays[i].Side == SvrPlugin.EyeMask.kBoth)
            {
                rightScreen = Rect.MinMaxRect(
                    rightCenter.x + eyeExtent.x * eyeRectMin.x,
                    rightCenter.y + eyeExtent.y * eyeRectMin.y,
                    rightCenter.x + eyeExtent.x * eyeRectMax.x,
                    rightCenter.y + eyeExtent.y * eyeRectMax.y);
                Graphics.DrawTexture(rightScreen, overlays[i].TexturePtr, textureRect, 0, 0, 0, 0);
            }
        }
 
	}

	public override void Shutdown()
	{
        base.Shutdown();
    }


    private SvrControllerState mControllerState = new SvrControllerState() { rotation = Quaternion.identity };
    private SvrControllerCaps mControllerCaps = new SvrControllerCaps() { deviceManufacturer = "SnapdragonXR", deviceIdentifier = "UnityEditor", caps = 0 };
    private int mControllerBattery = 100;

    //---------------------------------------------------------------------------------------------
    public override int ControllerStartTracking(string desc)
    {
        mControllerState.connectionState = (int)SvrController.svrControllerConnectionState.kConnected;
        return 0;
    }

    //---------------------------------------------------------------------------------------------
    public override void ControllerStopTracking(int handle)
    {
        mControllerState.connectionState = (int)SvrController.svrControllerConnectionState.kDisconnected;
    }

    //---------------------------------------------------------------------------------------------
    public override SvrControllerState ControllerGetState(int handle, int space)
    {
        //var state = new SvrControllerState() { rotation = Quaternion.identity };

        if (mControllerState.connectionState == (int)SvrController.svrControllerConnectionState.kConnected)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                Vector2 mouseNDC = Vector2.zero;
                mouseNDC.x = 2 * (Input.mousePosition.x / Screen.width) - 1f;
                mouseNDC.y = 2 * (Input.mousePosition.y / Screen.height) - 1f;

                Vector3 eulers = Vector3.zero;
                eulers.y = mouseNDC.x * 90f;  // +/- degrees
                eulers.x = -mouseNDC.y * 45f;  // +/- degrees

                mControllerState.rotation.eulerAngles = eulers;

                mControllerState.position = SvrManager.Instance.head.position - new Vector3(0.0f, -0.1f, 0.2f);
            }

            {
                int buttons = 0;
                // Trigger
                if (Input.GetMouseButton(0))
                    buttons |= (int)SvrController.svrControllerButton.PrimaryIndexTrigger;
                // Thumbpad
                if (Input.GetMouseButton(1))
                    buttons |= (int)SvrController.svrControllerButton.PrimaryThumbstick;
                // Start
                if (Input.GetKey(KeyCode.Return))
                    buttons |= (int)SvrController.svrControllerButton.Start;
                // Back
                if (Input.GetKey(KeyCode.Backspace))
                    buttons |= (int)SvrController.svrControllerButton.Back;
                // Dpad
                if (Input.GetKey(KeyCode.LeftArrow))
                    buttons |= (int)SvrController.svrControllerButton.DpadLeft;
                else if (Input.GetKey(KeyCode.RightArrow))
                    buttons |= (int)SvrController.svrControllerButton.DpadRight;
                else if (Input.GetKey(KeyCode.UpArrow))
                    buttons |= (int)SvrController.svrControllerButton.DpadUp;
                else if (Input.GetKey(KeyCode.DownArrow))
                    buttons |= (int)SvrController.svrControllerButton.DpadDown;

                mControllerState.buttonState = buttons;
            }
        }

        return mControllerState;
    }

    //---------------------------------------------------------------------------------------------
    public override void ControllerSendMessage(int handle, SvrController.svrControllerMessageType what, int arg1, int arg2)
    {
    }

    //---------------------------------------------------------------------------------------------
    public override object ControllerQuery(int handle, SvrController.svrControllerQueryType what)
    {
        object result = null;
        switch (what)
        {
            case SvrController.svrControllerQueryType.kControllerBatteryRemaining:
                {
                    result = mControllerBattery;
                }
                break;
            case SvrController.svrControllerQueryType.kControllerControllerCaps:
                {
                    result = mControllerCaps;
                }
                break;
        }
        return result;
    }

}
