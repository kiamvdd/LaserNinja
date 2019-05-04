using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform m_target;

    [SerializeField]
    private Camera m_camera;

    [SerializeField]
    private float m_maxCameraDistance = 5;

    [SerializeField]
    private float m_shakeDuration = 1;
    [SerializeField]
    private float m_shakeSpeed = 1;
    [SerializeField]
    private float m_shakeIntensity = 1;
    [SerializeField]
    private float m_shakeNoise = 1;

    public void Tick()
    {
        if (m_target == null)
            return;

        Vector3 adjustedMousePos = Input.mousePosition - new Vector3(Screen.width / 2f, Screen.height / 2f);
        adjustedMousePos /= Screen.width;

        Vector3 targetPos = m_target.position;

        Vector3 newPos = targetPos + adjustedMousePos * m_maxCameraDistance + Vector3.up * 2;
        newPos.z = transform.position.z;
        transform.position = newPos;


        if (Input.GetKeyDown("t"))
            StartShake(m_shakeDuration, m_shakeSpeed, m_shakeIntensity, m_shakeNoise);
    }

    public void SetTarget(Transform target)
    {
        m_target = target;
    }

    public void StartShake(float duration, float speed, float intensity, float noise)
    {
        Vector3 localPos = m_camera.transform.localPosition;
        localPos.x = 0;
        localPos.y = 0;

        m_camera.transform.localPosition = localPos;

        StartCoroutine(Shake(duration, speed, intensity, noise));
    }

    private IEnumerator Shake(float duration, float speed, float intensity, float noise)
    {
        speed /= 10;
        intensity /= 10;
        noise /= 100;

        float effectTimer = 0;

        float xTimer = Random.Range(0, 360);
        float xSpeed = speed + Random.Range(-speed / 10, speed / 10);

        float yTimer = Random.Range(0, 360);
        float ySpeed = speed + Random.Range(-speed / 10, speed / 10);

        while (effectTimer < duration) {
            effectTimer += Time.deltaTime;
            xTimer = (xTimer + Time.deltaTime * xSpeed * 100) % 360;
            yTimer = (yTimer + Time.deltaTime * ySpeed * 100) % 360;

            Vector3 localPos = m_camera.transform.localPosition;
            float rampedIntensity = intensity * (1 - effectTimer / duration);
            localPos.x = (Mathf.Sin(xTimer) + Random.Range(-noise, noise)) * rampedIntensity;
            localPos.y = (Mathf.Sin(yTimer) + Random.Range(-noise, noise)) * rampedIntensity;

            m_camera.transform.localPosition = localPos;
            yield return null;
        }

        yield return null;
    }
}
