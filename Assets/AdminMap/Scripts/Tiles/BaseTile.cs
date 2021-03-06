﻿using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTile : MonoBehaviour
{
    public State State { get; set; } = State.Available;
    public Vector2 Coordinate { get; set; }

    void Update()
    {
        if (State == State.Off)
        { 
            //GetComponentInChildren<Renderer>().material.color = Color.gray;
        }
    }
    public BaseTile Initialize(Vector2 coordinate)
    {
        Coordinate = coordinate;
        return this;
    }
}

public enum State
{
    Available,
    Unavailable,
    Off
}


