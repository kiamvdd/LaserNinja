using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBody2D : CharacterBody2D
{
    public bool TouchingLeftWall { get; private set; }
    public bool TouchingRightWall { get; private set; }

    [SerializeField]
    private float m_wallTestDistance;

    [SerializeField]
    private Transform[] m_leftWallTestOrigins;
    [SerializeField]
    private Transform[] m_rightWallTestOrigins;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        TestForWalls();
    }

    private void TestForWalls()
    {
        TouchingLeftWall = false;
        foreach (Transform t in m_leftWallTestOrigins) {
            RaycastHit2D hit = Physics2D.Raycast(t.position, Vector2.left, m_wallTestDistance, m_floorMask);
            if (hit.collider != null) {
                TouchingLeftWall = true;
                break;
            }
        }

        TouchingRightWall = false;
        foreach (Transform t in m_rightWallTestOrigins) {
            RaycastHit2D hit = Physics2D.Raycast(t.position, Vector2.right, m_wallTestDistance, m_floorMask);
            if (hit.collider != null) {
                TouchingRightWall = true;
                break;
            }
        }
    }

#if UNITY_EDITOR
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        foreach (Transform t in m_leftWallTestOrigins) {
            Debug.DrawRay(t.position, Vector3.left * m_wallTestDistance, TouchingLeftWall ? Color.red : Color.green);
        }

        foreach (Transform t in m_rightWallTestOrigins) {
            Debug.DrawRay(t.position, Vector3.right * m_wallTestDistance, TouchingRightWall ? Color.red : Color.green);
        }
    }
#endif
}
