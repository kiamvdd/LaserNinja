using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    protected bool m_initialized = false;
    protected Vector3 m_currentDirection;

    protected int m_layerMask = 0;

    [SerializeField]
    protected float m_velocity = 15;
    public delegate void ProjectileDestroyed(Projectile projectile);

    public event ProjectileDestroyed OnDestroyed;
    public virtual void Init(Vector3 direction, int layerMask)
    {
        m_layerMask = layerMask;
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

    protected abstract void CheckCollision();

    public virtual Vector3 GetPosition()
    {
        return transform.position;
    }

    private void OnDestroy()
    {
        OnDestroyed(this);
        OnDestroyed = null;
    }
}
