using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class SaveData
    {
        public System.DateTime lastPlayed;
        public System.TimeSpan playTime;
        public int lastSceneIndex;
    }

    public static GameManager Instance;
    public SoundManager SoundManager { get; private set; }
    public SaveData LastSavedData => _saveData;

    public MenuScreen settingsMenu;
    public GameObject exitMenu;

    [SerializeField] AudioClip mainMenuMusic;

    private SaveData _saveData = new SaveData();
    BinaryFormatter _binaryFormatter = new BinaryFormatter();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            SoundManager = GetComponent<SoundManager>();
            transform.parent = null;
            DontDestroyOnLoad(gameObject);

            if (HasSaveGame()) LoadSavedData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnMainMenuLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        SceneManager.sceneLoaded -= OnMainMenuLoaded;
        Time.timeScale = 1;
    }

    private void OnGameSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        SceneManager.sceneLoaded -= OnGameSceneLoaded;
        Time.timeScale = 1;
        SoundManager.PlayInGameMusic(scene.buildIndex);
        _saveData.lastSceneIndex = scene.buildIndex;
        SaveGame();
    }

    public bool HasSaveGame()
    {
        return File.Exists(Application.persistentDataPath + "/gamedata.sav");
    }

    public void LoadSavedData()
    {
        var file = new FileStream(Application.persistentDataPath + "/gamedata.sav", FileMode.Open);
        _saveData = _binaryFormatter.Deserialize(file) as SaveData;
        file.Close();
    }

    public void SaveGame()
    {
        _saveData.lastPlayed = System.DateTime.Now;
        _saveData.lastSceneIndex = SceneManager.GetActiveScene().buildIndex;

        var file = new FileStream(Application.persistentDataPath + "/gamedata.sav", FileMode.OpenOrCreate);
        _binaryFormatter.Serialize(file, _saveData);
        file.Close();
    }

    public void EnableSettingsMenu()
    {
        SoundManager.PauseMusic();
        settingsMenu.Show();
    }

    public void ToggleSettingsMenu()
    {
        if (settingsMenu.Visible)
        {
            SoundManager.ResumeMusic();
            settingsMenu.Hide();
        }
        else
        {
            SoundManager.PauseMusic();
            settingsMenu.Show();
        }
    }

    public void ToggleExitMenu()
    {
        exitMenu.SetActive(!exitMenu.activeInHierarchy);
    }

    public void LoadNewGame()
    {
        SceneManager.sceneLoaded += OnGameSceneLoaded;
        SceneManager.LoadScene(1);
    }

    public void LoadSavedGameScene()
    {
        SceneManager.sceneLoaded += OnGameSceneLoaded;
        SceneManager.LoadScene(_saveData.lastSceneIndex);
    }

    public void LoadNextGameScene()
    {
        var scene = SceneManager.GetActiveScene();
        if (scene.buildIndex == 5)
        {
            File.Delete(Application.persistentDataPath + "/gamedata.sav");
            LoadMainMenuScene();
            return;
        }
        SceneManager.sceneLoaded += OnGameSceneLoaded;
        SceneManager.LoadScene(scene.buildIndex + 1);
    }

    public void ReloadGameScene()
    {
        var scene = SceneManager.GetActiveScene();
        SceneManager.sceneLoaded += OnGameSceneLoaded;
        SceneManager.LoadScene(scene.buildIndex);
    }

    public void LoadMainMenuScene()
    {
        SceneManager.sceneLoaded += OnMainMenuLoaded;
        SceneManager.LoadScene(0);
    }
}