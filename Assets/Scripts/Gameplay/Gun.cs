using System;
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
    [SerializeField]
    private int m_shotCount = 3;
    public int ShotCount { get { return m_shotCount; } }

    public Action<int> OnShotcountChanged;

    [SerializeField]
    private LineRenderer m_aimingGuideLineRenderer;

    private bool m_aiming = false;

    private Projectile m_aimingGuideDummy = null;
    private List<Projectile> m_activeProjectiles = new List<Projectile>();

    private void Awake()
    {
        m_aimingGuideLineRenderer.enabled = false;
    }

    public void Fire(Vector3 direction)
    {
        if (!m_aiming || m_shotCount == 0)
            return;

        EventBus.OnTrickEvent(new TrickEventData(TrickEventData.TrickEventType.SHOOT));

        Projectile projectile = Instantiate(m_projectilePrefab);
        projectile.transform.position = m_barrel.position;
        projectile.Init(direction, ~(1 << m_owner.gameObject.layer));
        projectile.OnDestroyed += OnProjectileDestroyed;
        m_activeProjectiles.Add(projectile);

        m_animator.SetTrigger("Shoot");
        --m_shotCount;
        OnShotcountChanged.Invoke(-1);

        GameObject cameraControllerObj = GameObject.FindGameObjectWithTag("CameraController");
        if (cameraControllerObj != null) {
            CameraController controller = cameraControllerObj.GetComponent<CameraController>();
            if (controller != null)
                controller.StartShake(0.4f, 5, 1, 0);
        }
    }

    public void SetAimingGuideEnabled(bool enabled)
    {
        m_aiming = enabled;
        m_aimingGuideLineRenderer.enabled = enabled;
    }

    public void UpdateAimingDirection(Vector3 direction)
    {
        if (!m_aiming)
            return;

        if (m_aimingGuideDummy == null)
            m_aimingGuideDummy = m_projectilePrefab.GetComponent<Projectile>();

        Vector3[] positions = m_aimingGuideDummy.GetAimingTrajectory(m_barrel.position, direction);

        m_aimingGuideLineRenderer.positionCount = positions.Length; 
        m_aimingGuideLineRenderer.SetPositions(m_aimingGuideDummy.GetAimingTrajectory(m_barrel.position, direction));
    }

    public void DrawTrajectory()
    {
        BounceTrajectory trajectory = new BounceTrajectory(2, clampTrajectory: true);
    }

    private void OnProjectileDestroyed(Projectile projectile)
    {
        ++m_shotCount;
        OnShotcountChanged.Invoke(1);
        m_activeProjectiles.Remove(projectile);
    }

    private void OnDestroy()
    {
        foreach (Projectile projectile in m_activeProjectiles)
        {
            projectile.OnDestroyed -= OnProjectileDestroyed;
        }
    }
}
