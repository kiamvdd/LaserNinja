using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class TileWallCollider : MonoBehaviour
{
    private bool m_selected = false;
    private BoxCollider2D m_boxCollider;
    private double m_timeLastUpdated = 0;

    private void Update()
    {
        if (!Application.isPlaying) {
            if (EditorApplication.timeSinceStartup - m_timeLastUpdated > 1) {
                UpdateColliderToChildren();
                m_timeLastUpdated = EditorApplication.timeSinceStartup;
            }
        }
    }

    private void UpdateColliderToChildren()
    {
        if (transform.childCount == 0)
            return;

        if (m_boxCollider == null) {
            m_boxCollider = GetComponent<BoxCollider2D>();

            if (m_boxCollider == null) {
                m_boxCollider = gameObject.AddComponent<BoxCollider2D>();

                if (m_boxCollider == null) {
                    enabled = false;
                    return;
                }
            }
        }

        float maxX = float.MinValue;
        float maxY = float.MinValue;
        float minX = float.MaxValue;
        float minY = float.MaxValue;

        foreach (Transform t in transform) {
            SpriteRenderer s = t.gameObject.GetComponent<SpriteRenderer>();
            if (s == null)
                continue;

            float leftX = t.position.x - s.bounds.extents.x;
            float rightX = t.position.x + s.bounds.extents.x;
            float topY = t.position.y + s.bounds.extents.y;
            float bottomY = t.position.y - s.bounds.extents.y;

            if (leftX < minX)
                minX = leftX;

            if (rightX > maxX)
                maxX = rightX;

            if (topY > maxY)
                maxY = topY;

            if (bottomY < minY)
                minY = bottomY;
        }

        Vector3 topLeft = new Vector3(minX, maxY);
        Vector3 topRight = new Vector3(maxX, maxY);
        Vector3 bottomLeft = new Vector3(minX, minY);
        Vector3 bottomRight = new Vector3(maxX, minY);

        Vector3 center = topLeft + topRight + bottomLeft + bottomRight;
        center /= 4.0f;

        float width = Mathf.Abs(maxX - minX);
        float height = Mathf.Abs(maxY - minY);

        m_boxCollider.size = new Vector2(width, height);
        m_boxCollider.offset = center - transform.position;

        Debug.DrawLine(topLeft, topRight, Color.red, 1);
        Debug.DrawLine(topRight, bottomRight, Color.red, 1);
        Debug.DrawLine(bottomRight, bottomLeft, Color.red, 1);
        Debug.DrawLine(bottomLeft, topLeft, Color.red, 1);
    }
}
