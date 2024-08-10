using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EndOfStage : MonoBehaviour
{
    [SerializeField] private Text notice;
    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.GetType() == typeof(CapsuleCollider2D))
        {
            if (GameManager.Instance.gameData.score >= 25)
            {
                collider.gameObject.GetComponent<PlayerController>().enabled = false;
                collider.gameObject.GetComponent<PlayerController>().isDead = true;
                GameManager.Instance.NextStage();
            }
            else
            {
                Debug.Log("Need more score to proceed");
                StartCoroutine(DisplayNotice());
            }
        }
    }

    IEnumerator DisplayNotice()
    {
        Color color = notice.color;
        color.a = 1f;
        notice.color = color;
        yield return null;
        while (color.a > 0f)
        {
            color.a -= Time.deltaTime / 3f;
            notice.color = color;
            yield return null;
        }
    }
}
