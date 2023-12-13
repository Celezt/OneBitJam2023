using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class QuaternionExtensions
{
    /// <summary>
    /// Clamp quaternion in degrees.
    /// </summary>
    /// <param name="bounds">Degrees (x, y, z).</param>
    /// <returns></returns>
    /// <see cref="https://forum.unity.com/threads/how-do-i-clamp-a-quaternion.370041/#post-6531533"/>
    public static Quaternion Clamp(this Quaternion quaternion, Vector3 bounds)
    {
        quaternion.x /= quaternion.w;
        quaternion.y /= quaternion.w;
        quaternion.z /= quaternion.w;
        quaternion.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(quaternion.x);
        angleX = Mathf.Clamp(angleX, -bounds.x, bounds.x);
        quaternion.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        float angleY = 2.0f * Mathf.Rad2Deg * Mathf.Atan(quaternion.y);
        angleY = Mathf.Clamp(angleY, -bounds.y, bounds.y);
        quaternion.y = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleY);

        float angleZ = 2.0f * Mathf.Rad2Deg * Mathf.Atan(quaternion.z);
        angleZ = Mathf.Clamp(angleZ, -bounds.z, bounds.z);
        quaternion.z = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleZ);

        return quaternion.normalized;
    }

}
