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
    private Vector2 m_maxMoveVelocity = new Vector2(15, 0);
    public Vector2 MaxMoveVelocity { get { return m_maxMoveVelocity; } set { m_maxMoveVelocity = value; } }

    [SerializeField]
    private Vector2 m_acceleration = new Vector2(30, 0);
    public Vector2 Acceleration { get { return m_acceleration; } set { m_acceleration = value; } }

    [SerializeField]
    private Vector2 m_deceleration = new Vector2(60, 0);
    public Vector2 Deceleration { get { return m_deceleration; } set { m_deceleration = value; } }

    private Vector2 m_lastAcceleration = Vector2.zero;

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
        m_lastAcceleration = direction * Acceleration;
        m_body.AddForce(m_lastAcceleration);

        ClampVelocity();
    }

    private void Update()
    {
        Vector3 velocity = m_body.velocity;

        bool changingX = ((m_lastAcceleration.x > 0 && velocity.x < 0) || (m_lastAcceleration.x < 0 && velocity.x > 0));
        if (changingX || Mathf.Abs(m_lastAcceleration.x) <= float.Epsilon) {
            velocity.x = Mathf.MoveTowards(m_body.velocity.x, 0, Deceleration.x * Time.deltaTime);
        }

        bool changingY = ((m_lastAcceleration.y > 0 && velocity.y < 0) || (m_lastAcceleration.y < 0 && velocity.y > 0));
        if (changingY || Mathf.Abs(m_lastAcceleration.y) <= float.Epsilon) {
            velocity.y = Mathf.MoveTowards(velocity.y, 0, Deceleration.y * Time.deltaTime);
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

    protected virtual void FixedUpdate()
    {
        ClampVelocity();
        TestForGround();
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
            RaycastHit2D hit = Physics2D.Raycast(t.position, Vector2.down, m_floorRayCastDistance, (1 << m_groundLayerID));
            if (hit.collider != null) {
                IsGrounded = true;
                return;
            }
        }

        IsGrounded = false;
    }

#if UNITY_EDITOR
    protected virtual void OnDrawGizmos()
    {
        if (m_groundTestOrigins != null)

            foreach (Transform t in m_groundTestOrigins) {
                RaycastHit2D hit = Physics2D.Raycast(t.position, Vector2.down, m_floorRayCastDistance, (1 << m_groundLayerID));
                Debug.DrawRay(t.position, Vector2.down * m_floorRayCastDistance);
            }
    }
#endif
}
