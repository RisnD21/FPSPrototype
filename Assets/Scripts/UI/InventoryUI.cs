using UnityEngine;
using QuestDialogueSystem;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System;
using AudioSystem.SFX;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] Transform slotParent;
    [SerializeField] Transform slotPrefab;
    [SerializeField] int slotsCount = 24;
    List<SlotUI> slots;

    [SerializeField] GameObject itemMenu;
    [SerializeField] GameObject blocker;

    [SerializeField] Button singleButton;
    [SerializeField] TextMeshProUGUI singleButtonText;
    [SerializeField] GameObject dualButtonParent;
    [SerializeField] Button upperButton;
    [SerializeField] Button lowerButton;
    [SerializeField] TextMeshProUGUI upperButtonText;
    [SerializeField] TextMeshProUGUI lowerButtonText;

    [SerializeField] Button useItemButton;
    [SerializeField] Button dropItemButton;
    
    [SerializeField] Vector3 itemMenuOffset;

    [SerializeField] Damageable player;
    [SerializeField] TextMeshProUGUI healthPanel;

    public static event Action<InventorySlot> OnItemUse;
    InventorySlot currentSlot;
    public ItemData itemSelected;
    public void ItemSelected(ItemData item) => itemSelected = item;

    void Awake()
    {
        transform.localPosition = Vector3.zero;

        slots = new();

        for(int i = 0; i < slotsCount; i++)
        {
            SlotUI slot = Instantiate(slotPrefab, slotParent).GetComponent<SlotUI>();
            slot.InitializeSlot(this);
            slot.gameObject.SetActive(false);
            slots.Add(slot);
        }

        itemMenu.SetActive(false);
        blocker.SetActive(false);
    }

    public void Initialize()
    {
        SyncInventory();
    }

    void Start()
    {
        upperButton.onClick.AddListener(UseItem);
        lowerButton.onClick.AddListener(DestroyItem);
    }

    void OnEnable()
    {
        Locator.Inventory.OnItemAdd += UpdateSlot;
        Locator.Inventory.OnItemRemove += UpdateSlot;

        SyncInventory();
        
        if (SFXManager.Instance != null) SFXManager.Instance.PlaySound(SoundID.OpenInventory);
    }

    void OnDisable()
    {
        Locator.Inventory.OnItemAdd -= UpdateSlot;
        Locator.Inventory.OnItemRemove -= UpdateSlot;
        CloseItemMenu();
    }

    void UpdateSlot(ItemStack _)
    {
        SyncInventory();
    }

    void SyncInventory()
    {
        if (Locator.Inventory.Slots.Count != slotsCount)
        {
            Debug.LogError("[InventoryUI] Inventory slots amount mismatch");
            return;
        }

        for(int i = 0; i < slotsCount; i++)
        {
            slots[i].SetSlot(Locator.Inventory.Slots[i]);
        }
    }

    public void OpenItemMenu(InventorySlot slot, Vector3 position)
    {
        currentSlot = slot;
        var item = currentSlot.stack.Item;

        if (item.itemType == "Shield")
        {
            singleButton.onClick.RemoveAllListeners();

            singleButton.onClick.AddListener(DestroyItem);
            singleButtonText.text = "Discard";

            singleButton.gameObject.SetActive(true);
            dualButtonParent.SetActive(false);
        } 
        else if (item.HasAction() && item.CanDrop)
        {
            ResetTextAlpha(upperButtonText);
            ResetTextAlpha(lowerButtonText);

            singleButton.gameObject.SetActive(false);
            dualButtonParent.SetActive(true);
        }
        else if (item.HasAction())
        {
            singleButton.onClick.RemoveAllListeners();
            singleButton.onClick.AddListener(UseItem);
            singleButtonText.text = "Use";

            singleButton.gameObject.SetActive(true);
            dualButtonParent.SetActive(false);
        }
        else if (item.CanDrop)
        {
            singleButton.onClick.RemoveAllListeners();
            dropItemButton.onClick.AddListener(DestroyItem);

            singleButtonText.text = "Discard";

            singleButton.gameObject.SetActive(true);
            dualButtonParent.SetActive(false);
        }
        else return;

        itemMenu.transform.position = position + itemMenuOffset;
        itemMenu.SetActive(true);
        blocker.SetActive(true);
    }

    void ResetTextAlpha(TextMeshProUGUI text)
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1f);
    }

    void UseItem()
    {
        OnItemUse?.Invoke(currentSlot);
        CloseItemMenu();
    }

    public void UseItem(InventorySlot slot)
    {
        if(slot == null) return;
        OnItemUse?.Invoke(slot);
        CloseItemMenu();
    }

    void DestroyItem()
    {
        int toRemove = currentSlot.stack.Count;
        Locator.Inventory.TryRemoveFromSlot(currentSlot.stack, currentSlot, ref toRemove);
        CloseItemMenu();
    }

    public void CloseItemMenu()
    {
        itemMenu.SetActive(false);
        blocker.SetActive(false);
    }

    void Update()
    {
        healthPanel.text = player.Current + " / " + player.maxHealth.ToString();
    }
}
