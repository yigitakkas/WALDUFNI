using UnityEngine;
using DG.Tweening;

public partial class Card
{

    private void UpdatePlayed()
    {
        if (_playedUpdated) return;
        if (_placedOnArea && PlacedOpponentArea == null)
        {
            Played = true;
            _playedUpdated = true;
        }
        _roundPlaying = true;
    }

    public void SetOpponentArea(PlayArea area)
    {
        _placedOpponentArea = area;
    }

    public void SetPlayerArea(PlayArea area)
    {
        _placedArea = area;
    }

    public void SetOriginalPosition(Vector3 position)
    {
        _originalPosition = position;
    }

    private void RoundEnd()
    {
        _roundPlaying = false;
        KillTweens();
        StartIdleMovement();
    }
}
