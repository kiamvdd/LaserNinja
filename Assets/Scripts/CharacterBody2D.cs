using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBody2D : MonoBehaviour
{
    [SerializeField]
    private float m_floorRayCastDistance = 1f;

    private int m_groundLayerID;

    public bool IsGrounded { get; private set; }
    public bool IsKinematic { get { return m_body.isKinematic; } set { m_body.isKinematic = value; } }
    public float GravityScale { get { return m_body.gravityScale; } set { m_body.gravityScale = value; } }
    public float VelocityMagnitude { get { return m_body.velocity.magnitude; } }
    public Vector2 Velocity { get { return m_body.velocity; } }

    [SerializeField]
    private Rigidbody2D m_body;

    [SerializeField]
    private Transform[] m_groundTestOrigins;

    private void Awake()
    {
        m_groundLayerID = LayerMask.NameToLayer("Floor");
    }

    public void Move(Vector2 direction)
    {
        Vector3 velocity = m_body.velocity;

        if (IsKinematic)
            velocity.y = direction.y;

        velocity.x = direction.x;

        m_body.velocity = velocity;
    }

    public void ApplyImpulse(Vector2 impulse)
    {
        m_body.AddForce(impulse, ForceMode2D.Impulse);
    }

    public void FixedUpdate()
    {
        TestForGround();
    }

    private void TestForGround()
    {
        foreach (Transform t in m_groundTestOrigins) {
            RaycastHit2D hit = Physics2D.Raycast(t.position, Vector2.down, m_floorRayCastDistance, (1 << m_groundLayerID));
            if (hit.collider != null) {
                IsGrounded = true;
                return;
            }
        }

        IsGrounded = false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (m_groundTestOrigins != null)

        foreach (Transform t in m_groundTestOrigins) {
            RaycastHit2D hit = Physics2D.Raycast(t.position, Vector2.down, m_floorRayCastDistance, (1 << m_groundLayerID));
            Debug.DrawRay(t.position, Vector2.down * m_floorRayCastDistance);
        }
    }
#endif
}
