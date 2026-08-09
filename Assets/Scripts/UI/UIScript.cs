using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip buttonPressSound;
    
    private void Start()
    {
        SetUpOnButtonPressed();
    }

    private void SetUpOnButtonPressed()
    {
        foreach (var button in FindObjectsByType<Button>(FindObjectsInactive.Include))
            button.onClick.AddListener(OnButtonPressed);
    }

    private void OnButtonPressed()
    {
        audioSource.PlayOneShot(buttonPressSound);
    }
}
