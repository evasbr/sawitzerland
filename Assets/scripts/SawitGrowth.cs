using System.Collections;
using UnityEngine;

public class SawitGrowth : MonoBehaviour
{
    public GameObject nextStage;
    public float growTime = 7f;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(growTime);

        if (nextStage != null)
        {
            Instantiate(nextStage, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}