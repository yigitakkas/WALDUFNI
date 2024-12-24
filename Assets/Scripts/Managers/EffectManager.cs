using System;
using System.Collections;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance;

    public ParticleSystem explosionEffect;
    public ParticleSystem boostEffect;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayExplosionEffect(Vector3 position, float duration)
    {
        ParticleSystem effect = Instantiate(explosionEffect, position, Quaternion.identity);
        effect.Play();
        Destroy(effect.gameObject, effect.main.duration + effect.main.startLifetime.constantMax);
    }

    public void PlayBoostEffect(Vector3 startPosition, Vector3 targetPosition, float duration, Action onComplete = null)
    {
        ParticleSystem effect = Instantiate(boostEffect, startPosition, Quaternion.identity);
        effect.Play();
        StartCoroutine(MoveEffect(effect, startPosition, targetPosition, duration, onComplete));
    }

    private IEnumerator MoveEffect(ParticleSystem effect, Vector3 startPosition, Vector3 targetPosition, float duration, Action onComplete)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            effect.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(effect.gameObject, effect.main.duration + effect.main.startLifetime.constantMax);
        onComplete?.Invoke();
    }
}
