namespace Blobber
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;


    public delegate void CharacterDelegate(Character character);

    [Serializable]
    public class Character
    {
        public string name;
        public int level;
        public int hp;
        public int currentHP;
        public int xp;
        public List<int> traitIDs; // Indices into TraitAssets.traits.

        public event CharacterDelegate hpChanged = delegate { };
        public event CharacterDelegate xpChanged = delegate { };

        public const int NumTraits = 3;

        public bool isAlive { get { return currentHP > 0; } }

        public string GetDescription()
        {
            return $"Level: {level}\nXP: {xp}\nHP: {currentHP}/{hp}\nTraits:\n" + GetTraitDescriptions();
        }

        public string GetTraitDescriptions()
        {
            var s = string.Empty;
            foreach (var traitID in traitIDs)
            {
                var trait = TraitAssets.instance.GetTrait(traitID);
                if (trait == null) continue;
                if (!string.IsNullOrEmpty(s)) s += "\n";
                s += trait.GetDescription();
            }
            return s;
        }

        public void GainXP(int amount)
        {
            xp += amount;
            if (xp >= level * 100)
            {
                level = 1 + (xp / 100);
                hp = 10 * level;
                currentHP = hp;
                hpChanged(this);
            }
            xpChanged(this);
        }

        public void Heal(int amount)
        {
            currentHP = Mathf.Clamp(currentHP + amount, 0, hp);
            hpChanged(this);
        }

        public void TakeDamage(int damage, DamageType damageType)
        {
            foreach (var traitID in traitIDs)
            {
                var trait = TraitAssets.instance.GetTrait(traitID);
                if (trait == null) continue;
                damage = trait.AdjustDamage(damage, damageType, level);
            }
            currentHP = Mathf.Clamp(currentHP - damage, 0, hp);
            hpChanged(this);
        }

        public static Character CreateRandomCharacter()
        {
            var character = new Character();
            character.name = "Nameless";
            character.level = 1;
            character.hp = 10;
            character.currentHP = 10;
            character.xp = 0;
            character.traitIDs = new List<int>();
            var traitIDPool = TraitAssets.instance.GetShuffledTraitIDList();
            for (int i = 0; i < Mathf.Min(NumTraits, traitIDPool.Count); i++)
            {
                character.traitIDs.Add(traitIDPool[i]);
            }
            return character;
        }
    }
}