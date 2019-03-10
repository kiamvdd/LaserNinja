using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform m_target;

    [SerializeField]
    private Camera m_camera;

    [SerializeField]
    private float m_maxCameraDistance = 5;

    void LateUpdate()
    {
        Vector3 adjustedMousePos = Input.mousePosition - new Vector3(Screen.width / 2f, Screen.height / 2f);
        adjustedMousePos /= Screen.width;

        Vector3 targetPos = m_target.position;

        Vector3 newPos = targetPos + adjustedMousePos * m_maxCameraDistance + Vector3.up * 2;
        newPos.z = transform.position.z;
        transform.position = newPos;
    }

    public void SetTarget(Transform target)
    {
        m_target = target;
    }
}
