using System;
using UnityEngine;

public class XPSystem : MonoBehaviour
{
    [Header("XP & Level Settings")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private float currentXP = 0f;
    [SerializeField] private float xpToNextLevel = 100f; // Target XP untuk naik level

    // Event untuk memberi tahu UI agar mengupdate bar dan angka XP
    public static event Action<float, float, int> OnXPChanged;

    private void Start()
    {
        // Update UI pertama kali saat game dimulai
        NotifyXPChange();
    }

    // Fungsi utama untuk menambah XP (dipanggil dari objek yang dibersihkan/ditanam)
    public void AddXP(float amount)
    {
        currentXP += amount;
        Debug.Log($"[XP SYSTEM] Mendapatkan +{amount} XP. Total XP saat ini: {currentXP}/{xpToNextLevel}");

        // Cek apakah XP sudah cukup untuk naik level
        if (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }

        NotifyXPChange();
    }

    private void LevelUp()
    {
        currentXP -= xpToNextLevel; // Sisa XP dibawa ke level berikutnya
        currentLevel++;
        
        // Formula sederhana: tiap naik level, target XP berikutnya bertambah lebih berat (misal dikali 1.2)
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.2f); 

        Debug.Log($"[LEVEL UP] Selamat! Anda naik ke Level {currentLevel}! Target XP berikutnya: {xpToNextLevel}");
    }

    private void NotifyXPChange()
    {
        // Mengirim data: currentXP, xpToNextLevel, dan currentLevel ke UI
        OnXPChanged?.Invoke(currentXP, xpToNextLevel, currentLevel);
    }
}