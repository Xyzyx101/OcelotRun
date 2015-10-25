using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public GroundGenerator GroundGenerator;
    public PlayerController PlayerController;
    public Transform PlayerTransform;
    public float MinPlayerOffset;
    public float MaxPlayerOffset;
    public float TranslateSpeed;

    public float MinScale;
    public float MaxScale;
    public float MinSpeedPoint;
    public float MaxSpeedPoint;
    public float ScaleChangeSpeed;

    private Camera Camera;

    // Use this for initialization
    void Start()
    {
        Camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        // TODO get speed fromplayer controller
        float factor = (PlayerController.GetSpeed() - MinSpeedPoint) / (MaxSpeedPoint - MinSpeedPoint);
        factor = Mathf.Clamp01(factor);
        float targetScale = Mathf.Lerp(MinScale, MaxScale, factor);
        Camera.orthographicSize = Mathf.Lerp(Camera.orthographicSize, targetScale, ScaleChangeSpeed * Time.deltaTime);

        float playerOffset = Mathf.Lerp(MinPlayerOffset, MaxPlayerOffset, factor);

        float cameraX = Mathf.Lerp(transform.position.x, PlayerTransform.position.x + playerOffset, TranslateSpeed * Time.deltaTime);
        cameraX = Mathf.Min(Mathf.Max(-5.0f, cameraX), 5.0f);

        float cameraY = Mathf.Lerp(transform.position.y, PlayerTransform.position.y + playerOffset, TranslateSpeed * Time.deltaTime);
        cameraY = Mathf.Min(Mathf.Max(GroundGenerator.GetLastHeight() - 10.0f, cameraY), GroundGenerator.GetLastHeight() + 10f);

        transform.position = new Vector3(cameraX, cameraY, transform.position.z);
    }
}
