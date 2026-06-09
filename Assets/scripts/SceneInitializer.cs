using System.Collections;
using UnityEngine;

public class SceneInitializer : MonoBehaviour
{
    public CanvasGroup faderCanvasGroup;
    public float kecepatanTransisi = 1.5f;

    void Start()
    {
        StartCoroutine(FadeInGameplay());
    }

    private IEnumerator FadeInGameplay()
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
}