using UnityEngine;
using System.Collections;

public class TitleScreen : MonoBehaviour
{
    public float PlantStart;
    public float PlantDistance;
    public float PlantDuration;
    public GameObject Plants;

    public float HeadStart;
    public float HeadDistance;
    public float HeadDuration;
    public GameObject Head;
    public float StartTransitionTime = 5f;

    public float HeadStartGameDistance;
    public float HeadStartGameDuration;

    private bool StartMode = false;

    private float ElapsedTime = 0f;
    private float StartElapsedTime = 0f;
    


    // Use this for initialization
    void Start()
    {
        SoundManager.Instance.PlayMusic(SoundManager.MUSIC.TitleScreen);
        SoundManager.Instance.Play(SoundManager.SOUND.Snarl);
    }

    // Update is called once per frame
    void Update()
    {
        ElapsedTime += Time.deltaTime;
        float plantsY = EaseOutQuad(PlantStart, PlantDistance, PlantDuration, ElapsedTime);
        Plants.transform.localPosition = new Vector3(Plants.transform.localPosition.x, plantsY, Plants.transform.localPosition.z);

        if (!StartMode)
        {
            float headX = EaseOutQuart(HeadStart, HeadDistance, HeadDuration, ElapsedTime);
            Head.transform.localPosition = new Vector3(headX, Head.transform.localPosition.y, Head.transform.localPosition.z);
        }
        else
        {
            StartElapsedTime += Time.deltaTime;
            float headX = EaseInCubic(HeadStart, HeadStartGameDistance, HeadStartGameDuration, StartElapsedTime);
            Head.transform.localPosition = new Vector3(headX, Head.transform.localPosition.y, Head.transform.localPosition.z);
            if (StartElapsedTime > StartTransitionTime)
            {
                Application.LoadLevel("GameScene");
            }
        }
    }

    public void Play()
    {
        StartMode = true;
        HeadStart = Head.transform.localPosition.x;
        HeadStartGameDistance = HeadStartGameDistance + HeadDistance - HeadStart;
        SoundManager.Instance.Play(SoundManager.SOUND.Conga);
    }

    public void Options()
    {
        Debug.Log("Options");
    }

    public void Quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    float EaseOutQuad(float start, float distance, float duration, float elapsedTime)
    {
        elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
        return -distance * elapsedTime * (elapsedTime - 2) + start;
    }

    float EaseOutQuart(float start, float distance, float duration, float elapsedTime)
    {
        elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
        elapsedTime--;
        return -distance * (elapsedTime * elapsedTime * elapsedTime * elapsedTime - 1) + start;
    }

    static float EaseInCubic(float start, float distance,  float duration, float elapsedTime)
    {
        elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
        return distance * elapsedTime * elapsedTime * elapsedTime + start;
    }
}
