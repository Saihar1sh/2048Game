using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventsService : LazySingleton<EventsService>
{
    public delegate void OnNewGameEvent();
    public event OnNewGameEvent NewGameEvent;
    
    public delegate void OnNextMoveEvent();
    public event OnNextMoveEvent NextMoveEvent;
    
    public delegate void OnInputDirectionEvent(SwipeInputValues swipeInput);
    public event OnInputDirectionEvent InputDirectionEvent;


    protected override void Awake()
    {
        base.Awake();
    }

    public void AddSubscribersToOnInputDirectionEvent(OnInputDirectionEvent onInputDirectionFunc)
    {
        InputDirectionEvent += onInputDirectionFunc;
    }
    public void RemoveSubscribersToOnBlockOccupiedEvent(OnInputDirectionEvent onInputDirectionFunc)
    {
        InputDirectionEvent -= onInputDirectionFunc;
    }
    public void InvokeOnInputDirectionEvent(SwipeInputValues swipeInput)
    {
        InputDirectionEvent?.Invoke(swipeInput);
    }


    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}
