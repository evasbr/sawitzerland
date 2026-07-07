using System.Collections;
using UnityEngine;

public class SawitGrowth : MonoBehaviour
{
    public GameObject nextStage;
    public float growTime = 7f;
    public float xpReward = 6f;

    private XPSystem xpSystem;

    private void Awake()
    {
        xpSystem = FindObjectOfType<XPSystem>();
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(growTime);

        if (xpSystem != null)
            xpSystem.AddXP(xpReward);

        if (nextStage != null)
        {
            Instantiate(nextStage, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}