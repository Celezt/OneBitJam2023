using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnimationCurveBuilder
{
    public static AnimationCurve EaseInOut(float time1, float value1, float time2, float value2, float time3, float value3)
    {
        Keyframe[] array = new Keyframe[3]
        {
                new Keyframe(time1, value1, 0f, 0f),
                new Keyframe(time2, value2, 0f, 0f),
                new Keyframe(time3, value3, 0f, 0f)
        };
        return new AnimationCurve(array);
    }
}
