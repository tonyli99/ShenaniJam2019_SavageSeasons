namespace Blobber
{
    public class LootBox : Encounter
    {
        public override void StartEncounter()
        {
            Gameplay.instance.OpenLootBoxOffer();
        }
    }
}