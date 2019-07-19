namespace Blobber
{
    public class MapExit : Encounter
    {
        public override void StartEncounter()
        {
            base.StartEncounter();
            Gameplay.instance.ShowMapExit();
        }
    }
}