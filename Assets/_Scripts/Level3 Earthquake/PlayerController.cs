using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public SpriteRenderer sr;
    public BoxCollider2D boxCollider;

    [Header("Sprites")]
    public Sprite dropSprite;
    public Sprite coverSprite;
    public Sprite holdSprite;

    [Header("Scale Settings")]
    public float introScale = 0.51f;
    public float dropScale = 0.13f;
    public float underTableScale = 0.06f;

    public GameState currentState = GameState.Intro;

    private bool isDragging = false;
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;

        transform.localScale = Vector3.one * introScale;

        UpdateCollider();
    }

    void OnMouseDown()
    {
        if (currentState != GameState.Dragging) return;
        isDragging = true;
    }

    void OnMouseUp()
    {
        if (!isDragging) return;

        isDragging = false;
        TryDrop();
    }

    void Update()
    {
        if (isDragging)
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = pos;

            DetectTableHover();
        }
    }

    void TryDrop()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.5f);

        foreach (var hit in hits)
        {
            Table table = hit.GetComponent<Table>();
            if (table != null)
            {
                table.HandleDrop(this);
                return;
            }
        }

        SnapBack();
    }

    void DetectTableHover()
    {
        if (TableManager.Instance == null) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.5f);

        Table hoveredTable = null;

        foreach (var hit in hits)
        {
            Table t = hit.GetComponent<Table>();
            if (t != null)
            {
                hoveredTable = t;
                break;
            }
        }

        TableManager.Instance.HighlightTable(hoveredTable);
    }

    public void SetToDrop()
    {
        sr.sprite = dropSprite;

        transform.localScale = Vector3.one * dropScale;

        UpdateCollider();

        currentState = GameState.Dragging;

        if (TableManager.Instance != null)
            TableManager.Instance.EnableHover(true);
    }

    public void SnapBack()
    {
        transform.position = startPos;
    }

    public void SetUnderTable(Vector3 pos)
    {
        transform.position = pos;

        sr.sprite = coverSprite;

        transform.localScale = Vector3.one * underTableScale;

        UpdateCollider();

        currentState = GameState.UnderTable;

        isDragging = false;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.SetInstruction("Press the 'Hold' button until it is safe!");
        }

        if (TableManager.Instance != null)
            TableManager.Instance.EnableHover(false);
    }

    public void SetHold()
    {
        sr.sprite = holdSprite;
        UpdateCollider();
        currentState = GameState.Holding;
    }

    public void SetCover()
    {
        sr.sprite = coverSprite;
        UpdateCollider();
    }

    void UpdateCollider()
    {
        if (sr.sprite == null || boxCollider == null) return;

        boxCollider.size = sr.sprite.bounds.size;
        boxCollider.offset = sr.sprite.bounds.center;
    }
}