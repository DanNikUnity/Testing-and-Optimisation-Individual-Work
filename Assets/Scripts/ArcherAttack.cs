using UnityEngine;

public class ArcherAttack : MonoBehaviour
{
    public float attackDuration = 0.5f; // Original attack duration
    public float attackCooldown = 3f; // Original cooldown duration
    public GameObject projectilePrefab; // Magic projectile prefab
    public Transform projectileSpawnPoint; // Spawn point for magic projectiles

    private bool isAttacking = false; // Is currently attacking
    private bool isCooldown = false; // Is in cooldown
    private float attackStartTime; // Time when the attack started
    private float cooldownEndTime; // Time when cooldown ends
    private Transform currentTarget; // The current target
    private Character targetCharacter; // The target character

    private int damage = 2; // Damage dealt by the archer
    public Animator animator;

    private float slowFactor = 1f; // Slowness factor (1 = normal, 0.5 = 50% slower)

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

            if (elapsedTime >= attackDuration * slowFactor) // Adjust attack duration by slowFactor
            {
                isAttacking = false;
                isCooldown = true;
                cooldownEndTime = Time.time + attackCooldown + attackCooldown * slowFactor; // Adjust cooldown by slowFactor
                animator.SetFloat("speed", 0);
                animator.Play("Idle"); // Switch to Idle state
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

            animator.SetFloat("speed", 2 * slowFactor); // Adjust animation speed based on slowFactor

            GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
            Projectile projectileScript = projectile.GetComponent<Projectile>();
            projectileScript.Initialize(currentTarget, GetComponent<Character>(), targetCharacter, damage);

            // Adjust projectile speed based on slowFactor
            projectileScript.speed *= slowFactor;

            // Set direction, adjusting only the Y-axis
            Vector3 lookDirection = target.transform.position - projectile.transform.position;
            lookDirection.y = 0; // Ignore vertical direction
            if (lookDirection != Vector3.zero) // Avoid error if lookDirection is zero
            {
                projectile.transform.rotation = Quaternion.LookRotation(lookDirection);
            }
        }
    }

    // This method allows external classes (like a debuff system) to apply slowness
    public void ApplySlow(float slowPercentage)
    {
        slowFactor = 1f - Mathf.Clamp01(slowPercentage); // Adjust the slow factor (e.g., 50% slow = 0.5)

        if (animator != null)
        {
            animator.speed = slowFactor; // Modify the playback speed of all animations
        }
    }
}
