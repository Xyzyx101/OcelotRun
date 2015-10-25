using UnityEngine;
using System.Collections;

public class VineTrigger : MonoBehaviour
{
    public bool IsTouchingVine;
    private Collider2D vineCollider;

    void OnTriggerStay2D(Collider2D c)
    {
        IsTouchingVine = true;
        vineCollider = c;
    }

    void OnTriggerExit2D()
    {
        IsTouchingVine = false;
    }

    public VineSection GetVine()
    {
        return vineCollider.gameObject.GetComponent<VineSection>();
    }
}
