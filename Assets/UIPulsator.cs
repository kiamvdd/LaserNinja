using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPulsator : MonoBehaviour
{
    [SerializeField]
    private float m_scaleMultiplier;
    [SerializeField]
    private float m_scalePeriod;
    [SerializeField]
    private float m_rotationMultiplier;
    [SerializeField]
    private float m_rotationPeriod;

    private Vector3 m_startScale;
    private Vector3 m_startRotation;
    private void Awake()
    {
        m_startScale = transform.localScale;
        m_startRotation = transform.localRotation.eulerAngles;
    }

    private void Update()
    {
        transform.localScale = m_startScale + (Vector3.one * Mathf.Sin(Time.time * m_scalePeriod) * m_scaleMultiplier);
        Vector3 newRot = m_startRotation;
        transform.localRotation = Quaternion.Euler(m_startRotation.x, m_startRotation.y, m_startRotation.z + (Mathf.Sin(Time.time * m_rotationPeriod) * m_rotationMultiplier));   
    }
}
