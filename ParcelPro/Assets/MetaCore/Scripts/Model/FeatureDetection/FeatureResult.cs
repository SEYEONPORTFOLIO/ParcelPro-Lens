// 필요한 네임스페이스를 사용합니다.
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
            // [System.Serializable] 어트리뷰트를 사용하여 클래스를 직렬화 가능하게 만듭니다.
            [System.Serializable]
            public class FeatureResult //[클래스설명] 특징 검출 결과와 그에 대한 정보를 저장하고 직렬화/역직렬화하는 클래스.
            {
                // Feature 객체의 List를 저장하는 속성을 정의합니다.
                public List<Feature> features;

                // JSON 문자열을 FeatureResult 객체로 역직렬화하는 정적 메서드를 정의합니다.
                public static FeatureResult deserialize(string jsonString)
                {
                    FeatureResult obj = JsonConvert.DeserializeObject<FeatureResult>(jsonString);
                    return obj;
                }

                // FeatureResult 객체를 JSON 문자열로 직렬화하는 정적 메서드를 정의합니다.
                public static string serialize(FeatureResult jsonObject)
                {
                    return JsonConvert.SerializeObject(jsonObject);
                }

                // 객체 정보를 문자열로 반환하는 메서드를 정의합니다.
                public string info()
                {
                    string buff = string.Empty;

                    // Feature 객체의 정보를 문자열로 구성합니다.
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
