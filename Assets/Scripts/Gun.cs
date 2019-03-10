using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField]
    private Projectile m_projectilePrefab;

    [SerializeField]
    private Transform m_owner;

    [SerializeField]
    private Transform m_barrel;

    [SerializeField]
    private Animator m_animator;
    private bool m_canShoot = true;

    public void Fire(Vector3 direction)
    {
        if (!m_canShoot)
            return;

        Projectile projectile = Instantiate(m_projectilePrefab);
        projectile.transform.position = m_barrel.position;
        projectile.Init(direction, OnProjectileDestroyed);
        m_animator.SetTrigger("Shoot");
        m_canShoot = false;

        GameObject ccgo = GameObject.FindGameObjectWithTag("CameraController");
        CameraController cc = null;

        if (ccgo != null)
            cc = ccgo.GetComponent<CameraController>();

        if (cc != null)
            cc.SetTarget(projectile.transform);
    }

    private void OnProjectileDestroyed(Projectile projectile)
    {
        m_owner.position = projectile.GetPosition();
        m_canShoot = true;

        GameObject ccgo = GameObject.FindGameObjectWithTag("CameraController");
        CameraController cc = null;

        if (ccgo != null)
            cc = ccgo.GetComponent<CameraController>();

        if (cc != null)
            cc.SetTarget(m_owner);
    }
}
