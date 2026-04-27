using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SkinDatabase", menuName = "Skins/SkinDatabase")]
public class SkinDatabase : ScriptableObject
{
    public List<SkinData> allSkins;

    public SkinData GetSkinByID(int id)
    {
        return allSkins.Find(s => s.skinID == id);
    }
}