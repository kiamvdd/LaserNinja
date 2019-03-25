using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    float TakeDamage(float amount);
    void Destroy();
    bool IsAlive();
}
