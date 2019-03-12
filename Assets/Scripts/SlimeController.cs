using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeController : Character
{
    private Transform m_target;

    protected override void Awake()
    {
        base.Awake();

        GameObject obj = GameObject.FindGameObjectWithTag("Player");

        if (obj != null)
            m_target = obj.transform;
    }

    private void Update()
    {
        float h = m_target.position.x - transform.position.x;
        Move(new Vector2(h, 0));
    }
}
