using UnityEngine;
using System.Collections;

public class InteractableObject : MonoBehaviour
{
    [Header("Interact Settings")]
    [SerializeField] private string objectName = "Interactable Object";
    [SerializeField] private int xpReward = 15; // Jumlah XP yang didapat jika objek ini hancur
    
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
        originalPosition = transform.localPosition;
    }
    
    // Fungsi OnHit (Sekarang menerima komponen XPSystem milik player)
    public virtual void OnHit(ItemType toolUsed)
    {
        Debug.Log($"[LOG 1] Objek '{gameObject.name}' terkena pukulan!");

        if (toolUsed != requiredTool)
        {
            Debug.LogWarning($"[LOG 2] Alat salah! Butuh: {requiredTool}");
            return; 
        }

        currentHits++;
        
        // Efek goyang
        if (!isShaking && currentHits < maxHits)
        {
            StartCoroutine(ShakeObject());
        }
        
        if (currentHits >= maxHits)
        {
            PlaySound(destroySound);
            OnObjectDestroyed(); // Fungsi ini nanti yang panggil PlayerStats.instance.AddXP
        }
        else
        {
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
            float randomX = Random.Range(-shakeMagnitude, shakeMagnitude);
            transform.localPosition = new Vector3(originalPosition.x + randomX, originalPosition.y, originalPosition.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
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

    // Fungsi hancur yang dimodifikasi untuk memberi XP kepada player
    protected virtual void OnObjectDestroyed()
{
    // 1. Matikan Collider pohon agar tidak bisa dipukul lagi setelah hancur
    Collider2D col = GetComponent<Collider2D>();
    if (col != null) col.enabled = false;

    // 2. Sembunyikan semua visual sprite anak (batang, daun, dll)
    foreach (Transform child in transform)
    {
        child.gameObject.SetActive(false);
    }

    // 3. Panggil efek partikel kayu jika ada (menggunakan float untuk waktu destroy effect)
    if (destroyEffectPrefab != null)
    {
        Vector3 spawnPosition = transform.position + new Vector3(0, 1.0f, 0);
        GameObject effect = Instantiate(destroyEffectPrefab, spawnPosition, Quaternion.identity);
        Destroy(effect, 2.0f); // 2.0f di sini adalah float, ini sudah benar untuk Destroy
    }

    // 4. Kirim XP ke PlayerStats (xpReward harus berupa angka BULAT/int)
    if (PlayerStats.instance != null)
    {
        // Pastikan variabel 'xpReward' di bagian atas script dideklarasikan sebagai: public int xpReward = 20;
        PlayerStats.instance.AddXP(xpReward); 
    }

    // 5. Hancurkan objek pohon utama dari dunia game setelah 1 detik
    Destroy(gameObject, 1.0f); 
}
}