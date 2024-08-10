using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DiaryMenu : MonoBehaviour
{
    [SerializeField] private Text scoreLabel;
    [SerializeField] private Text deathsLabel;
    [SerializeField] private GameObject stageContainer;
    private List<GameObject> stageList;

    int score = 0;
    int deaths = 0;

    private bool menuIsActive = true;

    void Start()
    {
        deaths = PlayerPrefs.GetInt("Deaths", 0);
        deathsLabel.text = deaths.ToString();

        foreach (string sceneName in GameManager.Instance.sceneNames)
        {
            foreach(char c in PlayerPrefs.GetString(sceneName, ""))
            {
                if (c == '0')
                {
                    score += 5;
                }
            }
        }

        stageList = new();
        foreach(Transform child in stageContainer.transform)
        {
            stageList.Add(child.gameObject);
        }
        stageList.Sort((item1, item2) => item1.name.CompareTo(item2.name));

        scoreLabel.text = score.ToString();
        int UnlockedStage = PlayerPrefs.GetInt("UnlockedStage", 2);
        for (int i = 2; i <= UnlockedStage; i++)
        {
            stageList[i-2].GetComponent<Image>().color = Color.white;
        }

    }

    public void Back()
    {
        if (menuIsActive)
        {
            menuIsActive = false;
            GameManager.Instance.LoadScene("MainMenu");
        }
    }
}
