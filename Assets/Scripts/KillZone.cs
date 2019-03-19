using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Entered KIllzone");
        IDamageable damageable = collision.GetComponent<IDamageable>();

        if (damageable != null)
            damageable.Destroy();
    }
}
