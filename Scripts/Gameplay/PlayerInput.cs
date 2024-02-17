using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerInput : MonoBehaviour, IInitializable, ISubscriber
{ 
    public static PlayerInput Instance;

    public Action PlayerMouseDown;
    public Action PlayerMouseUp;

    public bool IsEnabled;// { get; private set; }

    private bool _isInitialized;
    private bool _firstTouch;
    private bool _gameTouch;

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
    public void Initialize()
    {
        SubscribeAll();

        _firstTouch = false;
    }
    public void SubscribeAll()
    {
        GameState.Instance.GameStarted += EnableInput;
        GameState.Instance.GameUnpaused += EnableInput;
        GameState.Instance.GamePaused += DisableInput;
    }
    public void UnsubscribeAll()
    {
        GameState.Instance.GameStarted -= EnableInput;
        GameState.Instance.GameUnpaused -= EnableInput;
        GameState.Instance.GamePaused -= DisableInput;
    }
    public void EnableInput()
    { 
        IsEnabled= true;
    }
    public void DisableInput() 
    {
        IsEnabled = false;
    }
    private void OnMouseDown()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);

        eventData.position = Input.mousePosition;

        var raycastResults = new List<RaycastResult>();

        EventSystem.current.RaycastAll(eventData, raycastResults);

        bool hitUI = false;

        foreach (var result in raycastResults)
        {
            if (result.gameObject.GetComponent<Button>() != null)
            {
                hitUI = true;
                break;
            }
        }

        if (hitUI)
            _gameTouch = false;
        else
            _gameTouch = true;

        if (!_gameTouch)
            return;
        if (!_firstTouch)
        {
            GameState.Instance.StartGame();
            _firstTouch = true;
        }

        if (!IsEnabled)
            return;

        PlayerMouseDown?.Invoke();   
    }
    private void OnMouseUp()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);

        eventData.position = Input.mousePosition;

        var raycastResults = new List<RaycastResult>();

        EventSystem.current.RaycastAll(eventData, raycastResults);

        bool hitUI = false;

        foreach (var result in raycastResults)
        {
            if (result.gameObject.GetComponent<Button>() != null)
            {
                hitUI = true;
                break;
            }
        }

        if (hitUI)
            _gameTouch = false;
        else
            _gameTouch = true;

        if (!_gameTouch)
            return;
        if (!IsEnabled)
            return;

        PlayerMouseUp?.Invoke();
    }
}