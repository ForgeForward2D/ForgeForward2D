using System;
using UnityEngine;

public abstract class Achievement : ScriptableObject
{
    public string title;
    public DateTime completionTime;
    public bool visible = true;
    public Sprite icon;

    public abstract string GetDescription();

    public abstract void CheckCompletion(Tracker tracker);
    
    public bool IsCompleted()
    {
        return completionTime != default;
    }
}
