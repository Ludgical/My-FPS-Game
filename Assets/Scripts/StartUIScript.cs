using UnityEngine;

public class StartUIScript : MonoBehaviour
{
    private References refs;

    private void Start()
    {
        refs = References.Refs;
        
        //Show UI on game start and hide on game completed
        refs.gameLogic.onPlay += () => gameObject.SetActive(false);
        refs.gameLogic.onResetScene += () => gameObject.SetActive(true);
    }
}
