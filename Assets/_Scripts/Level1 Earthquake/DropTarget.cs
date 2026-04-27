using UnityEngine;

public class DropTarget : MonoBehaviour
{
    public string requiredToolID;

    public GameObject fixedObject;
    public Transform popPoint;

    public float popScale = 1.2f;
    public bool isCompleted = false;
    public TargetTaskIcon taskIcon;

    public AudioClip correctSFX;
    public float sfxVolume = 1f;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
    }
}