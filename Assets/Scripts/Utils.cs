using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Helpers
{
    public static class Util
    {
        public static float VectorToAngle2D(Vector2 vector)
        {
            return Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
        }
    }

    public static class VectorExtensions
    {
        #region Vector2
        public static Vector2 ChangeX(this Vector2 v, float x)
        {
            return new Vector2(x, v.y);
        }

        public static Vector2 ChangeY(this Vector2 v, float y)
        {
            return new Vector2(v.x, y);
        }
        #endregion
        #region Vector3
        public static Vector3 ChangeX(this Vector3 v, float x)
        {
            return new Vector3(x, v.y, v.z);
        }

        public static Vector3 ChangeY(this Vector3 v, float y)
        {
            return new Vector3(v.x, y, v.z);
        }

        public static Vector3 ChangeZ(this Vector3 v, float z)
        {
            return new Vector3(v.x, v.y, z);
        }
        #endregion
    }

#if UNITY_EDITOR
    namespace EditorTools
    {
        public class ReorderableListItem
        {
            public delegate void GUICallback();
            public event GUICallback OnListDown;
            public event GUICallback OnListUp;
            public event GUICallback OnRemove;

            public void StartItemDraw()
            {
                EditorGUILayout.BeginHorizontal(GUI.skin.box);
            }

            public void StopItemDraw()
            {
                EditorGUILayout.BeginHorizontal(GUILayout.Width(30));
                if (GUILayout.Button("X", GUILayout.Width(25)))
                    OnRemove();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndHorizontal();
            }
        }
    }
#endif
}