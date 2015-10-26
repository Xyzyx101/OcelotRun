using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public enum SOUND
    {
        Falling,
        Pain,
        Conga,
        Landing,
        Meow,
        Snarl,
        Step,
        Vine,
        WoodClick,
        WoodToggle
    }

    public enum MUSIC
    {
        TitleScreen,
        LevelMusic,
    }

    public AudioClip TitleScreenMusic;
    public AudioClip[] LevelMusic;
    public AudioClip[] Falling;
    public AudioClip[] Pain;
    public AudioClip Conga;
    public AudioClip[] Landing;
    public AudioClip[] Meow;
    public AudioClip Snarl;
    public AudioClip[] Step;
    public AudioClip[] Vine;
    public AudioClip WoodClick;
    public AudioClip WoodToggle;
    
    public static SoundManager Instance { get; private set; }

    private AudioSource MusicSource;
    private AudioSource SFXSource;
    private AudioSource FootStepSource;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;

        MusicSource = gameObject.AddComponent<AudioSource>();
        MusicSource.volume = 0.6f;
        SFXSource = gameObject.AddComponent<AudioSource>();
        FootStepSource = gameObject.AddComponent<AudioSource>();
        FootStepSource.volume = 0.4f;
    }

    void Start()
    {

    }

    public void Play(SOUND sound)
    {
        switch (sound)
        {
            case SOUND.Falling:
                SFXSource.clip = Falling[(int)Random.Range(0f, Falling.Length - 0.00001f)];
                SFXSource.Play();
                break;
            case SOUND.Pain:
                SFXSource.clip = Pain[(int)Random.Range(0f, Pain.Length - 0.00001f)];
                SFXSource.Play();
                break;
            case SOUND.Conga:
                SFXSource.clip = Conga;
                SFXSource.Play();
                break;
            case SOUND.Landing:
                SFXSource.clip = Landing[(int)Random.Range(0f, Landing.Length - 0.00001f)];
                SFXSource.Play();
                break;
            case SOUND.Meow:
                // Meow should only play occasionally
                if(Random.value<0.3)
                {
                    SFXSource.clip = Meow[(int)Random.Range(0f, Meow.Length - 0.00001f)];
                    SFXSource.Play();
                }
                break;
            case SOUND.Snarl:
                SFXSource.clip = Snarl;
                SFXSource.Play();
                break;
            case SOUND.Step:
                if (Random.value < 0.5)
                {
                    FootStepSource.clip = Step[(int)Random.Range(0f, Step.Length - 0.00001f)];
                    FootStepSource.Play();
                }
                break;
            case SOUND.Vine:
                SFXSource.clip = Vine[(int)Random.Range(0f, Vine.Length - 0.00001f)];
                SFXSource.Play();
                break;
            case SOUND.WoodClick:
                SFXSource.clip = WoodClick;
                SFXSource.Play();
                break;
            case SOUND.WoodToggle:
                SFXSource.clip = WoodToggle;
                SFXSource.Play();
                break;
        }
    }

    public void PlayMusic(MUSIC music)
    {
        if (MusicSource.isPlaying)
        {
            MusicSource.Stop();
        }
        switch (music)
        {
            case MUSIC.TitleScreen:
                MusicSource.clip = TitleScreenMusic;
                MusicSource.Play();
                break;
            case MUSIC.LevelMusic:
                MusicSource.clip = LevelMusic[(int)Random.Range(0f, LevelMusic.Length-0.00001f)];
                MusicSource.Play();
                break;
        }
    }
}
