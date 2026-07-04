using UnityEngine;

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

        // Hancurkan objek secara utuh setelah 1 detik
        Destroy(gameObject, 1.0f); 
        Debug.Log($"[LOG 7] Perintah Destroy(gameObject) untuk '{gameObject.name}' telah dikirim ke Unity. Objek akan hilang dalam 1 detik.");
    }
}