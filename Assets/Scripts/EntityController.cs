using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityController : MonoBehaviour
{
    [SerializeField]
    protected MovementController2D m_movementController;

    [SerializeField]
    protected ViewController m_viewController;

    protected void Move(float horizontalDirection)
    {
        m_movementController.Move(horizontalDirection);
        m_viewController.Move(horizontalDirection);
    }

    protected void LookAt(Vector3 position)
    {
        m_viewController.LookAt(position);
    }
}
