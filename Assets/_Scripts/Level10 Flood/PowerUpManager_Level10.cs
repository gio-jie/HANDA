using UnityEngine;

public class PowerUpManager_Level10 : MonoBehaviour
{
    public static PowerUpManager_Level10 Instance;

    private bool hasJournal = false;
    private bool hasHardHat = false;

    void Awake()
    {
        Instance = this;
    }

    public void GiveRandomPowerUp(bool guaranteed)
    {
        if (!guaranteed && Random.value > 0.5f) return;

        int random = Random.Range(0, 3);

        switch (random)
        {
            case 0:
                hasJournal = true;
                break;

            case 1:
                StarManagerLevel10.Instance.AddTime(10f);
                break;

            case 2:
                hasHardHat = true;
                break;
        }
    }

    public bool UseJournal()
    {
        if (hasJournal)
        {
            hasJournal = false;
            return true;
        }
        return false;
    }

    public bool SkipNextHazard()
    {
        if (hasHardHat)
        {
            hasHardHat = false;
            return true;
        }
        return false;
    }
}