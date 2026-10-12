using UnityEngine;

public class KnightAttack : MonoBehaviour
{
    public float dashSpeed = 10f; // Original dash speed
    public float returnSpeed = 5f; // Original return speed
    public float attackDuration = 0.5f; // Original attack duration
    public float attackCooldown = 3f; // Original cooldown duration

    private Vector3 startPosition; // Starting position for the dash
    private bool isAttacking = false; // Is currently attacking
    private bool isReturning = false; // Is returning after the dash
    private bool isCooldown = false; // Is in cooldown after attack
    private float attackStartTime; // The time when the attack started
    private float cooldownEndTime; // The time when cooldown ends
    private Transform currentTarget; // The current target
    private Character targetCharacter; // The target character to damage

    private Character thisCharacter; // Reference to the character component
    private int damage = 2; // Damage dealt by the knight
    public Animator animator;

    private float slowFactor = 1f; // Slowness factor (1 = normal, 0.5 = 50% slower)

    void Start()
    {
        thisCharacter = GetComponent<Character>();
    }

    void Update()
    {
        if (isAttacking)
        {
            if (animator != null)
                animator.SetFloat("speed", 2 * slowFactor); // Scale animation speed by slowFactor

            float elapsedTime = Time.time - attackStartTime;
            float scaledAttackDuration = attackDuration * (1 / slowFactor); // Adjust attack duration based on slowFactor

            if (elapsedTime < scaledAttackDuration) // Scale attack duration by slowFactor
            {
                if (currentTarget != null)
                {
                    // Move towards the target with slowed dashSpeed
                    transform.position = Vector3.Lerp(startPosition, currentTarget.position, elapsedTime / scaledAttackDuration);
                }
                else
                {
                    CancelAttack(); // Cancel the attack if the target is null
                    return;
                }
            }
            else
            {
                if (targetCharacter != null)
                {
                    targetCharacter.TakeDamage(damage, thisCharacter); // Apply damage
                }

                isAttacking = false;
                isReturning = true; // Start returning after the dash
            }
        }
        else if (isReturning)
        {
            if (animator != null)
                animator.SetFloat("speed", 1.5f * slowFactor); // Apply slow to return speed

            // Move back to the start position with slowed returnSpeed
            transform.position = Vector3.MoveTowards(transform.position, startPosition, returnSpeed * slowFactor * Time.deltaTime);

            if (transform.position == startPosition)
            {
                isReturning = false;
                isCooldown = true;
                cooldownEndTime = Time.time + attackCooldown * (1 / slowFactor); // Adjust cooldown by slowFactor
                if (animator != null)
                {
                    animator.SetFloat("speed", 0);
                    animator.Play("Idle");
                }
            }
        }
        else if (isCooldown)
        {
            if (Time.time >= cooldownEndTime)
            {
                isCooldown = false; // Cooldown complete
            }
        }
    }

    public void StartAttack(Transform enemyTarget, Character target, int _damage)
    {
        if (!isAttacking && !isReturning && !isCooldown)
        {
            if (enemyTarget == null || target == null)
            {
                Debug.LogWarning("Target or Character is null, attack canceled.");
                return;
            }

            startPosition = transform.position;
            damage = _damage;
            currentTarget = enemyTarget; // Set the current target
            targetCharacter = target; // Set the target character
            isAttacking = true; // Start the attack
            attackStartTime = Time.time; // Record the attack start time
        }
    }

    public bool CanAttack()
    {
        if (isAttacking || isReturning)
        {
            return false;
        }
        return true;
    }

    private void CancelAttack()
    {
        Debug.LogWarning("Attack canceled due to null target.");
        isAttacking = false;
        isReturning = true;
        targetCharacter = null;
        currentTarget = null;
    }

    // This method allows external classes (like the Mage or a debuff system) to apply slowness
    public void ApplySlow(float slowPercentage)
    {
        slowFactor = 1f - Mathf.Clamp01(slowPercentage); // Adjust the slow factor (e.g., 50% slow = 0.5)

        if (animator != null)
        {
            animator.speed = slowFactor; // Modify playback speed of all animations
        }
    }
}
