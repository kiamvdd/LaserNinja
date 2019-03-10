using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    protected bool m_initialized = false;
    protected Vector3 m_currentDirection;

    [SerializeField]
    protected float m_velocity = 15;
    public delegate void ProjectileDestroyed(Projectile projectile);

    private ProjectileDestroyed m_onDestroy;
    public virtual void Init(Vector3 direction, ProjectileDestroyed onDestroy)
    {
        m_onDestroy += onDestroy;
        m_currentDirection = direction;
        m_initialized = true;
    }

    protected virtual void FixedUpdate()
    {
        if (!m_initialized)
            return;

        float rot_z = Utils.VectorToAngle2D(m_currentDirection);
        transform.rotation = Quaternion.Euler(0f, 0f, rot_z);
    }

    public virtual Vector3 GetPosition()
    {
        return transform.position;
    }

    private void OnDestroy()
    {
        m_onDestroy(this);
        m_onDestroy = null;
    }
}
