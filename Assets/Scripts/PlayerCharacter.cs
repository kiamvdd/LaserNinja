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

    private void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");

        Move(new Vector2(h, 0));
        LookAt(Camera.main.ScreenToWorldPoint(Input.mousePosition));

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

        if (Input.GetMouseButtonDown(0))
            m_gun.Fire(m_viewController.LookDirection);
    }
}