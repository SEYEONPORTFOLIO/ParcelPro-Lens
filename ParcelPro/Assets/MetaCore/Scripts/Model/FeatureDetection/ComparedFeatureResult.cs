// FeatureDetection ���ӽ����̽��� ����մϴ�.
using Pnc.Model.FeatureDetection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ComparedFeatureResult Ŭ������ ����(static) Ŭ������ ���ǵ˴ϴ�.
public static class ComparedFeatureResult //[Ŭ��������] Ư¡ ���� ����� �����ϰ� ��ȯ�ϴ� ���� ��ƿ��Ƽ Ŭ����.
{
    // FeatureResult ��ü�� �����ϱ� ���� private(static) ������ �����մϴ�.
    private static FeatureResult featureResult;

    // FeatureResult ��ü�� ��ȯ�ϴ� GetFeatureResult �޼��带 �����մϴ�.
    public static FeatureResult GetFeatureResult()
    {
        return featureResult;
    }

    // FeatureResult ��ü�� �����ϴ� SetFeatureResult �޼��带 �����մϴ�.
    public static void SetFeatureResult(FeatureResult featureResult_)
    {
        featureResult = featureResult_;
    }
}
