namespace Blobber
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class CombatPanel : MonoBehaviour
    {
        [Header("Enemy")]
        public Text enemyName;
        public Text enemyDescription;
        public Slider enemyHP;
        public Text enemyHPText;

        [Header("Actions")]
        public Text currentCharacterName;
        public List<Button> actionButtons;

        public List<GameObject> otherUIsToDisable;

        public Enemy enemy;

        public int currentActor = 0;

        public Party party { get { return Gameplay.instance.party; } }

        public void Open(Enemy enemy)
        {
            if (enemy == null) return;
            gameObject.SetActive(true);
            otherUIsToDisable.ForEach(x => x?.SetActive(false));
            this.enemy = enemy;
            enemyName.text = enemy.displayName;
            enemyDescription.text = enemy.description;
            enemyHP.maxValue = enemy.hp;
            enemyHP.value = enemy.currentHP;
            enemyHPText.text = enemy.currentHP + "/" + enemy.hp;
            enemy.hpChanged -= OnEnemyHPChanged;
            enemy.hpChanged += OnEnemyHPChanged;
            currentActor = 0;
            UpdateActionPanel();
        }

        public void Close()
        {
            enemy.hpChanged -= OnEnemyHPChanged;
            otherUIsToDisable.ForEach(x => x?.SetActive(true));
            gameObject.SetActive(false);
        }

        public void RunAway()
        {
            enemy.AttackWholeParty();
            Close();
        }

        private void OnEnemyHPChanged(Enemy enemy)
        {
            enemyHPText.text = enemy.currentHP + "/" + enemy.hp;
            enemyHP.value = enemy.currentHP;
            if (enemy.currentHP <= 0)
            {
                Victory();
            }
        }

        public void Victory()
        {
            Effects.ShowBlockingMessage($"Victory!\n{enemy.xp} XP, {enemy.currency} currency");
            party.currency += enemy.currency;
            foreach (var character in party.characters)
            {
                character?.GainXP(enemy.xp);
            }
            Gameplay.instance.RemoveCurrentEncounter();
            Close();
        }

        public void UpdateActionPanel()
        {
            var character = party.characters[currentActor];
            currentCharacterName.text = character.name;
            for (int i = 0; i < Mathf.Min(Character.NumTraits, character.traitIDs.Count); i++)
            {
                var trait = TraitAssets.instance.GetTrait(character.traitIDs[i]);
                if (trait == null) continue;
                actionButtons[i].GetComponentInChildren<Text>().text = trait.name;
                actionButtons[i].interactable = trait.damageType != DamageType.None;
            }
        }

        public void SkipAction()
        {
            NextCharacter();
        }

        public void ClickActionButton(int i)
        {
            var character = party.characters[currentActor];
            var trait = TraitAssets.instance.GetTrait(character.traitIDs[i]);
            if (trait != null) trait.Attack(enemy, character.level);
            NextCharacter();
        }

        public void NextCharacter()
        {
            currentActor++;
            if (currentActor >= party.characters.Count || !party.characters[currentActor].isAlive)
            {
                if (enemy.currentHP > 0) enemy.AttackFrontCharacter();
                currentActor = 0;
            }
            UpdateActionPanel();
        }
    }
}
