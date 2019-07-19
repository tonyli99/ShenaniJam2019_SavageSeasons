namespace Blobber
{
    using System.Collections.Generic;
    using UnityEngine;

    public delegate void EnemyDelegate(Enemy enemy);

    /// <summary>
    /// Combat encounter. Attacks either of the 2 front characters.
    /// </summary>
    public class Enemy : Encounter
    {
        public string displayName;
        [TextArea]
        public string description;
        public int level = 1;
        public int hp;
        public int currentHP;
        public int xp;
        public int currency;

        public Trait attackTrait;

        public List<Trait> defenseTraits;

        public event EnemyDelegate hpChanged = delegate { };

        public Party party { get { return Gameplay.instance.party; } }

        private void Awake()
        {
            currentHP = hp;
        }

        public override void StartEncounter()
        {
            base.StartEncounter();
            Gameplay.instance.StartCombat(this);
        }

        public void AttackFrontCharacter()
        {
            var numFrontChars = party.GetNumFrontCharacters();
            if (numFrontChars == 0) return;
            var target = party.characters[Random.Range(0, numFrontChars)];
            Effects.ShowBlockingMessage(displayName + " attacks " + target.name + " with " + attackTrait.name + ".");
            Attack(target);
        }

        public void AttackWholeParty()
        {
            Effects.ShowNonblockingMessage(displayName + " attacks party with " + attackTrait.name + ".");
            var wholeParty = new List<Character>(party.characters);
            foreach (var character in wholeParty)
            {
                Attack(character);
            }
        }

        public void Attack(Character character)
        {
            if (character == null || !character.isAlive) return;
            Effects.Flash(attackTrait.useColor, attackTrait.useSound);
            var damage = (int)(Random.Range(attackTrait.min, attackTrait.max + 1) * attackTrait.levelModifier * level);
            character.TakeDamage(damage, attackTrait.damageType);
        }

        public void TakeDamage(int damage, DamageType damageType)
        {
            foreach (var trait in defenseTraits)
            {
                if (trait == null) continue;
                damage = trait.AdjustDamage(damage, damageType, level);
            }
            currentHP = Mathf.Clamp(currentHP - damage, 0, hp);
            hpChanged(this);
        }

    }
}