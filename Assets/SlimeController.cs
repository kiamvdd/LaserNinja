using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeController : EntityController
{
    private Transform m_target;

    private void Awake()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Player");
        if (obj != null)
            m_target = obj.transform;
    }

    private void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        Move(h);
    }
}
