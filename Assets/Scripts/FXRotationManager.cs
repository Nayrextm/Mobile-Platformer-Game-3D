using System.Collections.Generic;
using UnityEngine;

public class FXRotationManager : MonoBehaviour
{
    private static readonly List<FXRotator> _activeItems = new List<FXRotator>();

    public static void Register(FXRotator item)
    {
        if (!_activeItems.Contains(item))
        {
            _activeItems.Add(item);
        }
    }

    public static void Unregister(FXRotator item)
    {
        if (_activeItems.Contains(item))
        {
            _activeItems.Remove(item);
        }
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;
        int count = _activeItems.Count;

        for (int i = 0; i < count; i++)
        {
            if (_activeItems[i] != null)
            {
                _activeItems[i].CachedTransform.Rotate(_activeItems[i].RotationVelocity * deltaTime, Space.Self);
            }
        }
    }
}