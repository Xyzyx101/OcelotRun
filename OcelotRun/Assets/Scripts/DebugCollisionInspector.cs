using UnityEngine;
using System.Collections;

public class DebugCollisionInspector : MonoBehaviour
{

    void OnCollisionEnter2D(Collision2D coll)
    {
        Debug.Log(coll);
    }

    void OnCollisionStay2D(Collision2D coll)
    {
        Debug.Log(coll);

    }

    void OnCollisionExit2D(Collision2D coll)
    {
        Debug.Log(coll);
    }
}
