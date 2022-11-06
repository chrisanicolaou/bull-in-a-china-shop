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
        DoorOpened,
        DoorClosed,
        ShopperQueued,
        ShopperServed,
        ShopperLeaving,
        ShopperThinking,
    }
}
