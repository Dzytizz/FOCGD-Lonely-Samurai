using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ranged enemy specific actions
public class RangedEnemy : Enemy
{
    [SerializeField] private RangedEnemySO data;
 
    private bool isMovingRandomly;
    private float movementChance;
    private Vector3 maxMovement;
    private GameObject projectilePrefab;

    private float timer = 0;

    private void Start()
    {
        SetData(data);
        isMovingRandomly = data.isMovingRandomly;
        movementChance = data.movementChance;
        maxMovement = data.maxMovement;
        projectilePrefab = data.projectilePrefab;

        if(isMovingRandomly)
        {
            StartCoroutine(RandomMovement(2));
        }

    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > 1 / attackSpeed)
        {
            ProjectileManager.Instance.SpawnProjectiles(projectilePrefab, transform.position);
            timer = 0;
        }
    }

    IEnumerator RandomMovement(float waitSeconds)
    {
        while(true)
        {
            yield return new WaitForSeconds(waitSeconds);

            float rand = UnityEngine.Random.value;
            if (rand < movementChance)
            {
                int direction = UnityEngine.Random.Range(0, 4);
                Vector3 offset = Vector3.zero;
                switch (direction)
                {
                    case 0:
                        offset.x = maxMovement.x;
                        break;
                    case 1:
                        offset.x = -maxMovement.x;
                        break;
                    case 2:
                        offset.z = maxMovement.z;
                        break;
                    case 3:
                        offset.z = -maxMovement.z;
                        break;
                }
                StartCoroutine(Move(offset));
            }
        }
    }

    IEnumerator Move(Vector3 direction)
    {
        Vector3 targetDirection = target.transform.position;
        targetDirection.y = transform.position.y;
        transform.LookAt(targetDirection);

        Vector3 currentPos = transform.position;
        Vector3 targetPos = currentPos + direction;
        targetPos.y = currentPos.y;

        Ray ray = new Ray(currentPos, direction);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, direction.magnitude))
        {
            targetPos = hit.point - ray.direction.normalized * colliderOffset;
        }

        float step = movementSpeed / direction.magnitude;
        float t = 0;
        while (t <= 1)
        {
            t += step * Time.deltaTime;
            transform.position = Vector3.Lerp(currentPos, targetPos, t);
            yield return null;
        }
    }
}
