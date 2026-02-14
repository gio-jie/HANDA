using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BagUI : MonoBehaviour
{
    public static BagUI Instance;

    public Image bagImage;
    public Sprite closedBag;
    public Sprite openBag;

    void Awake()
    {
        Instance = this;
    }

    public void OpenBag()
    {
        bagImage.sprite = openBag;
    }

    public void CloseBag()
    {
        bagImage.sprite = closedBag;
    }
}