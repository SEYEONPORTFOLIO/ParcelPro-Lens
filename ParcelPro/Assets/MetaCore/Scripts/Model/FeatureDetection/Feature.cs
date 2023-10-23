// �ʿ��� ���ӽ����̽��� ����մϴ�.
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
            // [System.Serializable] ��Ʈ����Ʈ�� ����Ͽ� Ŭ������ ����ȭ �����ϰ� ����ϴ�.
            [System.Serializable]
            public class Feature //[Ŭ���� ����]Ư¡�� ���� �̸�, ����, ��ġ, ���� ������ �����ϰ� ����ȭ/������ȭ�ϴ� Ŭ����.
            {
                // ���� �̸��� �����ϴ� �Ӽ��� �����մϴ�.
                public string filename { get; set; }

                // ���¸� �����ϴ� �Ӽ��� �����մϴ�.
                public int status { get; set; }

                // ��ġ�� �����ϴ� List�� �����ϸ� �ʱ� ũ��� 8�� �����˴ϴ�.
                public List<float> position = new List<float>(8);

                // ��ġ ������ �����ϴ� List�� �����ϸ� �ʱ� ũ��� 8�� �����˴ϴ�.
                public List<float> positionratio = new List<float>(8);

                // ���� ������ �����ϴ� �Ӽ��� �����մϴ�.
                public float arearatio;

                public List<float> getPos()
                {
                    return position;
                }

                public List<float> getPosratio()
                {
                    return positionratio;
                }
                // JSON ���ڿ��� Feature ��ü�� ������ȭ�ϴ� ���� �޼��带 �����մϴ�.
                public static Feature deserialize(string jsonString)
                {
                    Feature feature = JsonConvert.DeserializeObject<Feature>(jsonString);
                    return feature;
                }

                // Feature ��ü�� JSON ���ڿ��� ����ȭ�ϴ� ���� �޼��带 �����մϴ�.
                public static string serialize(Feature jsonObject)
                {
                    return JsonConvert.SerializeObject(jsonObject);
                }

                // ��ü ������ ���ڿ��� ��ȯ�ϴ� �޼��带 �����մϴ�.
                public string info()
                {
                    string buff = string.Empty;
                    // �� �޼���� ������ �������� �ʰ� �� ���ڿ��� ��ȯ�ϰ� �ֽ��ϴ�.
                    return buff;
                }
            }
        }
    }
}