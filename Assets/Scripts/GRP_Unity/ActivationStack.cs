using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ActivationStack : MonoBehaviour
{
    [Header("No starting items settings")]
    [Min(0)][SerializeField] int _defaultCapacity = 10;

    [Header("Starting items settings")]
    [SerializeField] GameObject[] _startingItems;
    [SerializeField] bool _nullAfterStart = false;
    Stack<GameObject> _items = null;

    public int Count { get => _items.Count; }

    void Start()
    {
        if (_startingItems.Length == 0)
        {
            _items = new(_defaultCapacity); // Just init with capacity
            return;
        }
        
        _items = new(_startingItems);

        // Only the last should be active
        for (int i = 0; i < _startingItems.Length; i++)
            _startingItems[i].SetActive(i == _startingItems.Length - 1);

        // In case you want the GC to dealloc the array
        if (_nullAfterStart)
            _startingItems = null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear() => _items.Clear();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(GameObject item) => _items.Contains(item);

    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // public void CopyTo(T[] array, int arrayIndex);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public GameObject Peek() => _items.Peek();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public GameObject Pop()
    {
        GameObject item = _items.Pop();
        _items.Peek().gameObject.SetActive(true);
        item.SetActive(false);
        return item;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PopDiscard() => Pop();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Push(GameObject item)
    {
        GameObject last;
        if (_items.TryPeek(out last))
            last.gameObject.SetActive(false);
        
        _items.Push(item);
        item.gameObject.SetActive(true);
    }

    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // public T[] ToArray();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void TrimExcess() => _items.TrimExcess();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryPeek(out GameObject result) => _items.TryPeek(out result);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryPop(out GameObject result)
    {
        bool pop = _items.TryPop(out result);
        result.gameObject.SetActive(false);
        return pop;
    }
}
