using UnityEngine;
using DG.Tweening;

public class PlayArea : MonoBehaviour
{
    public Transform SnapPoint;

    public bool IsPointInPlayArea(Vector3 point)
    {
        Collider2D collider = GetComponent<Collider2D>();
        return collider != null && collider.bounds.Contains(point);
    }

    public Vector3 GetSnapPosition()
    {
        return SnapPoint != null ? SnapPoint.position : transform.position;
    }
}
