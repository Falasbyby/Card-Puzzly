using UnityEngine;
using UnityEngine.UI;

public class SoundManager : Singleton<SoundManager>
{
    [Header("Game Sounds")]
    [SerializeField] private AudioClip cardTransitionSound;
    [SerializeField] private AudioClip effectConfettiSound;
    [SerializeField] private AudioClip victorySound;
    [SerializeField] private AudioClip loseSound;

    [Header("Audio Source")]
    [SerializeField] private AudioSource gameAudioSource;

    [Header("Volume Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float gameVolume = 1f;

    [Header("Sound Toggle")]
    [SerializeField] private bool soundEnabled = true;
    [SerializeField] private Toggle soundToggle;
    [SerializeField] private Image toggleImage;
    [SerializeField] private Sprite soundOnSprite;
    [SerializeField] private Sprite soundOffSprite;
    private const string SOUND_ENABLED_KEY = "SoundEnabled";

    private void Start()
    {
        soundEnabled = PlayerPrefs.GetInt(SOUND_ENABLED_KEY, 1) == 1;

        if (soundToggle != null)
        {
            soundToggle.isOn = soundEnabled;
            soundToggle.onValueChanged.AddListener(OnToggleValueChanged);
            UpdateToggleSprite(soundEnabled);
        }

        if (gameAudioSource == null)
        {
            gameAudioSource = gameObject.AddComponent<AudioSource>();
            gameAudioSource.playOnAwake = false;
        }
        gameAudioSource.volume = gameVolume;
    }

    private void OnToggleValueChanged(bool isOn)
    {
        soundEnabled = isOn;
        UpdateToggleSprite(isOn);
        PlayerPrefs.SetInt(SOUND_ENABLED_KEY, soundEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }
    public void PlayEffectConfetti()
    {
        gameVolume = 1f;
        PlayGameSound(effectConfettiSound);
    }
    private void UpdateToggleSprite(bool isOn)
    {
        if (toggleImage != null)
        {
            toggleImage.sprite = isOn ? soundOnSprite : soundOffSprite;
        }
    }

    public void PlayCardTransition()
    {
        gameVolume = 1;
        PlayGameSound(cardTransitionSound);
    }

    public void PlayVictory()
    {
        gameVolume = 0.5f;
        PlayGameSound(victorySound);
    }

    public void PlayLose()
    {
        PlayGameSound(loseSound);
    }

    private void PlayGameSound(AudioClip clip)
    {
        if (clip != null && soundEnabled)
        {
            gameAudioSource.PlayOneShot(clip, gameVolume);
        }
    }

    public void SetGameVolume(float volume)
    {
        gameVolume = Mathf.Clamp01(volume);
        gameAudioSource.volume = gameVolume;
    }

    public bool IsSoundEnabled()
    {
        return soundEnabled;
    }
}
