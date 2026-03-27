using System;
using UnityEngine;

public abstract class Achievement : ScriptableObject
{
    public string title;
    public bool visible = true;
    public Sprite icon;

    public DateTime completionTime = default;
    public bool IsCompleted => completionTime != default;

    public abstract string GetDescription();

    public abstract void CheckCompletion(Tracker tracker);

}
