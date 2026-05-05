using UnityEngine;

public class SkinApplier : MonoBehaviour
{
    [Header("Бази Даних")]
    [SerializeField] private SkinDatabase _skinDatabase;
    [SerializeField] private ColorDatabase _colorDatabase;

    [Header("Компоненти гравця")]
    [SerializeField] private Transform _visualContainer;
    [SerializeField] private TrailRenderer _trail;

    private void Start()
    {
       
        ApplyCustomization();
    }

   
    public void ApplyCustomization()
    {
        if (DatabaseManager.Instance == null) return;

        int currentSkinID = DatabaseManager.Instance.GetSelectedSkinID();
        int currentPlayerColorIndex = DatabaseManager.Instance.GetSelectedColorIndex();
        int currentTrailColorIndex = DatabaseManager.Instance.GetSelectedTrailColorIndex();

       
        PreviewCustomization(currentSkinID, currentPlayerColorIndex, currentTrailColorIndex);
    }

   
    public void PreviewCustomization(int skinID, int playerColorIndex, int trailColorIndex)
    {
        if (_skinDatabase == null || _colorDatabase == null || _visualContainer == null) return;

        SkinData skin = _skinDatabase.GetSkinByID(skinID);
        Color playerColor = _colorDatabase.GetColorByIndex(playerColorIndex);
        Color trailColor = _colorDatabase.GetColorByIndex(trailColorIndex);

        if (skin != null && skin.visualPrefab != null)
        {
            
            foreach (Transform child in _visualContainer)
            {
                Destroy(child.gameObject);
            }

           
            GameObject newModel = Instantiate(skin.visualPrefab, _visualContainer);
            newModel.transform.localPosition = Vector3.zero;
            newModel.transform.localRotation = Quaternion.identity;
            newModel.transform.localScale = Vector3.one;

            
            Renderer[] allRenderers = newModel.GetComponentsInChildren<Renderer>();
            MaterialPropertyBlock propBlock = new MaterialPropertyBlock();

            foreach (Renderer rend in allRenderers)
            {
                rend.GetPropertyBlock(propBlock);

                
                propBlock.SetColor("_Color", playerColor);
                propBlock.SetColor("_BaseColor", playerColor);
                propBlock.SetColor("_EmissionColor", playerColor);

                rend.SetPropertyBlock(propBlock);
            }


            
            if (_trail != null)
            {
                Gradient trailGradient = new Gradient();

                GradientColorKey[] colorKeys = new GradientColorKey[2];
                colorKeys[0] = new GradientColorKey(trailColor, 0.0f);
                colorKeys[1] = new GradientColorKey(trailColor, 1.0f);
               
                GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
                alphaKeys[0] = new GradientAlphaKey(1.0f, 0.0f); 
                alphaKeys[1] = new GradientAlphaKey(0.0f, 1.0f); 

                trailGradient.SetKeys(colorKeys, alphaKeys);
                _trail.colorGradient = trailGradient;
            }
        }
    }

    public void ClearTrailHistory()
    {
        if (_trail != null)
        {
            _trail.Clear(); 
        }
    }
}