using UnityEngine;

public class ObjectiveTracker : MonoBehaviour
{
    private References refs; 
    
    private static int _objectiveCounter;
    private static int _completedObjectiveCounter;

    private void Start()
    {
        refs = References.Refs;
        
        refs.gameLogic.onResetScene += () =>
        {
            _completedObjectiveCounter = 0;
        };
    }
    
    public void AddObjective()
    {
        _objectiveCounter++;
    }
    
    public void CompleteObjective()
    {
        _completedObjectiveCounter++;
        if (_completedObjectiveCounter == _objectiveCounter)
            refs.gameLogic.OnGameCompleted();
    }
}
