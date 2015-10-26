using UnityEngine;
using System.Collections;

public class CarCollisionCheck : MonoBehaviour {
    private PlayerController PlayerController;
	
	void Start () {
        PlayerController = GetComponentInParent<PlayerController>();
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Car")
        {
            PlayerController.GetHitByCar();
        }
    }
}
