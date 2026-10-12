using UnityEngine;

public class MageAttack : MonoBehaviour
{
    public float attackDuration = 0.5f; // Duration of attack
    public float attackCooldown = 3f; // Cooldown between attacks
    public GameObject projectilePrefab; // Magic projectile prefab
    public Transform projectileSpawnPoint; // Spawn point for magic projectiles

    private bool isAttacking = false; // Indicates if currently attacking
    private bool isCooldown = false; // Indicates if on cooldown
    private float attackStartTime; // Time when the attack started
    private float cooldownEndTime; // Time when cooldown ends
    private Transform currentTarget; // Current target
    private Character targetCharacter;

    private int damage = 2;
    public Animator animator;

    private float slowFactor = 1f; // Slowing factor (1 = normal, 0.5 = 50% slower)

    void Start()
    {
        // Initialization if needed
    }

    public bool CanAttack()
    {
        return !isAttacking && !isCooldown;
    }

    void Update()
    {
        if (isAttacking)
        {
            float elapsedTime = Time.time - attackStartTime;

            if (elapsedTime >= attackDuration)
            {
                isAttacking = false;
                isCooldown = true;
                cooldownEndTime = Time.time + attackCooldown + attackCooldown * slowFactor; // Adjust cooldown by slow factor
                animator.SetFloat("speed", 0);
                animator.Play("Idle");
            }
        }
        else if (isCooldown)
        {
            if (Time.time >= cooldownEndTime)
            {
                isCooldown = false;
            }
        }
    }

    public void StartAttack(Transform enemyTarget, Character target, int _damage)
    {
        if (CanAttack())
        {
            damage = _damage;
            currentTarget = enemyTarget;
            targetCharacter = target;
            isAttacking = true;
            attackStartTime = Time.time;

            
            animator.SetFloat("speed", 2 * slowFactor);

            GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
            projectile.GetComponent<Projectile>().Initialize(currentTarget, GetComponent<Character>(), targetCharacter, damage);
            projectile.GetComponent<Projectile>().speed *= slowFactor;

            // Adjust direction, only affecting the Y-axis
            Vector3 lookDirection = target.transform.position - projectile.transform.position;
            lookDirection.y = 0;
            if (lookDirection != Vector3.zero)
            {
                projectile.transform.rotation = Quaternion.LookRotation(lookDirection);
            }
        }
    }

    public void ApplySlow(float slowPercentage)
    {
        slowFactor = 1f - Mathf.Clamp01(slowPercentage); // Adjust the slow factor (e.g., 50% slow = 0.5)

        if (animator != null)
        {
            animator.speed = slowFactor; // Modify playback speed of all animations
        }
    }
}
