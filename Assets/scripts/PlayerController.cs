using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Pengaturan Pergerakan")]
    public float moveSpeed = 5f;
    private Vector2 movement;

    [Header("Pengaturan Interaksi")]
    public Transform hitPoint; // Posisi titik tempat serangan mendarat (harus berada di depan player)
    public float hitRadius = 0.5f; // Besarnya area deteksi serangan
    public LayerMask interactableLayer; // Layer untuk memfilter objek yang bisa dipukul (misal: "Interactables")

    [Header("Status Alat saat ini")]
    public ItemType currentTool = ItemType.BareHanded;

    // Komponen referensi
    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 lastMoveDirection = Vector2.down; // Arah hadap default: ke bawah

    void Start()
    {
        // Mengambil komponen yang menempel pada objek Player
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // Set arah default awal ke kanan (1f) agar animasi tidak ngefreeze di awal game
        if (anim != null)
        {
            anim.SetFloat("Horizontal", 1f);
        }

        // Mengirim data alat awal ke Animator
        UpdateToolAnimator();
    }

    void Update()
    {
        // 1. Mengambil Input Pergerakan (W, A, S, D / Panah)
        HandleMovementInput();

        // 2. Mengambil Input Ganti Alat (Q / Space)
        HandleToolSwapInput();

        // 3. Mengambil Input Interaksi (Klik Kiri)
        HandleActionInput();

        // 4. Mengirim data ke Animator Controller
        UpdateAnimator();
    }

    void FixedUpdate()
    {
        // Menggunakan FixedUpdate khusus untuk pergerakan fisik (Rigidbody)
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    private void HandleMovementInput()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Jika player sedang bergerak
        if (movement.x != 0 || movement.y != 0)
        {
            // Simpan arah hadap terakhir (normalized agar nilainya selalu 1 walau bergerak diagonal)
            lastMoveDirection = movement.normalized;

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
        // Menggunakan tombol Q atau Space (sesuai preferensi)
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
        // 0 berarti Klik Kiri Mouse
        if (Input.GetMouseButtonDown(0))
        {
            // 1. Jalankan animasi serangan
            anim.SetTrigger("Attack");

            // 2. Cek apakah ada objek di depan kita
            DetectAndHitObject();
        }
    }

    private void DetectAndHitObject()
    {
        if (hitPoint == null) return;

        // Membuat area deteksi berbentuk lingkaran tak kasat mata
        // OverlapCircle akan mendeteksi collider yang memiliki layer 'interactableLayer'
        Collider2D hitCollider = Physics2D.OverlapCircle(hitPoint.position, hitRadius, interactableLayer);

        if (hitCollider != null)
        {
            // Mencoba mendapatkan script InteractableObject dari objek yang terdeteksi
            InteractableObject interactable = hitCollider.GetComponent<InteractableObject>();
            if (interactable != null)
            {
                // Eksekusi fungsi OnHit pada objek dengan membawa informasi alat apa yang kita gunakan
                interactable.OnHit(currentTool);
            }
        }
    }

    private void UpdateAnimator()
    {
        // Memberitahu Animator kecepatan player (Speed > 0 artinya berjalan)
        anim.SetFloat("Speed", movement.sqrMagnitude);

        // Hanya update arah horizontal di animator jika player bergerak ke Kiri/Kanan
        // Jika player bergerak ke Atas/Bawah saja, parameter Horizontal tidak diganti (tetap kiri/kanan terakhir)
        if (movement.x != 0)
        {
            anim.SetFloat("Horizontal", movement.x);
            // Kode Debug Sementara: Menampilkan nilai di Console Unity
            Debug.Log("Input Terdeteksi! Mengirim nilai Horizontal ke Animator: " + movement.x);
        }
    }

    private void UpdateToolAnimator()
    {
        // Mengatur parameter "WeaponType" (0 = Tangan Kosong, 1 = Kapak, 2 = Beliung)
        anim.SetFloat("WeaponType", (float)currentTool);
    }

    // Fungsi visualisasi: Menggambar lingkaran merah di editor Unity untuk memudahkan mengatur besar/posisi 'hitPoint'
    private void OnDrawGizmosSelected()
    {
        if (hitPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(hitPoint.position, hitRadius);
        }
    }
}
