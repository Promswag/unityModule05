using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    static public GameManager Instance { get; private set; }

    [SerializeField] private Animator fadeAnimator;
    private GameObject player;
    private GameObject start;
    private List<GameObject> collectibles;
    public HashSet<string> sceneNames;

    public struct GameData {
        public int hp;
        public int score;
        public int deaths;
    };

    public GameData gameData;
    private StringBuilder stringBuilder;

    private enum StageState {
        OLD,
        NEW,
        END
    }

    private StageState state = StageState.OLD;
    private Scene activeScene;

    private bool menuIsActive;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.activeSceneChanged += OnSceneLoad;

            collectibles = new();
            stringBuilder = new();            
            sceneNames = new();
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
                sceneNames.Add(System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i)));
        }
    }

    void OnSceneLoad(Scene prevScene, Scene newScene)
    {
        menuIsActive = true;
        activeScene = newScene;
        Debug.LogFormat("{0} had been loaded.", activeScene.name);
        if (newScene.name == "MainMenu")
        {
            FindFirstObjectByType<Button>().interactable = PlayerPrefs.GetInt("UnlockedStage", 0) != 0;
            return;
        }
        else if (newScene.name == "Diary")
        {
            return;
        }
        else
        {
            start = GameObject.FindGameObjectWithTag("Start");
            if (start == null)
            {
                Debug.LogErrorFormat("Could not find 'Start' in Scene '{0}'.", activeScene.name);
            }
            else
            {
                player = GameObject.FindGameObjectWithTag("Player");
                if (player == null)
                {
                    Debug.LogErrorFormat("Could not find 'Player' in Scene '{0}'.", activeScene.name);
                }
                else
                    ResetPlayerPos();
            }

            collectibles.Clear();
            foreach(GameObject c in GameObject.FindGameObjectsWithTag("Collectible"))
                collectibles.Add(c);
            collectibles.Sort((item1, item2) => item1.name.CompareTo(item2.name));

            if (state == StageState.OLD)
            {

            } 
            else if (state == StageState.NEW)
            {
                PlayerPrefs.DeleteKey("Score");
                PlayerPrefs.DeleteKey("HP");
            } 
            else if (state == StageState.END)
            {

            }
            LoadProgress();
            UserInterface.Instance.UpdateUI();
        }
    }

    public void Fade()
    {
        fadeAnimator.SetTrigger("FadeIn");
    }

    public void NextStage()
    {
        if (menuIsActive)
        {
            menuIsActive = false;
            if (activeScene.buildIndex + 1 < SceneManager.sceneCountInBuildSettings)
            {
                SaveProgress(activeScene.buildIndex + 1);
                state = StageState.NEW;
                StartCoroutine(LoadingSceneFade(activeScene.buildIndex + 1));
            }
            else
            {
                SaveProgress(activeScene.buildIndex);
                state = StageState.END;
                StartCoroutine(LoadingSceneFade("MainMenu"));
            }
        }
    }

    public void ExitStage()
    {
        if (menuIsActive)
        {
            menuIsActive = false;
            state = StageState.OLD;
            SaveProgress(activeScene.buildIndex);
            LoadScene("MainMenu");
        }
    }

    public void ResumeGame()
    {
        if (menuIsActive)
        {
            menuIsActive = false;
            StartCoroutine(LoadingSceneFade(PlayerPrefs.GetInt("UnlockedStage", 2)));
        }
    }

    public void NewGame()
    {
        if (menuIsActive)
        {
            menuIsActive = false;
            PlayerPrefs.DeleteAll();
            StartCoroutine(LoadingSceneFade(PlayerPrefs.GetInt("UnlockedStage", 2)));
        }
    }

    public void Diary()
    {
        if (menuIsActive)
        {
            menuIsActive = false;
            StartCoroutine(LoadingSceneFade("Diary"));
        }
    }

    public void Quit()
    {
        if (menuIsActive)
        {
            menuIsActive = false;
            PlayerPrefs.DeleteAll();
            StartCoroutine(LoadingSceneFade(activeScene.buildIndex));
            Application.Quit();
        }
    }

    public void LoadProgress()
    {
        gameData.hp = PlayerPrefs.GetInt("HP", 3);
        gameData.score = PlayerPrefs.GetInt("Score", 0);
        gameData.deaths = PlayerPrefs.GetInt("Deaths", 0);
        string stageLeaves = PlayerPrefs.GetString(activeScene.name, "");
        
        Array leaves = stageLeaves.Split(',')
                    .Where(s => int.TryParse(s, out _))
                    .Select(int.Parse)
                    .ToArray();
        if (leaves.Length > collectibles.Count)
        {
            Debug.LogErrorFormat("PlayerPrefs leaves string of stage {0} is corrupted.", activeScene.name);
        }
        else
        {
            int i = 0;
            foreach (int leaf in leaves)
                collectibles[i++].SetActive(leaf == 1);
        }
    }

    public void SaveProgress(int sceneIndex)
    {
        PlayerPrefs.SetInt("HP", gameData.hp);
        PlayerPrefs.SetInt("Score", gameData.score);
        PlayerPrefs.SetInt("Deaths", gameData.deaths);
        PlayerPrefs.SetInt("UnlockedStage", sceneIndex);

        stringBuilder.Clear();
        foreach (GameObject collectible in collectibles)
        {
            stringBuilder.Append(collectible.activeInHierarchy ? 1 : 0);
            stringBuilder.Append(',');
        }
        PlayerPrefs.SetString(activeScene.name, stringBuilder.ToString());
    }

    public void LoadScene(int sceneIndex)
    {
        if (sceneIndex >= 0 && sceneIndex < SceneManager.sceneCountInBuildSettings)
            StartCoroutine(LoadingSceneFade(sceneIndex));
        else
            Debug.LogErrorFormat("Could not find scene with index {0}.", sceneIndex);
    }

    public void LoadScene(string sceneName)
    {
        if (sceneNames.Contains(sceneName))
            StartCoroutine(LoadingSceneFade(sceneName));
        else
            Debug.LogErrorFormat("Could not find scene with name {0}.", sceneName);
    }

    IEnumerator LoadingSceneFade(int sceneIndex)
    {
        Fade();
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(sceneIndex);
    }

    IEnumerator LoadingSceneFade(string sceneName)
    {
        Fade();
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(sceneName);
    }

    public void ResetPlayerPos()
    {
        player.transform.position = start.transform.position;
    }

    void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneLoad;
    }
}
