using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Character Shooter;
    private Transform target;
    private Character targetCharacter;
    private int damage;
    public float speed = 10f; // Скорость проектила

    public void Initialize(Transform target,Character thisCharacter, Character targetCharacter, int damage)
    {
        this.target = target;
        this.targetCharacter = targetCharacter;
        this.damage = damage;
        this.Shooter = thisCharacter;
    }

    void Update()
    {
        if (target != null)
        {
            // Двигаемся к цели
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

            // Проверяем достижение цели
            if (transform.position == target.position)
            {
                // Наносим урон цели
                if (targetCharacter != null)
                {
                    targetCharacter.TakeDamage(damage, Shooter);
                }

                // Удаляем снаряд
                Destroy(gameObject);
            }
        }
        else
        {
            // Удалить снаряд, если цель потеряна
            Destroy(gameObject);
        }
    }
}
