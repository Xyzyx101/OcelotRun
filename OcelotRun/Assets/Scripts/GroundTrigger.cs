using UnityEngine;
using System.Collections;

public class GroundTrigger : MonoBehaviour
{
    public bool IsOnGround;

    void OnTriggerStay2D()
    {
        IsOnGround = true;
    }

    void OnTriggerExit2D()
    {
        IsOnGround = false;
    }
}
