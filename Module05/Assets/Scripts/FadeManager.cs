using UnityEngine;

public class FadeManager : MonoBehaviour
{
    static public FadeManager Instance { get; private set; }
    private Animator animator;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
            animator = GetComponentInChildren<Animator>();
        }
    }

    public void Fade()
    {
        animator.SetTrigger("FadeIn");
    }
}
