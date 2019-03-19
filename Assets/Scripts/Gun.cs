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

    [SerializeField]
    private LineRenderer m_aimingGuideLineRenderer;

    private bool m_aiming = false;

    private Projectile m_aimingGuideDummy = null;

    private void Awake()
    {
        m_aimingGuideLineRenderer.enabled = false;
    }

    public void Fire(Vector3 direction, float timeBonus)
    {
        if (!m_aiming)
            return;

        Projectile projectile = Instantiate(m_projectilePrefab);
        projectile.transform.position = m_barrel.position;
        projectile.Init(direction, 0, ~(1 << m_owner.gameObject.layer));
        projectile.OnDestroyed += OnProjectileDestroyed;

        m_animator.SetTrigger("Shoot");

        GameObject cameraControllerObj = GameObject.FindGameObjectWithTag("CameraController");
        if (cameraControllerObj != null) {
            CameraController controller = cameraControllerObj.GetComponent<CameraController>();
            if (controller != null)
                controller.StartShake(0.4f, 5, 1, 0);
        }

        m_canShoot = false;
    }

    public void SetAimingGuideEnabled(bool enabled)
    {
        m_aiming = enabled;
        m_aimingGuideLineRenderer.enabled = enabled;
    }

    public void ShowAimingGuide(Vector3 direction)
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
    }
}
