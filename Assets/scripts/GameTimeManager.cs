using System;
using UnityEngine;

public class GameTimeManager : MonoBehaviour
{
    // Event yang akan berbunyi setiap 1 detik di dalam game
    public static event Action OnTick;

    private float tickTimer;
    private const float TICK_INTERVAL = 1f; // 1 detik real-time

    private void Update()
    {
        tickTimer += Time.deltaTime;
        if (tickTimer >= TICK_INTERVAL)
        {
            tickTimer -= TICK_INTERVAL;
            
            // Beritahu semua sistem (Energi & Sawit) bahwa 1 detik telah berlalu
            OnTick?.Invoke();
        }
    }
}