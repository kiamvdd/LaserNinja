using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController2D : MonoBehaviour
{
    [SerializeField]
    private float m_floorRayCastDistance = 1f;

    private int m_layerID;

    [SerializeField]
    private float m_jumpVelocity = 10f;

    public bool Grounded { get; private set; }

    [SerializeField]
    private Rigidbody2D m_body;

    [SerializeField]
    private Transform[] m_raycastOrigins;

    [SerializeField]
    private float m_movementSpeed = 10;
    public float MovementSpeed { get { return m_movementSpeed; } set { m_movementSpeed = value; } }

    private void Awake()
    {
        m_layerID = LayerMask.NameToLayer("Player");
    }

    public void Move(float xDirection)
    {
        Vector3 velocity = m_body.velocity;
        velocity.x = xDirection * MovementSpeed;
        m_body.velocity = velocity;
    }

    public void Jump()
    {
        if (Grounded) {
            Vector3 velocity = m_body.velocity;
            velocity.y = m_jumpVelocity;
            m_body.velocity = velocity;
        }
    }

    public void FixedUpdate()
    {
        TestForGround();

        m_body.gravityScale = m_body.velocity.y > 0 ? 3 : 6;
    }

    private void TestForGround()
    {
        foreach (Transform t in m_raycastOrigins) {
            RaycastHit2D hit = Physics2D.Raycast(t.position, Vector2.down, m_floorRayCastDistance, ~(1 << m_layerID));
            if (hit.collider != null) {
                Grounded = true;
                return;
            }
        }

        Grounded = false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (m_raycastOrigins != null)

        foreach (Transform t in m_raycastOrigins) {
            RaycastHit2D hit = Physics2D.Raycast(t.position, Vector2.down, m_floorRayCastDistance, ~(1 << m_layerID));
            Debug.DrawRay(t.position, Vector2.down * m_floorRayCastDistance);
        }
    }
#endif
}
