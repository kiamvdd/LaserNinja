using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserProjectile : Projectile
{
    private float m_timer = 0;
    private BounceTrajectory m_trajectory;

    [SerializeField]
    private float m_tailLength = 0.5f;

    [SerializeField]
    private LineRenderer m_lineRenderer;

    public override void Init(Vector3 direction, ProjectileDestroyed onDestroy)
    {
        m_trajectory = new BounceTrajectory(10, clampTrajectory: true, endOnFloor:true);
        m_trajectory.CalculateTrajectory(transform.position, direction);
        m_lineRenderer.positionCount = 2;
        Vector3 pos = m_trajectory.GetCurrentPosition();
        m_lineRenderer.SetPosition(0, pos);
        m_lineRenderer.SetPosition(1, pos);
        base.Init(direction, onDestroy);
    }

    protected override void FixedUpdate()
    {
        if (!m_initialized)
            return;

        base.FixedUpdate();

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

        m_timer += Time.fixedDeltaTime;

        if (m_trajectory.GetProgressFromDistance(tailDistance) >= 1)
            Destroy(gameObject);
    }
}
