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
    namespace Hands
    {
      [System.Serializable]
      public class HandsResult
      {
        public List<Hand> hands;


        public string cameraId;

        public string getCameraId() {
            return cameraId;
        }

        public void setCameraId(string cameraId) {
            this.cameraId = cameraId;
        }

        public List<Hand> getHands()
        {
          return hands;
        }

        public static HandsResult deserialize(string jsonString)
        {
          HandsResult obj = JsonConvert.DeserializeObject<HandsResult>(jsonString);
          return obj;
        }

        public static string serialize(HandsResult jsonObject)
        {
          return JsonConvert.SerializeObject(jsonObject);
        }


        public string info()
        {
          string buff = string.Empty;

          buff = $"[cameraId={getCameraId()}, hands length={hands.Count}] ";
          foreach (Hand hand in hands)
          {
            buff += hand.info() + " ";
          }
          return buff;
        }
      }
    }// hands
  } //model
} //pnc
