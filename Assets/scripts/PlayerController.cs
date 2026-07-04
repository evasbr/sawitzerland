using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Pengaturan Pergerakan")]
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator anim;
    private Vector2 lastMoveDirection = Vector2.down; // Arah hadap default: ke bawah

    [Header("Pengaturan Interaksi")]
    [Tooltip("Target posisi objek child HitPoint di depan Player")]
    public Transform hitPoint;
    [Tooltip("Jari-jari lingkaran deteksi pukulan")]
    public float hitRadius = 0.5f;
    [Tooltip("Pilih Physics Layer tempat objek interaktif berada (misal: Collision atau Interactables)")]
    public LayerMask interactableLayer;

    [Header("Status Alat saat Ini")]
    public ItemType currentTool = ItemType.BareHanded; 

    [Header("Komponen Audio")]
    public AudioSource audioSource;

    [Header("Daftar Suara Alat")]
    public AudioClip sfxBareHand;
    public AudioClip sfxAxe;
    public AudioClip sfxHoe;
    public AudioClip sfxPlanting;
    public AudioClip sfxScythe;
    public AudioClip sfxPickaxe;
    public AudioClip sfxWatering;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        
        if (audioSource == null) 
            audioSource = GetComponent<AudioSource>();

        // Set arah default awal ke kanan (1f) agar animasi tidak ngefreeze di awal game
        if (anim != null)
        {
            anim.SetFloat("Horizontal", 1f);
        }

        // Mengirim data alat awal ke Animator
        UpdateToolAnimator();
    }

    private void Update()
    {
        // 1. Mengambil Input Pergerakan
        HandleMovementInput();

        // 2. Mengambil Input Ganti Alat (Q / Space)
        HandleToolSwapInput();

        // 3. Mengambil Input Interaksi (Klik Kiri Mouse)
        HandleActionInput();

        // 4. Mengatur Parameter Animator
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        // Mengeksekusi pergerakan fisik Player
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    private void HandleMovementInput()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        // Jika player sedang bergerak
        if (moveInput.x != 0 || moveInput.y != 0)
        {
            // Simpan arah hadap terakhir (normalized agar nilainya selalu 1 walau bergerak diagonal)
            lastMoveDirection = moveInput.normalized;

            // Menggeser posisi `hitPoint` (titik pukulan) agar selalu berada tepat di depan arah hadap player
            // Angka 0.8f adalah jarak hitPoint dari tengah player, sesuaikan dengan ukuran sprite Anda
            if (hitPoint != null)
            {
                hitPoint.localPosition = new Vector3(lastMoveDirection.x, lastMoveDirection.y, 0) * 0.8f;
            }
        }
    }

    private void HandleToolSwapInput()
    {
        // Menggunakan tombol Q atau Space untuk memutar alat
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Q))
        {
            // Trik untuk memutar nilai enum: 0 -> 1 -> 2 -> 3 -> 4 -> 5 -> 6 -> 0 -> ...
            int nextToolIndex = ((int)currentTool + 1) % 7;
            currentTool = (ItemType)nextToolIndex;
            
            // Perbarui parameter WeaponType di Animator
            UpdateToolAnimator();
            
            Debug.Log("Mengganti alat ke: " + currentTool);
        }
    }

    private void HandleActionInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("[INPUT LOG] Klik kiri mouse terdeteksi! Memulai ayunan alat...");

            // Pemicu animasi serang
            if (anim != null) 
                anim.SetTrigger("Attack");

            // Putar suara ayunan alat
            PlayToolSFX(currentTool);

            // Jalankan deteksi objek di depan
            DetectAndHitObject();
        }
    }

    private void DetectAndHitObject()
    {
        if (hitPoint == null)
        {
            Debug.LogError("[PLAYER ERROR] Variabel 'hitPoint' masih KOSONG! Seret objek child HitPoint ke Inspector Player.");
            return;
        }

        // Cek apakah lingkaran deteksi mendeteksi collider di layer yang ditentukan
        Collider2D hitCollider = Physics2D.OverlapCircle(hitPoint.position, hitRadius, interactableLayer);

        if (hitCollider != null)
        {
            Debug.Log($"[PLAYER LOG] Menabrak objek: '{hitCollider.name}'");

            // Ambil script InteractableObject dari objek yang terkena atau induknya
            InteractableObject interactable = hitCollider.GetComponent<InteractableObject>();
            if (interactable == null) 
                interactable = hitCollider.GetComponentInParent<InteractableObject>();

            if (interactable != null)
            {
                interactable.OnHit(currentTool);
            }
            else
            {
                Debug.LogWarning($"[PLAYER LOG] Menabrak '{hitCollider.name}', tapi tidak ada script 'InteractableObject' menempel.");
            }
        }
        else
        {
            Debug.Log("[PLAYER LOG] Ayunan alat tidak mengenai objek apa pun di layer target.");
        }
    }

    private void UpdateAnimator()
    {
        if (anim == null) return;

        // Memberitahu Animator kecepatan player (Speed > 0 artinya berjalan)
        anim.SetFloat("Speed", moveInput.sqrMagnitude);

        // Hanya update arah horizontal di animator jika player bergerak ke Kiri/Kanan
        // Jika player bergerak ke Atas/Bawah saja, parameter Horizontal tidak diganti (tetap kiri/kanan terakhir)
        if (moveInput.x != 0)
        {
            anim.SetFloat("Horizontal", moveInput.x);
            Debug.Log("Input Terdeteksi! Mengirim nilai Horizontal ke Animator: " + moveInput.x);
        }
    }

    private void UpdateToolAnimator()
    {
        if (anim == null) return;

        // Mengatur parameter "WeaponType" (0 = Tangan Kosong, 1 = Kapak, 2 = Beliung, dst)
        anim.SetFloat("WeaponType", (float)currentTool);
    }

    private void PlayToolSFX(ItemType tool)
    {
        if (audioSource == null) return;

        AudioClip clipToPlay = null;
        switch (tool)
        {
            case ItemType.BareHanded: clipToPlay = sfxBareHand; break;
            case ItemType.Axe: clipToPlay = sfxAxe; break;
            case ItemType.Hoe: clipToPlay = sfxHoe; break;
            case ItemType.Seeds: clipToPlay = sfxPlanting; break;
            case ItemType.WateringCan: clipToPlay = sfxWatering; break;
            case ItemType.Scythe: clipToPlay = sfxScythe; break;
            case ItemType.Pickaxe: clipToPlay = sfxPickaxe; break;
        }

        if (clipToPlay != null)
        {
            audioSource.PlayOneShot(clipToPlay);
        }
    }

    // Menggambar visual lingkaran hitRadius di editor agar mudah dikalibrasi posisinya
    private void OnDrawGizmosSelected()
    {
        if (hitPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(hitPoint.position, hitRadius);
        }
    }
}