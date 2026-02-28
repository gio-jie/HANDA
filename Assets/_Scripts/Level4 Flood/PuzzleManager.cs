using UnityEngine;
using UnityEngine.UI;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;

    [Header("References")]
    public PuzzlePiece[] pieces;
    public Transform dragLayer;
    public ScrollRect scrollRect;

    [Header("UI")]
    public GameObject completionPanel;
    public GameObject referencePanel;

    private bool showingEdgesOnly = false;
    public AudioClip completionSFX;
    private AudioSource audioSource;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ShufflePieces();

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = 1f;
    }

    void ShufflePieces()
    {
        for (int i = 0; i < pieces.Length; i++)
        {
            int randomIndex = Random.Range(0, pieces.Length);
            pieces[i].transform.SetSiblingIndex(randomIndex);
        }
    }

    public bool IsPuzzleComplete()
    {
        foreach (PuzzlePiece piece in pieces)
        {
            if (!piece.IsPlaced())
                return false;
        }

        return true;
    }

    public void ShowCompletionPanel()
    {
        audioSource.PlayOneShot(completionSFX);
        StarManagerLevel4.Instance.OnPuzzleCompleted();
        //completionPanel.SetActive(true);
        StartCoroutine(AnimatePanelPop());
    }

    private System.Collections.IEnumerator AnimatePanelPop()
    {
        RectTransform panel = completionPanel.GetComponent<RectTransform>();

        float duration = 0.4f;
        float timer = 0f;

        Vector3 overshoot = Vector3.one * 1.15f;

        while (timer < duration)
        {
            float t = timer / duration;

            float scale = Mathf.Lerp(0f, 1.15f, Mathf.Sin(t * Mathf.PI * 0.5f));

            panel.localScale = Vector3.one * scale;

            timer += Time.deltaTime;
            yield return null;
        }

        panel.localScale = overshoot;

        float bounceDuration = 0.15f;
        timer = 0f;

        while (timer < bounceDuration)
        {
            float t = timer / bounceDuration;
            panel.localScale = Vector3.Lerp(overshoot, Vector3.one, t);
            timer += Time.deltaTime;
            yield return null;
        }

        panel.localScale = Vector3.one;
    }


    public void ToggleReferenceImage()
    {
        referencePanel.SetActive(true);
    }

    public void CloseReferenceImage()
    {
        referencePanel.SetActive(false);
    }

    public void ShowEdgePiecesOnly()
    {
        showingEdgesOnly = !showingEdgesOnly;

        foreach (PuzzlePiece piece in pieces)
        {
            if (!piece.isEdgePiece && !piece.IsPlaced())
                piece.gameObject.SetActive(showingEdgesOnly);
            else
                piece.gameObject.SetActive(true);
        }

        RefreshScrollLayout();
    }

    public void CheckEdgeCompletion()
    {
        bool allEdgesPlaced = true;
        foreach (PuzzlePiece piece in pieces)
        {
            if (piece.isEdgePiece && !piece.IsPlaced())
            {
                allEdgesPlaced = false;
                break;
            }
        }

        if (allEdgesPlaced)
        {
            foreach (PuzzlePiece piece in pieces)
            {
                if (!piece.IsPlaced())
                    piece.gameObject.SetActive(true);
            }
            showingEdgesOnly = false;
        }
    }

    public void RefreshScrollLayout()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(
            scrollRect.content.GetComponent<RectTransform>()
        );

        scrollRect.verticalNormalizedPosition = 1f;
    }
}