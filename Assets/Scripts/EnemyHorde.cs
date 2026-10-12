using UnityEngine;
using System.Linq;

public class EnemyHorde : MonoBehaviour
{
    public Character[] enemies;
    public Character[] AllyCharacters;

    public CharacterMovement characterMovement;
    public bool inFight;

    private void Start()
    {
        characterMovement = FindAnyObjectByType<CharacterMovement>();
    }
    private void FixedUpdate()
    {

        if (AllCharactersAreNull(enemies))
        {
            Destroy(gameObject);
        }
        else if (AllCharactersAreNull(AllyCharacters) && inFight)
        {
            Invoke("EndFight", 2f);
        }

        foreach (Character chr in AllyCharacters)
        {

        }
    }

    private bool AllCharactersAreNull(Character[] characters)
    {
        foreach (Character character in characters)
        {
            if (character != null)
            {
                return false;
            }
        }
        return true;
    }
    public void Update()
    {
        if (!inFight)
            return;
        foreach (Character enemy in enemies)
        {
            if (enemy != null)
            {
                Character target = GetTarget(enemy);
                if (target != null)
                {
                    if (enemy.isKnight && target.isKnight)
                    {
                        if (target.GetComponent<KnightAttack>().CanAttack())
                            enemy.Attack(target);
                    }
                    else if (enemy.isKnight)
                    {
                        enemy.Attack(target);
                    }
                    else if (target.isArcher || enemy.isMage)
                    {
                        enemy.Attack(target);
                    }
                    else
                    {
                        enemy.Attack(target);
                    }
                }
            }
        }

    }

    private Character GetTarget(Character enemy)
    {
        if (enemy.isKnight)
        {
            Character[] allyKnights = AllyCharacters.Where(c => c != null && c.isKnight).ToArray();
            if (allyKnights.Length > 0)
            {
                return allyKnights[Random.Range(0, allyKnights.Length)];
            }
            else
            {
                Character[] nonNullAllies = AllyCharacters.Where(c => c != null).ToArray();
                if (nonNullAllies.Length > 0)
                {
                    Character target = nonNullAllies[Random.Range(0, nonNullAllies.Length)];
                    return target;
                }
            }
        }
        else if (enemy.isMage || enemy.isArcher)
        {
            float roll = Random.value;
            if (roll <= 0.7f)
            {
                Character[] allyKnights = AllyCharacters.Where(c => c != null && c.isKnight).ToArray();
                if (allyKnights.Length > 0)
                {
                    return allyKnights[Random.Range(0, allyKnights.Length)];
                }
            }

            Character[] potentialTargets = AllyCharacters.Where(c => c != null && !c.isKnight).ToArray();
            if (potentialTargets.Length > 0)
            {
                return potentialTargets[Random.Range(0, potentialTargets.Length)];
            }

            // Если нет ни одного потенциального рыцаря или другой цели
            Character[] nonNullAllies = AllyCharacters.Where(c => c != null).ToArray();
            if (nonNullAllies.Length > 0)
            {
                Character target = nonNullAllies[Random.Range(0, nonNullAllies.Length)];
                return target;
            }
        }
        return null;
    }



    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the trigger!");
            other.GetComponent<EnemyHorde>().SwitchAllyTeam(enemies);
            other.GetComponent<EnemyHorde>().inFight = true;

            SwitchAllyTeam(other.GetComponent<EnemyHorde>().enemies);

            other.GetComponent<CharacterMovement>().StartFight();
            inFight = true;
        }
        else
        {
            Debug.Log("Something else entered the trigger: ");
        }
    }

    public void SwitchEnemyTeam(Character[] _enemies)
    {
        enemies = _enemies;
    }
    public void SwitchAllyTeam(Character[] allies)
    {
        AllyCharacters = allies;
    }

    public void EndFight()
    {
        inFight = false;
        GetComponent<CharacterMovement>().StopFight();
    }
}