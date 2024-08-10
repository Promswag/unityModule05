using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rigidBody;
    private CircleCollider2D detectionArea;
    private AudioSource audioSource;
    [SerializeField] private int damage = 1;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float lifetime = 5f;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        detectionArea = GetComponent<CircleCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        StartCoroutine(Lifetime());
    }

    public void SetVelocity(Vector2 position)
    {
        rigidBody.velocity = (position - (Vector2)transform.position).normalized * speed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        rigidBody.velocity = Vector2.zero;
        animator.SetTrigger("boom");
        audioSource.Play();
        if (collision.gameObject.CompareTag("Player"))
            collision.gameObject.GetComponent<PlayerController>().TakesDamage(damage);
        detectionArea.enabled = false;
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    IEnumerator Lifetime()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }
}
