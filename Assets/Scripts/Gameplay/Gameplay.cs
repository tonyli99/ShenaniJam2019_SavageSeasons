namespace Blobber
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Big old game manager class. Maintains the map, etc.
    /// </summary>
    public class Gameplay : MonoBehaviour
    {

        public const int RequiredKeys = 3;

        [Header("Map")]
        public Map map;
        public GameObject mapContainer;
        public List<GameObject> contentGameObjects;
        public int currentContentGameObjectIndex = -1;

        [Header("Party")]
        public PartyPanel partyPanel;
        public AddCharacterPanel addCharacterPanel;
        public Party party;

        [Header("Combat")]
        public CombatPanel combatPanel;

        [Header("LootBox")]
        public LootBoxPanel lootBoxPanel;

        [Header("Exit")]
        public GameObject mapExitPanel;

        [Header("Menu")]
        public GameObject menuPanel;

        private bool justStarted = true;

        public static Gameplay instance;

        public static bool isPaused = false;

        #region Setup

        private void Awake()
        {
            instance = this;
            justStarted = true;
        }

        private void Start()
        {
            SetupMap();
            SetupParty();
        }

        private void OnDestroy()
        {
            PlayerPrefs.SetString(PlayerPrefsKeys.PartyData, JsonUtility.ToJson(party));
        }

        #endregion

        #region Update

        private void Update()
        {
            if (isPaused) return;
            if (party.GetNumLivingCharacters() == 0 && !justStarted)
            {
                Defeat();
            }
            else if (party.characters.Count < Party.FullSize && !combatPanel.gameObject.activeInHierarchy && !addCharacterPanel.gameObject.activeInHierarchy)
            {
                OpenAddCharacterPanel();
            }
            justStarted = false;
        }

        public void Defeat()
        {
            Effects.ShowDialog("Your Party Was Defeated!", GotoMainMenu);
        }

        private void GotoMainMenu()
        {
            SceneChanger.LoadScene(SceneNames.MainMenu);
        }

        #endregion

        #region Map

        private void SetupMap()
        {
            if (!PlayerPrefs.HasKey(PlayerPrefsKeys.MapData))
            {
                SceneChanger.LoadScene(SceneNames.MainMenu);
                return;
            }
            map = JsonUtility.FromJson<Map>(PlayerPrefs.GetString(PlayerPrefsKeys.MapData));
            if (map == null)
            {
                SceneChanger.LoadScene(SceneNames.MainMenu);
                return;
            }
            contentGameObjects = new List<GameObject>();
            for (int i = 0; i < (map.width * map.height); i++)
            {
                contentGameObjects.Add(null);
            }
            for (int y = 0; y < map.height; y++)
            {
                for (int x = 0; x < map.width; x++)
                {
                    var tilePosition = new Vector3(-x, 0.5f, y);

                    // Instantiate tile:
                    var tileID = map.GetTileID(x, y);
                    var tile = MapAssets.instance.tiles[tileID];
                    var square = Instantiate(tile.model, tilePosition, tile.model.transform.rotation);
                    square.transform.SetParent(mapContainer.transform);

                    // Instantiate content:
                    var tileContentID = map.GetTileContentID(x, y);
                    var tileContent = (tileContentID != -1) ? MapAssets.instance.tileContents[tileContentID] : null;
                    if (tileContent != null)
                    {
                        if (tileContent == MapAssets.instance.startTileContent)
                        {
                            Camera.main.transform.position = tilePosition + new Vector3(0, 0.5f, 0);
                            Camera.main.transform.rotation = Quaternion.Euler(0, 180, 0);
                            Avatar.instance.mapPosition = new Vector2(x, y);
                        }
                        var content = Instantiate(tileContent.model, tilePosition, tileContent.model.transform.rotation);
                        content.transform.SetParent(square.transform);
                        contentGameObjects[map.GetIndex(x, y)] = content;
                    }
                }
            }
        }

        #endregion

        #region Party

        public void SetupParty()
        {
            if (PlayerPrefs.HasKey(PlayerPrefsKeys.PartyData))
            {
                party = JsonUtility.FromJson<Party>(PlayerPrefs.GetString(PlayerPrefsKeys.PartyData));
                if (party == null) party = new Party();
            }
            else
            {
                party = new Party();
            }
            party.characters.RemoveAll(x => !x.isAlive);
            RedrawParty();
        }

        public void OpenAddCharacterPanel()
        {
            addCharacterPanel.Open();
        }

        public void AddCharacterToParty(Character character)
        {
            if (party.characters.Count >= Party.FullSize) return;
            party.characters.Add(character);
            RedrawParty();
        }

        public void RedrawParty()
        {
            partyPanel.Redraw();
        }

        #endregion

        #region Menu

        public void OpenMenuPanel()
        {
            if (menuPanel.activeInHierarchy) return;
            menuPanel.SetActive(true);
        }

        public void RunAway()
        {
            party.currency /= 2;
            SceneChanger.LoadScene(SceneNames.MainMenu);

        }

        public void ShowMapExit()
        {
            mapExitPanel.SetActive(true);
        }

        public void ExitMap()
        {
            SceneChanger.LoadScene(SceneNames.MainMenu);
        }

        #endregion

        #region Encounters

        public void HandleEncounter(int x, int y)
        {
            currentContentGameObjectIndex = map.GetIndex(x, y);
            var tileContentGameObject = Gameplay.instance.contentGameObjects[currentContentGameObjectIndex];
            if (tileContentGameObject != null)
            {
                var encounter = tileContentGameObject.GetComponentInChildren<Encounter>();
                if (encounter != null) encounter.StartEncounter();
            }
        }

        public void RemoveCurrentEncounter()
        {
            if (0 <= currentContentGameObjectIndex && currentContentGameObjectIndex < contentGameObjects.Count)
            {
                Destroy(contentGameObjects[currentContentGameObjectIndex]);
                contentGameObjects[currentContentGameObjectIndex] = null;
            }
        }

        public void StartCombat(Enemy enemy)
        {
            combatPanel.Open(enemy);
        }

        public void OpenLootBoxOffer()
        {
            lootBoxPanel.Open();
        }

        public void BuyLootBox()
        {
            CombineDNA();
        }

        #endregion

        #region DNA

        public void CombineDNA()
        {
            if (party.GetNumLivingCharacters() == 1)
            {
                Effects.ShowDialog("Winds of change pass through " + party.characters[0].name + " alone.", null);
            }
            else
            {
                var char0 = party.characters[0];
                var char1 = party.characters[1];
                var newName = char0.name.Substring(0, (char0.name.Length + 1) / 2) + char1.name.Substring(0, char1.name.Length / 2);
                Effects.ShowDialog($"New Season! Winds of change combine {char0.name} and {char1.name} into {newName}.", null);
                char0.name = newName;
                char0.level = Mathf.Max(char0.level, char1.level);

                // Randomly pick traits:
                var list = new List<int>(char0.traitIDs);
                list.AddRange(char1.traitIDs);
                var n = list.Count;
                for (int i = 0; i < n; i++)
                {
                    int r = i + Random.Range(0, n - i);
                    var t = list[r];
                    list[r] = list[i];
                    list[i] = t;
                }
                for (int i = 0; i < Character.NumTraits; i++)
                {
                    char0.traitIDs[i] = list[i];
                }

                party.characters.RemoveAt(1);
                RedrawParty();
            }
        }

        #endregion

    }
}