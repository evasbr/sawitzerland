using UnityEngine;
using TMPro; // WAJIB MASUKKAN INI UNTUK TEXTMESH PRO

public class FloatingText : MonoBehaviour
{
    public float moveSpeed = 1.5f;   // Kecepatan teks melayang ke atas
    public float destroyTime = 0.8f; // Berapa lama teks bertahan sebelum hilang
    private TextMeshProUGUI textMesh;
    private Color textColor;

    private void Awake()
    {
        // Ambil komponen teks
        textMesh = GetComponentInChildren<TextMeshProUGUI>();
        if (textMesh != null) textColor = textMesh.color;
    }

    // Fungsi untuk mengubah isi angka XP-nya dari script luar
    public void SetXPText(int xpAmount)
    {
        if (textMesh == null) textMesh = GetComponentInChildren<TextMeshProUGUI>();
        textMesh.text = $"+{xpAmount} XP";
    }

    private void Update()
    {
        // 1. Gerakkan teks ke atas sumbu Y secara halus
        transform.position += new Vector3(0, moveSpeed * Time.deltaTime, 0);

        // 2. Efek memudar (Fade Out) perlahan
        destroyTime -= Time.deltaTime;
        if (destroyTime <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            // Kurangi nilai Alpha (transparansi) teks secara bertahap
            textColor.a = Mathf.Lerp(textColor.a, 0, Time.deltaTime * 4f);
            if (textMesh != null) textMesh.color = textColor;
        }
    }
}