using UnityEngine;

public class PlayerCharacter : Character
{
    [SerializeField]
    private float m_jumpForce = 10;
    [SerializeField]
    private float m_wallJumpForce = 10;
    private bool m_jumping = false;

    [SerializeField]
    private Gun m_gun;

    [SerializeField]
    private PlayerBody2D m_playerBody;

    [SerializeField]
    private float m_slowmoSpeed = 0.2f;
    private float m_physicsTimeStep;

    [SerializeField]
    private ParticleSystem m_deathParticles;

    [SerializeField]
    private ParticleSystem m_jumpParticles;

    [SerializeField]
    private SoundClip m_jumpSound;

    public class ShotInfo
    {
        public ShotInfo()
        {
            baseDamage = 0;
            playerMoving = false;
            playerJumping= false;
        }

        public float baseDamage;
        public bool playerMoving;
        public bool playerJumping;
    }

    protected override void Awake()
    {
        base.Awake();
        m_physicsTimeStep = Time.fixedDeltaTime;
    }

    private void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");

        LookAt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        Move(new Vector2(h, 0));

        // Jumping logic
        if (((m_playerBody.TouchingLeftWall && h < 0) || (m_playerBody.TouchingRightWall && h > 0)) && m_playerBody.Velocity.y < 0) {
            Vector2 v = m_playerBody.MaxMoveVelocity;
            v.y = 8;
            m_playerBody.MaxMoveVelocity = v;
        } else {
            Vector2 v = m_playerBody.MaxMoveVelocity;
            v.y = 32;
            m_playerBody.MaxMoveVelocity = v;
        }

        if (m_playerBody.IsGrounded && !m_jumping) {
            if (Input.GetButtonDown("Jump")) {
                Jump(new Vector2(0, m_jumpForce), true);
            }
        } else {
            if (m_jumping) {
                if (!Input.GetButton("Jump"))
                    m_jumping = false;

                if (m_playerBody.Velocity.y > 0 && m_jumping)
                    m_playerBody.GravityScale = 3;
                else
                    m_playerBody.GravityScale = 6;
            } else {
                if (Input.GetButtonDown("Jump")) {
                    if (m_playerBody.TouchingLeftWall) {
                        Vector2 v = m_playerBody.MaxMoveVelocity;
                        v.y = 32    ;
                        m_playerBody.MaxMoveVelocity = v;

                        Jump(new Vector2(m_wallJumpForce, m_jumpForce));

                    } else if (m_playerBody.TouchingRightWall) {
                        Vector2 v = m_playerBody.MaxMoveVelocity;
                        v.y = 32;
                        m_playerBody.MaxMoveVelocity = v;

                        Jump(new Vector2(-m_wallJumpForce, m_jumpForce));
                    }
                }
            }
        }

        // Aiming / gun logic

        if (Input.GetMouseButtonDown(0)) {
            m_gun.Fire(m_viewController.LookDirection, GetCurrentTrickInfo());
        }

        if (Input.GetMouseButtonDown(1)) {
            SetTimeScale(m_slowmoSpeed);
            m_gun.SetAimingGuideEnabled(true);
        }

        if (Input.GetMouseButton(1))
            m_gun.ShowAimingGuide(m_viewController.LookDirection);

        if (Input.GetMouseButtonUp(1) || Input.GetMouseButtonDown(0)) {
            m_gun.SetAimingGuideEnabled(false);
            SetTimeScale(1);
        }
    }

    private ShotInfo GetCurrentTrickInfo() {
        ShotInfo trickInfo = new ShotInfo();
        trickInfo.playerJumping = !m_playerBody.IsGrounded;
        trickInfo.playerMoving = Mathf.Abs(m_playerBody.Velocity.x) > float.Epsilon;

        return trickInfo;
    }

    private void SetTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
        Time.fixedDeltaTime = timeScale * 0.02f;
    }

    private void Jump(Vector2 jumpForce, bool impulse = false)
    {
        if (impulse)
            m_playerBody.ApplyImpulse(jumpForce);
        else
            m_playerBody.SetVelocity(jumpForce);

        m_jumping = true;
        m_jumpParticles.Play();
        m_jumpSound.Play();
        m_viewController.Animator.SetTrigger("Jump");
    }

    public override void Destroy()
    {
        m_deathParticles.transform.parent = null;
        m_deathParticles.Play();

        LevelTimer levelTimer = FindObjectOfType<LevelTimer>();
        levelTimer.EndLevel(false);

        SetTimeScale(1);

        base.Destroy();
    }

    protected override void Move(Vector2 direction)
    {
        m_playerBody.Move(direction);

        if (m_viewController != null)
            m_viewController.Move(direction);
    }
}