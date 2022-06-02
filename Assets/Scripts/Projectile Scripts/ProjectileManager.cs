using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Functions for projectile initialization goes here
// This script can be reached from any other script using ProjectileManager.Instance.stuff where "stuff" is any public variable/function
public class ProjectileManager : MonoBehaviour
{
    public static ProjectileManager Instance;

    [SerializeField] public GameObject target;

    private void Awake()
    {
        Instance = this;
    }

    // Function to be called whenever a projectiles should be created
    public void SpawnProjectiles(GameObject projectilePrefab, Vector3 position)
    {
        GameObject.Find("AudioManager")?.GetComponent<AudioManager>()?.PlaySound("shoot");
        switch (projectilePrefab.GetComponent<Projectile>().projectileType)
        {
            case ProjectileType.Basic:
                Basic(projectilePrefab, position);
                break;
            case ProjectileType.Homing:
                Homing(projectilePrefab, position);
                break;
            case ProjectileType.Triple:
                Triple(projectilePrefab, position);
                break;
            case ProjectileType.Hexagon:
                Hexagon(projectilePrefab, position);
                break;
            case ProjectileType.Bounce:
                Bounce(projectilePrefab, position);
                break;
        }
    }

    public void Basic(GameObject projectilePrefab, Vector3 position)
    {
        GameObject newProjectile = Instantiate(projectilePrefab, position, Quaternion.identity, this.transform);
        Projectile script = newProjectile.GetComponent<Projectile>();
        script.LookAtPlayer(target);
        script.ShootForward();
    }
    
    public void Homing(GameObject projectilePrefab, Vector3 position)
    {
        GameObject newProjectile = Instantiate(projectilePrefab, position, Quaternion.identity, this.transform);
        Projectile script = newProjectile.GetComponent<Projectile>();
        script.StartCoroutine(script.SeekPlayer(target));
    }
    
    public void Triple(GameObject projectilePrefab, Vector3 position)
    {
        float totalCount = 3;
        float angle = 30f;
        for (int i = -(int)totalCount / 2; i < Mathf.Ceil(totalCount / 2); i++)
        {
            GameObject newProjectile = Instantiate(projectilePrefab, position, Quaternion.identity, this.transform);
            Projectile script = newProjectile.GetComponent<Projectile>();
            script.LookAtPlayer(target);
            script.RotateAroundY(i * angle);
            script.ShootForward();
        }
    }
    
    public void Hexagon(GameObject projectilePrefab, Vector3 position)
    {
        int ngon = 6;
        for (int i = 0; i < ngon; i++)
        {
            GameObject newProjectile = Instantiate(projectilePrefab, position, Quaternion.identity);
            Projectile script = newProjectile.GetComponent<Projectile>();
            script.LookAtPlayer(target);
            script.RotateAroundY(360 / ngon * i);
            script.ShootForward();
        }
    }

    public void Bounce(GameObject projectilePrefab, Vector3 position)
    {
        float totalCount = 3;
        float angle = 30f;
        float offset = UnityEngine.Random.Range(-20, 20);

        for (int i = -(int)totalCount / 2; i < Mathf.Ceil(totalCount / 2); i++)
        {
            Quaternion quaternionAngle = Quaternion.AngleAxis(i * angle, Vector3.up);
            Vector3 pos = position - quaternionAngle * gameObject.transform.forward;
            GameObject newProjectile = Instantiate(projectilePrefab, pos, quaternionAngle, this.transform);
            Projectile script = newProjectile.GetComponent<Projectile>();
            script.LookAtV3(Vector3.zero);
            script.RotateAroundY(i * angle + offset);
            script.ShootForward();
        }
    }
}

public enum ProjectileType
{
    Basic,
    Homing,
    Triple,
    Hexagon,
    Bounce
};


