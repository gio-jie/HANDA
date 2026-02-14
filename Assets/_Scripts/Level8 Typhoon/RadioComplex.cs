using UnityEngine;

public class RadioComplex : MonoBehaviour
{
    // MGA KAILANGAN NATIN
    public GameObject batteryOnTable;  // Yung battery na pupulutin
    public GameObject batteryOnRadio;  // Yung battery na lilitaw sa radyo
    public GameObject tunerNeedle;     // Yung karayom na iikot
    public GameObject winLight;        // Iilaw pag nanalo

    // STATUS NG LARO
    private bool hasPower = false;     // May kuryente na ba?
    private float currentAngle = 0f;   // Anggulo ng Tuner
    
    // SETTINGS
    public float rotateSpeed = 100f;   // Bilis ng ikot
    public float targetAngleMin = 80f; // Dapat tumama sa 80 degrees
    public float targetAngleMax = 100f; // Hanggang 100 degrees

    void Start()
    {
        // Siguraduhing nakapatay ang ilaw at battery sa radyo sa simula
        batteryOnRadio.SetActive(false); 
        winLight.GetComponent<SpriteRenderer>().color = Color.red; 
    }

    void Update()
    {
        // LOGIC 1: Check kung nanalo na
        CheckSignal();
    }

    // TATAWAGIN KAPAG CLINICK ANG BATTERY SA MESA
    public void PickUpBattery()
    {
        hasPower = true;
        batteryOnTable.SetActive(false); // Mawawala sa mesa
        batteryOnRadio.SetActive(true);  // Lilitaw sa radyo
        Debug.Log("POWER ON! Rinig mo na ang static.");
    }

    // TATAWAGIN NG BUTTONS (Left/Right)
    public void RotateTuner(float direction)
    {
        // Kung wala pang battery, wag umikot
        if (!hasPower) 
        {
            Debug.Log("Wala pang battery!");
            return;
        }

        // I-rotate ang needle (Z-axis)
        // direction is 1 (Left) or -1 (Right)
        tunerNeedle.transform.Rotate(0, 0, direction * rotateSpeed * Time.deltaTime);
    }

    void CheckSignal()
    {
        if (!hasPower) return;

        // Kunin ang current angle ng needle
        float zAngle = tunerNeedle.transform.rotation.eulerAngles.z;

        // Unity complication: Ang angles ay 0 to 360. 
        // Aayusin natin para madali basahin.
        if (zAngle > 180) zAngle -= 360;

        // LOGIC 2: Kung ang angle ay pasok sa Target Area
        if (zAngle >= targetAngleMin && zAngle <= targetAngleMax)
        {
            winLight.GetComponent<SpriteRenderer>().color = Color.green;
            Debug.Log("CLEAR SIGNAL: 'Stay indoors! Stay safe!'");
        }
        else
        {
            winLight.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }
}