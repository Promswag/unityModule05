using UnityEngine;

public class MainMenuInterface : MonoBehaviour
{
    public void Resume()
    {
        GameManager.Instance.ResumeGame();    
    }

    public void NewGame()
    {
        GameManager.Instance.NewGame();    
    }

    public void Diary()
    {
        GameManager.Instance.Diary();    
    }

    public void Quit()
    {
        GameManager.Instance.Quit();    
    }
}
