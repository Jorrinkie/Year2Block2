using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventManager : MonoBehaviour
{
    public static EventManager current;

    private void Awake()
    {
        current = this;

    }

    public event Action Ontreelook;

    public void Treelook()
    {
        Ontreelook?.Invoke();
    }

    public event Action OnCastleLook;
    
    public void CastleLook()
    {
        Ontreelook?.Invoke();
    }
}
