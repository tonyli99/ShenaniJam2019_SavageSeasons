namespace Blobber
{
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;
    using UnityEngine.UI;

    public class MainMenu : MonoBehaviour
    {
        [Header("Maps")]
        public List<TextAsset> builtinMaps;
        public Transform mapPanel;
        public Button mapButtonTemplate;

        [Header("Party")]
        public Text partyText;

        private Party party = null;

        private void Start()
        {
            SetupParty();
            SetupMapButtons();
        }

        // Update the party description text.
        public void SetupParty()
        {
            if (PlayerPrefs.HasKey(PlayerPrefsKeys.PartyData))
            {
                party = JsonUtility.FromJson<Party>(PlayerPrefs.GetString(PlayerPrefsKeys.PartyData));
            }
            if (party == null) party = new Party();
            partyText.text = string.Empty;
            foreach (var character in party.characters)
            {
                partyText.text += character.name + " (Lv. " + character.level + ")\n";
            }
            if (string.IsNullOrEmpty(partyText.text)) partyText.text = "Party will form when you enter a dungeon.";
        }

        public void DisbandParty()
        {
            PlayerPrefs.DeleteKey(PlayerPrefsKeys.PartyData);
            party = new Party();
            SetupParty();
        }

        // Update the list of maps that the player can choose to enter,
        // with built-in maps first and then user-created maps.
        public void SetupMapButtons()
        {
            // Add buttons for builtin maps:
            for (int i = 0; i < builtinMaps.Count; i++)
            {
                var builtin = builtinMaps[i];
                var instance = Instantiate(mapButtonTemplate, mapPanel);
                var map = JsonUtility.FromJson<Map>(builtin.text);
                instance.GetComponentInChildren<Text>().text = map.title;
                var mapIndex = i;
                instance.onClick.AddListener(() => { LoadBuiltinMap(mapIndex); });
            }
            // Add buttons for user maps:
            var dirInfo = new DirectoryInfo(Application.persistentDataPath);
            var files = dirInfo.GetFiles("*.map");
            foreach (var file in files)
            {
                try
                {
                    var filename = Path.GetFileNameWithoutExtension(file.Name);
                    var data = File.ReadAllText(Map.GetFullPath(filename));
                    var map = JsonUtility.FromJson<Map>(data);
                    var instance = Instantiate(mapButtonTemplate, mapPanel);
                    instance.GetComponentInChildren<Text>().text = !string.IsNullOrWhiteSpace(map.title) ? map.title : filename;
                    instance.onClick.AddListener(() => { LoadUserMap(filename); });
                }
                catch (System.Exception) { }
            }
            mapButtonTemplate.gameObject.SetActive(false);
        }

        public void LoadBuiltinMap(int i)
        {
            LoadMap(builtinMaps[i].text);
        }

        public void LoadUserMap(string filename)
        {
            LoadMap(File.ReadAllText(Map.GetFullPath(filename)));
        }

        public void LoadMap(string data)
        {
            PlayerPrefs.SetString(PlayerPrefsKeys.MapData, data);
            SceneChanger.LoadScene(SceneNames.Gameplay);
        }

        public void OpenMapEditor()
        {
            SceneChanger.LoadScene(SceneNames.MapEditor);
        }

        public void Quit()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}