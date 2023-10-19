// FeatureDetection 네임스페이스를 사용합니다.
using Pnc.Model.FeatureDetection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ComparedFeatureResult 클래스는 정적(static) 클래스로 정의됩니다.
public static class ComparedFeatureResult //[클래스설명] 특징 검출 결과를 저장하고 반환하는 정적 유틸리티 클래스.
{
    // FeatureResult 객체를 저장하기 위한 private(static) 변수를 선언합니다.
    private static FeatureResult featureResult;

    // FeatureResult 객체를 반환하는 GetFeatureResult 메서드를 정의합니다.
    public static FeatureResult GetFeatureResult()
    {
        return featureResult;
    }

    // FeatureResult 객체를 설정하는 SetFeatureResult 메서드를 정의합니다.
    public static void SetFeatureResult(FeatureResult featureResult_)
    {
        featureResult = featureResult_;
    }
}
