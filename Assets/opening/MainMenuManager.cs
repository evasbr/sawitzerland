using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public CanvasGroup faderCanvasGroup; 
    public float kecepatanTransisi = 1.5f;

    void Start()
    {
        StartCoroutine(FadeIn());
    }

    public void ClickStartButton()
    {
        StartCoroutine(FadeOutAndMoveScene());
    }

    private IEnumerator FadeIn()
    {
        faderCanvasGroup.alpha = 1;
        while (faderCanvasGroup.alpha > 0)
        {
            faderCanvasGroup.alpha -= Time.deltaTime * kecepatanTransisi;
            yield return null;
        }
        faderCanvasGroup.blocksRaycasts = false; 
    }

    private IEnumerator FadeOutAndMoveScene()
    {
        faderCanvasGroup.blocksRaycasts = true;
        
        yield return new WaitForSeconds(0.2f); 

        while (faderCanvasGroup.alpha < 1)
        {
            faderCanvasGroup.alpha += Time.deltaTime * kecepatanTransisi;
            yield return null;
        }

        // Setelah layar hitam pekat, baru pindah scene
        SceneManager.LoadScene("GameScene"); 
    }
}