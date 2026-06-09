using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panel Utama")]
    public GameObject mainMenuPanel;   // Ganti jadi GameObject agar bisa di-nonaktifkan total
    public GameObject backstoryPanel;  // Ganti jadi GameObject agar bisa di-aktifkan total
    
    [Header("Canvas Group untuk Smooth Fade")]
    public CanvasGroup mainMenuCanvasGroup;
    public CanvasGroup backstoryCanvasGroup;
    public CanvasGroup faderCanvasGroup;

    [Header("Pengaturan Transisi")]
    public float kecepatanTransisi = 2.0f;
    private bool sedangDiBackstory = false;

    void Start()
    {
        Time.timeScale = 1f;
        
        // Memastikan kondisi awal scene rapi lewat kode
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (backstoryPanel != null) backstoryPanel.SetActive(false); // Sembunyikan backstory di awal
        
        StartCoroutine(FadeInAwal());
    }

    public void ClickStartButton()
    {
        StartCoroutine(TampilkanBackstory());
    }

    void Update()
    {
        if (sedangDiBackstory && Input.GetMouseButtonDown(0))
        {
            sedangDiBackstory = false;
            StartCoroutine(FadeOutDanPindahScene());
        }
    }

    private IEnumerator FadeInAwal()
    {
        faderCanvasGroup.alpha = 1;
        while (faderCanvasGroup.alpha > 0)
        {
            faderCanvasGroup.alpha -= Time.deltaTime * kecepatanTransisi;
            yield return null;
        }
    }

    private IEnumerator TampilkanBackstory()
    {
        if (mainMenuCanvasGroup != null) mainMenuCanvasGroup.blocksRaycasts = false;

        // 1. AKTIFKAN objek BackstoryPanel terlebih dahulu sebelum di-fade in
        if (backstoryPanel != null) backstoryPanel.SetActive(true);
        backstoryCanvasGroup.alpha = 0;
        backstoryCanvasGroup.blocksRaycasts = true;

        // 2. Lakukan efek transisi tukar panel secara smooth
        float progress = 0;
        while (progress < 1)
        {
            progress += Time.deltaTime * kecepatanTransisi;
            if (mainMenuCanvasGroup != null) mainMenuCanvasGroup.alpha = 1 - progress;
            backstoryCanvasGroup.alpha = progress;
            yield return null;
        }

        // 3. Matikan total MainMenuPanel agar tidak memakan performa di latar belakang
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);

        sedangDiBackstory = true;
    }

    private IEnumerator FadeOutDanPindahScene()
{
    // 1. Pastikan objek Fader aktif secara struktural di Unity
    if (faderCanvasGroup != null)
    {
        faderCanvasGroup.gameObject.SetActive(true); 
        faderCanvasGroup.alpha = 0f; // Mulai dari transparan saat backstory selesai
        faderCanvasGroup.blocksRaycasts = true; // Kunci klik agar player tidak klik ganti scene berkali-kali
    }

    // 2. Proses menggelapkan layar secara perlahan (dari 0 menuju 1)
    float progress = 0;
    while (progress < 1f)
    {
        progress += Time.deltaTime * kecepatanTransisi;
        if (faderCanvasGroup != null)
        {
            faderCanvasGroup.alpha = progress;
        }
        yield return null;
    }

    // 3. Pastikan alpha benar-benar mentok di hitam pekat sebelum pindah scene
    if (faderCanvasGroup != null) faderCanvasGroup.alpha = 1f;

    // Kasih jeda sangat singkat (0.1 detik) di kondisi hitam pekat agar mata pemain siap
    yield return new WaitForSeconds(0.1f);

    // 4. Baru pindah ke GameScene setelah layar hitam pekat sempurna
    SceneManager.LoadScene("GameScene");
}
}