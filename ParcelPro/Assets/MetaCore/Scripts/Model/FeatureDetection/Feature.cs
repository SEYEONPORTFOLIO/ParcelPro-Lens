// 필요한 네임스페이스를 사용합니다.
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
            // [System.Serializable] 어트리뷰트를 사용하여 클래스를 직렬화 가능하게 만듭니다.
            [System.Serializable]
            public class Feature //[클래스 설명]특징의 파일 이름, 상태, 위치, 비율 정보를 저장하고 직렬화/역직렬화하는 클래스.
            {
                // 파일 이름을 저장하는 속성을 정의합니다.
                public string filename { get; set; }

                // 상태를 저장하는 속성을 정의합니다.
                public int status { get; set; }

                // 위치를 저장하는 List를 정의하며 초기 크기는 8로 설정됩니다.
                public List<float> position = new List<float>(8);

                // 위치 비율을 저장하는 List를 정의하며 초기 크기는 8로 설정됩니다.
                public List<float> positionratio = new List<float>(8);

                // 영역 비율을 저장하는 속성을 정의합니다.
                public float arearatio;

                public List<float> getPos()
                {
                    return position;
                }

                public List<float> getPosratio()
                {
                    return positionratio;
                }
                // JSON 문자열을 Feature 객체로 역직렬화하는 정적 메서드를 정의합니다.
                public static Feature deserialize(string jsonString)
                {
                    Feature feature = JsonConvert.DeserializeObject<Feature>(jsonString);
                    return feature;
                }

                // Feature 객체를 JSON 문자열로 직렬화하는 정적 메서드를 정의합니다.
                public static string serialize(Feature jsonObject)
                {
                    return JsonConvert.SerializeObject(jsonObject);
                }

                // 객체 정보를 문자열로 반환하는 메서드를 정의합니다.
                public string info()
                {
                    string buff = string.Empty;
                    // 이 메서드는 정보를 구성하지 않고 빈 문자열을 반환하고 있습니다.
                    return buff;
                }
            }
        }
    }
}