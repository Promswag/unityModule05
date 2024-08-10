using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour
{
    static public UserInterface Instance;
    [SerializeField] private Text hpLabel;
    [SerializeField] private Text scoreLabel;

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
        }
    }

    void OnSceneLoad(Scene oldScene, Scene newScene)
    {
        if (newScene.name == "MainMenu" || newScene.name == "Diary")
        {
            if (gameObject != null)
                Destroy(gameObject);
        }
        else
        {
            UpdateUI();
        }
    }

    public void UpdateUI()
    {
        hpLabel.text = GameManager.Instance.gameData.hp.ToString();
        scoreLabel.text = GameManager.Instance.gameData.score.ToString();
    }

    void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneLoad;
    }
}
