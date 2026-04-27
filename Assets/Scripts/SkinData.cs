using UnityEngine;

[CreateAssetMenu(fileName = "NewSkin", menuName = "Skins/SkinData")]
public class SkinData : ScriptableObject
{
    public int skinID;                  
    public string skinDisplayName;      
    public GameObject visualPrefab;     
}