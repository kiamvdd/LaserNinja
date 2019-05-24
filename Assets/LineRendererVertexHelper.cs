using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// takes in an array of transforms and uses them for the vertices of the linerenderer
[ExecuteInEditMode]
public class LineRendererVertexHelper : MonoBehaviour
{
    [SerializeField]
    private Transform[] m_transforms;
    [SerializeField]
    private LineRenderer m_lineRenderer;

    private void Update()
    {
        m_lineRenderer.positionCount = m_transforms.Length;
        for (int i = 0; i < m_transforms.Length; i++)
        {
            m_lineRenderer.SetPosition(i, m_transforms[i].position);
        }
    }
}
