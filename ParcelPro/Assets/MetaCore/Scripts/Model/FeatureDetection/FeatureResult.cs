// �ʿ��� ���ӽ����̽��� ����մϴ�.
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
            // [System.Serializable] ��Ʈ����Ʈ�� ����Ͽ� Ŭ������ ����ȭ �����ϰ� ����ϴ�.
            [System.Serializable]
            public class FeatureResult //[Ŭ��������] Ư¡ ���� ����� �׿� ���� ������ �����ϰ� ����ȭ/������ȭ�ϴ� Ŭ����.
            {
                // Feature ��ü�� List�� �����ϴ� �Ӽ��� �����մϴ�.
                public List<Feature> features;

                // JSON ���ڿ��� FeatureResult ��ü�� ������ȭ�ϴ� ���� �޼��带 �����մϴ�.
                public static FeatureResult deserialize(string jsonString)
                {
                    FeatureResult obj = JsonConvert.DeserializeObject<FeatureResult>(jsonString);
                    return obj;
                }

                // FeatureResult ��ü�� JSON ���ڿ��� ����ȭ�ϴ� ���� �޼��带 �����մϴ�.
                public static string serialize(FeatureResult jsonObject)
                {
                    return JsonConvert.SerializeObject(jsonObject);
                }

                // ��ü ������ ���ڿ��� ��ȯ�ϴ� �޼��带 �����մϴ�.
                public string info()
                {
                    string buff = string.Empty;

                    // Feature ��ü�� ������ ���ڿ��� �����մϴ�.
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
