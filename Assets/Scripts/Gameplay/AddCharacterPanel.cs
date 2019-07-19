namespace Blobber
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;

    public class AddCharacterPanel : MonoBehaviour
    {
        public Text characterDescriptionText;
        public InputField nameInputField;
        public Button acceptButton;

        public Character character;

        public void Open()
        {
            character = Character.CreateRandomCharacter();
            characterDescriptionText.text = character.GetDescription();
            nameInputField.text = string.Empty;
            acceptButton.interactable = false;
            gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(nameInputField.gameObject);
        }

        public void OnCharacterNameChange(string text)
        {
            acceptButton.interactable = !string.IsNullOrWhiteSpace(text);
        }

        public void AcceptCharacter()
        {
            character.name = nameInputField.text;
            Gameplay.instance.AddCharacterToParty(character);
            gameObject.SetActive(false);
        }
    }
}