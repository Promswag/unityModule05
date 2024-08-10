using UnityEngine;

public class Collectible : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.GetType() == typeof(CapsuleCollider2D))
        {
            gameObject.SetActive(false);
            GameManager.Instance.gameData.score += 5;
            UserInterface.Instance.UpdateUI();
        }
    }
}
