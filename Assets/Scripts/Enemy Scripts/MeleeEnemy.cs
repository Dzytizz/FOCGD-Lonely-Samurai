using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Melee enemy specific actions
public class MeleeEnemy : Enemy
{
    [SerializeField] private MeleeEnemySO data;

    private int meleeDamage;
    private float dashDistance;
    private bool isFollowing;

    private bool isDashing;
    private NavMeshAgent agent;
    public bool isAttacking = false;
    private IEnumerator coroutine;

    private void Start()
    {
        SetData(data);
        meleeDamage = data.meleeDamage;
        dashDistance = data.dashDistance;
        isFollowing = data.isFollowing;
        agent = GetComponent<NavMeshAgent>();
        if(isFollowing)
        {
            agent.speed = movementSpeed;
        }

        StartCoroutine(Move(attackSpeed));
    }

    private void Update()
    {
        if(isFollowing)
        {
            agent.SetDestination(target.transform.position);
        }
    }

    IEnumerator Move(float attackSpeed)
    {
        while(!isFollowing)
        {
            Vector3 targetDirection = target.transform.position;
            targetDirection.y = transform.position.y;
            transform.LookAt(targetDirection);
            yield return new WaitForSeconds(1 / attackSpeed);
            isDashing = true;
            transform.LookAt(targetDirection);

            Vector3 currentPos = transform.position;
            Vector3 targetPos = currentPos + transform.forward * dashDistance;
            targetPos.y = currentPos.y;

            Ray ray = new Ray(currentPos, transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, dashDistance, ~(1 << 6))) // ignore player layer
            {
                targetPos = hit.point - ray.direction.normalized * colliderOffset;
            }

            float step = movementSpeed / dashDistance;
            float t = 0;
            while (t <= 1)
            {
                t += step * Time.deltaTime;
                transform.position = Vector3.Lerp(currentPos, targetPos, t);
                yield return null;
            }
            isDashing = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(isDashing && other.CompareTag("Player"))
        {
            other.GetComponent<Player>().TakeDamage(meleeDamage);
        }
        if(isFollowing && !isAttacking && other.CompareTag("Player"))
        {
            isAttacking = true;
            coroutine = DealDamage(1 / attackSpeed, other);
            StartCoroutine(coroutine);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isDashing && other.CompareTag("Player"))
        {
            other.GetComponent<Player>().TakeDamage(meleeDamage);
        }
        if (isFollowing && isAttacking && other.CompareTag("Player"))
        {
            isAttacking = false;
        }
    }

    IEnumerator DealDamage(float seconds, Collider other)
    {
        while (isAttacking)
        {
            yield return new WaitForSeconds(seconds);
            if (gameObject != null &&  other != null && isAttacking)
            {
                other.GetComponent<Player>().TakeDamage(meleeDamage);
            }
            else
            {
                StopCoroutine(coroutine);
            }
        }
    }
}
