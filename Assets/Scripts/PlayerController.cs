using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : EntityController
{
    [SerializeField]
    private Gun m_gun;

    private void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");

        Move(h);

        if (Input.GetButtonDown("Jump"))
            m_movementController.Jump();

        if (Input.GetMouseButtonDown(0))
            m_gun.Fire(m_viewController.LookDirection);
    }
}