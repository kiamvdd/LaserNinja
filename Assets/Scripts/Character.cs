using UnityEngine;

public class Character : MonoBehaviour, IDamageable
{

    protected float m_health;

    [SerializeField]
    private LevelTimer m_levelTimer;

    [SerializeField]
    private float m_maxHealth = 1;

    [SerializeField]
    protected CharacterBody2D m_characterBody;

    [SerializeField]
    protected float m_movementSpeed = 10;

    [SerializeField]
    protected ViewController m_viewController;

    protected virtual void Awake()
    {
        m_levelTimer = GameObject.FindObjectOfType<LevelTimer>();
        m_health = m_maxHealth;
    }

    protected void Move(Vector2 direction)
    {
        m_characterBody.Move(direction * m_movementSpeed);

        if (m_viewController != null)
            m_viewController.Move(direction);
    }

    protected void LookAt(Vector3 position)
    {
        m_viewController.LookAt(position);
    }

    public void TakeDamage(float amount)
    {
        m_health -= amount;

        if (m_health <= 0) {
            m_levelTimer.AddTime((amount - 1) * 2);
            Destroy();
        }
    }

    public virtual void Destroy()
    {
        Destroy(gameObject);
    }
}
