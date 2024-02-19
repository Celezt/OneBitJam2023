using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[ExecuteAlways, HideMonoScript]
public class RectConstraint : MonoBehaviour
{
    [SerializeField]
    private RectTransform _constraintRect;
    [SerializeField]
    private Vector2 _offset;
    [SerializeField]
    private bool2 _constraintAxis = new bool2(true, true);

    private Vector2 _previousOffset;
    private Rect _previousWorldConstraintRect;
    private Rect _previousWorldRect;
    private RectTransform _rectTransform;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();    
    }

    private void LateUpdate()
    {
        if (!_constraintRect)
            return;

        Rect worldRect = _rectTransform.GetWorldRect();
        Rect worldConstraintRect = _constraintRect.GetWorldRect();

#if UNITY_EDITOR
        bool isDragging = ToolUtility.IsFocusDraggingScene(_rectTransform);
#endif
        // No changes has been made.
        if (worldRect == _previousWorldRect &&
            worldConstraintRect == _previousWorldConstraintRect
#if UNITY_EDITOR
            && !isDragging
#endif
            )
            return;

        Vector3 position = _rectTransform.position;
        Vector3 previousPosition = position;
        Vector2 pivot = _rectTransform.pivot;
        Vector2 anchorMin = _rectTransform.anchorMin;
        Vector2 anchorMax = _rectTransform.anchorMax;

        Vector2 anchor = new Vector2(
            worldConstraintRect.x + worldConstraintRect.width * (anchorMin.x + anchorMax.x) / 2,
            worldConstraintRect.y + worldConstraintRect.height * (anchorMin.y + anchorMax.y) / 2);

        if (worldConstraintRect.x != _previousWorldConstraintRect.x ||
            worldConstraintRect.width != _previousWorldConstraintRect.width ||
            _offset.x != _previousOffset.x)
        {
            position.x = anchor.x + _offset.x;
        }

        if (worldConstraintRect.y != _previousWorldConstraintRect.y ||
            worldConstraintRect.height != _previousWorldConstraintRect.height ||
            _offset.y != _previousOffset.y)
        {
            position.y = anchor.y + _offset.y;
        }

        if (_constraintAxis.x)
        {
            if (worldRect.x < worldConstraintRect.x)
                position = new Vector3(worldConstraintRect.x, position.y, position.z);
            else if (worldRect.xMax > worldConstraintRect.xMax)
                position = new Vector3(worldConstraintRect.xMax - worldRect.width, position.y, position.z);

            if (previousPosition.x != position.x)
                position.x += pivot.x * worldRect.width;
        }

        if (_constraintAxis.y)
        {
            if (worldRect.y < worldConstraintRect.y)
                position = new Vector3(position.x, worldConstraintRect.y, position.z);
            else if (worldRect.yMax > worldConstraintRect.yMax)
                position = new Vector3(position.x, worldConstraintRect.yMax - worldRect.height, position.z);

            if (previousPosition.y != position.y)
                position.y += pivot.y * worldRect.height;
        }
#if UNITY_EDITOR
        if (isDragging)
        {

        }
#endif

        if (previousPosition != position)
            _rectTransform.position = position;

        _previousWorldRect = worldRect;
        _previousWorldConstraintRect = worldConstraintRect;
        _previousOffset = _offset;
    }
}
