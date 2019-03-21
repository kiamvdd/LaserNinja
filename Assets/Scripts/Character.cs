using UnityEngine;

public abstract class Character : MonoBehaviour, IDamageable
{

    protected float m_health;

    [SerializeField]
    private float m_maxHealth = 1;

    [SerializeField]
    protected float m_movementSpeed = 10;

    [SerializeField]
    protected View m_viewController;

    protected virtual void Awake()
    {
        m_health = m_maxHealth;
    }

    protected abstract void Move(Vector2 direction);

    protected virtual void LookAt(Vector3 position)
    {
        m_viewController.LookAt(position);
    }

    public virtual void TakeDamage(float amount)
    {
        m_health -= amount;

        if (m_health <= 0) {
            Destroy();
        }
    }

    public virtual void Destroy()
    {
        Destroy(gameObject);
    }

    public virtual bool IsAlive()
    {
        return m_health > 0;
    }
}
