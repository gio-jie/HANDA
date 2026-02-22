using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CookTask : MonoBehaviour
{
    [Header("UI References")]
    public List<Button> ingredientButtons;
    public List<Image> patternSlots;
    public List<Image> checkMarks;
    public Button cookButton;
    public Button closeButton;

    [Header("Settings")]
    public float cookButtonScaleSpeed = 2f;
    public float flashDuration = 0.5f;
    public float popScale = 1.3f;
    public float popSpeed = 0.1f;
    public float patternPopDelay = 0.15f;

    [Header("SFX")]
    public AudioClip correctClickSFX;
    public AudioClip cookClickSFX;
    [Range(0f, 5f)]
    public float sfxVolume = 1.5f;

    private AudioSource audioSource;
    private List<int> correctPattern = new List<int>();
    private List<int> playerInput = new List<int>();
    private bool taskCompleted = false;
    private bool cookAnimating = false;

    // Cache original scales
    private Dictionary<Transform, Vector3> originalScales = new Dictionary<Transform, Vector3>();
    // Track running flash coroutines per button
    private Dictionary<Button, Coroutine> flashCoroutines = new Dictionary<Button, Coroutine>();
    // Track running pop coroutines per transform to prevent stacking
    private Dictionary<Transform, Coroutine> popCoroutines = new Dictionary<Transform, Coroutine>();

    // Prevent multiple wrong clicks
    private bool isFlashing = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        // Cache original scales
        foreach (Button btn in ingredientButtons)
            originalScales[btn.transform] = btn.transform.localScale;
        foreach (Image img in patternSlots)
            originalScales[img.transform] = img.transform.localScale;
        originalScales[cookButton.transform] = cookButton.transform.localScale;
    }

    void OnEnable()
    {
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(OnCloseButtonClick);
        }

        if (taskCompleted)
        {
            ShowPatternInstant();
            SetIngredientButtonsInteractable(false);
            cookButton.gameObject.SetActive(false);
            SetCheckMarks(true);
        }
        else
        {
            ResetTask();
        }
    }

    #region Pattern Generation

    void GeneratePattern()
    {
        correctPattern.Clear();
        List<int> temp = new List<int> { 0, 1, 2, 3 };

        for (int i = 0; i < 4; i++)
        {
            int index = Random.Range(0, temp.Count);
            correctPattern.Add(temp[index]);
            temp.RemoveAt(index);
        }

        StartCoroutine(ShowPatternWithPop());
        ResetCheckMarks();
    }

    IEnumerator ShowPatternWithPop()
    {
        for (int i = 0; i < patternSlots.Count; i++)
        {
            patternSlots[i].gameObject.SetActive(i < correctPattern.Count);

            if (i < correctPattern.Count)
            {
                patternSlots[i].sprite = ingredientButtons[correctPattern[i]].image.sprite;

                CanvasGroup cg = patternSlots[i].GetComponent<CanvasGroup>();
                if (cg == null)
                    cg = patternSlots[i].gameObject.AddComponent<CanvasGroup>();

                cg.alpha = 0f;
                patternSlots[i].transform.localScale = originalScales[patternSlots[i].transform];
            }
        }

        for (int i = 0; i < correctPattern.Count; i++)
        {
            Image slot = patternSlots[i];
            CanvasGroup cg = slot.GetComponent<CanvasGroup>();
            cg.alpha = 1f;

            yield return StartCoroutine(SafePop(slot.transform));
            yield return new WaitForSeconds(patternPopDelay);
        }
    }

    void ShowPatternInstant()
    {
        for (int i = 0; i < patternSlots.Count; i++)
        {
            patternSlots[i].gameObject.SetActive(i < correctPattern.Count);

            if (i < correctPattern.Count)
            {
                patternSlots[i].sprite = ingredientButtons[correctPattern[i]].image.sprite;

                CanvasGroup cg = patternSlots[i].GetComponent<CanvasGroup>();
                if (cg == null)
                    cg = patternSlots[i].gameObject.AddComponent<CanvasGroup>();

                cg.alpha = 1f;
                patternSlots[i].transform.localScale = originalScales[patternSlots[i].transform];
            }
        }
    }

    #endregion

    #region Ingredient Click

    public void ClickIngredient(int index)
    {
        if (taskCompleted || isFlashing) return;

        playerInput.Add(index);

        // Start pop safely
        if (popCoroutines.ContainsKey(ingredientButtons[index].transform))
            StopCoroutine(popCoroutines[ingredientButtons[index].transform]);
        popCoroutines[ingredientButtons[index].transform] = StartCoroutine(SafePop(ingredientButtons[index].transform));

        int slotIndex = playerInput.Count - 1;

        // Wrong input
        if (playerInput[slotIndex] != correctPattern[slotIndex])
        {
            StartCoroutine(HandleWrongClick(ingredientButtons[index]));
            return;
        }

        // Correct input
        if (slotIndex < checkMarks.Count)
            checkMarks[slotIndex].gameObject.SetActive(true);

        if (correctClickSFX != null)
            audioSource.PlayOneShot(correctClickSFX, sfxVolume);

        if (playerInput.Count == correctPattern.Count)
        {
            cookButton.gameObject.SetActive(true);
            SetIngredientButtonsInteractable(false);

            if (!cookAnimating)
                StartCoroutine(PulseCookButton());
        }
    }

    IEnumerator HandleWrongClick(Button btn)
    {
        isFlashing = true;

        AudioManager.instance.PlaySFX(AudioManager.instance.warningSound);

        if (flashCoroutines.ContainsKey(btn) && flashCoroutines[btn] != null)
            StopCoroutine(flashCoroutines[btn]);

        flashCoroutines[btn] = StartCoroutine(FlashRed(btn));

        playerInput.Clear();
        yield return new WaitForSeconds(flashDuration);

        GeneratePattern();
        isFlashing = false;
    }

    IEnumerator FlashRed(Button btn)
    {
        Color original = btn.image.color;
        btn.image.color = Color.red;
        yield return new WaitForSeconds(flashDuration);
        btn.image.color = original;
        flashCoroutines.Remove(btn);
    }

    #endregion

    #region Pop Button

    IEnumerator SafePop(Transform button)
    {
        Vector3 original = originalScales[button];
        Vector3 target = original * popScale;
        float timer = 0f;

        while (timer < popSpeed)
        {
            timer += Time.deltaTime;
            button.localScale = Vector3.Lerp(original, target, timer / popSpeed);
            yield return null;
        }

        timer = 0f;
        while (timer < popSpeed)
        {
            timer += Time.deltaTime;
            button.localScale = Vector3.Lerp(target, original, timer / popSpeed);
            yield return null;
        }

        button.localScale = original;
        popCoroutines.Remove(button);
    }

    #endregion

    #region Cook Button Animation

    IEnumerator PulseCookButton()
    {
        cookAnimating = true;
        Vector3 originalScale = originalScales[cookButton.transform];

        while (cookButton.gameObject.activeSelf)
        {
            float scale = 1f + Mathf.Sin(Time.time * cookButtonScaleSpeed) * 0.1f;
            cookButton.transform.localScale = originalScale * scale;
            yield return null;
        }

        cookButton.transform.localScale = originalScale;
        cookAnimating = false;
    }

    public void OnCookButtonClick()
    {
        if (taskCompleted) return;

        taskCompleted = true;

        if (cookClickSFX != null)
            audioSource.PlayOneShot(cookClickSFX, sfxVolume);

        cookButton.gameObject.SetActive(false);
        SetCheckMarks(true);

        StarManagerLevel9.Instance.CompleteTask("Cook");

        StartCoroutine(CloseAfterSound());
    }

    IEnumerator CloseAfterSound()
    {
        yield return new WaitForSeconds(1f);
        Level9PanelManager.Instance.CloseCurrentPanel();
    }

    #endregion

    #region Close Button

    public void OnCloseButtonClick()
    {
        if (!taskCompleted)
        {
            ResetTask();
        }

        Level9PanelManager.Instance.CloseCurrentPanel();
    }

    #endregion

    #region Reset & Checkmarks

    public void ResetTask()
    {
        playerInput.Clear();
        GeneratePattern();
        SetIngredientButtonsInteractable(true);
        cookButton.gameObject.SetActive(false);
        ResetCheckMarks();
    }

    void ResetCheckMarks()
    {
        foreach (Image img in checkMarks)
            img.gameObject.SetActive(false);
    }

    void SetCheckMarks(bool value)
    {
        foreach (Image img in checkMarks)
            img.gameObject.SetActive(value);
    }

    void SetIngredientButtonsInteractable(bool value)
    {
        foreach (Button btn in ingredientButtons)
            btn.interactable = value;
    }

    public bool IsCompleted()
    {
        return taskCompleted;
    }

    #endregion
}