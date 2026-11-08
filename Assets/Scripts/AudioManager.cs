
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Background Music")]
    public AudioClip backgroundMusic;

    [Header("Sound Effects")]
    public AudioClip pickUpSound;
    public AudioClip placeSound;
    public AudioClip invalidSound;
    public AudioClip clearSound;
    public AudioClip comboSound;
    public AudioClip gameOverSound;
    public AudioClip buttonClickSound;

    [Header("Volume")]
    [Range(0f, 1f)]
    public float musicVolume = 0.5f;

    [Range(0f, 1f)]
    public float sfxVolume = 0.8f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (musicSource != null)
        {
            musicSource.loop = true;
            musicSource.playOnAwake = false;
            musicSource.volume = musicVolume;

            if (backgroundMusic != null)
            {
                musicSource.clip = backgroundMusic;
                musicSource.Play();
            }
        }

        if (sfxSource != null)
        {
            sfxSource.playOnAwake = false;
            sfxSource.volume = sfxVolume;
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip, sfxVolume);
        }
    }

    public void PlayPickUp()
    {
        PlaySound(pickUpSound);
    }

    public void PlayPlace()
    {
        PlaySound(placeSound);
    }

    public void PlayInvalid()
    {
        PlaySound(invalidSound);
    }

    public void PlayClear()
    {
        PlaySound(clearSound);
    }

    public void PlayCombo()
    {
        PlaySound(comboSound);
    }

    public void PlayGameOver()
    {
        PlaySound(gameOverSound);
    }

    public void PlayButtonClick()
    {
        PlaySound(buttonClickSound);
    }
}
