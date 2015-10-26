using UnityEngine;
using System.Collections;

public class Road : MonoBehaviour {

    public GameObject[] Cars;
    public Transform CarSpawnPoint;
    public Transform CarDestination;
    public float SpawnRate;
    public float MinSpeed;
    public float MaxSpeed;

    private float SpawnTimer;


	// Use this for initialization
	void Start () {
        SpawnTimer = SpawnRate;
    }
	
	// Update is called once per frame
	void Update () {
        SpawnTimer -= Time.deltaTime;
        if (SpawnTimer<0)
        {
            if (Random.value<0.35)
            {
                GameObject car = (GameObject)Instantiate(Cars[(int)Random.Range(0f, Cars.Length - 0.00001f)], CarSpawnPoint.position, Quaternion.identity);
                Car carScript = car.GetComponent<Car>();
                carScript.Init(CarSpawnPoint, CarDestination, Random.Range(MinSpeed, MaxSpeed));
            }
            SpawnTimer = SpawnRate;
        }
	}
}
