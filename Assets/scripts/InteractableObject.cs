using UnityEngine;
using System.Collections;

public class InteractableObject : MonoBehaviour
{
    [Header("Komponen Audio")]
    public AudioSource audioSource;

    [Header("Setelan Ketahanan")]
    public int maxHits = 3;
    protected int currentHits = 0;

    [Header("Validasi Alat")]
    [Tooltip("Pilih alat apa yang WAJIB digunakan untuk menghancurkan objek ini")]
    public ItemType requiredTool = ItemType.BareHanded; 

    [Header("Koleksi SFX per Pukulan")]
    [Tooltip("Masukkan suara untuk Pukulan 1, Pukulan 2, dst. Sesuai urutan.")]
    public AudioClip[] hitSounds; 
    public AudioClip destroySound; // Suara khusus pas objeknya hancur/hilang

    [Header("Efek Visual Goyang")]
    public float shakeDuration = 0.15f; // Berapa lama pohon goyang
    public float shakeMagnitude = 0.1f;  // Seberapa kuat goyangnya
    private Vector3 originalPosition;
    private bool isShaking = false;

    [Header("Efek Partikel Hancur")]
    public GameObject destroyEffectPrefab;

    private void Start()
    {
        // Simpan posisi awal pohon agar setelah goyang bisa balik ke tempat semula
        originalPosition = transform.localPosition;
    }
    
    // Fungsi utama yang dipanggil oleh Player
    public virtual void OnHit(ItemType toolUsed)
    {
        // LOGGING 1: Memastikan fungsi ini kepanggil oleh PlayerController
        Debug.Log($"[LOG 1] Objek '{gameObject.name}' BERHASIL TERKENA PUKULAN! Player menggunakan alat: {toolUsed}");

        // Cek apakah alat yang dipakai Player sudah cocok
        if (toolUsed != requiredTool)
        {
            // LOGGING 2: Jika alatnya salah
            Debug.LogWarning($"[LOG 2] Pukulan DITOLAK pada '{gameObject.name}'! Alat salah. Butuh: {requiredTool}, tapi Player pakai: {toolUsed}");
            return; 
        }

        currentHits++;

        // PANGGIL EFEK GOYANG DI SINI SAAT DIPUKUL
        if (!isShaking && currentHits < maxHits)
        {
            StartCoroutine(ShakeObject());
        }
        
        // LOGGING 3: Menampilkan hitungan hit saat ini
        Debug.Log($"[LOG 3] Pukulan MASUK pada '{gameObject.name}'. Hit saat ini: {currentHits} / {maxHits}");

        // 1. Tentukan suara dan aksi yang akan diputar
        if (currentHits >= maxHits)
        {
            // LOGGING 4: Memicu proses kehancuran
            Debug.Log($"[LOG 4] '{gameObject.name}' mencapai batas Max Hits! Menjalankan OnObjectDestroyed()...");
            PlaySound(destroySound);
            OnObjectDestroyed();
        }
        else
        {
            // Ambil suara sesuai urutan pukulan saat ini
            if (hitSounds.Length > 0)
            {
                int soundIndex = Mathf.Min(currentHits - 1, hitSounds.Length - 1);
                PlaySound(hitSounds[soundIndex]);
            }
        }
    }

    private IEnumerator ShakeObject()
    {
        isShaking = true;
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            // Bikin posisi acak sedikit ke kanan/kiri/atas/bawah
            float randomX = Random.Range(-shakeMagnitude, shakeMagnitude);
            transform.localPosition = new Vector3(originalPosition.x + randomX, originalPosition.y, originalPosition.z);

            elapsedTime += Time.deltaTime;
            yield return null; // Tunggu sampai frame berikutnya
        }

        // Kembalikan ke posisi semula setelah selesai goyang
        transform.localPosition = originalPosition;
        isShaking = false;
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    protected virtual void OnObjectDestroyed()
    {
        // Matikan Collider objek induk agar tidak bisa dipukul lagi
        if (GetComponent<Collider2D>() != null) 
        {
            GetComponent<Collider2D>().enabled = false;
            Debug.Log($"[LOG 5] Collider pada '{gameObject.name}' telah dimatikan.");
        }

        // MATIKAN SEMUA VISUAL DI OBJEK ANAK (CHILD)
        int childCount = 0;
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
            childCount++;
        }
        Debug.Log($"[LOG 6] Berhasil me-nonaktifkan {childCount} objek anak (visual puzzle) dari '{gameObject.name}'.");

        if (destroyEffectPrefab != null)
        {
            // Buat posisi partikel sedikit lebih tinggi dari pohon
            Vector3 spawnPosition = transform.position + new Vector3(0, 1.8f, 0); 
            GameObject effect = Instantiate(destroyEffectPrefab, spawnPosition, Quaternion.identity);
            Destroy(effect, 2.0f); // Hancurkan sisa partikel setelah 2 detik agar tidak menumpuk di memori
        }
        
        // Hancurkan objek secara utuh setelah 1 detik
        Destroy(gameObject, 1.0f); 
        Debug.Log($"[LOG 7] Perintah Destroy(gameObject) untuk '{gameObject.name}' telah dikirim ke Unity. Objek akan hilang dalam 1 detik.");
    }
}