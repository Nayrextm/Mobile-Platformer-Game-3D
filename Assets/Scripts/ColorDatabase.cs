using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct ColorItemData
{
    [ColorUsage(true, true)]
    public Color color;

    [Min(0)] 
    public int price;
}

[CreateAssetMenu(fileName = "ColorDatabase", menuName = "Skins/ColorDatabase")]
public class ColorDatabase : ScriptableObject
{
    [SerializeField] private List<ColorItemData> _availableColors = new List<ColorItemData>();

    public int Count => _availableColors != null ? _availableColors.Count : 0;

    public Color GetColorByIndex(int index)
    {
        if (_availableColors == null || _availableColors.Count == 0) return Color.white;
        if (index < 0 || index >= _availableColors.Count) return _availableColors[0].color;

        return _availableColors[index].color;
    }

    public int GetPriceByIndex(int index)
    {
        if (_availableColors == null || _availableColors.Count == 0) return 0;
        if (index < 0 || index >= _availableColors.Count) return 0;

        return _availableColors[index].price;
    }
}