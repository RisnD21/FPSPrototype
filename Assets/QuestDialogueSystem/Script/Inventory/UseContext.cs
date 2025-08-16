using QuestDialogueSystem;
using UnityEngine;

public readonly struct UseContext
{
    public readonly GameObject user;
    public readonly IInventory inventory;

    public UseContext (GameObject user, IInventory inventory = null)
    {
        this.user = user;
        this.inventory = inventory;
    }
}