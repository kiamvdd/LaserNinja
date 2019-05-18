using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MovingPlatform : MonoBehaviour
{
    public enum StartType
    {
        TIMERSTART,
        TOUCH,
        TRIGGER,
    }

    [SerializeField]
    private StartType m_startType;
    [SerializeField]
    private Transform m_startPoint;
    [SerializeField]
    private Transform m_endPoint;
    [SerializeField]
    private bool m_loop;
    [SerializeField]
    private float m_speed;
    [SerializeField]
    private Rigidbody2D m_rigidBody;
    [SerializeField]
    private UnityEvent m_onTargetReached;

    private float m_timer;

    private float m_cycleTime;

    private bool m_active = false;

    // Start is called before the first frame update
    private void Awake()
    {
        m_cycleTime = Vector3.Distance(m_startPoint.position, m_endPoint.position) / m_speed;
        if (m_startType == StartType.TIMERSTART)
        {
            EventBus.OnTimerStart += Activate;
        }
    }

    private void OnDestroy()
    {
        EventBus.OnTimerStart -= Activate;
    }

    private void Activate()
    {
        m_active = true;
    }

    private void FixedUpdate()
    {
        if (m_active)
        {
            m_timer += Time.fixedDeltaTime;
            if (m_timer >= m_cycleTime)
            {
                if (m_loop)
                {
                    Transform t = m_startPoint;
                    m_startPoint = m_endPoint;
                    m_endPoint = t;

                    m_timer = 0;
                }
                else
                {
                    m_timer = m_cycleTime;
                    this.enabled = false;
                }

                if (m_onTargetReached != null)
                    m_onTargetReached.Invoke();
            }

            transform.position = Vector3.Lerp(m_startPoint.position, m_endPoint.position, m_timer / m_cycleTime);
            //m_rigidBody.MovePosition(Vector3.Lerp(m_startPoint.position, m_endPoint.position, m_timer / m_cycleTime));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (m_active || m_startType != StartType.TOUCH)
            return;

        PlayerBody2D body = collision.gameObject.GetComponent<PlayerBody2D>();
        if (body != null)
        {
            Activate();
        }
    }

    public void ActivateFromTrigger()
    {
        m_active = true;
    }
}
