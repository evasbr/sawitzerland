using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance;

    [Header("Data Level & XP Aktual")]
    public int currentLevel = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 100;

    [Header("UI Coming Soon")]
    [SerializeField] private GameObject comingSoonPanel;

    [Header("Komponen UI Custom")]
    [Tooltip("Seret objek Image 'XP_Bar_Fill' (yang bertipe Filled) ke sini")]
    public Image xpFillImage;
    [Tooltip("Seret objek TextMeshPro 'XP_Text_Display' ke sini")]
    public TextMeshProUGUI xpText;
    [Tooltip("Seret objek TextMeshPro di dalam koin emas ke sini jika ada")]
    public TextMeshProUGUI levelText;

    // Variabel penanda status game (ditambahkan agar tidak error)
    private bool gameSelesai = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateXPBarUI();
        
        // Memastikan panel coming soon tertutup di awal permainan
        if (comingSoonPanel != null)
        {
            comingSoonPanel.SetActive(false);
        }
    }

    // Fungsi ini dipanggil secara global oleh objek interaktif pas mereka hancur
    public void AddXP(int amount)
    {
        // Jika game sudah selesai, hentikan penambahan XP
        if (gameSelesai) return;

        currentXP += amount;
        Debug.Log($"[XP LOG] +{amount} XP didapat! Status: {currentXP}/{xpToNextLevel}");

        // FITUR DEMO: Jika XP mencukupi target Level 1, langsung kunci dan tampilkan Coming Soon
        if (currentXP >= xpToNextLevel)
        {
            currentXP = xpToNextLevel; // Mengunci visual bar agar penuh (100/100)
            UpdateXPBarUI();
            TriggerComingSoon();
            return;
        }

        UpdateXPBarUI();
    }

    private void TriggerComingSoon()
    {
        gameSelesai = true;
        
        // HAPUS ATAU KOMENTARI BARIS INI:
        // Time.timeScale = 0f; 

        // Sebagai gantinya, matikan script pergerakan player kamu secara spesifik di sini, contoh:
        // GameObject.FindWithTag("Player").GetComponent<PlayerController>().enabled = false;

        // Munculkan panel penutup
        if (comingSoonPanel != null)
        {
            comingSoonPanel.SetActive(true);
        }

        Debug.LogWarning("Level 1 Selesai! Game dikunci, tetapi waktu Unity tetap berjalan agar tombol UI responsif.");
    }

    // Fungsi ini dihubungkan ke Button "Restart" di dalam panel Coming Soon kamu via Inspector
    public void RestartGame()
    {
        Time.timeScale = 1f; // WAJIB: Kembalikan waktu ke normal sebelum reload scene agar game tidak freeze lagi
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    // Fungsi LevelUp dinonaktifkan sementara untuk versi demo Level 1 ini
    private void LevelUp()
    {
        currentXP -= xpToNextLevel; 
        currentLevel++;
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.5f);
        Debug.LogWarning($"[LEVEL UP] Naik ke Level {currentLevel}! Target baru: {xpToNextLevel} XP");
    }

    public void UpdateXPBarUI()
    {
        // Perhitungan matematika linear berdasarkan angka aktual (0.0f sampai 1.0f)
        if (xpFillImage != null)
        {
            float fillPercentage = (float)currentXP / xpToNextLevel;
            xpFillImage.fillAmount = fillPercentage;
        }

        // Tampilkan angka mentah aktual di dalam Bar
        if (xpText != null)
        {
            xpText.text = $"{currentXP} / {xpToNextLevel} XP";
        }

        // Sinkronisasi angka level ke koin emas
        if (levelText != null)
        {
            levelText.text = currentLevel.ToString();
        }
    }
}