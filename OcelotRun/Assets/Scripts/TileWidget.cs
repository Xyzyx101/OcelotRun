using UnityEngine;
using System.Collections;

public class TileWidget : MonoBehaviour
{
    public GameObject[] Flat2;
    public GameObject[] Flat3;
    public GameObject[] Flat4;
    public GameObject[] Slope2;
    public GameObject[] Slope3;
    public GameObject[] Slope4;
    public GameObject[] End;

    public GameObject GetFlat2()
    {
        int val = (int)Random.Range(0.0f, (float)Flat2.Length);
        return Flat2[val];
    }
    public GameObject GetFlat3()
    {
        int val = (int)Random.Range(0.0f, (float)Flat3.Length);
        return Flat3[val];
    }
    public GameObject GetFlat4()
    {
        int val = (int)Random.Range(0.0f, (float)Flat4.Length);
        return Flat4[val];
    }
    public GameObject GetSlope2()
    {
        int val = (int)Random.Range(0.0f, (float)Slope2.Length);
        return Slope2[val];
    }
    public GameObject GetSlope3()
    {
        int val = (int)Random.Range(0.0f, (float)Slope3.Length);
        return Slope3[val];
    }
    public GameObject GetSlope4()
    {
        int val = (int)Random.Range(0.0f, (float)Slope4.Length);
        return Slope4[val];
    }
    public GameObject GetEnd()
    {
        int val = (int)Random.Range(0.0f, (float)End.Length);
        return End[val];
    }
}
