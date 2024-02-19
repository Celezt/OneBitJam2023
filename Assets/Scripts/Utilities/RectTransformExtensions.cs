using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RectTransformExtensions
{
    private static Camera _camera;

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

    /// <see cref="https://forum.unity.com/threads/how-to-get-a-rect-in-screen-space-from-a-recttransform.1490806/#post-9333725"/>
    public static Rect GetScreenRect(this RectTransform rectTransform)
    {
        Span<Vector3> corners = stackalloc Vector3[4];
        GetWorldCorners(rectTransform, corners);

        if (!_camera)
            _camera = Camera.current;

        if (!_camera)
            return new Rect();

        // Convert world space to screen space in pixel values and round to integers.
        for (int i = 0; i < corners.Length; i++)
        {
            corners[i] = _camera.WorldToScreenPoint(corners[i]);
            corners[i] = new Vector3(Mathf.RoundToInt(corners[i].x), Mathf.RoundToInt(corners[i].y), corners[i].z);
        }

        // Calculate the screen space rectangle
        float x = Mathf.Min(corners[0].x, corners[1].x, corners[2].x, corners[3].x);
        float y = Mathf.Min(corners[0].y, corners[1].y, corners[2].y, corners[3].y);
        float width = Mathf.Max(corners[0].x, corners[1].x, corners[2].x, corners[3].x) - x;
        float height = Mathf.Max(corners[0].y, corners[1].y, corners[2].y, corners[3].y) - y;

        return new Rect(x, y, width, height);
    }
}
