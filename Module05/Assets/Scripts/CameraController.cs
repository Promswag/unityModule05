using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraController : MonoBehaviour
{
    private Camera _camera;
    [SerializeField] private Tilemap mapLayout;
    [SerializeField] private PlayerController player;
    [SerializeField] private SpriteRenderer background;

    float mapX, mapY;
    private float minX, maxX, minY, maxY;
    
    private Vector3 center;

    void Awake()
    {
        _camera = GetComponent<Camera>();
        mapLayout.CompressBounds();
        center = (Vector3)mapLayout.size / 2.0f + (Vector3)mapLayout.origin;
        mapX = mapLayout.size.x;
        mapY = mapLayout.size.y;

        float vExtent = _camera.orthographicSize;
        float hExtent = vExtent * Screen.width / Screen.height;

        minX = hExtent - mapX / 2.0f + center.x;
        maxX = mapX / 2.0f - hExtent + center.x;
        minY = vExtent - mapY / 2.0f + center.y;
        maxY = mapY / 2.0f - vExtent + center.y;
    }

    void LateUpdate()
    {
        var v3 = player.transform.position;
        v3.x = Mathf.Clamp(v3.x, minX, maxX);
        v3.y = Mathf.Clamp(v3.y, minY, maxY);
        background.transform.position = center + v3 * 0.5f;
        v3.z = -1f;
        transform.position = v3;
    }
}
