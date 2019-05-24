using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeController : Character
{
    private Transform m_target;

    private LevelTimer m_levelTimer;

    [SerializeField]
    private float m_detectionRadius = 10f;

    [SerializeField]
    private LayerMask m_occlusionMask;

    [SerializeField]
    protected CharacterBody2D m_characterBody;

    [SerializeField]
    private ParticleSystem m_deathParticles;

    [SerializeField]
    private SoundClip m_deathSound;

    protected override void Awake()
    {
        base.Awake();

        GameObject obj = GameObject.FindGameObjectWithTag("Player");

        if (obj != null)
            m_target = obj.transform;

        m_levelTimer = FindObjectOfType<LevelTimer>();
    }

    private void Update()
    {
        if (m_target == null)
            return;

        Vector3 targetDirection = m_target.position - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, targetDirection.normalized, targetDirection.magnitude, m_occlusionMask);

        Debug.DrawRay(transform.position, targetDirection, ( hit.collider != null ? Color.red : ( Vector2.Distance(transform.position, m_target.position) > m_detectionRadius ? Color.magenta : Color.green ) ));
        if (Vector2.Distance(transform.position, m_target.position) > m_detectionRadius || hit.collider != null)
            return;

        float h = Mathf.Clamp(m_target.position.x - transform.position.x, -1, 1) * m_movementSpeed;
        Move(new Vector2(h, 0));
    }

    public override float TakeDamage(float amount)
    {
        return base.TakeDamage(amount);
    }

    public override void Destroy()
    {
        m_deathSound.Play();
        m_deathParticles.transform.parent = null;
        m_deathParticles.Play();
        base.Destroy();
    }

    protected override void Move(Vector2 direction)
    {
        m_characterBody.Move(direction);

        if (m_viewController != null)
            m_viewController.Move(direction);
    }
}
