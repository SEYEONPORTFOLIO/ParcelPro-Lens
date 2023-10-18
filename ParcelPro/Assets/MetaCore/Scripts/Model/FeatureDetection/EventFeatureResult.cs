using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace Pnc
{
    namespace Model
    {
        namespace FeatureDetection
        {
            [System.Serializable]
            public class EventFeatureResult : PncEvent
            {
                public FeatureResult featureResult;

                public FeatureResult getFeatureResult()
                {
                    return featureResult;
                }

                public void setFeatureResult(FeatureResult featureresult)
                {
                    this.featureResult = featureresult;
                }

                public string info()
                {
                    string buff = string.Empty;

                    buff = $"EventFeatureResult=[owner={getOwner()}, event type={getEventType()}" +
                    $", timestamp={getTimestamp()}, featureresult={featureResult.info()}] ";

                    return buff;
                }
            }
        }
    }
}
