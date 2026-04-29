using UnityEngine;
using UnityEngine.UI;

public class HoldManager : MonoBehaviour
{
    public static HoldManager Instance;

    public GameObject holdUI;
    public Slider slider;

    [Header("Settings")]
    public float holdDuration = 5f; // total time to fill

    private bool holding = false;
    private bool isActive = false; // only true when under table

    public PlayerController player;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        holdUI.SetActive(false);
        slider.value = 0f;
    }

    // ✅ Called when player is correctly under the table
    public void StartHold()
    {
        isActive = true;
        holdUI.SetActive(true);
        slider.value = 0f;
    }

    public void OnHoldDown()
    {
        if (!isActive) return;

        holding = true;
        player.SetHold();
    }

    public void OnHoldUp()
    {
        if (!isActive) return;

        holding = false;
        player.SetCover();
    }

    void Update()
    {
        if (!isActive) return;

        float rate = Time.deltaTime / holdDuration;

        if (holding)
        {
            slider.value += rate;
        }
        else
        {
            slider.value -= rate;
        }

        slider.value = Mathf.Clamp01(slider.value);

        // ✅ Prevent multiple win triggers
        if (slider.value >= 1f)
        {
            isActive = false;
            holding = false;

            StarManagerEarthquake3.Instance.TriggerWin();
        }
    }
}