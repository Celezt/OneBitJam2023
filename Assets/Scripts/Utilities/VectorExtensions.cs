using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public static class VectorExtensions
{
    public static Vector3 _yz(this Vector2 v) => new Vector3(0, v.x, v.y);
    public static Vector3 x_z(this Vector2 v) => new Vector3(v.x, 0, v.y);
    public static Vector3 xy_(this Vector2 v) => new Vector3(v.x, v.y, 0);
    public static Vector3 _yz(this Vector3 v) => new Vector3(0, v.y, v.z);
    public static Vector3 x_z(this Vector3 v) => new Vector3(v.x, 0, v.z);
    public static Vector3 xy_(this Vector3 v) => new Vector3(v.x, v.y, 0);
    public static Vector3 __z(this Vector3 v) => new Vector3(0, 0, v.z);
    public static Vector3 x__(this Vector3 v) => new Vector3(v.x, 0, 0);
    public static Vector3 _y_(this Vector3 v) => new Vector3(0, v.y, 0);
}
