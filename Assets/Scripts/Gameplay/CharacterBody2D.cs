using System.Text;
using UnityEngine;

public class CharacterBody2D : MonoBehaviour
{
    [SerializeField]
    private float m_floorRayCastDistance = 1f;

    [SerializeField]
    protected LayerMask m_floorMask;

    public bool IsGrounded { get; private set; }
    public bool IsKinematic { get { return m_body.isKinematic; } set { m_body.isKinematic = value; } }
    public float GravityScale { get { return m_body.gravityScale; } set { m_body.gravityScale = value; } }
    public float VelocityMagnitude { get { return m_body.velocity.magnitude; } }
    public Vector2 Velocity { get { return m_body.velocity; } }

    [SerializeField]
    private Vector2 m_maxMoveVelocity = new Vector2(15, 0);
    public Vector2 MaxMoveVelocity { get { return m_maxMoveVelocity; } set { m_maxMoveVelocity = value; } }

    [SerializeField]
    private Vector2 m_groundAcceleration = new Vector2(30, 0);
    public Vector2 GroundAcceleration { get { return m_groundAcceleration; } set { m_groundAcceleration = value; } }

    [SerializeField]
    private Vector2 m_airAcceleration = new Vector2(30, 0);
    public Vector2 AirAcceleration { get { return m_airAcceleration; } set { m_airAcceleration = value; } }

    [SerializeField]
    private Vector2 m_groundAutoDeceleration = new Vector2(120, 0);
    public Vector2 GroundAutoDeceleration { get { return m_groundAutoDeceleration; } set { m_groundAutoDeceleration = value; } }

    [SerializeField]
    private Vector2 m_airAutoDeceleration = new Vector2(60, 0);
    public Vector2 AirAutoDeceleration { get { return m_airAutoDeceleration; } set { m_airAutoDeceleration = value; } }

    [SerializeField]
    private Vector2 m_groundDirectionChangeDeceleration = new Vector2(60, 0);
    public Vector3 GroundDirectionChangeDeceleration { get { return m_groundDirectionChangeDeceleration; } set { m_groundDirectionChangeDeceleration = value; } }

    [SerializeField]
    private Vector2 m_airDirectionChangeDeceleration = new Vector2(60, 0);
    public Vector3 AirDirectionChangeDeceleration { get { return m_airDirectionChangeDeceleration; } set { m_airDirectionChangeDeceleration = value; } }

    private Vector2 m_lastAcceleration = Vector2.zero;

    [SerializeField]
    protected Rigidbody2D m_body;

    [SerializeField]
    private Transform[] m_groundTestOrigins;

    public void Move(Vector2 direction)
    {
        Vector2 changeMultiplier = Vector2.zero;

        if ((m_lastAcceleration.x > 0 && m_body.velocity.x < 0) || (m_lastAcceleration.x < 0 && m_body.velocity.x > 0))
            changeMultiplier.x = 1;

        if ((m_lastAcceleration.y > 0 && m_body.velocity.y < 0) || (m_lastAcceleration.y < 0 && m_body.velocity.y > 0))
            changeMultiplier.y = 1;


        m_lastAcceleration = direction * (IsGrounded ? GroundAcceleration : AirAcceleration) + direction * changeMultiplier * (IsGrounded ? GroundDirectionChangeDeceleration : AirDirectionChangeDeceleration);
    }

    protected virtual void FixedUpdate()
    {
        m_body.AddForce(m_lastAcceleration);
        ClampVelocity();
        Decelerate();
        TestForGround();
    }

    private void Decelerate()
    {
        Vector3 velocity = m_body.velocity;

        if (IsGrounded) {
            if (Mathf.Abs(m_lastAcceleration.x) <= float.Epsilon) {
                velocity.x = Mathf.MoveTowards(m_body.velocity.x, 0, GroundAutoDeceleration.x * Time.fixedDeltaTime);
            }

            if (Mathf.Abs(m_lastAcceleration.y) <= float.Epsilon) {
                velocity.y = Mathf.MoveTowards(velocity.y, 0, GroundAutoDeceleration.y * Time.fixedDeltaTime);
            }
        } else {
            if (Mathf.Abs(m_lastAcceleration.x) <= float.Epsilon) {
                velocity.x = Mathf.MoveTowards(m_body.velocity.x, 0, AirAutoDeceleration.x * Time.fixedDeltaTime);
            }

            if (Mathf.Abs(m_lastAcceleration.y) <= float.Epsilon) {
                velocity.y = Mathf.MoveTowards(velocity.y, 0, AirAutoDeceleration.y * Time.fixedDeltaTime);
            }
        }

        m_body.velocity = velocity;
    }

    public void ApplyImpulse(Vector2 impulse)
    {
        m_body.AddForce(impulse, ForceMode2D.Impulse);
    }

    public void SetVelocity(Vector2 velocity)
    {
        m_body.velocity = velocity;
    }

    private void ClampVelocity()
    {
        Vector3 velocity = m_body.velocity;

        if (MaxMoveVelocity.x != 0)
            velocity.x = Mathf.Clamp(velocity.x, -MaxMoveVelocity.x, MaxMoveVelocity.x);

        if (MaxMoveVelocity.y != 0)
            velocity.y = Mathf.Clamp(velocity.y, -MaxMoveVelocity.y, MaxMoveVelocity.y);

        m_body.velocity = velocity;
    }

    private void TestForGround()
    {
        foreach (Transform t in m_groundTestOrigins) {
            RaycastHit2D hit = Physics2D.Raycast(t.position, Vector2.down, m_floorRayCastDistance, m_floorMask);
            if (hit.collider != null) {
                transform.SetParent(hit.collider.transform);
                IsGrounded = true;
                return;
            }
        }

        transform.SetParent(null);
        IsGrounded = false;
    }

#if UNITY_EDITOR && !FAKE_BUILD
    protected virtual void OnDrawGizmos()
    {
        if (m_groundTestOrigins != null)

            foreach (Transform t in m_groundTestOrigins) {
                RaycastHit2D hit = Physics2D.Raycast(t.position, Vector2.down, m_floorRayCastDistance, m_floorMask);
                Debug.DrawRay(t.position, Vector2.down * m_floorRayCastDistance);
            }
    }
#endif
}