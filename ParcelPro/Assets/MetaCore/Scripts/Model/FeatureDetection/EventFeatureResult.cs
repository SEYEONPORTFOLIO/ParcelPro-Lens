// 필요한 네임스페이스를 사용합니다.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// JSON 직렬화를 위해 Newtonsoft.Json 라이브러리를 사용합니다.
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace Pnc
{
    namespace Model
    {
        namespace FeatureDetection
        {
            // [System.Serializable] 어트리뷰트를 사용하여 클래스를 직렬화 가능하게 만듭니다.
            [System.Serializable]
            public class EventFeatureResult : PncEvent //[클래스 설명]  PncEvent를 확장하고 특징 검출 결과를 포함하는 이벤트 객체 클래스.
            {
                // FeatureResult 객체를 포함하는 속성을 정의합니다.
                public FeatureResult featureResult;

                // FeatureResult 객체를 반환하는 메서드를 정의합니다.
                public FeatureResult getFeatureResult()
                {
                    return featureResult;
                }

                // FeatureResult 객체를 설정하는 메서드를 정의합니다.
                public void setFeatureResult(FeatureResult featureresult)
                {
                    this.featureResult = featureresult;
                }

                // 객체 정보를 문자열로 반환하는 메서드를 정의합니다.
                public string info()
                {
                    string buff = string.Empty;

                    // 객체 정보를 문자열로 구성합니다.
                    buff = $"EventFeatureResult=[owner={getOwner()}, event type={getEventType()}" +
                    $", timestamp={getTimestamp()}, featureresult={featureResult.info()}] ";

                    return buff;
                }
            }
        }
    }
}
