using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class RescueUnitDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Unit Settings")]
    public string unitType; // E.g., "Firetruck", "Ambulance"
    public float cooldownTime = 5f; // Gaano katagal bago magamit ulit

    [Header("UI & Visuals")]
    public Image unitImage; // Ang mismong picture ng truck
    public Image cooldownOverlay; // Isang gray na image na naka-patong
    public TMP_Text cooldownText; // Timer text na lilitaw sa ibabaw ng gray overlay

    private Vector3 startPos;
    private Transform startParent;
    private CanvasGroup canvasGroup;
    [HideInInspector] public bool isCoolingDown = false;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (!canvasGroup) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // Siguraduhing normal ang kulay sa simula
        if (cooldownOverlay) cooldownOverlay.gameObject.SetActive(false);
        if (cooldownText) cooldownText.text = "";
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isCoolingDown) return; // Bawal i-drag pag naka-cooldown!

        startPos = transform.position;
        startParent = transform.parent;
        
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isCoolingDown) return;
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isCoolingDown) return;
        
        canvasGroup.blocksRaycasts = true;
        transform.SetParent(startParent);
        transform.position = startPos;
    }

    // --- TATAWAGIN NG ZONE KAPAG TAMA ANG PAG-DROP ---
    public void StartCooldown()
    {
        if (!isCoolingDown)
        {
            StartCoroutine(CooldownRoutine());
        }
    }

    IEnumerator CooldownRoutine()
    {
        isCoolingDown = true;
        canvasGroup.blocksRaycasts = false; // Bawal muna i-click
        unitImage.color = Color.gray; // Gawing gray ang truck

        if (cooldownOverlay) cooldownOverlay.gameObject.SetActive(true);
        if (cooldownText) cooldownText.gameObject.SetActive(true);

        float timer = cooldownTime;
        while (timer > 0)
        {
            if (cooldownText) cooldownText.text = Mathf.CeilToInt(timer).ToString();
            timer -= Time.deltaTime;
            yield return null;
        }

        // Tapos na ang cooldown, ibalik sa normal!
        isCoolingDown = false;
        canvasGroup.blocksRaycasts = true;
        unitImage.color = Color.white;
        
        if (cooldownOverlay) cooldownOverlay.gameObject.SetActive(false);
        if (cooldownText) cooldownText.text = "";
    }
}