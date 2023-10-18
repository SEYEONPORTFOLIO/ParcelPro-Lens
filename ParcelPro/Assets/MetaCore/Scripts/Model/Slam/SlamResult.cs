using UnityEngine;
using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace Pnc
{
  namespace Model
  {
    namespace Slam
    {
      [System.Serializable]
      public class SlamResult
      {
        //plane detect state
        public static int PLANE_DETECTED = 233;
        public static int PLANE_NOT_DETECTED = 1234;

        //slam state
        public static int SLAM_SYSTEM_NOT_READY = -1;
        public static int SLAM_NO_IMAGES_YET = 0;
        public static int SLAM_NOT_INITIALIZED = 1;
        public static int SLAM_ON = 2;
        public static int SLAM_RECENTLY_LOST = 3;
        public static int SLAM_LOST = 4;
        public static int SLAM_OK_KLT = 4;
                
        int trackingResult;
        int planeDetect;

        string trackingResultDesc;
        string planeDetectDesc;

        public List<float> m; //model matrix
        public List<float> v; //view matrix
        public List<float> p; //projection matrix

        public string cameraId;

        public int getTrackingResult() {
          return trackingResult;
        }

        public void setTrackingResult(int result) {
            trackingResult = result;
        }

        public string getTrackingResultDesc() {
            return trackingResultDesc;
        }

        public void setTrackingResultDesc(string desc) {
            trackingResultDesc = desc;
        }

        public int getPlaneDetect() {
            return planeDetect;
        }

        public void setPlaneDetect(int result) {
            planeDetect = result;
        }

        public string getPlaneDetectDesc() {
            return planeDetectDesc;
        }

        public void setPlaneDetectDesc(string desc) {
            planeDetectDesc = desc;
        }

        public List<float> getModel()
        {
          return m;
        }

        public List<float> getView()
        {
          return v;
        }

        public List<float> getProjection()
        {
          return p;
        }

        public string getCameraId() {
            return cameraId;
        }

        public void setCameraId(string cameraId) {
            this.cameraId = cameraId;
        }

        // public static SlamResult deserialize(string jsonString)
        // {
        //   SlamResult obj = JsonConvert.DeserializeObject<SlamResult>(jsonString);
        //   return obj;
        // }

        // public static string serialize(SlamResult jsonObject)
        // {
        //   return JsonConvert.SerializeObject(jsonObject);
        // }

        public string info()
        {
          string buff = string.Empty;

          buff = $"[SlamResult cameraId={getCameraId()}, trackingResult={getTrackingResult()}->{getTrackingResultDesc()}"
          + $", planeDetect={getPlaneDetect()}->{getPlaneDetectDesc()}, m.length={m.Count}, v.length={v.Count}, p.length={p.Count}";
          return buff;
        }        
      }
    }// slam
  } //model
} //pnc
