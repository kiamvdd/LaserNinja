using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TrickList : MonoBehaviour
{
    [SerializeField]
    private TrickText m_textPrefab;

    public void AddTrick(Trick trick)
    {
        AddTrickText(trick.Name, trick.TimeBonus);
    }

    private void AddTrickText(string trickname, float time)
    {
        TrickText text;
        text = Instantiate(m_textPrefab, transform);
        text.transform.localScale = Vector3.one;
        TimeSpan ts = TimeSpan.FromSeconds(time);
        ts.ToString(@"ss\:ff");
        text.Init(trickname + " +" + ts.ToString(@"ss\:ff"));
    }
}
