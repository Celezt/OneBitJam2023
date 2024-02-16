using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RectTransformExtensions
{
    public static void GetLocalCorners(this RectTransform rectTransform, Span<Vector3> fourCornersSpan)
    {
        if (fourCornersSpan.Length < 4)
        {
            Debug.LogError("Calling GetLocalCorners with a span that is less than 4 elements.");
            return;
        }

        Rect rect = rectTransform.rect;
        float x = rect.x;
        float y = rect.y;
        float xMax = rect.xMax;
        float yMax = rect.yMax;
        fourCornersSpan[0] = new Vector3(x, y, 0f);
        fourCornersSpan[1] = new Vector3(x, yMax, 0f);
        fourCornersSpan[2] = new Vector3(xMax, yMax, 0f);
        fourCornersSpan[3] = new Vector3(xMax, y, 0f);
    }

    public static void GetWorldCorners(this RectTransform rectTransform, Span<Vector3> fourCornersSpan)
    {
        if (fourCornersSpan.Length < 4)
        {
            Debug.LogError("Calling GetLocalCorners with a span that is less than 4 elements.");
            return;
        }

        GetLocalCorners(rectTransform, fourCornersSpan);
        Matrix4x4 matrix4x = rectTransform.localToWorldMatrix;
        for (int i = 0; i < 4; i++)
        {
            fourCornersSpan[i] = matrix4x.MultiplyPoint(fourCornersSpan[i]);
        }
    }

    public static Rect GetWorldRect(this RectTransform rectTransform)
    {
        Span<Vector3> corners = stackalloc Vector3[4];
        GetWorldCorners(rectTransform, corners);
        // Get the bottom left corner.
        Vector3 position = corners[0];

        Vector2 size = new Vector2(
            rectTransform.lossyScale.x * rectTransform.rect.size.x,
            rectTransform.lossyScale.y * rectTransform.rect.size.y);

        return new Rect(position, size);
    }
}
