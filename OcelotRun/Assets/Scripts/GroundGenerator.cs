using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GroundGenerator : MonoBehaviour
{
    public PlayerController PlayerController;
    public List<Chunk> Chunks;
    public TileWidget TileWidget;
    public float MoveSpeed;
    public GameObject[] InitialGround;
    public GameObject[] DirtPrefabs;

    public static GameObject Dirt1x;
    public static GameObject Dirt2x;

    private float lastHeight = 0;
    static public VinePool VinePool;

    void Start()
    {
        int seed = (int)(Time.realtimeSinceStartup * 1000);
        Debug.LogFormat("Seed:{0}", seed);
        Random.seed = seed;
        Chunks = new List<Chunk>();

        foreach (GameObject initialGroundChunk in InitialGround)
        {
            Chunk newChunk = new Chunk();
            newChunk.InitWithKnownChunk(initialGroundChunk);
            Chunks.Add(newChunk);
        }
        lastHeight = 0;

        Dirt1x = DirtPrefabs[0];
        Dirt2x = DirtPrefabs[1];

        GameObject vinePoolObj = (GameObject)GameObject.FindGameObjectWithTag("VinePool");
        VinePool = vinePoolObj.GetComponent<VinePool>();
    }

    void Update()
    {
        MoveSpeed = PlayerController.GetSpeed();
        Chunk firstChunk = Chunks[0];
        if (firstChunk.GetPos().x < (-Chunk.ChunkWidth * 3))
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

    public void InitWithKnownChunk(GameObject initialGround)
    {
        parent.transform.position = initialGround.transform.position;
        initialGround.transform.parent = parent.transform;
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
            if (child.tag == "VineRoot")
            {
                child.transform.parent = GroundGenerator.VinePool.transform;
            }
            else
            {
                UnityEngine.Object.Destroy(child.gameObject);
            }

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
                //Debug.Log("Generate Flat");
                GenerateFlat();
                break;
            case ChunkType.Hilly:
                //Debug.Log("Generate Hilly");
                GenerateHilly();
                break;
            case ChunkType.SmallHole:
                //Debug.Log("Generate Small Hole");
                GenerateSmallHole();
                break;
            case ChunkType.BigHole:
                //Debug.Log("Generate Big Hole");
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
                CreateDirtFlat((float)(totalTiles - tilesRemaining) * tileSize, height, sectionWidth);
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
                    CreateDirtDown((float)(totalTiles - tilesRemaining) * tileSize, height, sectionWidth);
                    height -= tileSize;
                }
                else
                {
                    sectionPos = parent.transform.TransformPoint(new Vector3((float)(totalTiles - tilesRemaining + sectionWidth) * tileSize, height + 0.64f, 0.0f));
                    CreateDirtUp((float)(totalTiles - tilesRemaining) * tileSize, height, sectionWidth);
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
        GenerateVines((int)(Random.value * 4) + 2, parent.transform.position.x, parent.transform.position.y, (float)totalTiles * tileSize);
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
                CreateDirtDown((float)(totalTiles - tilesRemaining) * tileSize, height, sectionWidth);
                height -= tileSize;
            }
            else
            {
                sectionPos = parent.transform.TransformPoint(new Vector3((float)(totalTiles - tilesRemaining + sectionWidth) * tileSize, height + 0.64f, 0.0f));
                sectionScale = Vector3.Scale(sectionScale, new Vector3(-1.0f, 1.0f, 1.0f));
                CreateDirtUp((float)(totalTiles - tilesRemaining) * tileSize, height, sectionWidth);
                height += tileSize;
            }

            // Create section
            GameObject newSection = (GameObject)UnityEngine.Object.Instantiate(section, sectionPos, Quaternion.identity);
            newSection.transform.parent = parent.transform;
            newSection.transform.localScale = sectionScale;
            tilesRemaining -= sectionWidth;
        }
        GenerateVines((int)(Random.value * 3) + 1, parent.transform.position.x, parent.transform.position.y, (float)totalTiles * tileSize);
    }

    private void GenerateSmallHole()
    {
        bool lastSectionWasHole = false;
        while (tilesRemaining > 0)
        {
            int sectionWidth = GetSectionWidth(tilesRemaining);
            GameObject section = null;
            Vector3 sectionPos = Vector3.zero;
            Vector3 sectionScale = Vector3.one;

            int holeSize = (int)Random.Range(4.0f, 7.0f);
            if (holeSize + 4 < tilesRemaining && Random.value > 0.5f && !lastSectionWasHole)  // if the whole fits add it maybe
            {
                // cap the previous section
                GameObject endSection = tileWidget.GetEnd();
                sectionPos = parent.transform.TransformPoint(new Vector3((float)(totalTiles - tilesRemaining) * tileSize, height, 0.0f));
                GameObject newSection = (GameObject)UnityEngine.Object.Instantiate(endSection, sectionPos, Quaternion.identity);
                newSection.transform.parent = parent.transform;

                CreateDirtFlat((float)(totalTiles - tilesRemaining) * tileSize, height, 1);

                // Make the actual hole
                tilesRemaining -= holeSize - 1;
                height += (int)Random.Range(-2.0f, 2.0f);

                // cap the next section
                GameObject startSection = tileWidget.GetEnd();
                sectionPos = parent.transform.TransformPoint(new Vector3((float)(totalTiles - tilesRemaining + 1 /*plus one because it is reversed*/ ) * tileSize, height, 0.0f));
                newSection = (GameObject)UnityEngine.Object.Instantiate(startSection, sectionPos, Quaternion.identity);
                newSection.transform.parent = parent.transform;
                newSection.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);

                CreateDirtFlat((float)(totalTiles - tilesRemaining) * tileSize, height, 1);

                tilesRemaining -= 1;
                lastSectionWasHole = true;
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
                if (Random.value < 0.65) // Flip
                {
                    sectionPos = parent.transform.TransformPoint(new Vector3((float)(totalTiles - tilesRemaining) * tileSize, height, 0.0f));
                    CreateDirtDown((float)(totalTiles - tilesRemaining) * tileSize, height, sectionWidth);
                    height -= tileSize;
                }
                else
                {
                    sectionPos = parent.transform.TransformPoint(new Vector3((float)(totalTiles - tilesRemaining + sectionWidth) * tileSize, height + 0.64f, 0.0f));
                    sectionScale = Vector3.Scale(sectionScale, new Vector3(-1.0f, 1.0f, 1.0f));
                    CreateDirtUp((float)(totalTiles - tilesRemaining) * tileSize, height, sectionWidth);
                    height += tileSize;
                }

                // Create section
                GameObject newSection = (GameObject)UnityEngine.Object.Instantiate(section, sectionPos, Quaternion.identity);
                newSection.transform.parent = parent.transform;
                newSection.transform.localScale = sectionScale;
                tilesRemaining -= sectionWidth;
                lastSectionWasHole = false;
            }
        }
        GenerateVines((int)(Random.value * 4), parent.transform.position.x, parent.transform.position.y, (float)totalTiles * tileSize);
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

                // There will always be a vine in the first half of a large hole
                GenerateVines( 1, sectionPos.x, sectionPos.y-2.0f*tileSize, (float)holeSize* 0.5f * tileSize + 2.0f * tileSize);
                // plus up to two more
                GenerateVines((int)(Random.value * 3), sectionPos.x, sectionPos.y - 2.0f * tileSize, (float)holeSize * 0.5f * tileSize);

                CreateDirtFlat((float)(totalTiles - tilesRemaining) * tileSize, height, 1);

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

                CreateDirtFlat((float)(totalTiles - tilesRemaining - 1) * tileSize, height, 1);
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
                if (Random.value < 0.65) // Flip
                {
                    sectionPos = parent.transform.TransformPoint(new Vector3((float)(totalTiles - tilesRemaining) * tileSize, height, 0.0f));
                    CreateDirtDown((float)(totalTiles - tilesRemaining) * tileSize, height, sectionWidth);
                    height -= tileSize;
                }
                else
                {
                    sectionPos = parent.transform.TransformPoint(new Vector3((float)(totalTiles - tilesRemaining + sectionWidth) * tileSize, height + 0.64f, 0.0f));
                    sectionScale = Vector3.Scale(sectionScale, new Vector3(-1.0f, 1.0f, 1.0f));
                    CreateDirtUp((float)(totalTiles - tilesRemaining) * tileSize, height, sectionWidth);
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

    void CreateDirtFlat(float x, float y, int size)
    {
        switch (size)
        {
            case 1:
                {
                    Vector3 dirtPos = parent.transform.TransformPoint(x, y - tileSize * 0.5f, 0.0f);
                    GameObject newDirt = (GameObject)Object.Instantiate(GroundGenerator.Dirt1x, dirtPos, Quaternion.identity);
                    newDirt.transform.parent = parent.transform;
                    break;
                }
            case 2:
                {
                    Vector3 dirtPos = parent.transform.TransformPoint(x, y - tileSize * 0.5f, 0.0f);
                    GameObject newDirt = (GameObject)Object.Instantiate(GroundGenerator.Dirt2x, dirtPos, Quaternion.identity);
                    newDirt.transform.parent = parent.transform;
                    break;
                }
            case 3:
                {
                    Vector3 dirtPos = parent.transform.TransformPoint(x, y - tileSize * 0.5f, 0.0f);
                    GameObject newDirt = (GameObject)Object.Instantiate(GroundGenerator.Dirt2x, dirtPos, Quaternion.identity);
                    newDirt.transform.parent = parent.transform;
                    dirtPos = parent.transform.TransformPoint(x + 2.0f * tileSize, y - tileSize * 0.5f, 0.0f);
                    newDirt = (GameObject)Object.Instantiate(GroundGenerator.Dirt1x, dirtPos, Quaternion.identity);
                    newDirt.transform.parent = parent.transform;
                    break;
                }
            case 4:
                {
                    Vector3 dirtPos = parent.transform.TransformPoint(x, y - tileSize * 0.5f, 0.0f);
                    GameObject newDirt = (GameObject)Object.Instantiate(GroundGenerator.Dirt2x, dirtPos, Quaternion.identity);
                    newDirt.transform.parent = parent.transform;
                    dirtPos = parent.transform.TransformPoint(x + 2.0f * tileSize, y - tileSize * 0.5f, 0.0f);
                    newDirt = (GameObject)Object.Instantiate(GroundGenerator.Dirt2x, dirtPos, Quaternion.identity);
                    newDirt.transform.parent = parent.transform;
                    break;
                }
        }
    }

    void CreateDirtDown(float x, float y, int size)
    {
        switch (size)
        {
            case 2:
                {
                    Vector3 dirtPos = parent.transform.TransformPoint(x, y - tileSize * 0.95f, 0.0f);
                    GameObject newDirt = (GameObject)Object.Instantiate(GroundGenerator.Dirt2x, dirtPos, Quaternion.identity);
                    newDirt.transform.parent = parent.transform;
                    break;
                }
            case 3:
                {
                    Vector3 dirtPos = parent.transform.TransformPoint(x, y - tileSize * 0.75f, 0.0f);
                    GameObject newDirt = (GameObject)Object.Instantiate(GroundGenerator.Dirt2x, dirtPos, Quaternion.identity);
                    newDirt.transform.parent = parent.transform;
                    dirtPos = parent.transform.TransformPoint(x + 2.0f * tileSize, y - tileSize * 1.5f, 0.0f);
                    newDirt = (GameObject)Object.Instantiate(GroundGenerator.Dirt1x, dirtPos, Quaternion.identity);
                    newDirt.transform.parent = parent.transform;
                    break;
                }
            case 4:
                {
                    Vector3 dirtPos = parent.transform.TransformPoint(x, y - tileSize * 0.65f, 0.0f);
                    GameObject newDirt = (GameObject)Object.Instantiate(GroundGenerator.Dirt2x, dirtPos, Quaternion.identity);
                    newDirt.transform.parent = parent.transform;
                    dirtPos = parent.transform.TransformPoint(x + 2.0f * tileSize, y - tileSize * 1.5f, 0.0f);
                    newDirt = (GameObject)Object.Instantiate(GroundGenerator.Dirt2x, dirtPos, Quaternion.identity);
                    newDirt.transform.parent = parent.transform;
                    break;
                }
        }
    }

    void CreateDirtUp(float x, float y, int size)
    {
        switch (size)
        {
            case 2:
                {
                    Vector3 dirtPos = parent.transform.TransformPoint(x, y - tileSize * 0.25f, 0.0f);
                    GameObject newDirt = (GameObject)Object.Instantiate(GroundGenerator.Dirt2x, dirtPos, Quaternion.identity);
                    newDirt.transform.parent = parent.transform;
                    break;
                }
            case 3:
                {
                    Vector3 dirtPos = parent.transform.TransformPoint(x, y - tileSize * 0.4f, 0.0f);
                    GameObject newDirt = (GameObject)Object.Instantiate(GroundGenerator.Dirt2x, dirtPos, Quaternion.identity);
                    newDirt.transform.parent = parent.transform;
                    dirtPos = parent.transform.TransformPoint(x + 2.0f * tileSize, y + tileSize * 0.5f, 0.0f);
                    newDirt = (GameObject)Object.Instantiate(GroundGenerator.Dirt1x, dirtPos, Quaternion.identity);
                    newDirt.transform.parent = parent.transform;
                    break;
                }
            case 4:
                {
                    Vector3 dirtPos = parent.transform.TransformPoint(x, y - tileSize * 0.5f, 0.0f);
                    GameObject newDirt = (GameObject)Object.Instantiate(GroundGenerator.Dirt2x, dirtPos, Quaternion.identity);
                    newDirt.transform.parent = parent.transform;
                    dirtPos = parent.transform.TransformPoint(x + 2.0f * tileSize, y + tileSize * 0.5f, 0.0f);
                    newDirt = (GameObject)Object.Instantiate(GroundGenerator.Dirt2x, dirtPos, Quaternion.identity);
                    newDirt.transform.parent = parent.transform;
                    break;
                }
        }
    }

    // Generates count vines at coords between (x,y) and (x+xWidth, y)
    void GenerateVines(int count, float x, float y, float xWidth)
    {
        for (int i = 0; i < count; ++i)
        {
            float xPos = Random.Range(x, x+xWidth);
            float yPos = Random.Range(y + 12.0f, y + 16.0f);
            GameObject newVine = GroundGenerator.VinePool.GetVineRoot();
            newVine.transform.position = new Vector3(xPos, yPos, 0.0f);
            newVine.transform.parent = parent.transform;
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
