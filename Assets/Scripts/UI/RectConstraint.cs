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
    private bool2 _constraintAxis = new bool2(true, true);

    private RectTransform _rectTransform;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();    
    }

    private void LateUpdate()
    {
        if (!_constraintRect)
            return;

        Vector3 position = _rectTransform.position;
        Vector3 previousPosition = position;
        Vector2 pivot = _rectTransform.pivot;

        Rect worldConstraintRect = _constraintRect.GetWorldRect();
        Rect worldRect = _rectTransform.GetWorldRect();

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

        if (previousPosition != position)
            _rectTransform.position = position;
    }

}
