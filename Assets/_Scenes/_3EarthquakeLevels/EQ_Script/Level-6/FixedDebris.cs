using UnityEngine;
using System.Collections;

public class FixedDebris : MonoBehaviour
{
    public int laneIndex; // 0=Left, 1=Center, 2=Right
    public bool isCrack = false; // Check(True) kung crack ito para walang warning

    private bool hasWarned = false;
    private bool isHit = false;
    private RectTransform rect;
    private RunnerPlayer player;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        player = FindFirstObjectByType<RunnerPlayer>();
    }

    void Update()
    {
        if (Level6ManagerEQ.instance != null && !Level6ManagerEQ.instance.isGameActive) return;
        if (player == null) return;

        // Kunin ang eksaktong layo ng debris kay Jobert sa screen
        float myY = rect.position.y;
        float playerY = player.GetComponent<RectTransform>().position.y;
        float distance = myY - playerY;

        // 1. DETECTOR: Mag-warning kung malapit na pumasok sa screen (hal. 1000 pixels ang layo)
        if (!hasWarned && !isCrack && distance < 1000f && distance > 200f)
        {
            hasWarned = true;
            Level6ManagerEQ.instance.ShowWarning(laneIndex);
        }

        // 2. HITBOX: Pag bumangga na kay Jobert
        if (!isHit && distance < 100f && distance > -100f)
        {
            if (player.GetCurrentLane() == laneIndex)
            {
                isHit = true;
                Level6ManagerEQ.instance.TakeDamage();
                
                // Masisira ang poste/puno, pero maiiwan ang bitak sa kalsada
                if (!isCrack) gameObject.SetActive(false); 
            }
        }
    }
}