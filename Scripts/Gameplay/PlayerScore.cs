using DG.Tweening;
using System.Collections;
using UnityEngine;

public class PlayerScore : MonoBehaviour, ISubscriber, IInitializable
{
    public static PlayerScore Instance;

    public int Score { get; private set; }

    private bool _isInitialized;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }
    private void OnEnable()
    {
        if (!_isInitialized)
            return;

        SubscribeAll();
    }
    private void OnDisable()
    {
        UnsubscribeAll();
    }
    public void SubscribeAll()
    {
        GameState.Instance.GameFinished += Save;
    }

    public void UnsubscribeAll()
    {
        GameState.Instance.GameFinished += Save;
    }

    public void Initialize()
    {
        _isInitialized = true;
    }
    public void AddScore(int count)
    {
        Score += count;

        Camera.main.DOShakePosition(0.4f, 0.2f, fadeOut: true).SetUpdate(true);
        Camera.main.DOShakeRotation(0.4f, 0.2f, fadeOut: true).SetUpdate(true);

        AudioVibrationManager.Instance.PlaySound(AudioVibrationManager.Instance.Line, Random.Range(1, 1.1f));

        GameState.Instance.ScoreAdded?.Invoke();
        Save();
    }
    private void Save()
    {
        if (Score > PlayerPrefs.GetInt("HighScore", 0))
            PlayerPrefs.SetInt("HighScore", Score);
    }
}