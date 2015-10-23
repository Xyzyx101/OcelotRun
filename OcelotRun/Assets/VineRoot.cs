using UnityEngine;
using System.Collections;

public class VineRoot : MonoBehaviour {
    
    public GameObject[] VineSections;
    public float SectionLength;
    public int SectionCount;

    // Use this for initialization
    void Start () {
        GameObject firstSection = GetComponentInChildren<VineSection>().gameObject;
        VineSection vineSectionScript = firstSection.GetComponent<VineSection>();
        vineSectionScript.Root = transform;
        if (SectionCount>0)
        {
            Build(this.gameObject, firstSection, SectionCount-1);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    void OnDestroy() {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }



    void Build(GameObject root, GameObject parent, int sections)
    {
        Vector3 offset = new Vector3(0.0f, -SectionLength, 0.0f);
        Vector3 newPos = parent.transform.position + offset;
        GameObject newVine = GetSection(newPos.x, newPos.y);
        newVine.transform.parent = transform;
        VineSection vineSectionScript = newVine.GetComponent<VineSection>();
        vineSectionScript.Root = root.transform;
        Rigidbody2D newBody = newVine.GetComponent<Rigidbody2D>();
        HingeJoint2D newHinge =  parent.AddComponent<HingeJoint2D>();
        newHinge.anchor = offset;
        newHinge.connectedBody = newBody;
        if(sections>0)
        {
            Build(root, newVine.gameObject, sections - 1);
        }
    }

    GameObject GetSection(float x, float y)
    {
        int vineIndex = (int)Random.Range(0.0f, (float)VineSections.Length - 0.000001f);
        GameObject newSection = (GameObject)UnityEngine.Object.Instantiate(VineSections[vineIndex], new Vector3(x, y, 0.0f), Quaternion.identity);
        return newSection;
    }
}
