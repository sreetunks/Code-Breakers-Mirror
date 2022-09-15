using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    class SaveData
    {
        public System.DateTime lastPlayed;
        public System.TimeSpan playTime;
        public int lastSceneIndex;
    }

    public static GameManager Instance;
    public SoundManager SoundManager { get; private set; }

    public GameObject saveMenu;
    public MenuScreen settingsMenu;
    public GameObject creditsMenu;
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

    private void OnGameSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        SceneManager.sceneLoaded -= OnGameSceneLoaded;
        SoundManager.PlayInGameMusic();
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

    public void ToggleSaveMenu()
    {
        saveMenu.SetActive(!saveMenu.activeInHierarchy);
        StartCoroutine(MenuSoundEffect());
    }

    public void EnableSettingsMenu()
    {
        settingsMenu.Show();
    }

    public void ToggleSettingsMenu()
    {
        if (settingsMenu.Visible)
            settingsMenu.Hide();
        else
            settingsMenu.Show();

        SoundManager.PauseMusic();
        StartCoroutine(MenuSoundEffect());
    }

    public void ToggleCreditsMenu()
    {
        creditsMenu.SetActive(!creditsMenu.activeInHierarchy);
        StartCoroutine(MenuSoundEffect());
    }

    public void ToggleExitMenu()
    {
        exitMenu.SetActive(!exitMenu.activeInHierarchy);
        StartCoroutine(MenuSoundEffect());
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
        SceneManager.sceneLoaded += OnGameSceneLoaded;
        SceneManager.LoadScene(scene.buildIndex + 1);
    }

    public void ReloadGameScene()
    {
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
    }

    public void LoadMainMenuScene() { SceneManager.LoadScene(0); }

    public IEnumerator MenuSoundEffect()
    {
        SoundManager.menuSFX.mute = false;
        SoundManager.menuSFX.Play();
        yield return new WaitForSeconds(0.25f);
        SoundManager.menuSFX.mute = true;
        SoundManager.menuSFX.Stop();
    }
}