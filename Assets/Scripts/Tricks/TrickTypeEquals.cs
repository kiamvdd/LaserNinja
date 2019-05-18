using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TrickTypeEquals : TrickCondition
{
    [Tooltip("The event(s) that will result in a SUCCESS.")]
    [EnumFlag]
    public TrickEventData.TrickEventType SuccessEventType;

    [Tooltip("The event(s) that will result in a FAIL.")]
    [EnumFlag]
    public TrickEventData.TrickEventType EventBlacklist;

    [Tooltip("If true, Event Blacklist will act as a whitelist instead.")]
    public bool InvertBlackList;

    public override ConditionState TestCondition(TrickEventData eventData)
    {
        int blacklistComparison = ((int)EventBlacklist & (int)eventData.Type);

        if (InvertBlackList) {
            if (blacklistComparison == 0)
                return ConditionState.FAIL;
        } else {
            if (blacklistComparison > 0)
                return ConditionState.FAIL;
        }

        if (((int)SuccessEventType & (int)eventData.Type) > 0)
            return ConditionState.SUCCESS;

        return ConditionState.RUNNING;
    }
#if UNITY_EDITOR && !FAKE_BUILD
    private string blacklistLabel = "Event Blacklist";
    private string whitelistLabel = "Event Whitelist";
    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginVertical();
        SuccessEventType = (TrickEventData.TrickEventType)EditorGUILayout.EnumFlagsField("Success Event", SuccessEventType);
        EventBlacklist = (TrickEventData.TrickEventType)EditorGUILayout.EnumFlagsField(InvertBlackList ? whitelistLabel : blacklistLabel, EventBlacklist);
        InvertBlackList = EditorGUILayout.Toggle("Use Whitelist", InvertBlackList);
        EditorGUILayout.EndVertical();
    }

    public override string GetInspectorHeaderName()
    {
        return "Tricktype Equals";
    }
#endif
}
