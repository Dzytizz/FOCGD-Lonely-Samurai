using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerMeleeEnemy : MultiplayerEnemy
{
    private int meleeDamage;
    private float dashDistance;
    private bool isDashing;

    private void Start()
    {
        meleeDamage = 15;
        dashDistance = 5;

        if (isServer)
        {
            StartCoroutine(Move(attackSpeed));
        }
    }

    IEnumerator Move(float attackSpeed)
    {
        while (true)
        {
            Vector3 targetDirection;
            float distance1 = Vector3.Distance(gameObject.transform.position, targets[0].transform.position);
            float distance2 = Vector3.Distance(gameObject.transform.position, targets[1].transform.position);
            if(distance1 < distance2)
            {
                targetDirection = targets[0].transform.position;
            }
            else
            {
                targetDirection = targets[1].transform.position;
            }
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
        if (isDashing && other.CompareTag("Player"))
        {
            other.GetComponent<MultiplayerPlayer>().TakeDamage(meleeDamage);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isDashing && other.CompareTag("Player"))
        {
            other.GetComponent<MultiplayerPlayer>().TakeDamage(meleeDamage);
        }
    }
}
