using UnityEngine;
using System.Collections;

public class WaterSystem : MonoBehaviour
{
    public static WaterSystem Instance;

    public RectTransform waterTransform;
    public float riseAmount;
    public float riseDuration = 0.8f;
    public int maxFails = 5;

    private int failCount = 0;
    private bool isRising = false;

    void Awake()
    {
        Instance = this;
    }

    public void RaiseWater()
    {
        if (isRising) return;

        failCount++;

        StartCoroutine(SmoothRise());

        if (failCount >= maxFails)
        {
            StartCoroutine(EndAfterRise());
        }
    }

    IEnumerator SmoothRise()
    {
        isRising = true;

        Vector3 startPos = waterTransform.localPosition;
        Vector3 targetPos = startPos + new Vector3(0, riseAmount, 0);

        float time = 0f;

        // OPTIONAL: Play swirl animation when water starts rising
        // Animator anim = waterTransform.GetComponent<Animator>();
        // if (anim != null)
        // {
        //     anim.SetTrigger("Swirl"); 
        // }

        while (time < riseDuration)
        {
            float t = time / riseDuration;
            t = Mathf.SmoothStep(0, 1, t);

            waterTransform.localPosition =
                Vector3.Lerp(startPos, targetPos, t);

            time += Time.deltaTime;
            yield return null;
        }
        waterTransform.localPosition = targetPos;

        isRising = false;
    }

    IEnumerator EndAfterRise()
    {
        yield return new WaitForSeconds(riseDuration);

        StarManagerLevel2.Instance.EndLevel(false);
    }
}