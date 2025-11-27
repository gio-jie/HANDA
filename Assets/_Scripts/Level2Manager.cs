using UnityEngine;
using TMPro;

public class Level2Manager : MonoBehaviour
{
    public int totalTasks = 5; // 3 Nails + 2 Tapes
    public int tasksDone = 0;
    public TMP_Text scoreText; // Optional: Para makita progress

    public void TaskCompleted()
    {
        tasksDone++;
        Debug.Log("Repairs: " + tasksDone + "/" + totalTasks);
        
        if(scoreText != null)
             scoreText.text = "Repairs: " + tasksDone + "/" + totalTasks;

        if (tasksDone >= totalTasks)
        {
            Debug.Log("LEVEL 2 COMPLETE! HOUSE SECURED!");
            // Dito natin ilalagay ang Win Panel later
        }
    }
}