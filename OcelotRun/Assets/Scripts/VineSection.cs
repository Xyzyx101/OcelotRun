using UnityEngine;
using System.Collections;

public class VineSection : MonoBehaviour {
    public Transform Root;

    private HingeJoint2D RootHinge, LeftHinge, RightHinge;
    
    void Start()
    {
        Transform sprite = GetComponentInChildren<SpriteRenderer>().transform;
        float scaleX = 1.0f, scaleY = 1.0f;
        if (Random.value<0.5)
        {
            scaleX *= -1.0f;
        }
        if (Random.value < 0.5f)
        {
            scaleY *= -1.0f;
        }
        sprite.localScale = new Vector3(scaleX, scaleY, 1.0f);
    }

    public void Grab(Rigidbody2D torsoRB, Rigidbody2D leftArmRB, Rigidbody2D rightArmRB,Vector2 handOffset)
    {
        LeftHinge = gameObject.AddComponent<HingeJoint2D>();
        LeftHinge.connectedBody = leftArmRB;
        LeftHinge.anchor = new Vector2(0f, 0f);
        LeftHinge.connectedAnchor = handOffset;

        RightHinge = gameObject.AddComponent<HingeJoint2D>();
        RightHinge.connectedBody = rightArmRB;
        RightHinge.anchor = new Vector2(0f, -0.3f);
        RightHinge.connectedAnchor = handOffset;

        RootHinge = Root.gameObject.AddComponent<HingeJoint2D>();
        RootHinge.connectedBody = torsoRB;
        float offset = 1.3f * (Root.transform.position.y - transform.position.y);
        RootHinge.connectedAnchor = new Vector2(1.2f, offset);
    }

    public void Release()
    {
        if (LeftHinge!=null)
        {
            Destroy(LeftHinge);
        }

        if(RightHinge!= null)
        {
            Destroy(RightHinge);
        }

        if(RootHinge!=null)
        {
            Destroy(RootHinge);
        }
    }
}
