using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioSource AudioSource;
    public AudioSource SfxSource;
    public AudioSource ClickSource;

    public AudioClip ButtonClickSound;

    public AudioClip MainMenuMusic;
    public List<AudioClip> GameMusics;

    public AudioClip CardPlayedSound;
    public AudioClip CardPickupSound;
    public AudioClip CardErrorSound;
    public AudioClip CardDrawSound;
    public AudioClip CardUpgradeSound;

    public AudioClip PlayerWonSound;
    public AudioClip OpponentWonSound;



    private int _lastPlayedIndex = -1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        PlayMusic(MainMenuMusic, true);
    }

    public void PlayMusic(AudioClip musicClip, bool loop = true)
    {
        AudioSource.clip = musicClip;
        AudioSource.loop = loop;
        AudioSource.Play();
    }

    public void StopMusic(float fadeDuration)
    {
        StartCoroutine(FadeOutMusic(fadeDuration));
    }
    public void PlaySFX(AudioClip sfxClip)
    {
        SfxSource.PlayOneShot(sfxClip);
    }

    public void PlayRandomGameMusic()
    {
        if (GameMusics != null && GameMusics.Count > 0)
        {
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, GameMusics.Count);
            }
            while (randomIndex == _lastPlayedIndex);

            _lastPlayedIndex = randomIndex;

            AudioClip randomMusic = GameMusics[randomIndex];
            SoundManager.Instance.CrossfadeMusic(randomMusic);
        }
        else
        {
            Debug.LogWarning("GameMusics listesi boþ veya atanmadý!");
        }
    }

    public void CrossfadeMusic(AudioClip newClip, float fadeDuration = 1f)
    {
        StartCoroutine(FadeOutAndIn(newClip, fadeDuration));
    }

    private IEnumerator FadeOutAndIn(AudioClip newClip, float duration)
    {
        float startVolume = AudioSource.volume;

        while (AudioSource.volume > 0)
        {
            AudioSource.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }

        AudioSource.Stop();
        AudioSource.clip = newClip;
        AudioSource.Play();

        while (AudioSource.volume < startVolume)
        {
            AudioSource.volume += startVolume * Time.deltaTime / duration;
            yield return null;
        }
    }

    private IEnumerator FadeOutMusic(float duration)
    {
        float startVolume = AudioSource.volume;
        while (AudioSource.volume > 0)
        {
            AudioSource.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }

        AudioSource.Stop();
        AudioSource.volume = startVolume; 
    }

    public void ButtonClick()
    {
        ClickSource.PlayOneShot(ButtonClickSound);
    }
}
