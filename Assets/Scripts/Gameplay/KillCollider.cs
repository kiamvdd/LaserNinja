using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillCollider : MonoBehaviour
{
    [SerializeField]
    private LayerMask m_layerMask;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((m_layerMask.value & (1 << collision.collider.gameObject.layer)) == 0)
            return;

        IDamageable damageable = collision.collider.GetComponent<IDamageable>();
        if (damageable != null)
            damageable.Destroy();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (( m_layerMask.value & ( 1 << collision.gameObject.layer ) ) == 0)
            return;

        IDamageable damageable = collision.GetComponent<IDamageable>();

        if (damageable != null)
            damageable.Destroy();
    }
}