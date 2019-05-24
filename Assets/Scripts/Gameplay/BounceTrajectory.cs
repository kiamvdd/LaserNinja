using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceTrajectory : Trajectory
{
    private List<Vector3> m_trajectoryPositions = new List<Vector3>();
    private List<float> m_distances = new List<float>();
    private float m_totalDistance = 0;

    private int m_currentCornerCount = 0;

    private Vector3 m_currentPosition;
    private Vector3 m_currentDirection;

    private bool m_endOnFloor;
    private bool m_clampTrajectory;

    private int m_numBounces;
    private float m_distanceTraversed = 0;

    public Action OnBounce;

    public BounceTrajectory(int numBounces, bool clampTrajectory = false, bool endOnFloor = false)
    {
        m_clampTrajectory = clampTrajectory;
        m_endOnFloor = endOnFloor;
        m_numBounces = numBounces;
    }

    /// <summary>
    /// Calculates the trajectory given a starting position and velocity, using the number of bounces set in the constructor.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="velocity"></param>
    public override void CalculateTrajectory(Vector3 position, Vector3 velocity)
    {
        int currentBounce = 0;
        m_distanceTraversed = 0;
        m_totalDistance = 0;
        m_trajectoryPositions.Clear();
        m_trajectoryPositions.Add(position);
        m_distances.Clear();
        m_currentCornerCount = 0;

        Vector3 currentPosition = position;
        m_currentPosition = currentPosition;
        Vector3 currentDirection = velocity.normalized;
        m_currentDirection = currentDirection;

        bool continueTrajectory = true;
        while (currentBounce <= m_numBounces && continueTrajectory) {
            RaycastHit2D hit = Physics2D.Raycast(currentPosition + currentDirection * 0.01f, currentDirection, Mathf.Infinity, (1 << LayerMask.NameToLayer("Floor")));

            if (hit.collider != null) {
                if (m_endOnFloor && Vector2.Dot(hit.normal, Vector2.up) > 0.8f)
                    continueTrajectory = false;
                    
                currentPosition = hit.point;
                currentDirection = Vector2.Reflect(currentDirection, hit.normal);
            } else {
                currentPosition = currentPosition + currentDirection * 10;
                continueTrajectory = false;
            }

            m_trajectoryPositions.Add(currentPosition);

            float distance = Vector3.Distance(currentPosition, m_trajectoryPositions[m_trajectoryPositions.Count - 2]);
            m_totalDistance += distance;
            m_distances.Add(distance);
            currentBounce++;
        }
    }

    /// <summary>
    /// Increases the current distance traversed along the path, and updates trajectory position and direction accordingly.
    /// Returns the progress along the path, ranging from 0 to 1.
    /// </summary>
    /// <param name="velocity"></param>
    /// <returns></returns>
    public override float Continue(float velocity)
    {
        m_distanceTraversed += velocity;

        int cornerCount = GetCornersBetween(-1, m_distanceTraversed).Count;
        if (cornerCount > m_currentCornerCount) {
            m_currentCornerCount++;
            OnBounce();
        }

        GetTrajectoryAtDistance(m_distanceTraversed, out Vector3 position, out Vector3 direction);

        m_currentPosition = position;
        m_currentDirection = direction;

        return m_distanceTraversed / m_totalDistance;
    }

    public override Vector3 GetCurrentDirection()
    {
        return m_currentDirection;
    }

    public override Vector3 GetCurrentPosition()
    {
        return m_currentPosition;
    }

    public override float GetCurrentDistance()
    {
        return m_distanceTraversed;
    }

    /// <summary>
    /// Provides the position and direction of the trajectory at a given distance. 
    /// Distance value can exceed the limits of the trajectory, in which case the values will be extrapolated from the last section of the trajectory.
    /// </summary>
    /// <param name="distance"></param>
    /// <param name="position"></param>
    /// <param name="direction"></param>
    public override void GetTrajectoryAtDistance(float distance, out Vector3 position, out Vector3 direction)
    {
        float tempDist = m_clampTrajectory ? Mathf.Clamp(distance, 0, m_totalDistance) : distance;
        int leftIndex = 0;
        float interval = m_distances[0];

        for (int i = 0; i < m_distances.Count - 1; i++) {
            if (tempDist - m_distances[i] > 0) {
                tempDist -= m_distances[i];
                interval = m_distances[i + 1];
                leftIndex++;
            } else {
                break;
            }
        }

        float lerpVal = tempDist / interval;
        position = Vector3.LerpUnclamped(m_trajectoryPositions[leftIndex], m_trajectoryPositions[leftIndex + 1], lerpVal);
        direction = (m_trajectoryPositions[leftIndex + 1] - m_trajectoryPositions[leftIndex]).normalized;
    }

    /// <summary>
    /// Returns any corner points between two distances along the trajectory. Returns an empty list if no corner points are located between the two values.
    /// </summary>
    /// <param name="d1"></param>
    /// <param name="d2"></param>
    /// <returns></returns>
    public List<Vector3> GetCornersBetween(float d1, float d2, bool includeEndPoints = true)
    {
        List<Vector3> m_corners = new List<Vector3>();
        if ((d1 < 0 && d2 < 0) || (d1 > m_totalDistance && d2 > m_totalDistance) || d1 == d2)
            return m_corners;

        float leftVal = d1 < d2 ? d1 : d2;
        float rightVal = d1 < d2 ? d2 : d1;

        float tempDist = 0;

        for (int i = 0; i < m_distances.Count; i++) {
            if (tempDist > leftVal && tempDist < rightVal) {
                if (!(i == 0 && !includeEndPoints))
                    m_corners.Add(m_trajectoryPositions[i]);
            }

            tempDist += m_distances[i];
        }
        
        if (includeEndPoints && tempDist > leftVal && tempDist < rightVal)
            m_corners.Add(m_trajectoryPositions[m_trajectoryPositions.Count - 1]);

        return m_corners;
    }

    public override float GetTrajectoryLength()
    {
        return m_totalDistance;
    }

    public override float GetDistanceFromProgress(float progress)
    {
        return m_totalDistance * progress;
    }

    public override float GetProgressFromDistance(float distance)
    {
        return distance / m_totalDistance;
    }

    private void OnDestroy()
    {
        OnBounce = null;
    }

#if UNITY_EDITOR && !FAKE_BUILD
    public override void DrawDebugTrajectory()
    {
        for (int i = 0; i < m_trajectoryPositions.Count - 1; i++) {
            Debug.DrawLine(m_trajectoryPositions[i], m_trajectoryPositions[i + 1], Color.white);
        }

        foreach (Vector3 pos in m_trajectoryPositions) {
            Debug.DrawLine(pos, pos + Vector3.down, Color.white, 1);
        }
    }
#endif
}
