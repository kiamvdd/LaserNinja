using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Helpers.EditorTools;

[CustomEditor(typeof(Trick))]
public class TrickEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Trick trick = (Trick)target;

        trick.Name = EditorGUILayout.TextField("Name", trick.Name);
        trick.TimeBonus = EditorGUILayout.FloatField("Bonus time", trick.TimeBonus);

        EditorGUILayout.BeginVertical();
        if (GUILayout.Button("New equality conditional")) {
            trick.TrickConditions.Add(new TrickTypeEquals());
            EditorUtility.SetDirty(target);
        }//
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.BeginVertical();
        for (int i = 0; i < trick.TrickConditions.Count; i++) {
            ReorderableListItem listItem = new ReorderableListItem();
            listItem.OnRemove += trick.TrickConditions[i].TagForRemoval;

            listItem.StartItemDraw();
            trick.TrickConditions[i].DrawConditionalGUI();
            listItem.StopItemDraw();
        }
        EditorGUILayout.EndVertical();

        //RemoveTaggedItems(trick.TrickConditions);
    }

    //private void RemoveTaggedItems(List<TrickConditional> items)
    //{
    //    for (int i = items.Count - 1; i >= 0; i--) {
    //        if (items[i].RemoveFromGUI) {
    //            items.RemoveAt(i);
    //            EditorUtility.SetDirty(target);
    //        }
    //    }
    //}
}