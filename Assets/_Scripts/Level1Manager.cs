using UnityEngine;
using UnityEngine.UI; 

public class Level1Manager : MonoBehaviour
{
    public int currentScore = 0;
    public int itemsNeeded = 3; // Kailangan natin ng 3 items (Flashlight, Radio, Food)
    public GameObject winPanel; // Ito yung lalabas kapag nanalo ka

    // Function kapag pinindot ang TAMANG item
    public void ClickCorrectItem(GameObject itemClicked)
    {
        // 1. Dagdag score
        currentScore = currentScore + 1;
        Debug.Log("Score: " + currentScore);

        // 2. Mawawala ang item sa screen (parang nilagay sa bag)
        itemClicked.SetActive(false);

        // 3. Check kung kumpleto na ang items
        if (currentScore >= itemsNeeded)
        {
            ShowWinScreen();
        }
    }

    // Function kapag pinindot ang MALING item
    public void ClickWrongItem()
    {
        Debug.Log("Mali yan! Hindi kailangan sa bag.");
        // Sa susunod, pwede tayo maglagay ng sound effect dito
    }

    // Function para ipakita ang "Level Complete"
    void ShowWinScreen()
    {
        Debug.Log("LEVEL COMPLETE!");
        winPanel.SetActive(true); // Ipakita ang Win Panel
    }
}