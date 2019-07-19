namespace Blobber
{
    using UnityEngine;
    using UnityEngine.UI;

    public class LootBoxPanel : MonoBehaviour
    {
        public Button buyButton;
        public AudioClip findKeyClip;
        public AudioClip findCoinsClip;

        public const int Cost = 15;

        public Party party { get { return Gameplay.instance.party; } }

        public void Open()
        {
            buyButton.interactable = party.currency >= Cost;
            gameObject.SetActive(true);
        }

        public void Close()
        {
            Gameplay.instance.RemoveCurrentEncounter();
            gameObject.SetActive(false);
        }

        public void Buy()
        {
            Close();
            party.currency -= Cost;
            var result = Random.Range(1, 100);
            if (result < 25) // 0-25%: DNA Chaos
            {
                Gameplay.instance.CombineDNA();
            }
            else if (result < 50) // 25-50%: Find key
            {
                party.keysFound++;
                Effects.Flash(Color.yellow, findKeyClip);
                if (party.keysFound >= Gameplay.RequiredKeys)
                {
                    Effects.ShowBlockingMessage("You found the last key and can turn off The Machine! You can continue to adventure.");
                }
                else
                {
                    Effects.ShowBlockingMessage("You found key #" + party.keysFound + "! " + Gameplay.RequiredKeys + " keys will turn off The Machine.");
                }
            }
            else if (result < 75) // 50-75%: Currency
            {
                var amount = Random.Range(1, 100);
                party.currency += amount;
                Effects.Flash(Color.yellow, findCoinsClip);
                Effects.ShowBlockingMessage("You find " + amount + " coins!");
            }
            else // 75-100%: Nothing
            {
                Effects.ShowBlockingMessage("Empty! Better luck next time!");
            }
        }

    }
}