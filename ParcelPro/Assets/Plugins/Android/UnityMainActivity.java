package kr.co.pncsolution.aidlmodule;

import android.net.wifi.WifiManager;
import android.net.wifi.WifiInfo;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.os.Bundle;
import android.util.Log;
import android.view.KeyEvent;
import android.view.View;

import kr.co.pncsolution.aidlmodule.unity.OverrideUnityActivity;
import kr.co.pncsolution.service.model.PncEvent;

import android.content.Intent;
import android.content.ComponentName;
import android.content.Context;
import android.content.pm.PackageManager;
import android.content.pm.ApplicationInfo;
import android.net.Uri;
import android.view.Display;
import android.view.Surface;
import android.os.Handler;
import android.app.Activity;
import android.view.WindowManager;
import android.view.Window;
import android.provider.Settings;

import com.unity3d.player.UnityPlayer;

public class UnityMainActivity extends OverrideUnityActivity {

  private static String TAG = "UnityMainActivity";
  private static boolean DEBUG = true;

  private AidlLoaderPlugin _aidlPlugin = null;

  @Override
  protected void onCreate(Bundle savedInstanceState)
  {
      super.onCreate(savedInstanceState);

      if(DEBUG) Log.i(TAG, ":::::##########     onCreate     ##########:::::");
      sendMessageToUnity("OnEventArrived", PncEvent.CreateEmptyEvent("system", "onCreate"));

      // _aidlPlugin = new AidlLoaderPlugin(this);
      // showMainActivity("");

      getRotation();
  }

  @Override
  protected void onStart() {
    super.onStart();

    if(DEBUG) Log.i(TAG, ":::::##########     onStart     ##########:::::");
    sendMessageToUnity("OnEventArrived", PncEvent.CreateEmptyEvent("system", "onStart"));
  }

  @Override
  protected void onResume() {
    super.onResume();

    if(DEBUG) Log.i(TAG, ":::::##########     onResume     ##########:::::");
    sendMessageToUnity("OnEventArrived", PncEvent.CreateEmptyEvent("system", "onResume"));
  }

  @Override
  protected void onPause() {
    super.onPause();

    if(DEBUG) Log.i(TAG, ":::::##########     onPause     ##########:::::");
      sendMessageToUnity("OnEventArrived", PncEvent.CreateEmptyEvent("system", "onPause"));

  }

  @Override
  protected void onStop() {
    super.onStop();

    if(DEBUG) Log.i(TAG, ":::::##########     onStop     ##########:::::");
    sendMessageToUnity("OnEventArrived", PncEvent.CreateEmptyEvent("system", "onStop"));
  }

  @Override
  protected void onDestroy() {
    super.onDestroy();

    if(DEBUG) Log.i(TAG, ":::::##########     onDestroy     ##########:::::");
    sendMessageToUnity("OnEventArrived", PncEvent.CreateEmptyEvent("system", "onDestroy"));
  }

  @Override
  protected void showMainActivity(String setToColor) {
      //if(DEBUG) Log.i(TAG, ":::::##########     showMainActivity     ##########:::::");
  }

  // @Override
  // public boolean onKeyDown(int keyCode, KeyEvent event) {
  //     if (keyCode == KeyEvent.KEYCODE_VOLUME_DOWN) {
  //         _aidlPlugin.onEventArrived(PncEvent.CreateEmptyEvent("system", "volumeDown"));
  //     } else if (keyCode == KeyEvent.KEYCODE_VOLUME_UP) {
  //         _aidlPlugin.onEventArrived(PncEvent.CreateEmptyEvent("system", "volumeUp"));
  //     }
  //     return true;
  // }

  @Override
  public void onWindowFocusChanged(boolean hasFocus) {
      super.onWindowFocusChanged(hasFocus);
      if (hasFocus) {
          hideSystemUI();
      }
  }

  private void hideSystemUI() {
      // Enables regular immersive mode.
      // For "lean back" mode, remove SYSTEM_UI_FLAG_IMMERSIVE.
      // Or for "sticky immersive," replace it with SYSTEM_UI_FLAG_IMMERSIVE_STICKY
      View decorView = getWindow().getDecorView();
      decorView.setSystemUiVisibility(
              View.SYSTEM_UI_FLAG_IMMERSIVE
                      // Set the content to appear under the system bars so that the
                      // content doesn't resize when the system bars hide and show.
                      | View.SYSTEM_UI_FLAG_LAYOUT_STABLE
                      | View.SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION
                      | View.SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN
                      // Hide the nav bar and status bar
                      | View.SYSTEM_UI_FLAG_HIDE_NAVIGATION
                      | View.SYSTEM_UI_FLAG_FULLSCREEN);
  }

  // Shows the system bars by removing all the flags
  // except for the ones that make the content appear under the system bars.
  private void showSystemUI() {
      View decorView = getWindow().getDecorView();
      decorView.setSystemUiVisibility(
              View.SYSTEM_UI_FLAG_LAYOUT_STABLE
                      | View.SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION
                      | View.SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN);
  }

  public int onWifiCheck(Context context)
  {
    int nResult = -1;

    WifiManager wifi = (WifiManager)context.getApplicationContext().getSystemService(context.WIFI_SERVICE);

    if(DEBUG) Log.i(TAG, "onWifiCheck isWifiEnabled : " + wifi.isWifiEnabled());
    if(!wifi.isWifiEnabled())
    {
        return nResult;
    }

    WifiInfo info = wifi.getConnectionInfo();
    // String serviceInfo = info.getSSID();
    // if(DEBUG) Log.i(TAG, "onWifiCheck ssid" + serviceInfo + ", bssid: " + info.getBSSID() + ", RSSI: " + info.getRssi());

    // if(serviceInfo != null && serviceInfo.contains("unknown ssid")) {
    //   ConnectivityManager connectivityManager = (ConnectivityManager)context.getSystemService(context.CONNECTIVITY_SERVICE);
    //   if(DEBUG) Log.i(TAG, "onWifiCheck get ConnectivityManager");

    //   //NetworkInfo wifiInfo = connectivityManager.getNetworkInfo(ConnectivityManager.TYPE_WIFI);
    //   NetworkInfo wifiInfo = connectivityManager.getActiveNetworkInfo();
    //   if(DEBUG) Log.i(TAG, "onWifiCheck NetworkInfo:" + wifiInfo.isConnected());

    //   if(!wifiInfo.isConnected())
    //   {
    //       return nResult;
    //   }

    //   String wifiName = wifiInfo.getExtraInfo();
    //   if(DEBUG) Log.i(TAG, "onWifiCheck wifiName:" + wifiName);

    //   if(wifiName != null)
    //   {
    //     if (wifiName.startsWith("\"")) {
    //         wifiName = wifiName.substring(1, wifiName.length());
    //     }
    //     if (wifiName.endsWith("\"")) {
    //         wifiName = wifiName.substring(0, wifiName.length() - 1);
    //     }
    //   }
    //   serviceInfo = wifiName;
    //   if(DEBUG) Log.i(TAG, "onWifiCheck ssid2:" + serviceInfo);
    // }

    //List<ScanResult> results = manager.getScanResults();
    nResult = WifiManager.calculateSignalLevel(info.getRssi(), 5);
    if(DEBUG) Log.i(TAG, "onWifiCheck level : " + nResult);

    return nResult;
  }

  public String getWifiSSID(Context context)
  {
    WifiManager wifi = (WifiManager)context.getApplicationContext().getSystemService(context.WIFI_SERVICE);

    if(DEBUG) Log.i(TAG, "getWifiSSID isWifiEnabled : " + wifi.isWifiEnabled());
    if(!wifi.isWifiEnabled())
    {
        return "";
    }

    WifiInfo info = wifi.getConnectionInfo();
    String serviceInfo = info.getSSID();
    if(DEBUG) Log.i(TAG, "getWifiSSID ssid" + serviceInfo + ", bssid: " + info.getBSSID() + ", RSSI: " + info.getRssi());

    return serviceInfo;
  }

  private int getBrightness(Context context)
  {
    int screenBrightness = 255;
    try {
        screenBrightness = Settings.System.getInt(context.getContentResolver(), Settings.System.SCREEN_BRIGHTNESS);
        Log.i(TAG, String.format("getBrightness screenBrightness : %d", screenBrightness));
    } catch (Exception e) {
        e.printStackTrace();
    }
    return screenBrightness;
    // Window window = getWindow();
    // WindowManager.LayoutParams layoutParams = window.getAttributes();
    
    // Log.i(TAG, String.format("getBrightness screenBrightness : %.3f", layoutParams.screenBrightness));
    // return layoutParams.screenBrightness;
  }

  private int changeScreenBrightness(Context context, int value)
  {
    Log.i(TAG, String.format("changeScreenBrightness value : %d", value));
    Settings.System.putInt(context.getContentResolver(), Settings.System.SCREEN_BRIGHTNESS, value);
    // Window window = getWindow();
    // WindowManager.LayoutParams layoutParams = window.getAttributes();
    // layoutParams.screenBrightness = value * 1.0f / 255;
    // window.setAttributes(layoutParams);

    return 0;
  }

  public void test()
  {
    if(DEBUG) Log.i(TAG, "test wifi call : ");
    return;
  }

  /*
    * unity 에서 특정패키지를 실행하는 함수
    */

  public static void goApp(Context context, String packageName, String activityName) {
    //AndroidManifest.xml 에 추가
    
    if(DEBUG) Log.w(TAG, ":::::     goApp, package=" + packageName + ", avtivity=" + activityName + "     :::::");

    new Handler().postDelayed(new Runnable() {
      @Override
      public void run() {
        tryGoApp(context, packageName, activityName);
      }
    }, 50);      

    //((Activity)context).finish();
    
  }

  public static void tryGoApp(Context context, String packageName, String activityName) {      
    try {        

      Intent intent = context.getPackageManager().getLaunchIntentForPackage(packageName);

      //intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
      intent.addFlags(Intent.FLAG_ACTIVITY_REORDER_TO_FRONT);
      if(activityName!=null && activityName.length()>0) {
          ComponentName compName = new ComponentName(packageName, activityName);
          intent.setComponent(compName);
      } else {
          intent.addCategory(Intent.CATEGORY_LAUNCHER);
      }
      context.startActivity(intent);
    } catch (Exception e) {
      e.printStackTrace();
      String url = "market://details?id=" + packageName;
      try {
          Intent i = new Intent(Intent.ACTION_VIEW, Uri.parse(url));
          context.startActivity(i);
      } catch(Exception e0) {
        Log.e(TAG, "Not found activity");
      }
    }
  }

  public int getRotation() {

    Display display = getWindowManager().getDefaultDisplay();


    float newRotation = 0.0f;
    int orientation = display.getRotation();
    switch(orientation)
    {
        case Surface.ROTATION_0:
            // portrait on hand-held device
            if(DEBUG) Log.i(TAG, ":::::     orientation=ROTATION_0     :::::");
            break;
        case Surface.ROTATION_90:
            // left landscape on hand-held device
            if(DEBUG) Log.i(TAG, ":::::     orientation=ROTATION_90     :::::");
            break;
        case Surface.ROTATION_180:
            // most hand-held devices do not have flip rotation
            if(DEBUG) Log.i(TAG, ":::::     orientation=ROTATION_180     :::::");
            break;
        case Surface.ROTATION_270:
            // right landscape on hand-held device
            if(DEBUG) Log.i(TAG, ":::::     orientation=ROTATION_270     :::::");    
            break;
    }  
    return orientation;
  }

  public void sendMessageToUnity(String function, String param) {
    if(DEBUG) Log.i(TAG, ":::::     sendMessageToUnity::func=" + function + ", param=" + param + "     :::::");
    try {
        UnityPlayer.UnitySendMessage("AndroidServiceBridge", function, param);
    } catch (NoClassDefFoundError e) {
        //unity 환경 아님..
        Log.e(TAG, ":::::     Exception what=" + e.getMessage() + "     :::::");
    }
  }
  
  public int getAddList(String packageName)
  {
    //if(DEBUG) Log.i(TAG, ":::::     getAddList packageName=" + packageName);
    int result = 0;
    try {
      ApplicationInfo ai = getPackageManager().getApplicationInfo(packageName, PackageManager.GET_META_DATA);
      if (ai.metaData != null) {
        String addList = ai.metaData.getString("kr.co.pncsolution.arlauncher.addlist");        
        if(addList != null && addList.equals("yes"))
        {
          if(DEBUG) Log.i(TAG, ":::::     getAddList addList=" + addList);
          result = 1;
        }
      }
    } catch (PackageManager.NameNotFoundException e) {
      // if we can't find it in the manifest, just return null
      result = 0;
    }

    return result;
  }
}
