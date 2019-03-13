using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Character
{
    [SerializeField]
    private float m_jumpForce = 10;
    private bool m_jumping = false;

    [SerializeField]
    private Gun m_gun;

    [SerializeField]
    private float m_slowmoSpeed = 0.2f;
    private float m_physicsTimeStep;

    [SerializeField]
    private ParticleSystem m_deathParticles;

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

        if (m_characterBody.IsGrounded && !m_jumping) {
            if (Input.GetButtonDown("Jump")) {
                m_characterBody.ApplyImpulse(new Vector2(0, m_jumpForce));
                m_jumping = true;
            }
        } else {
            if (!Input.GetButton("Jump"))
                m_jumping = false;

            if (m_characterBody.Velocity.y > 0 && m_jumping)
                m_characterBody.GravityScale = 3;
            else
                m_characterBody.GravityScale = 6;
        }


        // Aiming / gun logic

        if (Input.GetMouseButtonDown(0))
            m_gun.Fire(m_viewController.LookDirection);

        if (Input.GetMouseButtonDown(1)) {
            Time.timeScale = m_slowmoSpeed;
            Time.fixedDeltaTime = m_slowmoSpeed * 0.02f;
            m_gun.SetAimingGuideEnabled(true);
        }

        if (Input.GetMouseButton(1))
            m_gun.ShowAimingGuide(m_viewController.LookDirection);

        if (Input.GetMouseButtonUp(1) || Input.GetMouseButtonDown(0)) {
            m_gun.SetAimingGuideEnabled(false);
            Time.timeScale = 1;
            Time.fixedDeltaTime = 0.02f;
        }
    }

    public override void Destroy()
    {
        m_deathParticles.transform.parent = null;
        m_deathParticles.Play();
        base.Destroy();
    }
}