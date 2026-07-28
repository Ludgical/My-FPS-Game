using UnityEngine;

public class ObjectiveTracker : MonoBehaviour
{
    private References refs; 
    
    private static int _objectiveCounter;

    private void Start()
    {
        refs = References.Refs;
    }
    
    public void AddObjective()
    {
        _objectiveCounter++;
    }
    
    public void CompleteObjective()
    {
        _objectiveCounter--;
        if (_objectiveCounter == 0)
            refs.gameLogic.OnGameCompleted();
    }
}
