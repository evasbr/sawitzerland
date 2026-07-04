using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerData", menuName = "Game/Player Data")]
public class PlayerDataSO : ScriptableObject
{
    [Header("Energy Settings")]
    public float maxEnergy = 100f;
    public float currentEnergy;

    [Header("Time Settings")]
    public int currentMinute;
    public int currentHour;
    public int currentDay;
    public float timeTicker;

    // Fungsi untuk mengisi ulang energi / reset waktu saat game mulai
    public void ResetData()
    {
        currentEnergy = maxEnergy;
        currentMinute = 0;
        currentHour = 6; // Game dimulai jam 6 pagi
        currentDay = 1;
        timeTicker = 0;
    }
}