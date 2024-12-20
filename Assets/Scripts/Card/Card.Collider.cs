using UnityEngine;
using DG.Tweening;

public partial class Card
{
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Mathf.Abs(_mainCamera.transform.position.z - transform.position.z);
        return _mainCamera.ScreenToWorldPoint(mousePosition);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out PlayArea playArea))
        {
            _currentPlayArea = playArea;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out PlayArea playArea) && _currentPlayArea == playArea)
        {
            _currentPlayArea = null;
        }
    }
}
