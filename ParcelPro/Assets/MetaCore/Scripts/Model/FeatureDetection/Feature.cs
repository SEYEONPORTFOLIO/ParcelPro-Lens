using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;


namespace Pnc
{
    namespace Model
    {
        namespace FeatureDetection
        {
            [System.Serializable]
            public class Feature
            {
                public string filename { get; set; }
                public int status { get; set; }

                public List<float> position = new List<float>(8);

                public List<float> positionratio = new List<float>(8);

                public float arearatio;

                //public string rectpoint { get; set; }

                public List<float> getPos()
                {
                    return position;
                }

                public List<float> getPosratio()
                {
                    return positionratio;
                }

                public static Feature deserialize(string jsonString)
                {
                    Feature feature = JsonConvert.DeserializeObject<Feature>(jsonString);
                    return feature;
                }

                public static string serialize(Feature jsonObject)
                {
                    return JsonConvert.SerializeObject(jsonObject);
                }


                public string info()
                {
                    string buff = string.Empty;
                    //buff = $"[id={id}, isDetect={status}, targetName={name}";
                    return buff;
                }
            }
        }
    }
}