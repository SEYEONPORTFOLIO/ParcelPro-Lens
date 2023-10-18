using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace util
{
  public class AndroidUtils {
    public static void showToast(string message)
    {
      AndroidJavaObject _currentActivity;
      AndroidJavaClass _unityPlayer;
      AndroidJavaObject _context;
      AndroidJavaObject _toast;

        _unityPlayer = 
        	new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        
        _currentActivity = _unityPlayer
        	.GetStatic<AndroidJavaObject>("currentActivity");
        	
        	
        _context = _currentActivity
        	.Call<AndroidJavaObject>("getApplicationContext");

        _currentActivity.Call
        (
	        "runOnUiThread", 
	        new AndroidJavaRunnable(() =>
	        {
	            AndroidJavaClass toastobject 
	            = new AndroidJavaClass("android.widget.Toast");
	            
	            AndroidJavaObject javaString 
	            = new AndroidJavaObject("java.lang.String", message);
	            
	            _toast = toastobject.CallStatic<AndroidJavaObject>
	            (
	            	"makeText", 
	            	_context, 
	            	javaString, 
	            	toastobject.GetStatic<int>("LENGTH_SHORT")
	            );
	            
	            _toast.Call("show");
	        })
	     );
    }

    public static void cancelToast()
    {
        // _currentActivity.Call("runOnUiThread", 
        // 	new AndroidJavaRunnable(() =>
	      //   {
	      //       if (_toast != null) _toast.Call("cancel");
	      //   }));
    }


    public static void launchApp(string packagename)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        // AndroidJavaObject pm = currentActivity.Call<AndroidJavaObject>("getPackageManager");

        // AndroidJavaObject intent = pm.Call<AndroidJavaObject>("getLaunchIntentForPackage", packagename);
        // intent.setFlags(Intent.FLAG_ACTIVITY_REORDER_TO_FRONT);
        string activityname = "";
        AndroidJavaObject param1 = new AndroidJavaObject("java.lang.String", packagename);
        AndroidJavaObject param2 = new AndroidJavaObject("java.lang.String", activityname);

        currentActivity.CallStatic("goApp", currentActivity, param1, param2);

    }


    public static void goAction(string command) {
      //{"홈으로","드로잉","블루투스 설정","와이파이 설정",,"피플","앱 종료","박스월드"};

      switch(command) {
        // case "홈으로":
        //   launchApp("kr.co.pncsolution.arlauncher");
        // break;
        case "드로잉":
          launchApp("kr.co.pncsolution.ardrawing"); 
        break;
        case "블루투스 설정":
          showToast("'[Bluetooth setting] Sorry, Not support yet!");
        break;
        case "와이파이 설정":
          launchApp("kr.co.pncsolution.QRwificonnector");
        break;
        case "피플":
          launchApp("kr.co.pncsolution.people");
        break;
        case "박스월드":
          launchApp("kr.co.pncsolution.boxworld");
        break;
        // case "앱 종료":
        //   goAppClose();
        // break;
        default:
        break;
      }

    } 

    public static float getEuclideanDistance_3D(Vector3 point1, Vector3 point2) {
        float fX = point1.x - point2.x;
        float fY = point1.y - point2.y;
        float fZ = point1.z - point2.z;

        return Mathf.Sqrt((fX * fX) + (fY * fY) + (fZ * fZ));
    }

    private void goHome() {
      launchApp("kr.co.pncsolution.arlauncher");
    }

    private void goArDrawing() {
      launchApp("kr.co.pncsolution.ardrawing"); 
    }

    private void goBluetoothSettings() {
      showToast("'[Bluetooth setting] Sorry, Not support yet!");
    }

    private void goWifiSettings() {
      launchApp("kr.co.pncsolution.QRwificonnector");
    }
    private void goArPeople() {
      launchApp("kr.co.pncsolution.people");
    }

    private void goAppClose() {
      showToast("'[Close App] Sorry, Not support yet!");
    }

    private void goBoxworld() {
      launchApp("kr.co.pncsolution.boxworld");
    }

  } //AndroidUtils
} //namespace util
