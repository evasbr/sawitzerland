using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panel Utama UI")]
    public GameObject mainMenuPanel;   
    public GameObject backstoryPanel;  
    public GameObject moveTutorialPanel;   // Panel Baru: Tutorial Gerak
    public GameObject weaponTutorialPanel; // Panel Baru: Weapon Compatibility
    
    [Header("Canvas Group untuk Smooth Fade")]
    public CanvasGroup mainMenuCanvasGroup;
    public CanvasGroup backstoryCanvasGroup;
    public CanvasGroup moveTutorialCanvasGroup;   // CanvasGroup Baru
    public CanvasGroup weaponTutorialCanvasGroup; // CanvasGroup Baru
    public CanvasGroup faderCanvasGroup;

    [Header("Pengaturan Transisi")]
    public float kecepatanTransisi = 2.0f;
    
    // State Tracker untuk mendeteksi klik kiri (Next) di setiap halaman
    private enum CutsceneState { MainMenu, Backstory, MoveTutorial, WeaponTutorial, FinalFadeOut }
    private CutsceneState currentState = CutsceneState.MainMenu;

    void Start()
    {
        Time.timeScale = 1f;
        
        // Memastikan kondisi awal scene rapi lewat kode
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (backstoryPanel != null) backstoryPanel.SetActive(false); 
        if (moveTutorialPanel != null) moveTutorialPanel.SetActive(false); 
        if (weaponTutorialPanel != null) weaponTutorialPanel.SetActive(false); 
        
        StartCoroutine(FadeInAwal());
    }

    // Dipanggil oleh Button "Mulai" di Main Menu
    public void ClickStartButton()
    {
        if (currentState == CutsceneState.MainMenu)
        {
            StartCoroutine(TransisiPanel(mainMenuPanel, mainMenuCanvasGroup, backstoryPanel, backstoryCanvasGroup, CutsceneState.Backstory));
        }
    }

    void Update()
    {
        // Mendeteksi Klik Kiri Player untuk melanjutkan halaman (hanya jalan di luar MainMenu & FinalFade)
        if (Input.GetMouseButtonDown(0))
        {
            CheckNextSlide();
        }
    }

    private void CheckNextSlide()
    {
        switch (currentState)
        {
            case CutsceneState.Backstory:
                // Dari Backstory lanjut ke Tutorial Gerak (WASD)
                StartCoroutine(TransisiPanel(backstoryPanel, backstoryCanvasGroup, moveTutorialPanel, moveTutorialCanvasGroup, CutsceneState.MoveTutorial));
                break;

            case CutsceneState.MoveTutorial:
                // Dari Tutorial Gerak lanjut ke Penjelasan Senjata
                StartCoroutine(TransisiPanel(moveTutorialPanel, moveTutorialCanvasGroup, weaponTutorialPanel, weaponTutorialCanvasGroup, CutsceneState.WeaponTutorial));
                break;

            case CutsceneState.WeaponTutorial:
                // Dari Penjelasan Senjata langsung memicu Fade Out Hitam & Pindah ke Permainan Asli
                currentState = CutsceneState.FinalFadeOut;
                StartCoroutine(FadeOutDanPindahScene());
                break;
        }
    }

    private IEnumerator FadeInAwal()
    {
        faderCanvasGroup.alpha = 1;
        faderCanvasGroup.blocksRaycasts = true;
        while (faderCanvasGroup.alpha > 0)
        {
            faderCanvasGroup.alpha -= Time.deltaTime * kecepatanTransisi;
            yield return null;
        }
        faderCanvasGroup.blocksRaycasts = false;
    }

    // REFACTORING: Fungsi modular universal untuk transisi antar panel UI agar kode bersih
    private IEnumerator TransisiPanel(GameObject panelLama, CanvasGroup cgLama, GameObject panelBaru, CanvasGroup cgBaru, CutsceneState stateBerikutnya)
    {
        // Kunci input sementara selama transisi biar player tidak nge-spam klik
        if (cgLama != null) cgLama.blocksRaycasts = false;
        
        // Aktifkan panel baru dan set alpha ke 0 (transparan)
        if (panelBaru != null) panelBaru.SetActive(true);
        if (cgBaru != null)
        {
            cgBaru.alpha = 0;
            cgBaru.blocksRaycasts = false; // Belum bisa diklik sampai fade selesai
        }

        // Jalankan transisi silang secara mulus
        float progress = 0;
        while (progress < 1)
        {
            progress += Time.deltaTime * kecepatanTransisi;
            if (cgLama != null) cgLama.alpha = 1 - progress;
            if (cgBaru != null) cgBaru.alpha = progress;
            yield return null;
        }

        // Pastikan nilai akhir mutlak
        if (cgLama != null) cgLama.alpha = 0;
        if (cgBaru != null) cgBaru.alpha = 1;

        // Nonaktifkan total panel lama agar hemat performa
        if (panelLama != null) panelLama.SetActive(false);
        
        // Aktifkan interaksi klik pada panel baru
        if (cgBaru != null) cgBaru.blocksRaycasts = true;

        // Perbarui status cutscene saat ini
        currentState = stateBerikutnya;
    }

    private IEnumerator FadeOutDanPindahScene()
    {
        if (faderCanvasGroup != null)
        {
            faderCanvasGroup.gameObject.SetActive(true); 
            faderCanvasGroup.alpha = 0f; 
            faderCanvasGroup.blocksRaycasts = true; // Kunci klik total
        }

        float progress = 0;
        while (progress < 1f)
        {
            progress += Time.deltaTime * kecepatanTransisi;
            if (faderCanvasGroup != null) faderCanvasGroup.alpha = progress;
            yield return null;
        }

        if (faderCanvasGroup != null) faderCanvasGroup.alpha = 1f;
        yield return new WaitForSeconds(0.1f);

        // Pindah ke Permainan Asli
        SceneManager.LoadScene("GameScene");
    }
}