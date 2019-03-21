using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventBus
{
    public delegate void EnemyDeathDelegate(PlayerCharacter.ShotInfo trickInfo);
    public static EnemyDeathDelegate OnEnemyKilled;
}
