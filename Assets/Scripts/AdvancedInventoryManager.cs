using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class AdvancedInventoryManager : MonoBehaviour
{
    public enum TabType { Forms, PlayerColor, TrailColor }

    [Header("3D Манекен")]
    [SerializeField] private SkinApplier _previewApplier;
    [Header("Керування Манекеном")]
    [SerializeField] private MannequinAnimator _mannequinAnimator;

    [Header("Бази Даних")]
    [SerializeField] private SkinDatabase _skinDatabase;
    [SerializeField] private ColorDatabase _colorDatabase;

    [Header("UI: Блок 2 (Опис та Дії)")]
    [SerializeField] private TMP_Text _selectedItemName;
    [SerializeField] private Image _selectedItemIcon;
    [SerializeField] private Button _equipButton;
    [SerializeField] private TMP_Text _equipButtonText;

    [Header("UI: Блок 3 (Каталог)")]
    [SerializeField] private CustomizationSlot _slotPrefab;
    [SerializeField] private Transform _containerForms;
    [SerializeField] private Transform _containerColors;
    [SerializeField] private Transform _containerTrails;

    [Header("Кнопки Вкладок")]
    [SerializeField] private Button _tabFormsBtn;
    [SerializeField] private Button _tabColorsBtn;
    [SerializeField] private Button _tabTrailsBtn;

   
    private TabType _currentTab = TabType.Forms;
    private int _previewingID = 0;

   
    private List<CustomizationSlot> _formSlots = new List<CustomizationSlot>();
    private List<CustomizationSlot> _colorSlots = new List<CustomizationSlot>();
    private List<CustomizationSlot> _trailSlots = new List<CustomizationSlot>();

    private void Start()
    {
        
        _equipButton.onClick.AddListener(EquipCurrentItem);
        _tabFormsBtn.onClick.AddListener(() => SwitchTab(TabType.Forms));
        _tabColorsBtn.onClick.AddListener(() => SwitchTab(TabType.PlayerColor));
        _tabTrailsBtn.onClick.AddListener(() => SwitchTab(TabType.TrailColor));

        GenerateAllSlots();
        SwitchTab(TabType.Forms); 
    }

    private void GenerateAllSlots()
    {
        
        foreach (var skin in _skinDatabase.allSkins)
        {
            var slot = Instantiate(_slotPrefab, _containerForms);
            slot.itemIcon.color = Color.white; 
            slot.slotButton.onClick.AddListener(() => OnSlotClicked(TabType.Forms, skin.skinID, skin.skinDisplayName, Color.white));
            _formSlots.Add(slot);
        }

       
        for (int i = 0; i < _colorDatabase.availableColors.Count; i++)
        {
            int index = i;
            Color color = _colorDatabase.GetColorByIndex(index);
            var slot = Instantiate(_slotPrefab, _containerColors);
            slot.itemIcon.color = color;
            slot.slotButton.onClick.AddListener(() => OnSlotClicked(TabType.PlayerColor, index, "Color #" + index, color));
            _colorSlots.Add(slot);
        }

        
        for (int i = 0; i < _colorDatabase.availableColors.Count; i++)
        {
            int index = i;
            Color color = _colorDatabase.GetColorByIndex(index);
            var slot = Instantiate(_slotPrefab, _containerTrails);
            slot.itemIcon.color = color;
            slot.slotButton.onClick.AddListener(() => OnSlotClicked(TabType.TrailColor, index, "Trail #" + index, color));
            _trailSlots.Add(slot);
        }
    }

    private void SwitchTab(TabType newTab)
    {
        _currentTab = newTab;

        
        _containerForms.gameObject.SetActive(_currentTab == TabType.Forms);
        _containerColors.gameObject.SetActive(_currentTab == TabType.PlayerColor);
        _containerTrails.gameObject.SetActive(_currentTab == TabType.TrailColor);

        
        int equippedID = GetEquippedIdForTab(_currentTab);

       
        if (_currentTab == TabType.Forms)
            OnSlotClicked(_currentTab, equippedID, _skinDatabase.GetSkinByID(equippedID).skinDisplayName, Color.white);
        else
            OnSlotClicked(_currentTab, equippedID, "Color #" + equippedID, _colorDatabase.GetColorByIndex(equippedID));

        RefreshSlotsUI();

        UpdateMannequinState();
    }

    private void OnSlotClicked(TabType tab, int itemID, string itemName, Color itemColor)
    {
        if (_currentTab != tab) return;

        _previewingID = itemID;

        if (_selectedItemName != null) _selectedItemName.text = itemName;
        if (_selectedItemIcon != null) _selectedItemIcon.color = itemColor;


        RefreshSlotsUI();
        UpdateMannequinPreview();
    }

    private void RefreshSlotsUI()
    {
        int equippedID = GetEquippedIdForTab(_currentTab);
        List<CustomizationSlot> activeSlots = GetActiveSlotList();

        for (int i = 0; i < activeSlots.Count; i++)
        {
            int slotID = (_currentTab == TabType.Forms) ? _skinDatabase.allSkins[i].skinID : i;

            bool isEquipped = (slotID == equippedID);
            bool isSelected = (slotID == _previewingID);

            activeSlots[i].SetState(isEquipped, isSelected);
        }

        if (_previewingID == equippedID)
        {
            _equipButtonText.text = "EQUIPPED";
            _equipButton.interactable = false;
        }
        else
        {
            _equipButtonText.text = "EQUIP";
            _equipButton.interactable = true;
        }
    }

    private void EquipCurrentItem()
    {
        if (DatabaseManager.Instance == null) return;

        if (_currentTab == TabType.Forms) DatabaseManager.Instance.SaveSelectedSkinID(_previewingID);
        else if (_currentTab == TabType.PlayerColor) DatabaseManager.Instance.SaveSelectedColorIndex(_previewingID);
        else if (_currentTab == TabType.TrailColor) DatabaseManager.Instance.SaveSelectedTrailColorIndex(_previewingID);

        RefreshSlotsUI();

    }

    private int GetEquippedIdForTab(TabType tab)
    {
        if (DatabaseManager.Instance == null) return 0;
        if (tab == TabType.Forms) return DatabaseManager.Instance.GetSelectedSkinID();
        if (tab == TabType.PlayerColor) return DatabaseManager.Instance.GetSelectedColorIndex();
        return DatabaseManager.Instance.GetSelectedTrailColorIndex();
    }

    private List<CustomizationSlot> GetActiveSlotList()
    {
        if (_currentTab == TabType.Forms) return _formSlots;
        if (_currentTab == TabType.PlayerColor) return _colorSlots;
        return _trailSlots;
    }


    private void UpdateMannequinPreview()
    {
        if (_previewApplier == null || DatabaseManager.Instance == null) return;

        
        int tempSkinID = DatabaseManager.Instance.GetSelectedSkinID();
        int tempColorIndex = DatabaseManager.Instance.GetSelectedColorIndex();
        int tempTrailIndex = DatabaseManager.Instance.GetSelectedTrailColorIndex();

        
        if (_currentTab == TabType.Forms) tempSkinID = _previewingID;
        else if (_currentTab == TabType.PlayerColor) tempColorIndex = _previewingID;
        else if (_currentTab == TabType.TrailColor) tempTrailIndex = _previewingID;

       
        _previewApplier.PreviewCustomization(tempSkinID, tempColorIndex, tempTrailIndex);
    }

    private void UpdateMannequinState()
    {
        if (_mannequinAnimator == null) return;

        
        if (_currentTab == TabType.TrailColor)
        {
            _mannequinAnimator.SetAnimationState(true);
        }
        else 
        {
            _mannequinAnimator.SetAnimationState(false);
        }
    }
}