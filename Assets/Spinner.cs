using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    [SerializeField]
    private float m_spinSpeed = 1f;

    private void Update()
    {
        transform.Rotate(new Vector3(0, 0, m_spinSpeed * Time.deltaTime));
    }
}
