using UnityEngine;
using System.Collections;

public class ParallaxChunk : MonoBehaviour
{

    public PlayerController PlayerController;
    public GameObject[] Sprites;
    public float MinCount;
    public float MaxCount;
    public float DistanceFraction;

    private const float BaseChunkWidth = 12.80f;
    private float ChunkWidth;

    // Use this for initialization
    void Start()
    {
        PlayerController = FindObjectOfType<PlayerController>();
        ChunkWidth = BaseChunkWidth * DistanceFraction;
        int spriteCount = (int)Random.Range(MinCount, MaxCount);
        for (int i = 0; i < spriteCount; ++i)
        {
            float xPos = Random.Range(0f, ChunkWidth);
            int spriteIndex = (int)Random.Range(0f, Sprites.Length - 0.0001f);
            GameObject newSprite = (GameObject)Instantiate(Sprites[spriteIndex], new Vector3(transform.position.x + xPos, transform.position.y, transform.position.z), Quaternion.identity);
            newSprite.transform.parent = transform;
            float xScale = Random.Range(0.85f, 1.15f);
            if(Random.value<0.5)
            {
                xScale *= -1f;
            }
            float yScale = Random.Range(0.85f, 1.15f);
            newSprite.transform.localScale = new Vector3(xScale, yScale, 1f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x < (-BaseChunkWidth * 3))
        {
            Destroy(gameObject);
        }

        float moveAmount = PlayerController.GetSpeed()* DistanceFraction * Time.deltaTime;
        Vector3 pos = transform.localPosition;
        transform.localPosition = new Vector3(pos.x - moveAmount, pos.y, pos.z);
    }
}
