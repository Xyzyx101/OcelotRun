using UnityEngine;
using System.Collections;

public class VinePool : MonoBehaviour {
    public GameObject VineRootPrefab;
    public int PoolSize;

	// Use this for initialization
	void Start () {
	    for(int i = 0; i < PoolSize; ++i)
        {
            GameObject newVine = (GameObject)Instantiate(VineRootPrefab, transform.position, Quaternion.identity);
            newVine.transform.parent = transform;
        }
	}
	
	public GameObject GetVineRoot()
    {
        if(transform.childCount==0)
        {
            Debug.LogWarning("Vine pool empty.  Instatiating new vine.");
            GameObject newVine = (GameObject)Instantiate(VineRootPrefab, new Vector3(0.0f, 9.0f, 0.0f), Quaternion.identity);
            newVine.transform.parent = transform;
        }
        return transform.GetChild(0).gameObject;
    }
}
