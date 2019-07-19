namespace Blobber
{
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;

    public class MapEditor : MonoBehaviour
    {
        [Header("Menu")]

        public GameObject messagePanel;
        public Text messageText;

        [Space]
        public GameObject loadPanel;
        public Transform loadFilenamePanel;
        public Button loadButtonTemplate;
        private List<Button> loadButtons = new List<Button>();

        [Space]
        public GameObject savePanel;
        public InputField saveAsInputField;

        [Space]
        public InputField nameInputField;

        [Space]
        public Transform tileButtonPanel;
        public Text selectedTileDescription;
        public TileButtonTemplate tileButtonTemplate;
        private List<TileButtonTemplate> tileButtons = new List<TileButtonTemplate>();
        public Transform tileContentButtonPanel;
        public Text selectedTileContentDescription;
        public TileButtonTemplate tileContentButtonTemplate;
        private List<TileButtonTemplate> tileContentButtons = new List<TileButtonTemplate>();
        public Button fillMapButton;

        [Space]
        public List<InputField> messageInputFields;

        [Header("Map")]

        public GridLayoutGroup mapGrid;
        public TilePanelTemplate tilePanelTemplate;
        private List<TilePanelTemplate> tilePanels = new List<TilePanelTemplate>();

        public Map map = new Map();

        private List<Map> undoList = new List<Map>();
        private const int MaxUndoSize = 10;

        private int selectedTileID = -1;
        private int selectedTileContentID = -1;

        #region Setup

        private void Start()
        {
            SetupMenuPanel();
            tilePanelTemplate.gameObject.SetActive(false);
            CreateNewMap();
            undoList.Clear();
#if UNITY_WEBGL
            ShowMessage("Browser security doesn't allow the web version to save maps although you can still try out the editor. To save your creations, use the Windows version.");
#endif
        }

        private void SetupMenuPanel()
        {
            loadButtonTemplate.gameObject.SetActive(false);
            for (int i = 0; i < MapAssets.instance.tiles.Count; i++)
            {
                var instance = Instantiate<TileButtonTemplate>(tileButtonTemplate, tileButtonPanel);
                int id = i;
                instance.GetComponent<Button>().onClick.AddListener(() => { SelectID(id, -1); });
                instance.tileImage.sprite = MapAssets.instance.tiles[i].icon;
                instance.highlightImage.gameObject.SetActive(false);
                tileButtons.Add(instance);
            }
            for (int i = 0; i < MapAssets.instance.tileContents.Count; i++)
            {
                var instance = Instantiate<TileButtonTemplate>(tileContentButtonTemplate, tileContentButtonPanel);
                int id = i;
                instance.GetComponent<Button>().onClick.AddListener(() => { SelectID(-1, id); });
                instance.tileImage.sprite = MapAssets.instance.tileContents[i].icon;
                instance.highlightImage.gameObject.SetActive(false);
                tileContentButtons.Add(instance);
            }
            tileButtonTemplate.gameObject.SetActive(false);
            tileContentButtonTemplate.gameObject.SetActive(false);
            fillMapButton.interactable = false;
        }

        #endregion

        #region Edit Map

        public void RecordUndo()
        {
            if (map == null) return;
            if (undoList.Count >= MaxUndoSize) undoList.RemoveAt(0);
            undoList.Add(new Map(map));
        }

        public void Undo()
        {
            if (undoList.Count == 0) return;
            map = undoList[undoList.Count - 1];
            undoList.RemoveAt(undoList.Count - 1);
            RedrawMap();
        }

        public void RedrawMap()
        {
            for (int i = tilePanels.Count - 1; i >= 0; i--)
            {
                Destroy(tilePanels[i].gameObject);
            }
            tilePanels.Clear();

            mapGrid.constraintCount = map.width;
            for (int y = 0; y < map.height; y++)
            {
                for (int x = 0; x < map.width; x++)
                {
                    var tileID = map.GetTileID(x, y);
                    var tileContentID = map.GetTileContentID(x, y);
                    var tilePanel = Instantiate<TilePanelTemplate>(tilePanelTemplate, mapGrid.transform);
                    tilePanel.gameObject.SetActive(true);
                    tilePanel.Assign(tileID, tileContentID);
                    var tileX = x;
                    var tileY = y;
                    tilePanel.GetComponent<Button>().onClick.AddListener(() => { ClickTile(tileX, tileY); });
                    tilePanels.Add(tilePanel);
                }
            }
        }

        public void SelectID(int tileID, int tileContentID)
        {
            selectedTileID = tileID;
            selectedTileContentID = tileContentID;
            for (int i = 0; i < tileButtons.Count; i++)
            {
                tileButtons[i].highlightImage.gameObject.SetActive(i == selectedTileID);
            }
            for (int i = 0; i < tileContentButtons.Count; i++)
            {
                tileContentButtons[i].highlightImage.gameObject.SetActive(i == selectedTileContentID);
            }
            fillMapButton.interactable = selectedTileID != -1;
            selectedTileDescription.gameObject.SetActive(tileID != -1);
            selectedTileDescription.text = (tileID != -1) ? MapAssets.instance.tiles[selectedTileID].description: string.Empty;
            selectedTileContentDescription.gameObject.SetActive(tileContentID != -1);
            selectedTileContentDescription.text = (tileContentID != -1) ? MapAssets.instance.tileContents[selectedTileContentID].description : string.Empty;

        }

        public void ClickTile(int x, int y)
        {
            var index = map.GetIndex(x, y);
            if (selectedTileID != -1)
            {
                RecordUndo();
                map.tileIDs[index] = selectedTileID;
                tilePanels[index].tileImage.sprite = MapAssets.instance.tiles[selectedTileID].icon;
            }
            if (selectedTileContentID != -1)
            {
                RecordUndo();
                var isContentAlreadyThere = map.tileContentIDs[index] == selectedTileContentID;
                if (isContentAlreadyThere)
                {
                    map.tileContentIDs[index] = -1;
                    tilePanels[index].contentImage.gameObject.SetActive(false);
                }
                else
                {
                    map.tileContentIDs[index] = selectedTileContentID;
                    tilePanels[index].contentImage.sprite = MapAssets.instance.tileContents[selectedTileContentID].icon;
                    tilePanels[index].contentImage.gameObject.SetActive(true);
                }
            }
        }

        public void FillMapWithSelectedTile()
        {
            RecordUndo();
            for (int i = 0; i < map.tileIDs.Count; i++)
            {
                map.tileIDs[i] = selectedTileID;
            }
            RedrawMap();
        }

#endregion

        #region Menu Buttons

        public void ShowMessage(string message)
        {
            messageText.text = message;
            messagePanel.SetActive(true);
        }

        public void CreateNewMap()
        {
            RecordUndo();
            map.Initialize(Map.DefaultWidth, Map.DefaultHeight);
            RedrawMap();
        }

        public void ClickedSaveMap()
        {
            var startIndex = MapAssets.instance.tileContents.FindIndex(x => x == MapAssets.instance.startTileContent);
            var hasStartContent = map.tileContentIDs.Contains(startIndex);
            if (hasStartContent)
            {
                savePanel.SetActive(true);
            }
            else
            {
                ShowMessage("Add a Start point (green arrow) first.");
            }
        }

        public void SaveNow()
        {
            try
            {
                var filename = saveAsInputField.text;
                filename = Path.GetFileNameWithoutExtension(filename);
                if (string.IsNullOrEmpty(filename))
                {
                    ShowMessage("No filename specified.");
                    return;
                }
                if (string.IsNullOrEmpty(map.title) || map.title == Map.DefaultTitle) map.title = nameInputField.text;
                if (string.IsNullOrEmpty(map.title)) map.title = filename;
                map.messages.Clear();
                for (int i = 0; i < messageInputFields.Count; i++)
                {
                    map.messages.Add(messageInputFields[i].text);
                }
                var data = JsonUtility.ToJson(map);
                File.WriteAllText(Map.GetFullPath(filename), data);
                ShowMessage("Saved:\n" + filename);
            }
            catch (System.Exception e)
            {
                ShowMessage("Save failed: " + e.Message);
            }
        }

        public void OpenLoadPanel()
        {
            foreach (var loadButton in loadButtons)
            {
                Destroy(loadButton.gameObject);
            }
            loadButtons.Clear();

            var dirInfo = new DirectoryInfo(Application.persistentDataPath);
            var files = dirInfo.GetFiles("*.map");
            foreach (var file in files)
            {
                var filename = Path.GetFileNameWithoutExtension(file.Name);
                var instance = Instantiate(loadButtonTemplate, loadFilenamePanel);
                instance.GetComponentInChildren<Text>().text = filename;
                instance.onClick.AddListener(() => { LoadMap(filename); });
                instance.gameObject.SetActive(true);
                loadButtons.Add(instance);
            }
            loadPanel.SetActive(true);
        }

        public void LoadMap(string filename)
        {
            loadPanel.SetActive(false);
            RecordUndo();
            try
            {
                var data = File.ReadAllText(Map.GetFullPath(filename));
                var newMap = JsonUtility.FromJson<Map>(data);
                map = newMap;
                nameInputField.text = map.title;
                for (int i = 0; i < map.messages.Count; i++)
                {
                    messageInputFields[i].text = map.messages[i];
                }
                RedrawMap();
                saveAsInputField.text = filename;
            }
            catch (System.Exception e)
            {
                ShowMessage("Load failed: " + e.Message);
            }
        }

        public void ReturnToMainMenu()
        {
            SceneChanger.LoadScene(SceneNames.MainMenu);
        }

        #endregion

    }
}