namespace Blobber
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// Manages the panel of 4 characters in the lower right.
    /// </summary>
    public class PartyPanel : MonoBehaviour
    {
        public List<CharacterPanel> characterPanels;

        [Header("Character Details")]
        public GameObject characterDetailsPanel;
        public Text characterNameText;
        public Text characterInfoText;
        public List<Button> characterTraitButtons;

        public Party party { get { return Gameplay.instance.party; } }

        private Character character = null;

        public void Redraw()
        {
            for (int i = 0; i < Party.FullSize; i++)
            {
                var charPanel = characterPanels[i];
                if (i >= party.characters.Count)
                {
                    charPanel.gameObject.SetActive(false);
                }
                else
                {
                    var character = party.characters[i];
                    charPanel.gameObject.SetActive(true);
                    charPanel.nameText.text = character.name;
                    charPanel.levelText.text = "Level " + character.level;
                    charPanel.hpSlider.maxValue = character.hp;
                    charPanel.hpSlider.value = character.currentHP;
                    charPanel.hpText.text = character.currentHP + "/" + character.hp;
                    charPanel.deadImage.gameObject.SetActive(character.currentHP <= 0);
                    character.xpChanged -= OnXPChanged;
                    character.xpChanged += OnXPChanged;
                    character.hpChanged -= OnHPChanged;
                    character.hpChanged += OnHPChanged;
                }
            }
        }

        private void OnHPChanged(Character character)
        {
            if (!character.isAlive)
            {
                MoveCharacterToBack(party.characters.FindIndex(x => x == character));
            }
            else
            {
                Redraw();
            }
        }

        private void OnXPChanged(Character character)
        {
            Redraw();
        }

        public void MoveCharacterToFront(int i)
        {
            var character = party.characters[i];
            party.characters.RemoveAt(i);
            party.characters.Insert(0, character);
            Redraw();
        }

        public void MoveCharacterToBack(int i)
        {
            var character = party.characters[i];
            party.characters.RemoveAt(i);
            party.characters.Add(character);
            Redraw();
        }

        public void InspectCharacter(int characterIndex)
        {
            character = party.characters[characterIndex];
            characterNameText.text = character.name;
            characterInfoText.text = character.GetDescription() + 
                "\nParty Currency: " + party.currency +
                "\nKeys Found: " + party.keysFound;
            for (int i = 0; i < Mathf.Min(Character.NumTraits, character.traitIDs.Count); i++)
            {
                var trait = TraitAssets.instance.GetTrait(character.traitIDs[i]);
                if (trait == null) continue;
                characterTraitButtons[i].gameObject.SetActive(trait.damageType == DamageType.Heal);
                characterTraitButtons[i].GetComponentInChildren<Text>().text = trait.name;
            }
            characterDetailsPanel.SetActive(true);
        }

        public void UseNoncombatTrait(int i)
        {
            if (character == null) return;
            var trait = TraitAssets.instance.GetTrait(character.traitIDs[i]);
            if (trait == null) return;
            trait.Attack(null, character.level);
        }

    }
}