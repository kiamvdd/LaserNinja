using UnityEngine;
using Helpers;
using UnityEditor;

public class PlayerCharacter : Character
{
    #region Data members
    [Header("Body")]
    [SerializeField]
    private PlayerBody2D m_playerBody;

    [Header("Movement")]
    private PlayerMovementState m_state = PlayerMovementState.IDLE;

    [SerializeField]
    private float m_jumpForce = 10;
    [SerializeField]
    private float m_wallJumpForce = 10;

    [SerializeField]
    private float m_maxFallVelocity = 32;

    [SerializeField]
    private float m_jumpingGravity = 3;
    [SerializeField]
    private float m_fallingGravity = 6;

    [SerializeField]
    private float m_maxWallSlideVelocity = 1;

    [SerializeField]
    private float m_slowmoSpeed = 0.2f;
    private float m_physicsTimeStep;

    [Header("FX")]
    [SerializeField]
    private ParticleSystem m_deathParticles;

    [SerializeField]
    private ParticleSystem m_jumpParticles;

    [SerializeField]
    private SoundClip m_jumpSound;

    private CameraController m_cameraController;

    [SerializeField]
    private AmmoUI m_ammoUI;

    [Header("Other")]
    [SerializeField]
    private Gun m_gun;
    #endregion

    protected override void Awake()
    {
        base.Awake();
        m_gun.OnShotcountChanged += UpdateAmmoUI;
        m_ammoUI.SetAmmoCount(3);
        m_cameraController = FindObjectOfType<CameraController>();
        m_physicsTimeStep = Time.fixedDeltaTime;
    }

    private void OnDestroy()
    {
        m_gun.OnShotcountChanged -= UpdateAmmoUI;
    }

    private void UpdateAmmoUI(int ammoChangeAmount)
    {
        if (ammoChangeAmount > 0 && Input.GetMouseButton(1))
        {
            m_gun.SetAimingGuideEnabled(true);
            SetTimeScale(m_slowmoSpeed);
        }

        m_ammoUI.ModifyAmmoCountBy(ammoChangeAmount);
        m_ammoUI.FlashIcons();
    }

    public enum PlayerMovementState
    {
        IDLE,
        RUNNING,
        JUMPING,
        FALLING,
        WALLSLIDING,
    }

    private void Update()
    {

        switch (m_state) {
            case PlayerMovementState.IDLE:
                ExecuteIdleState();
                break;
            case PlayerMovementState.RUNNING:
                ExecuteRunningState();
                break;
            case PlayerMovementState.JUMPING:
                ExecuteJumpingState();
                break;
            case PlayerMovementState.FALLING:
                ExecuteFallingState();
                break;
            case PlayerMovementState.WALLSLIDING:
                ExecuteWallSlideState();
                break;
            default:
                break;
        }

        m_cameraController.Tick();

        // Aiming / gun logic
        LookAt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (Input.GetMouseButtonDown(0)) {
            m_gun.Fire(m_viewController.LookDirection);
        }

        if (Input.GetMouseButtonDown(1) && m_gun.ShotCount > 0) {
            SetTimeScale(m_slowmoSpeed);
            m_gun.SetAimingGuideEnabled(true);
        }

        if (Input.GetMouseButton(1))
            m_gun.UpdateAimingDirection(m_viewController.LookDirection);

        if (Input.GetMouseButtonUp(1) || Input.GetMouseButtonDown(0)) {
            m_gun.SetAimingGuideEnabled(false);
            SetTimeScale(1);
        }
    }

    private void ExecuteIdleState()
    {
        float h = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(h) > float.Epsilon) {
            SwitchMovementState(PlayerMovementState.RUNNING);
            return;
        }

        if (!m_playerBody.IsGrounded) {
            SwitchMovementState(PlayerMovementState.FALLING);
            EventBus.OnTrickEvent(new TrickEventData(TrickEventData.TrickEventType.PLAYERFALL));
            return;
        }

        if (Input.GetButton("Jump")) {
            Jump(new Vector2(0, m_jumpForce), true);
            EventBus.OnTrickEvent(new TrickEventData(TrickEventData.TrickEventType.PLAYERJUMP));
            SwitchMovementState(PlayerMovementState.JUMPING);
            return;
        }
    }

    private void ExecuteRunningState()
    {
        float h = Input.GetAxisRaw("Horizontal");
        Move(new Vector2(h, 0));

        if (!m_playerBody.IsGrounded) {
            SwitchMovementState(PlayerMovementState.FALLING);
            EventBus.OnTrickEvent(new TrickEventData(TrickEventData.TrickEventType.PLAYERFALL));
            return;
        }

        if (Input.GetButton("Jump")) {
            Jump(new Vector2(0, m_jumpForce), true);
            EventBus.OnTrickEvent(new TrickEventData(TrickEventData.TrickEventType.PLAYERJUMP));
            SwitchMovementState(PlayerMovementState.JUMPING);
            return;
        }

        if (Mathf.Abs(h) < float.Epsilon) {
            SwitchMovementState(PlayerMovementState.IDLE);
            return;
        }
    }

    private void ExecuteJumpingState()
    {
        float h = Input.GetAxisRaw("Horizontal");
        Move(new Vector2(h, 0));

        if (!Input.GetButton("Jump") || m_playerBody.Velocity.y < 0)
            SwitchMovementState(PlayerMovementState.FALLING);
    }

    private void ExecuteFallingState()
    {
        float h = Input.GetAxisRaw("Horizontal");
        Move(new Vector2(h, 0));

        if (m_playerBody.IsGrounded) {
            SwitchMovementState(PlayerMovementState.IDLE);
            return;
        }

        if (((m_playerBody.TouchingLeftWall && h < 0) || (m_playerBody.TouchingRightWall && h > 0))) {
            SwitchMovementState(PlayerMovementState.WALLSLIDING);
        } else if (Input.GetButtonDown("Jump")) {
            if (m_playerBody.TouchingLeftWall) {
                Jump(new Vector2(m_wallJumpForce, m_jumpForce));
                EventBus.OnTrickEvent(new TrickEventData(TrickEventData.TrickEventType.PLAYERWALLJUMP));
                SwitchMovementState(PlayerMovementState.JUMPING);
                return;
            } else if (m_playerBody.TouchingRightWall) {
                Jump(new Vector2(-m_wallJumpForce, m_jumpForce));
                EventBus.OnTrickEvent(new TrickEventData(TrickEventData.TrickEventType.PLAYERWALLJUMP));
                SwitchMovementState(PlayerMovementState.JUMPING);
                return;
            }
        }
    }

    private void ExecuteWallSlideState()
    {
        float h = Input.GetAxisRaw("Horizontal");
        Move(new Vector2(h, 0));

        if (Input.GetButtonDown("Jump")) {
            if (m_playerBody.TouchingLeftWall) {
                Jump(new Vector2(m_wallJumpForce, m_jumpForce));
                EventBus.OnTrickEvent(new TrickEventData(TrickEventData.TrickEventType.PLAYERWALLJUMP));
                SwitchMovementState(PlayerMovementState.JUMPING);
                return;
            } else if (m_playerBody.TouchingRightWall) {
                Jump(new Vector2(-m_wallJumpForce, m_jumpForce));
                EventBus.OnTrickEvent(new TrickEventData(TrickEventData.TrickEventType.PLAYERWALLJUMP));
                SwitchMovementState(PlayerMovementState.JUMPING);
                return;
            }
        }

        if (m_playerBody.IsGrounded) {
            SwitchMovementState(PlayerMovementState.IDLE);
        } else if (!((m_playerBody.TouchingLeftWall && h < 0) || (m_playerBody.TouchingRightWall && h > 0))) {
            SwitchMovementState(PlayerMovementState.FALLING);
            EventBus.OnTrickEvent(new TrickEventData(TrickEventData.TrickEventType.PLAYERFALL));
        }
    }

    private void SwitchMovementState(PlayerMovementState state)
    {
        // On state exit
        switch (m_state) {
            case PlayerMovementState.IDLE:
                break;
            case PlayerMovementState.RUNNING:
                break;
            case PlayerMovementState.JUMPING:
                break;
            case PlayerMovementState.FALLING:
                if (state == PlayerMovementState.IDLE)
                    EventBus.OnTrickEvent(new TrickEventData(TrickEventData.TrickEventType.PLAYERLAND));
                break;
            case PlayerMovementState.WALLSLIDING:
                m_playerBody.MaxMoveVelocity = m_playerBody.MaxMoveVelocity.ChangeY(m_maxFallVelocity);
                break;
            default:
                break;
        }

        // On state enter
        switch (state) {
            case PlayerMovementState.IDLE:
                break;
            case PlayerMovementState.RUNNING:
                break;
            case PlayerMovementState.JUMPING:
                m_playerBody.GravityScale = m_jumpingGravity;
                break;
            case PlayerMovementState.FALLING:
                m_playerBody.GravityScale = m_fallingGravity;
                break;
            case PlayerMovementState.WALLSLIDING:
                EventBus.OnTrickEvent(new TrickEventData(TrickEventData.TrickEventType.PLAYERWALLRIDE));
                m_playerBody.MaxMoveVelocity = m_playerBody.MaxMoveVelocity.ChangeY(m_maxWallSlideVelocity);
                break;
            default:
                break;
        }

        m_state = state;
    }

    private void SetTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
        Time.fixedDeltaTime = timeScale * m_physicsTimeStep;
    }

    private void Jump(Vector2 jumpForce, bool impulse = false)
    {
        if (impulse)
            m_playerBody.ApplyImpulse(jumpForce);
        else
            m_playerBody.SetVelocity(jumpForce);

        m_jumpParticles.Play();
        m_jumpSound.Play();
        m_viewController.Animator.SetTrigger("Jump");
    }
    protected override void Move(Vector2 direction)
    {
        m_playerBody.Move(direction);

        if (m_viewController != null)
            m_viewController.Move(direction);
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
}