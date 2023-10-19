// �ʿ��� ���ӽ����̽��� ����մϴ�.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// JSON ����ȭ�� ���� Newtonsoft.Json ���̺귯���� ����մϴ�.
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace Pnc
{
    namespace Model
    {
        namespace FeatureDetection
        {
            // [System.Serializable] ��Ʈ����Ʈ�� ����Ͽ� Ŭ������ ����ȭ �����ϰ� ����ϴ�.
            [System.Serializable]
            public class EventFeatureResult : PncEvent //[Ŭ���� ����]  PncEvent�� Ȯ���ϰ� Ư¡ ���� ����� �����ϴ� �̺�Ʈ ��ü Ŭ����.
            {
                // FeatureResult ��ü�� �����ϴ� �Ӽ��� �����մϴ�.
                public FeatureResult featureResult;

                // FeatureResult ��ü�� ��ȯ�ϴ� �޼��带 �����մϴ�.
                public FeatureResult getFeatureResult()
                {
                    return featureResult;
                }

                // FeatureResult ��ü�� �����ϴ� �޼��带 �����մϴ�.
                public void setFeatureResult(FeatureResult featureresult)
                {
                    this.featureResult = featureresult;
                }

                // ��ü ������ ���ڿ��� ��ȯ�ϴ� �޼��带 �����մϴ�.
                public string info()
                {
                    string buff = string.Empty;

                    // ��ü ������ ���ڿ��� �����մϴ�.
                    buff = $"EventFeatureResult=[owner={getOwner()}, event type={getEventType()}" +
                    $", timestamp={getTimestamp()}, featureresult={featureResult.info()}] ";

                    return buff;
                }
            }
        }
    }
}
