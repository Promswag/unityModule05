using System.Collections;
using UnityEngine;

public class LeavesSpawner : MonoBehaviour
{
    [SerializeField] private GameObject fallingLeafPrefab;
    private float spawnTime = 0f;
    private float elapsedTime;

    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime > spawnTime)
        {
            elapsedTime = 0;
            spawnTime = Random.Range(0f, 2f);
            Vector3 pos = new(Random.Range(-25f, 15f), transform.position.y);
            Quaternion rotation = Quaternion.Euler(Vector3.forward * Random.Range(-180f, 180f));

            GameObject leaf = Instantiate(fallingLeafPrefab, pos, rotation, transform);
            leaf.GetComponent<SpriteRenderer>().sortingOrder = Random.Range(0f, 1f) < 0.5f ? 1 : 100;
            leaf.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(0.5f, 2f), Random.Range(-1.5f, -3f));
            leaf.GetComponent<Animator>().SetFloat("speed", Random.Range(0.5f, 1.5f));
            StartCoroutine(Lifetime(leaf));
        }
    }

    private IEnumerator Lifetime(GameObject leaf)
    {
        yield return new WaitForSeconds(20f);
        Destroy(leaf);
    }
}
