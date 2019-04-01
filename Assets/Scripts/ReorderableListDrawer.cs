using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ReorderableListDrawer
{
    private List<int> m_listOrder = new List<int>();
    private List<int> m_removedItemIndices = new List<int>();
    private int m_currentItemIndex = 0;
    public void StartList(int listLength, GUIStyle style = null)
    {
        m_listOrder = new List<int>(listLength);
        for (int i = 0; i < listLength; i++) {
            m_listOrder.Add(i);
        }

        m_removedItemIndices = new List<int>();

        EditorGUILayout.BeginVertical(style ?? GUI.skin.box);
    }

    public void StartItemDraw(int itemIndex, string name = "", GUIStyle style = null)
    {
        // using unorthodox indentation here to make it easier to see how the GUI is laid out with vertical / horizontal grouping
        // please forgive me for this sin
            m_currentItemIndex = itemIndex;
            EditorGUILayout.BeginVertical(style ?? GUI.skin.box);
                EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.BeginVertical();
                        if (name != "")
                            EditorGUILayout.LabelField(name, EditorStyles.boldLabel);
    }

    public void EndItemDraw()
    {
                        EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal(GUILayout.Width(30));
                        EditorGUILayout.BeginVertical();

                            EditorGUI.BeginDisabledGroup(m_listOrder.Count <= 1 || m_currentItemIndex <= 0);
                                if (GUILayout.Button("▲")) {
                                    SafeSwapIndices(m_currentItemIndex, m_currentItemIndex - 1);
                                }
                            EditorGUI.EndDisabledGroup();

                            EditorGUI.BeginDisabledGroup(m_listOrder.Count <= 1 || m_currentItemIndex >= m_listOrder.Count - 1);
                                if (GUILayout.Button("▼")) {
                                    SafeSwapIndices(m_currentItemIndex, m_currentItemIndex + 1);
                                }
                            EditorGUI.EndDisabledGroup();

                            if (GUILayout.Button("X")) {
                                m_removedItemIndices.Add(m_currentItemIndex);
                            }

                        EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
    }

    public void EndList()
    {
        EditorGUILayout.EndVertical();

        for (int i = m_listOrder.Count - 1; i >= 0; i--) {
            if (m_removedItemIndices.Contains(i)) {
                m_listOrder.RemoveAt(i);
            }
        }
    }

    public List<int> GetRemovedIndices()
    {
        return m_removedItemIndices;
    }

    public List<int> GetFinalListOrder()
    {
        return m_listOrder;
    }

    private void SafeSwapIndices(int index1, int index2)
    {
        if (index1 < 0 || index1 >= m_listOrder.Count || index2 < 0 || index2 >= m_listOrder.Count)
            return;

        int temp = m_listOrder[index1];
        m_listOrder[index1] = m_listOrder[index2];
        m_listOrder[index2] = temp;
    }
}
