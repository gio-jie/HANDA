using UnityEngine;
using System.Collections;

public class Table : MonoBehaviour
{
    public bool isCorrect;
    public Transform snapPoint;

    [Header("Cross Mark")]
    public Transform crossMark;

    private Vector3 originalScale;
    private Vector3 targetScale;
    private Vector3 crossOriginalScale;

    [Header("Hover Settings")]
    public float hoverMultiplier = 1.1f;
    public float smoothSpeed = 8f;
    public float bounceAmount = 0.05f;

    private Coroutine scaleRoutine;
    private Coroutine crossRoutine;

    [Header("Warning Text")]
    public string warningText;

    private bool isLocked = false;
    private bool hoverEnabled = true;

    void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;

        if (crossMark != null)
        {
            crossOriginalScale = crossMark.localScale; // 🔥 FIX: store REAL size
            crossMark.localScale = Vector3.zero;
        }
    }

    void Update()
    {
        // 🔥 ONLY APPLY SCALE IF NOT LOCKED
        if (!isLocked)
        {
            transform.localScale = Vector3.Lerp(
                transform.localScale,
                targetScale,
                Time.deltaTime * smoothSpeed
            );
        }
    }

    public void HandleDrop(PlayerController player)
    {
        if (player.currentState != GameState.Dragging) return;

        if (isCorrect)
        {
            StarManagerEarthquake3.Instance.RegisterCorrectItem();

            LockTable();

            player.SetUnderTable(snapPoint.position);

            Camera cam = Camera.main;
            if (cam != null)
            {
                var follow = cam.GetComponent<CameraFollowEarthquake3>();
                if (follow != null)
                    follow.ZoomToPlayer(player.transform);
            }

            if (HoldManager.Instance != null)
                HoldManager.Instance.StartHold();
        }
        else
        {
            StarManagerEarthquake3.Instance.RegisterWrongItem();
            StartCoroutine(WrongTable(player));
        }
    }

    IEnumerator WrongTable(PlayerController player)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        LockTable();

        if (UIManager.Instance != null)
            UIManager.Instance.ShowTemporaryInstruction(warningText);

        // ❌ CROSS POP IN
        if (crossMark != null)
        {
            if (crossRoutine != null) StopCoroutine(crossRoutine);
            crossRoutine = StartCoroutine(PopCross(true));
        }

        // flash red
        for (int i = 0; i < 3; i++)
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(0.15f);
            sr.color = Color.white;
            yield return new WaitForSeconds(0.15f);
        }

        player.SnapBack();

        // 🔥 WAIT A SINGLE MOMENT BEFORE EXIT ANIMATION
        yield return new WaitForSeconds(0.1f);

        // ❌ START EXIT FOR BOTH AT SAME TIME
        if (crossMark != null)
        {
            if (crossRoutine != null) StopCoroutine(crossRoutine);
            crossRoutine = StartCoroutine(PopCross(false));
        }

        // 🔥 FORCE TABLE TO START RETURNING IMMEDIATELY
        UnlockTable(); 
        targetScale = originalScale;

        // small sync delay so both finish visually together
        yield return new WaitForSeconds(0.25f);

        if (UIManager.Instance != null)
            UIManager.Instance.RestoreMissionText();
    }

    public void SetHover(bool isHovering)
    {
        if (!hoverEnabled || isLocked) return;

        if (scaleRoutine != null)
            StopCoroutine(scaleRoutine);

        scaleRoutine = StartCoroutine(ScaleBounce(isHovering));
    }

    IEnumerator ScaleBounce(bool isHovering)
    {
        Vector3 baseTarget = isHovering
            ? originalScale * hoverMultiplier
            : originalScale;

        Vector3 overshoot = isHovering
            ? baseTarget * (1f + bounceAmount)
            : originalScale;

        float t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime * 10f;

            transform.localScale = Vector3.Lerp(
                transform.localScale,
                overshoot,
                t
            );

            yield return null;
        }

        targetScale = baseTarget;
    }

    IEnumerator PopCross(bool show)
    {
        float t = 0;

        Vector3 start = crossMark.localScale;

        // 🔥 FIX: use REAL scale, not Vector3.one
        Vector3 end = show ? crossOriginalScale : Vector3.zero;
        Vector3 overshoot = show ? crossOriginalScale * 1.2f : Vector3.zero;

        while (t < 1f)
        {
            t += Time.deltaTime * 10f;

            crossMark.localScale = Vector3.Lerp(
                start,
                show ? overshoot : end,
                t
            );

            yield return null;
        }

        crossMark.localScale = end;
    }

    void LockTable()
    {
        isLocked = true;
        hoverEnabled = false;

        if (scaleRoutine != null)
            StopCoroutine(scaleRoutine);

        targetScale = originalScale;
    }

    void UnlockTable()
    {
        isLocked = false;
        hoverEnabled = true;

        targetScale = originalScale;
    }
}