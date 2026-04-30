using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ResultDetails : MonoBehaviour
{
    public static ResultDetails Instance;

    public GameObject panel;
    public Image image;
    public TMP_Text descriptionText;
    public RectTransform contentRoot;

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void Show(PlayerAnswerData data)
    {
        panel.SetActive(true);

        image.sprite = data.chosenImage;
        descriptionText.text = data.description;

        StartCoroutine(PopIn());
    }

    public void Close()
    {
        panel.SetActive(false);
    }

    IEnumerator PopIn()
    {
        contentRoot.localScale = Vector3.zero;

        float t = 0f;
        float duration = 0.25f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float scale = EaseOutBack(t / duration);

            contentRoot.localScale = Vector3.one * scale;
            yield return null;
        }

        contentRoot.localScale = Vector3.one;
    }

    float EaseOutBack(float t)
    {
        float c1 = 1.70158f;
        float c2 = c1 * 1.525f;
        return 1 + (c2 * Mathf.Pow(t - 1, 3) + c1 * Mathf.Pow(t - 1, 2));
    }
}