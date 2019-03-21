using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TrickList : MonoBehaviour
{
    [SerializeField]
    private TrickText m_textPrefab;

    private void Awake()
    {
        EventBus.OnEnemyKilled += OnEnemyKilled;
    }

    private void OnEnemyKilled(PlayerCharacter.ShotInfo shotInfo)
    {
        AddTrickText("Kill", shotInfo.baseDamage * 0.5f);

        if (shotInfo.playerJumping)
            AddTrickText("Aerial", 1);

        if (shotInfo.playerMoving)
            AddTrickText("Running", 1);
    }

    private void AddTrickText(string trickname, float time)
    {
        TrickText text;
        text = Instantiate(m_textPrefab);
        text.transform.parent = transform;
        TimeSpan ts = TimeSpan.FromSeconds(time);
        ts.ToString(@"ss\:ff");
        text.Init(trickname + " +" + ts.ToString(@"ss\:ff"));
    }

    private void OnDestroy()
    {
        EventBus.OnEnemyKilled -= OnEnemyKilled;
    }
}
