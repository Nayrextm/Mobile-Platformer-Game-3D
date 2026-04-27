using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ColorDatabase", menuName = "Skins/ColorDatabase")]
public class ColorDatabase : ScriptableObject
{
    [ColorUsage(true, true)]
    public List<Color> availableColors;

    public Color GetColorByIndex(int index)
    {
        if (availableColors == null || availableColors.Count == 0) return Color.white;
        if (index < 0 || index >= availableColors.Count) return availableColors[0];

        return availableColors[index];
    }
}