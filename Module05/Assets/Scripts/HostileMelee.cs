using System.Collections;
using UnityEngine;

public class HostileMelee : MonoBehaviour
{
    private Animator animator;
    private CircleCollider2D detectionArea;
    private CapsuleCollider2D strikeArea;
    private AudioSource audioSource;

    private bool isOnCooldown;
    [SerializeField] int damage;

    void Awake()
    {
        animator = GetComponent<Animator>();
        detectionArea = GetComponent<CircleCollider2D>();
        strikeArea = GetComponent<CapsuleCollider2D>();
        strikeArea.enabled = false;
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.GetType() == typeof(CapsuleCollider2D))
        {
            if (isOnCooldown)
            {
                Debug.Log(collider.GetType());
                strikeArea.enabled = false;
                collider.GetComponent<PlayerController>().TakesDamage(damage);
                return;
            }

            isOnCooldown = true;
            detectionArea.enabled = false;

            animator.SetTrigger("attack");
            Debug.LogFormat("{0} is attacking!", gameObject.name);
            
            FlipX(collider.transform.position);
        }
    }

    void FlipX(Vector3 pos)
    {
        float delta = (transform.position - pos).x;
        if (delta < 0)
            transform.rotation = Quaternion.Euler(Vector3.up * 180);
        else
            transform.rotation = Quaternion.identity;
    }

    public void StrikeOn()
    {
        audioSource.Play();
        strikeArea.enabled = true;
    }

    public void StrikeOff()
    {
        strikeArea.enabled = false;
    }

    public IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(2f);
        isOnCooldown = false;
        detectionArea.enabled = true;
    }
}
