using System.Collections.Generic;
using UnityEngine;

class MinHeap
{
    private List<(float priority, Vector2Int value)> _data = new();

    public int Count => _data.Count;

    public void Push(float priority, Vector2Int value)
    {
        _data.Add((priority, value));
        SiftUp(_data.Count - 1);
    }

    public Vector2Int Pop()
    {
        var top = _data[0].value;
        int last = _data.Count - 1;
        _data[0] = _data[last];
        _data.RemoveAt(last);
        if (_data.Count > 0) SiftDown(0);
        return top;
    }

    private void SiftUp(int i)
    {
        while (i > 0)
        {
            int parent = (i - 1) / 2;
            if (_data[i].priority < _data[parent].priority)
            {
                (_data[i], _data[parent]) = (_data[parent], _data[i]);
                i = parent;
            }
            else break;
        }
    }

    private void SiftDown(int i)
    {
        int count = _data.Count;
        while (true)
        {
            int smallest = i;
            int left = 2 * i + 1;
            int right = 2 * i + 2;
            if (left < count && _data[left].priority < _data[smallest].priority)
                smallest = left;
            if (right < count && _data[right].priority < _data[smallest].priority)
                smallest = right;
            if (smallest == i) break;
            (_data[i], _data[smallest]) = (_data[smallest], _data[i]);
            i = smallest;
        }
    }
}
