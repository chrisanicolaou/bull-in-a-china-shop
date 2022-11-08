using UnityEngine;

namespace CharaGaming.BullInAChinaShop.Enums
{
    public enum GameEvent
    {
        ItemPurchased,
        ItemSold,
        ShopperRequestingEntry,
        ShopperDeniedEntry,
        ShopperApprovedEntry,
        ShopperRequestingExit,
        DoorOpened,
        DoorClosed,
        ShopperQueued,
        ShopperServed,
        ShopperLeavingQueue,
        ShopperThinking,
    }
}
