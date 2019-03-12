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
        projectile.Init(direction,  ~(1 << m_owner.gameObject.layer));
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

    public void DrawTrajectory()
    {
        BounceTrajectory trajectory = new BounceTrajectory(2, clampTrajectory: true);
    }

    private void OnProjectileDestroyed(Projectile projectile)
    {
        m_canShoot = true;
    }
}
