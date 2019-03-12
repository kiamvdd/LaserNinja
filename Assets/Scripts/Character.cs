using UnityEngine;

public class Character : MonoBehaviour, IDamageable
{

    protected float m_health;

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

        Debug.Log("Took damage: " + amount + " damage bringing health down from " + m_health + " to " + (m_health - amount) + ".");
        m_health -= amount;

        if (m_health <= 0)
            OnDestroyed();
    }

    public void OnDestroyed()
    {
        Destroy(gameObject);
    }
}
