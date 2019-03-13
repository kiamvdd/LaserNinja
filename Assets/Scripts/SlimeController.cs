using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeController : Character
{
    private Transform m_target;

    [SerializeField]
    private ParticleSystem m_deathParticles;


    protected override void Awake()
    {
        base.Awake();

        GameObject obj = GameObject.FindGameObjectWithTag("Player");

        if (obj != null)
            m_target = obj.transform;
    }

    private void Update()
    {
        if (m_target == null)
            return;

        Vector3 targetDirection = m_target.position - transform.position;
        int layerMask = ~((1 << m_target.gameObject.layer) | (1 << gameObject.layer));
        RaycastHit2D hit = Physics2D.Raycast(transform.position, targetDirection.normalized, targetDirection.magnitude, layerMask);

        if (hit.collider != null)
            return;

        float h = Mathf.Clamp(m_target.position.x - transform.position.x, -1, 1) * m_movementSpeed;
        Move(new Vector2(h, 0));
    }

    public override void Destroy()
    {
        m_deathParticles.transform.parent = null;
        m_deathParticles.Play();
        base.Destroy();
    }
}
