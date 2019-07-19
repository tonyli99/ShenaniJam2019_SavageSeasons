namespace Blobber
{
    using UnityEngine;

    /// <summary>
    /// ScriptableObject that defines a trait (ability). Each trait
    /// can attack (DamageType.None means no attack) and reduce
    /// damage when attacked (defenseType None means no damage reduction).
    /// </summary>
    [CreateAssetMenu(menuName = "Game/Traits/Trait", order = 0)]
    public class Trait : ScriptableObject
    {
        [TextArea]
        public string description;
        public float levelModifier = 1.2f;

        [Header("Attack")]
        public DamageType damageType = DamageType.None;
        public int min;
        public int max;

        [Header("Defense")]
        public DamageType defenseType = DamageType.None;
        public int damageReduction;

        [Header("Use")]
        public Color useColor = Color.red;
        public AudioClip useSound;

        public virtual void Attack(Enemy enemy, int level)
        {
            Effects.Flash(useColor, useSound);
            var damage = (int)(Random.Range(min, max + 1) * levelModifier * level);
            if (damageType == DamageType.Heal)
            {
                foreach (var character in Gameplay.instance.party.characters)
                {
                    if (character.isAlive)
                    {
                        character.Heal(damage);
                    }
                }
            }
            else if (enemy != null)
            {
                enemy.TakeDamage(damage, damageType);
            }
        }

        public virtual int AdjustDamage(int damage, DamageType damageType, int level)
        {
            var modifiedDamageReduction = (int)(damageReduction * levelModifier * level);
            return (damageType == defenseType) ? Mathf.Max(0, damage - damageReduction) : damage;
        }

        public virtual string GetDescription() { return description; }
    }
}
