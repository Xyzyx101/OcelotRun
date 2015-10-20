using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GroundGenerator : MonoBehaviour
{
    public List<Chunk> Chunks;
    public TileWidget TileWidget;
    public float MoveSpeed;
    private float lastHeight = 0;

    void Start()
    {
        int seed = (int)Time.realtimeSinceStartup * 1000;
        Debug.LogFormat("Seed:{0}", seed);
        Random.seed = seed;
        Chunks = new List<Chunk>();

        Chunk newChunk = new Chunk();
        newChunk.Init(TileWidget, Chunk.ChunkWidth * 0.5f, 0.0f);
        lastHeight = lastHeight + newChunk.GetEndHeight();
        Chunks.Add(newChunk);
    }

    // Update is called once per frame
    void Update()
    {
        Chunk firstChunk = Chunks[0];
        if (firstChunk.GetPos().x < (-Chunk.ChunkWidth * 2))
        {
            Chunks.Remove(firstChunk);
            firstChunk.Destroy();
        }

        float moveAmount = MoveSpeed * Time.deltaTime;
        foreach (Chunk chunk in Chunks)
        {
            chunk.Move(moveAmount);
        }

        Chunk lastChunk = Chunks[Chunks.Count - 1];
        float debug = lastChunk.GetPos().x;
        if (lastChunk.GetPos().x < Chunk.ChunkWidth)
        {
            Chunk newChunk = new Chunk();
            newChunk.Init(TileWidget, lastChunk.GetPos().x + Chunk.ChunkWidth, lastHeight);
            lastHeight = lastHeight + newChunk.GetEndHeight();
            Chunks.Add(newChunk);
        }
    }
}

[System.Serializable]
public class Chunk
{
    public const float ChunkWidth = 12.80f;
    public float height;
    public float offsetX;
    private GameObject parent = new GameObject();
    private const float tileSize = 0.64f;
    private int tilesRemaining;
    private int totalTiles;
    private TileWidget tileWidget;

    public void Init(TileWidget tileWidget, float xOffest, float height)
    {
        this.tileWidget = tileWidget;
        parent.transform.position = new Vector3(xOffest, height, 0.0f);
        tilesRemaining = (int)(ChunkWidth / tileSize);
        totalTiles = (int)(ChunkWidth / tileSize);
        Generate();
    }

    public void Move(float amount)
    {
        Vector3 pos = parent.transform.localPosition;
        parent.transform.localPosition = new Vector3(pos.x - amount, pos.y, pos.z);
    }

    public Vector3 GetPos()
    {
        return parent.transform.position;
    }

    public float GetEndHeight()
    {
        return height;
    }

    public void Destroy()
    {
        foreach (Transform child in parent.transform)
        {
            UnityEngine.Object.Destroy(child.gameObject);
        }
        UnityEngine.Object.Destroy(parent);
    }

    private ChunkType GetNewChunkType()
    {
        float rand = Random.value;
        if (rand < 0.3f)
        {
            return ChunkType.Flat;
        }
        else if (rand < 0.62f)
        {
            return ChunkType.Hilly;
        }
        else if (rand < 0.85f)
        {
            return ChunkType.SmallHole;
        }
        else
        {
            return ChunkType.BigHole;
        }
    }

    private void Generate()
    {
        ChunkType chunkType = GetNewChunkType();
        switch (chunkType)
        {
            case ChunkType.Flat:
                Debug.Log("Generate Flat");
                GenerateFlat();
                break;
            case ChunkType.Hilly:
                Debug.Log("Generate Hilly");
                GenerateHilly();
                break;
            case ChunkType.SmallHole:
                Debug.Log("Generate Small Hole");
                GenerateSmallHole();
                break;
            case ChunkType.BigHole:
                Debug.Log("Generate Big Hole");
                GenerateBigHole();
                break;
        }
    }

    // Get section width will guarantee you are never left with 1 odd tile remaining
    private int GetSectionWidth(int tilesRemaining)
    {
        if (tilesRemaining < 4)
        {
            return tilesRemaining;
        }
        else if (tilesRemaining == 4)
        {
            if (Random.value < 0.5f)
            {
                return 2;
            }
            else
            {
                return 4;
            }
        }
        else if (tilesRemaining == 5)
        {
            if (Random.value < 0.5f)
            {
                return 3;
            }
            else
            {
                return 2;
            }
        }
        else
        {
            float val = Random.value;
            if (val < 0.4f)
            {
                return 2;
            }
            else if (val < 0.7f)
            {
                return 3;
            }
            else
            {
                return 4;
            }
        }
    }

    private void GenerateFlat()
    {
        while (tilesRemaining > 0)
        {
            int sectionWidth = GetSectionWidth(tilesRemaining);
            GameObject section = null;
            Vector3 sectionPos = Vector3.zero;
            Vector3 sectionScale = Vector3.one;

            if (Random.value < 0.75f)
            {
                // Flat section
                switch (sectionWidth)
                {
                    case 2:
                        section = tileWidget.GetFlat2();
                        break;
                    case 3:
                        section = tileWidget.GetFlat3();
                        break;
                    case 4:
                        section = tileWidget.GetFlat4();
                        break;
                }
                if (Random.value < 0.5)
                {
                    sectionPos = parent.transform.TransformPoint(new Vector3((float)(totalTiles - tilesRemaining) * tileSize, height, 0.0f));
                }
                else
                {
                    sectionPos = parent.transform.TransformPoint(new Vector3((float)(totalTiles - tilesRemaining + sectionWidth) * tileSize, height, 0.0f));
                    sectionScale = Vector3.Scale(sectionScale, new Vector3(-1.0f, 1.0f, 1.0f));
                }
            }
            else
            {
                // Slope Section
                switch (sectionWidth)
                {
                    case 2:
                        section = tileWidget.GetSlope2();
                        break;
                    case 3:
                        section = tileWidget.GetSlope3();
                        break;
                    case 4:
                        section = tileWidget.GetSlope4();
                        break;
                }
                if (Random.value < 0.5) // Flip
                {
                    sectionPos = parent.transform.TransformPoint(new Vector3((float)(totalTiles - tilesRemaining) * tileSize, height, 0.0f));
                    height -= tileSize;
                }
                else
                {
                    sectionPos = parent.transform.TransformPoint(new Vector3((float)(totalTiles - tilesRemaining + sectionWidth) * tileSize, height + 0.64f, 0.0f));
                    sectionScale = Vector3.Scale(sectionScale, new Vector3(-1.0f, 1.0f, 1.0f));
                    height += tileSize;
                }
            }

            // Create section
            GameObject newSection = (GameObject)UnityEngine.Object.Instantiate(section, sectionPos, Quaternion.identity);
            newSection.transform.parent = parent.transform;
            newSection.transform.localScale = sectionScale;
            tilesRemaining -= sectionWidth;
        }
    }

    private void GenerateHilly()
    {
        while (tilesRemaining > 0)
        {
            int sectionWidth = GetSectionWidth(tilesRemaining);
            GameObject section = null;
            Vector3 sectionPos = Vector3.zero;
            Vector3 sectionScale = Vector3.one;

            // Slope Section
            switch (sectionWidth)
            {
                case 2:
                    section = tileWidget.GetSlope2();
                    break;
                case 3:
                    section = tileWidget.GetSlope3();
                    break;
                case 4:
                    section = tileWidget.GetSlope4();
                    break;
            }
            if (Random.value < 0.5) // Flip
            {
                sectionPos = parent.transform.TransformPoint(new Vector3((float)(totalTiles - tilesRemaining) * tileSize, height, 0.0f));
                height -= tileSize;
            }
            else
            {
                sectionPos = parent.transform.TransformPoint(new Vector3((float)(totalTiles - tilesRemaining + sectionWidth) * tileSize, height + 0.64f, 0.0f));
                sectionScale = Vector3.Scale(sectionScale, new Vector3(-1.0f, 1.0f, 1.0f));
                height += tileSize;
            }

            // Create section
            GameObject newSection = (GameObject)UnityEngine.Object.Instantiate(section, sectionPos, Quaternion.identity);
            newSection.transform.parent = parent.transform;
            newSection.transform.localScale = sectionScale;
            tilesRemaining -= sectionWidth;
        }
    }

    private void GenerateSmallHole()
    {
        while (tilesRemaining > 0)
        {
            int sectionWidth = GetSectionWidth(tilesRemaining);
            GameObject section = null;
            Vector3 sectionPos = Vector3.zero;
            Vector3 sectionScale = Vector3.one;

            int holeSize = (int)Random.Range(4.0f, 7.0f);
            if (holeSize + 4 < tilesRemaining && Random.value > 0.5f)  // if the whole fits add it maybe
            {
                // cap the previous section
                GameObject endSection = tileWidget.GetEnd();
                sectionPos = parent.transform.TransformPoint(new Vector3((float)(totalTiles - tilesRemaining) * tileSize, height, 0.0f));
                GameObject newSection = (GameObject)UnityEngine.Object.Instantiate(endSection, sectionPos, Quaternion.identity);
                newSection.transform.parent = parent.transform;

                // Make the actual hole
                tilesRemaining -= holeSize - 1;
                height += (int)Random.Range(-1.0f, 3.0f);

                // cap the next section
                GameObject startSection = tileWidget.GetEnd();
                sectionPos = parent.transform.TransformPoint(new Vector3((float)(totalTiles - tilesRemaining + 1 /*plus one because it is reversed*/ ) * tileSize, height, 0.0f));
                newSection = (GameObject)UnityEngine.Object.Instantiate(startSection, sectionPos, Quaternion.identity);
                newSection.transform.parent = parent.transform;
                newSection.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
                tilesRemaining -= 1;
            }
            else // else add another slope piece
            {
                // Slope Section
                switch (sectionWidth)
                {
                    case 2:
                        section = tileWidget.GetSlope2();
                        break;
                    case 3:
                        section = tileWidget.GetSlope3();
                        break;
                    case 4:
                        section = tileWidget.GetSlope4();
                        break;
                }
                if (Random.value < 0.5) // Flip
                {
                    sectionPos = parent.transform.TransformPoint(new Vector3((float)(totalTiles - tilesRemaining) * tileSize, height, 0.0f));
                    height -= tileSize;
                }
                else
                {
                    sectionPos = parent.transform.TransformPoint(new Vector3((float)(totalTiles - tilesRemaining + sectionWidth) * tileSize, height + 0.64f, 0.0f));
                    sectionScale = Vector3.Scale(sectionScale, new Vector3(-1.0f, 1.0f, 1.0f));
                    height += tileSize;
                }

                // Create section
                GameObject newSection = (GameObject)UnityEngine.Object.Instantiate(section, sectionPos, Quaternion.identity);
                newSection.transform.parent = parent.transform;
                newSection.transform.localScale = sectionScale;
                tilesRemaining -= sectionWidth;
            }
        }
    }

    private void GenerateBigHole()
    {
        while (tilesRemaining > 0)
        {
            int sectionWidth = GetSectionWidth(tilesRemaining);
            GameObject section = null;
            Vector3 sectionPos = Vector3.zero;
            Vector3 sectionScale = Vector3.one;

            int holeSize = (int)Random.Range(7.0f, 14.0f);
            if (holeSize + 4 < tilesRemaining && Random.value > 0.5f)  // if the whole fits add it maybe
            {
                // cap the previous section
                GameObject endSection = tileWidget.GetEnd();
                sectionPos = parent.transform.TransformPoint(new Vector3((float)(totalTiles - tilesRemaining) * tileSize, height, 0.0f));
                GameObject newSection = (GameObject)UnityEngine.Object.Instantiate(endSection, sectionPos, Quaternion.identity);
                newSection.transform.parent = parent.transform;

                // Make the actual hole
                tilesRemaining -= holeSize - 1;
                height += (int)Random.Range(-3.0f, 2.0f);

                // cap the next section
                GameObject startSection = tileWidget.GetEnd();
                sectionPos = parent.transform.TransformPoint(new Vector3((float)(totalTiles - tilesRemaining + 1 /*plus one because it is reversed*/ ) * tileSize, height, 0.0f));
                newSection = (GameObject)UnityEngine.Object.Instantiate(startSection, sectionPos, Quaternion.identity);
                newSection.transform.parent = parent.transform;
                newSection.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
                tilesRemaining -= 1;
            }
            else // else add another slope piece
            {
                // Slope Section
                switch (sectionWidth)
                {
                    case 2:
                        section = tileWidget.GetSlope2();
                        break;
                    case 3:
                        section = tileWidget.GetSlope3();
                        break;
                    case 4:
                        section = tileWidget.GetSlope4();
                        break;
                }
                if (Random.value < 0.5) // Flip
                {
                    sectionPos = parent.transform.TransformPoint(new Vector3((float)(totalTiles - tilesRemaining) * tileSize, height, 0.0f));
                    height -= tileSize;
                }
                else
                {
                    sectionPos = parent.transform.TransformPoint(new Vector3((float)(totalTiles - tilesRemaining + sectionWidth) * tileSize, height + 0.64f, 0.0f));
                    sectionScale = Vector3.Scale(sectionScale, new Vector3(-1.0f, 1.0f, 1.0f));
                    height += tileSize;
                }

                // Create section
                GameObject newSection = (GameObject)UnityEngine.Object.Instantiate(section, sectionPos, Quaternion.identity);
                newSection.transform.parent = parent.transform;
                newSection.transform.localScale = sectionScale;
                tilesRemaining -= sectionWidth;
            }
        }
    }
}

enum ChunkType
{
    Flat,
    Hilly,
    BigHole,
    SmallHole
}
