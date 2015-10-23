using UnityEngine;
using System.Collections;

public class VineSection : MonoBehaviour {
    public Transform Root;

    void Start()
    {
        Transform sprite = GetComponentInChildren<SpriteRenderer>().transform;
        float scaleX = 1.0f, scaleY = 1.0f;
        if (Random.value<0.5)
        {
            scaleX *= -1.0f;
        }
        if (Random.value<0.5f)
        {
            scaleY *= -1.0f;
        }
        sprite.localScale = new Vector3(scaleX, scaleY, 1.0f);
    }
}
