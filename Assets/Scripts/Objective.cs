using UnityEngine;

public abstract class Objective : MonoBehaviour
{
    [SerializeField] private References refs;
    
    private static int _objectiveCounter;
    private static int _completedObjectiveCounter;
    
    private bool objectiveIsCompleted;
    
    protected virtual void Awake()
    {
        _objectiveCounter++;
    }

    protected virtual void Start()
    {
        refs = References.Refs;

        refs.gameLogic.onCompleted += () =>
        {
            _completedObjectiveCounter = 0;
            objectiveIsCompleted = false;
        };
    }
    
    protected void CompleteObjective()
    {
        if (objectiveIsCompleted)
            return;
        objectiveIsCompleted = true;
        
        _completedObjectiveCounter++;
        if (_completedObjectiveCounter == _objectiveCounter)
            refs.gameLogic.OnGameCompleted();
    }
}
