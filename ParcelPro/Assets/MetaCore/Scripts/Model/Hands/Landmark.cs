using System.Collections.Generic;
using Newtonsoft.Json;

namespace Pnc
{
  namespace Model
  {
    namespace Hands
    {
      [System.Serializable]
      public class Landmark
      {
        public Landmark(float first, float second, float third)
        {
          x = first;
          y = second;
          z = third;
        }

        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }

        public string pointToString()
        {
          string point = "Landmark(" + x + " " + y + " " + z + ")";
          return point;
        }

        public static Landmark deserialize(string jsonString)
        {
          Landmark obj = JsonConvert.DeserializeObject<Landmark>(jsonString);
          return obj;
        }

        public static string serialize(Landmark jsonObject)
        {
          return JsonConvert.SerializeObject(jsonObject);
        }

      }
    } //hands
  } //model
} //pnc