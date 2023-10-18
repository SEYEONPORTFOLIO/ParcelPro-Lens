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
      public class Hand
      {
        public string type { get; set; }     //left or right
        public int gesture { get; set; }     //gesture
        public string distance { get; set; }
        public List<Landmark> landmarks;

        public List<Landmark> getLandmarks()
        {
          return landmarks;
        }

        public static Hand deserialize(string jsonString)
        {
          Hand hand = JsonConvert.DeserializeObject<Hand>(jsonString);
          return hand;
        }

        public static string serialize(Hand jsonObject)
        {
          return JsonConvert.SerializeObject(jsonObject);
        }


        public string info()
        {
          string buff = string.Empty;

          buff = $"[length={landmarks.Count}, type={type}, gesture={gesture}, distance={distance}";

          foreach (Landmark landmark in landmarks)
          {
            buff += landmark.pointToString() + " ";
          }
          return buff;
        }

      }
    } // hands
  } //model
} //pnc
