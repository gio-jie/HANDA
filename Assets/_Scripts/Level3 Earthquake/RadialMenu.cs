using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RadialMenu : MonoBehaviour
{
    public Button runBtn;
    public Button standBtn;
    public Button dropBtn;

    public Image runImg;
    public Image standImg;

    public PlayerController player;

    void OnEnable()
    {
        transform.localScale = Vector3.zero;
        StartCoroutine(PopAnimation());

        UIManager.Instance.SetInstruction("Choose the correct action during an earthquake");

    }

    IEnumerator PopAnimation()
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 6f;
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            yield return null;
        }
    }

    void Start()
    {
        runBtn.onClick.AddListener(() => Wrong(runImg));
        standBtn.onClick.AddListener(() => Wrong(standImg));
        dropBtn.onClick.AddListener(Correct);
    }

    void Correct()
    {
        StarManagerEarthquake3.Instance.RegisterCorrectItem();

        player.SetToDrop();
        gameObject.SetActive(false);

        UIManager.Instance.SetInstruction("Drag your character and find a place to hide");
    }

    void Wrong(Image img)
    {
        StarManagerEarthquake3.Instance.RegisterWrongItem();
        StartCoroutine(FlashRed(img));
    }

    IEnumerator FlashRed(Image img)
    {
        for (int i = 0; i < 3; i++)
        {
            img.color = Color.red;
            yield return new WaitForSeconds(0.15f);
            img.color = Color.white;
            yield return new WaitForSeconds(0.15f);
        }
    }
}