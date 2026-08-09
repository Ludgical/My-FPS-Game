using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLogic : MonoBehaviour
{
    private References refs;
    
    public Action onPlay;
    public Action onPlayTimed;
    public Action onPlayTutorial;
    public Action onCompleted;

    [NonSerialized] public bool gameIsOn;
    private GameMode gameMode;
    private float startTime;

    private void Start()
    {
        refs = References.Refs;

        SetUpOnPlayTimed();
        SetUpOnPlayTutorial();
        SetUpOnPlay();
        SetUpOnCompleted();
    }

    private void SetUpOnPlayTimed()
    {
        onPlayTimed += () =>
        {
            gameMode = GameMode.Timed;
            startTime = Time.time;
            onPlay?.Invoke();
        };
    }

    private void SetUpOnPlayTutorial()
    {
        onPlayTutorial += () =>
        {
            gameMode = GameMode.Tutorial;
            onPlay?.Invoke();
        };
    }

    private void SetUpOnPlay()
    {
        onPlay += () =>
        {
            gameIsOn = true;
            refs.gameUtil.HideCursor();
        };
    }

    private void SetUpOnCompleted()
    {
        onCompleted += () =>
        {
            if (gameMode == GameMode.Timed)
                OnFinishedTimed();
            if (gameMode == GameMode.Tutorial)
                OnFinishTutorial();
        };
    }

    public void GameCompleted()
    {
        onCompleted?.Invoke();
        StartCoroutine(ResetScene());
        return;

        IEnumerator ResetScene()
        {
            yield return new WaitForSeconds(refs.gameData.completedToResetDelay);
            Settings.Save();
            GameUtil.FirstTime = false;
            SceneManager.LoadScene("GameScene");
        }
    }

    private void OnFinishedTimed()
    {
        var missionTime = (int)((Time.time - startTime) * 100);
        var bestTime = GetBestTime();
        var isNewBest = bestTime == -1 || missionTime < GetBestTime();
        if (isNewBest)
            SetBestTime(missionTime);
        
        GameUtil.MissionTime = missionTime;
        GameUtil.IsNewBestTime = isNewBest;
        GameUtil.LastGameMode = GameMode.Timed;
    }

    private void OnFinishTutorial()
    {
        GameUtil.LastGameMode = GameMode.Tutorial;
    }
    
    public int GetBestTime()
    {
        return PlayerPrefs.GetInt("BestTime", -1);
    }

    private void SetBestTime(int time)
    {
        PlayerPrefs.SetInt("BestTime", time);
    }
}
