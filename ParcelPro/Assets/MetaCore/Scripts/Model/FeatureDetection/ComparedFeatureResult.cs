using Pnc.Model.FeatureDetection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ComparedFeatureResult
{
    private static FeatureResult featureResult;

    public static FeatureResult GetFeatureResult()
    {
        return featureResult;
    }

    public static void SetFeatureResult(FeatureResult featureResult_)
    {
        featureResult = featureResult_;
    }
}
