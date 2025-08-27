// IPlayerInventoryBridge.cs
public interface IPlayerInventoryBridge
{
    // Remove from the player's inventory when they drop onto the station UI.
    // Return true if removed successfully, false if not (e.g., not enough count).
    bool TakeFromPlayer(ItemDefinition def, int count);

    // Give back to the player (e.g., when retrieving or leaving without Begin).
    void GiveToPlayer(ItemDefinition def, int count);
}
