using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class UIComponentBase : MonoBehaviour
{
    // the runtime type this component expects (helps parent decide what to send)
    public abstract Type StateType { get; }

    // non-generic refresh that accepts any object
    public abstract void RefreshUIDynamic(object state);

    public bool IsOpen => gameObject.activeSelf;

    public void SetActive(bool active)
    {
        if (active == gameObject.activeSelf) return;
        gameObject.SetActive(active);
    }
}

public abstract class UIComponent<T> : UIComponentBase
{
    private List<UIComponentBase> children;

    public List<UIComponentBase> GetChildren()
    {
        if (children == null)
        {
            children = new List<UIComponentBase>(GetComponentsInChildren<UIComponentBase>())
                .Where(child => child.transform.parent == this.transform)
                .ToList();
            Debug.Assert(children != null, "Children null after init");
        }
        return children;
    }

    public sealed override Type StateType => typeof(T);

    public override void RefreshUIDynamic(object state)
    {
        // If state is null and T is nullable, refresh with null
        if (state == null)
        {
           if (default(T) == null)
            {
                RefreshUI(default); // Calls RefreshUI with null
            }
            else
            {
                Debug.LogWarning($"Received null state for {gameObject.name}, but {typeof(T)} is a Value Type and cannot be null.");
            }
            return;
        }

        if (state is T t)
        {
            RefreshUI(t);
            return;
        }
        Debug.LogError($"Tried to Refresh UI with incompatible type. Provided: {state}, Expected: {StateType}");
    }

    public abstract void RefreshUI(T state);
}