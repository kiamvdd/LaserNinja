using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static float VectorToAngle2D(Vector2 vector)
    {
        return Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
    }
}
