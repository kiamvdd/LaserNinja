using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserProjectile : Projectile
{
    private BounceTrajectory m_trajectory;

    [SerializeField]
    private float m_tailLength = 0.5f;

    [SerializeField]
    private LineRenderer m_lineRenderer;

    private bool m_markedForDestroy = false;

    public override void Init(Vector3 direction, int layerMask)
    {
        m_trajectory = new BounceTrajectory(10, clampTrajectory: true);
        m_trajectory.CalculateTrajectory(transform.position, direction);

        GameObject cameraControllerObj = GameObject.FindGameObjectWithTag("CameraController");
        if (cameraControllerObj != null) {
            CameraController controller = cameraControllerObj.GetComponent<CameraController>();
            if (controller != null)
                m_trajectory.OnBounce += () => { controller.StartShake(0.2f, 5, 0.5f, 0); };
        }

        m_lineRenderer.positionCount = 2;
        Vector3 pos = m_trajectory.GetCurrentPosition();
        m_lineRenderer.SetPosition(0, pos);
        m_lineRenderer.SetPosition(1, pos);
        base.Init(direction, layerMask);
    }

    protected override void FixedUpdate()
    {
        if (!m_initialized)
            return;

        base.FixedUpdate();

        MoveAlongTrajectory();

        if (!m_markedForDestroy)
            CheckCollision();
    }

    protected override void CheckCollision()
    {
        Vector3 rayDirection = m_currentDirection * m_velocity * Time.deltaTime;
        RaycastHit2D hit = Physics2D.Raycast(transform.position - rayDirection, m_currentDirection, rayDirection.magnitude, m_layerMask);

        if (hit.collider != null) {
            IDamageable damageable = hit.collider.gameObject.GetComponent<IDamageable>();
            if (damageable != null) {
                damageable.TakeDamage(CalculateDamage());

                GameObject cameraControllerObj = GameObject.FindGameObjectWithTag("CameraController");
                if (cameraControllerObj != null) {
                    CameraController controller = cameraControllerObj.GetComponent<CameraController>();
                    if (controller != null)
                        controller.StartShake(0.4f, 5, 1, 0);
                }

                MarkForDestroy();
            }
        }
    }

    private float CalculateDamage()
    {
        return m_trajectory.GetCornersBetween(-1, m_trajectory.GetCurrentDistance(), includeEndPoints: false).Count + 1;
    }

    private void MoveAlongTrajectory()
    {
        float trajectoryProgress = m_trajectory.Continue(m_velocity * Time.deltaTime);
        transform.position = m_trajectory.GetCurrentPosition();

        float tailDistance = Mathf.Clamp(m_trajectory.GetCurrentDistance() - m_tailLength, 0, Mathf.Infinity);
        m_trajectory.GetTrajectoryAtDistance(tailDistance, out Vector3 tailPos, out Vector3 dir);

        List<Vector3> corners = m_trajectory.GetCornersBetween(tailDistance, m_trajectory.GetCurrentDistance());
        m_lineRenderer.positionCount = corners.Count + 2;

        // Set the line position to head, tail, and any corners inbetween
        m_lineRenderer.SetPosition(0, transform.position);

        for (int i = 0; i < corners.Count; i++) {
            m_lineRenderer.SetPosition(1 + i, corners[corners.Count - i - 1]);
        }

        m_lineRenderer.SetPosition(corners.Count + 1, tailPos);

        m_currentDirection = m_trajectory.GetCurrentDirection();

        if (m_trajectory.GetProgressFromDistance(tailDistance) >= 1) {
            MarkForDestroy();
        }
    }

    private void MarkForDestroy()
    {
        m_markedForDestroy = true;
        Destroy(gameObject);
    }

    public override Vector3[] GetAimingTrajectory(Vector3 startPosition, Vector3 direction)
    {
        if (m_aimGuideTrajectory == null)
            m_aimGuideTrajectory = new BounceTrajectory(1, true);

        BounceTrajectory t = m_aimGuideTrajectory as BounceTrajectory;
        t.CalculateTrajectory(startPosition, direction);
        List<Vector3> corners = t.GetCornersBetween(-1, t.GetDistanceFromProgress(1) + 1);

        return corners.ToArray();
    }
}
