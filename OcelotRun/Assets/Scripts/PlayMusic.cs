using UnityEngine;
using System.Collections;

public class PlayMusic : MonoBehaviour {
    float StartMusicTimer = 3f;

	// Use this for initialization
	void Start () {
        SoundManager.Instance.Play(SoundManager.SOUND.Conga);
    }

    void Update ()
    {
        StartMusicTimer -= Time.deltaTime;
        if (StartMusicTimer<0)
        {
            SoundManager.Instance.PlayMusic(SoundManager.MUSIC.LevelMusic);
            Destroy(this);
        }
    }
}
