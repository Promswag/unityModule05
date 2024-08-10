using System.Collections;
using Unity.Mathematics;
using UnityEditor.AssetImporters;
using UnityEngine;

public class HostileRanged : MonoBehaviour
{
    private Animator animator;
    private CircleCollider2D detectionArea;
    private GameObject target;
    private AudioSource audioSource;

    [SerializeField] private Transform muzzle;
    [SerializeField] private Projectile projectile;

    private bool isOnCooldown;

    void Awake()
    {
        animator = GetComponent<Animator>();
        detectionArea = GetComponent<CircleCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (isOnCooldown)
            return;

        isOnCooldown = true;
        detectionArea.enabled = false;

        animator.SetTrigger("attack");
        Debug.LogFormat("{0} is attacking!", gameObject.name);
        target = collider.gameObject;
        FlipX(collider.transform.position);
    }

    void FlipX(Vector3 pos)
    {
        float delta = (transform.position - pos).x;
        if (delta < 0)
            transform.rotation = Quaternion.Euler(Vector3.up * 180);
        else
            transform.rotation = Quaternion.identity;
    }

    public void FireProjectile()
    {
        Instantiate(projectile, muzzle.position, quaternion.identity).SetVelocity(target.transform.position);
        audioSource.Play();
    }

    public IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(2f);
        isOnCooldown = false;
        detectionArea.enabled = true;
    }
}
