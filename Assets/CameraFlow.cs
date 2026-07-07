using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Pengikutan")]
    public Transform target;          // Tarik objek Player ke sini
    public float smoothing = 5f;      // Kecepatan kamera mengikuti player (makin besar makin instan)
    public Vector3 offset;            // Jarak aman kamera (biasanya Z diisi -10)

    [Header("Batas Kamera (Ujung Tile)")]
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    private void LateUpdate()
    {
        if (target == null) return;

        // 1. Tentukan posisi tujuan kamera berdasarkan posisi player + offset
        Vector3 targetPosition = target.position + offset;

        // 2. KUNCI posisi X dan Y agar tidak melewati batas angka yang kita tentukan
        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

        // 3. Pastikan posisi Z kamera tetap berada di jarak aman (-10) agar tidak menempel di player
        targetPosition.z = transform.position.z;

        // 4. Gerakkan kamera secara smooth menuju target posisi yang sudah dikunci
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing * Time.deltaTime);
    }
}