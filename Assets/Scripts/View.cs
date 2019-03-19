using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class View : MonoBehaviour
{
    [SerializeField]
    private Transform m_spriteTransform;

    [SerializeField]
    private Transform m_handPivot;

    [SerializeField]
    private Animator m_animator;
    public Animator @Animator { get { return m_animator; } }

    public Vector3 LookDirection { get; private set; }

    public void LookAt(Vector2 position)
    {
        Vector3 lookDir = new Vector3(position.x, position.y) - m_handPivot.position;
        lookDir.Normalize();

        LookDirection = lookDir;

        float rot_z = Utils.VectorToAngle2D(lookDir);
        m_handPivot.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);

        Vector3 scale = m_spriteTransform.localScale;
        scale.x = transform.position.x > position.x ? -1 : 1;
        m_spriteTransform.localScale = scale;
    }

    public void Move(Vector2 direction)
    {
        m_animator.SetBool("Walking", Mathf.Abs(direction.x) > 0.5f);
    }
}
