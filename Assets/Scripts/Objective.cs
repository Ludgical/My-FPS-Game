using UnityEngine;

public abstract class Objective : MonoBehaviour
{
    private static int _objectiveCounter;
    private static int _completedObjectiveCounter;
    
    private bool objectiveIsCompleted;
    
    protected void Awake()
    {
        _objectiveCounter++;
    }
    
    protected void CompleteObjective()
    {
        if (objectiveIsCompleted)
            return;
        objectiveIsCompleted = true;
        
        _completedObjectiveCounter++;
        if (_completedObjectiveCounter == _objectiveCounter)
        {
            _completedObjectiveCounter = 0;
            References.Refs.gameLogic.OnGameCompleted();
        }
    }
}
