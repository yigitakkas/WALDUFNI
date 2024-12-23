using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarManager : MonoBehaviour
{
    public GameObject StarPrefab;
    public Vector2 SpawnAreaSize = new Vector2(10, 5);
    public Vector2 SpawnIntervalRange = new Vector2(1f, 3f);
    public Vector2 StarsPerSpawnRange = new Vector2(1, 5); 
    public float StarSpeed = 5f;
    public float StarLifetime = 3f;

    private void Start()
    {
        StartCoroutine(SpawnStars());
    }

    private IEnumerator SpawnStars()
    {
        while (true)
        {
            float spawnInterval = Random.Range(SpawnIntervalRange.x, SpawnIntervalRange.y);

            yield return new WaitForSeconds(spawnInterval);

            int starsToSpawn = Random.Range((int)StarsPerSpawnRange.x, (int)StarsPerSpawnRange.y + 1);

            for (int i = 0; i < starsToSpawn; i++)
            {
                SpawnAndMoveStar();
            }
        }
    }

    private void SpawnAndMoveStar()
    {
        float x = Random.Range(-SpawnAreaSize.x / 2, SpawnAreaSize.x / 2);
        float y = Random.Range(2, SpawnAreaSize.y / 2);
        Vector3 spawnPosition = new Vector3(x, y, 0);

        GameObject star = Instantiate(StarPrefab, spawnPosition, Quaternion.identity);

        StartCoroutine(MoveAndDestroyStar(star));
    }

    private IEnumerator MoveAndDestroyStar(GameObject star)
    {
        Animator animator = star.GetComponent<Animator>();
        if (animator != null)
        {
            animator.Play("StarAnimation");
        }

        float elapsedTime = 0f;
        Vector3 startPosition = star.transform.position;
        Vector3 targetPosition = startPosition + new Vector3(Random.Range(-5, 5), -10, 0);

        while (elapsedTime < StarLifetime)
        {
            star.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime * StarSpeed);
            elapsedTime += Time.deltaTime;
            if (star.transform.position.y < 0.6f)
                break;
            yield return null;
        }

        Destroy(star);
    }
}
