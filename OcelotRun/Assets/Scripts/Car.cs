﻿using UnityEngine;
using System.Collections;

public class Car : MonoBehaviour
{
    public float Speed;
    public Transform Origin;
    public Transform Destination;

    public Sprite[] Sprites;
    
    private Collider2D Collider;

    private float DeltaTime;
    private SpriteRenderer Renderer;

    public void Init(Transform origin, Transform destination, float speed)
    {
        Origin = origin;
        Destination = destination;
        Speed = speed;

        Renderer = GetComponentInChildren<SpriteRenderer>();
        Renderer.sprite = Sprites[(int)Random.Range(0f, Sprites.Length - 0.0001f)];
        Renderer.sortingOrder = -100;
        Renderer.enabled = false;

        Collider = GetComponentInChildren<Collider2D>();
        Collider.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Not sure why this needs to be here.  The cars always flash one frame full size even though init happens before start.
        Renderer.enabled = true;

        DeltaTime += Time.deltaTime * Speed;
        float factor = DeltaTime / Speed;

        // Origin will equal null when the road gets destroyed with a car still on it.
        if (factor > 1f || Origin == null)
        {
            Destroy(gameObject);
            return;
        }
        else if (factor > 0.75f)
        {
            Collider.enabled = true;
        }

        transform.position = Vector3.Lerp(Origin.position, Destination.position, factor);
        transform.localScale = Vector3.one * factor;
        Renderer.sortingOrder = -100 + (int)(100f * factor);
    }
}