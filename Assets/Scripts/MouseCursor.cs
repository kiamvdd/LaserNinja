using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCursor : MonoBehaviour
{
    [SerializeField]
    private Camera m_camera;
    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 position = m_camera.ScreenToWorldPoint(Input.mousePosition);
        position.z = m_camera.transform.position.z + 1;
        transform.position = position;
    }
}
