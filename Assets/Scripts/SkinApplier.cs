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
        if (DatabaseManager.Instance == null || _skinDatabase == null || _colorDatabase == null || _visualContainer == null) return;

       
        int currentSkinID = DatabaseManager.Instance.GetSelectedSkinID();
        int currentPlayerColorIndex = DatabaseManager.Instance.GetSelectedColorIndex();
        int currentTrailColorIndex = DatabaseManager.Instance.GetSelectedTrailColorIndex();

       
        SkinData skin = _skinDatabase.GetSkinByID(currentSkinID);
        Color playerColor = _colorDatabase.GetColorByIndex(currentPlayerColorIndex);
        Color trailColor = _colorDatabase.GetColorByIndex(currentTrailColorIndex);

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

                // Встановлюємо колір для різних типів шейдерів
                propBlock.SetColor("_Color", playerColor);           
                propBlock.SetColor("_BaseColor", playerColor);       
                propBlock.SetColor("_EmissionColor", playerColor);   

                rend.SetPropertyBlock(propBlock);
            }

            if (_trail != null)
            {
                _trail.startColor = trailColor;
                _trail.endColor = new Color(trailColor.r, trailColor.g, trailColor.b, 0f);
            }
        }
    }
}