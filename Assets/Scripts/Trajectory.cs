using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Trajectory : MonoBehaviour
{
    /// <summary>
    /// Will recalculate the trajectory using position, velocity, and other values passed at trajectory creation.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="velocity"></param>
    public abstract void CalculateTrajectory(Vector3 position, Vector3 velocity);
    public abstract float Continue(float velocity);
    public abstract Vector3 GetCurrentPosition();
    public abstract Vector3 GetCurrentDirection();
    public abstract void DrawDebugTrajectory();
    public abstract float GetCurrentDistance();
    public abstract void GetTrajectoryAtDistance(float distance, out Vector3 position, out Vector3 direction);
    public abstract float GetTrajectoryLength();
    public abstract float GetDistanceFromProgress(float progress);
    public abstract float GetProgressFromDistance(float distance);
}
