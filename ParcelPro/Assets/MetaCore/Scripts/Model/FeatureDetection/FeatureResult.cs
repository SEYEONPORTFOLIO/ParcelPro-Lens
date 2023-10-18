using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Pnc
{
    namespace Model
    {
        namespace FeatureDetection
        {
            [System.Serializable]
            public class FeatureResult
            {
                public List<Feature> features;

                //public string cameraId;
                //public string getCameraId()
                //{
                //    return cameraId;
                //}
                //public void setCameraId(string cameraId)
                //{
                //    this.cameraId = cameraId;
                //}

                public List<Feature> getFeatures()
                {
                    return features;
                }

                public static FeatureResult deserialize(string jsonString)
                {
                    FeatureResult obj = JsonConvert.DeserializeObject<FeatureResult>(jsonString);
                    return obj;
                }

                public static string serialize(FeatureResult jsonObject)
                {
                    return JsonConvert.SerializeObject(jsonObject);
                }


                public string info()
                {
                    string buff = string.Empty;

                    //buff = $"[cameraId={getCameraId()}] ";
                    foreach (Feature feature in features)
                    {
                        buff += feature.info() + " ";
                    }
                    return buff;
                }
            }
        }
    }
}
