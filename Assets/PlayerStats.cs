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

    [Header("Komponen UI Custom")]
    [Tooltip("Seret objek Image 'XP_Bar_Fill' (yang bertipe Filled) ke sini")]
    public Image xpFillImage;
    [Tooltip("Seret objek TextMeshPro 'XP_Text_Display' ke sini")]
    public TextMeshProUGUI xpText;
    [Tooltip("Seret objek TextMeshPro di dalam koin emas ke sini jika ada")]
    public TextMeshProUGUI levelText;

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
    }

    // Fungsi ini dipanggil secara global oleh objek interaktif pas mereka hancur
    public void AddXP(int amount)
    {
        currentXP += amount;
        Debug.Log($"[XP LOG] +{amount} XP didapat! Status: {currentXP}/{xpToNextLevel}");

        if (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }

        UpdateXPBarUI();
    }

    private void LevelUp()
    {
        currentXP -= xpToNextLevel; // Sisa XP ditransfer ke level berikutnya
        currentLevel++;
        
        // Naikkan batas target XP level berikutnya secara dinamis (contoh: naik 50% lebih sulit)
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