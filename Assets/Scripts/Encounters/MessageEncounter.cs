namespace Blobber
{
    /// <summary>
    /// Shows a text message. Use to provide clues or ambiance to the map.
    /// </summary>
    public class MessageEncounter : Encounter
    {
        public int messageIndex;

        public override void StartEncounter()
        {
            base.StartEncounter();
            if (Gameplay.instance.map.messages == null) return;
            var message = (0 <= messageIndex && messageIndex < Gameplay.instance.map.messages.Count) ? Gameplay.instance.map.messages[messageIndex] : null;
            if (!string.IsNullOrWhiteSpace(message))
            {
                Effects.ShowNonblockingMessage(Gameplay.instance.map.messages[messageIndex]);
            }
            Gameplay.instance.RemoveCurrentEncounter();
        }
    }
}