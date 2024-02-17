using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class GameTimer : MonoBehaviour, ISubscriber
{
    public static  GameTimer Instance;

    public float Score { get; private set; }

    private bool _timerWorking;

    private void Awake()
    {
        if(Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }
    private void FixedUpdate()
    {
        if (_timerWorking)
        {
            Score += Time.fixedDeltaTime;
        }
    }
    private void StartTimer()
    {
        _timerWorking = true;
    }
    private void StopTimer()
    {
        _timerWorking = false;
    }

    public void SubscribeAll()
    {
        GameState.Instance.GameStarted += StartTimer;
        GameState.Instance.GameFinished += StopTimer;
    }

    public void UnsubscribeAll()
    {
        GameState.Instance.GameStarted -= StartTimer;
        GameState.Instance.GameFinished -= StopTimer;
    }
}
