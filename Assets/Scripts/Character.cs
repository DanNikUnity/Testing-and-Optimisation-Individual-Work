using Bubbles;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    public int MaxHealth;
    public int CurrentHealth;

    public int Damage;

    public bool isKnight;
    public bool isArcher;
    public bool isMage;
    public bool isPlayer;

    public bool isPhysical;

    [HideInInspector]
    public int MaxMana;
    [HideInInspector]
    public int CurrentMana;
    public Transform projectilePivot;

    public GameObject healthBarPrefab; // Префаб для HealthBar
    public GameObject selectedPrefab; // Префаб для SelectedPrefab

    private GameObject selectedPrefabInstance;

    public Vector3 healthBarOffset = new Vector3(0, 0, 0); // Смещение для HealthBar
    private GameObject healthBarInstance; // Экземпляр HealthBar
    private Slider healthSlider; // Слайдер для отображения здоровья

    public Bubble activeBubble;

    public bool DontAttack;

    public float SpeedMultiplier = 1f;
    private void Awake()
    {
        // Remove existing CapsuleCollider if present
        CapsuleCollider capsuleCollider = gameObject.GetComponent<CapsuleCollider>();
        if (capsuleCollider != null && !transform.CompareTag("Boss"))
        {
            Destroy(capsuleCollider);
        }

        // Add and configure SphereCollider
        if (gameObject.GetComponent<SphereCollider>() == null)
        {
            SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
            sphereCollider.isTrigger = false; // Set to true if interaction only, false if physics needed
            sphereCollider.center = new Vector3(0, 1f, 1.5f); // Match the center of the CapsuleCollider
            sphereCollider.radius = 1.25f; // Match the radius of the CapsuleCollider
        }
    }

    public void AddShield(Bubble bubble)
    {
        if (activeBubble != null)
        {
            activeBubble.JustDelete(this);
            activeBubble = bubble;
        }
        else 
        activeBubble = bubble;

        if (activeBubble != null && transform.GetComponentInParent<EnemyHorde>() != null && activeBubble is PlasmaBubble plasmaBubble)
        {
            plasmaBubble.PerformPlasmaBubble(this, transform.GetComponentInParent<EnemyHorde>());
        }
        else if(activeBubble != null)
        activeBubble.PerformBubble(this);
    }


    private void Start()
    {
        CurrentHealth = MaxHealth;

        if (isMage)
        {
            MaxMana = 4;
            CurrentMana = MaxMana;
        }

        // Создание HealthBar
        if (healthBarPrefab != null)
        {
            healthBarInstance = Instantiate(healthBarPrefab, transform.position + healthBarOffset, Quaternion.identity);
            healthSlider = healthBarInstance.GetComponentInChildren<Slider>();
            healthBarInstance.transform.SetParent(transform); // Удерживайте HealthBar над головой
            HealthBar healthBarScript = healthBarInstance.GetComponent<HealthBar>();
            if (healthBarScript != null)
            {
                healthBarScript.character = this; // Установка ссылки на этот экземпляр персонажа
            }
        }
    }

    private void Update()
    {
        // Detect left mouse button click
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider != null && hit.collider.gameObject == gameObject)
                {
                    Debug.Log($"Clicked on {name}");
                    SpawnSelectedPrefab();
                }
                else
                {
                    DespawnSelectedPrefab();
                }
            }
        }
    }

    private void SpawnSelectedPrefab()
    {
        // Spawn the selection prefab at the Shooter's position and set it as a child
        if (selectedPrefab != null)
        {
            // Destroy the existing instance if one already exists
            if (selectedPrefabInstance != null)
            {
                Destroy(selectedPrefabInstance);
            }

            selectedPrefabInstance = Instantiate(selectedPrefab, transform.position, Quaternion.identity);
            selectedPrefabInstance.transform.SetParent(transform); // Set the prefab as a child of this GameObject

            // Optional: Adjust local position if needed
            selectedPrefabInstance.transform.localPosition = new Vector3(0, 0, 0); // Offset above the Shooter
        }
    }

    private void DespawnSelectedPrefab()
    {
        // Destroy the selection prefab if it exists
        if (selectedPrefabInstance != null)
        {
            Destroy(selectedPrefabInstance);
            selectedPrefabInstance = null;
        }
    }

    public void SetClass(string characterClass)
    {
        isKnight = (characterClass == "Knight");
        isArcher = (characterClass == "Archer");
        isMage = (characterClass == "Mage");
        isPlayer = characterClass == "Player";
    }

    public void Attack(Character target)
    {
        if (DontAttack)
            return;
        if (isKnight)
            KnightAttack(target);
        if (isArcher)
            ArcherAttack(target);
        if (isMage)
            MageAttack(target);
    }

    private void ArcherAttack(Character target)
    {
        if (target == null) return;
        ArcherAttack archerAttackComponent = GetComponent<ArcherAttack>();
        if (archerAttackComponent != null)
        {
            archerAttackComponent.StartAttack(target.projectilePivot, target, Damage);
        }
    }

    private void MageAttack(Character target)
    {
        if (target == null) return;
        MageAttack mageAttackComponent = GetComponent<MageAttack>();
        if (mageAttackComponent != null)
        {
            mageAttackComponent.StartAttack(target.projectilePivot, target, Damage);
        }
    }

    private void KnightAttack(Character target)
    {
        if (target == null) return;
        KnightAttack knightAttackComponent = GetComponent<KnightAttack>();
        if (knightAttackComponent != null)
        {
            knightAttackComponent.StartAttack(target.transform, target, Damage);
        }
    }

    public void Heal(int amount)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, MaxHealth);
    }

    public void TakeDamage(int damage, Character Shooter)
    {

        if (activeBubble != null && activeBubble.Name == "Reflection")
        {
            activeBubble.AttackedReflectionShield(this, Shooter);
        }
        else if (activeBubble != null && activeBubble is MetalBubble metalBubble)
        {
            // Check if the attack is physical or magical
            metalBubble.HandleAttack(this, Shooter);

        }
        else if(activeBubble != null && activeBubble is StoneBubble stoneBubble)
        {
            stoneBubble.HandleAttack(this, Shooter);
        }
        else if (activeBubble != null && activeBubble is ArcaneBubble arcaneBubble)
        {
            arcaneBubble.HandleAttack(this, Shooter);
        }
        else
        {

            CurrentHealth -= damage;
        }



        if (activeBubble != null && activeBubble is MetalBubble MetalBubble == false)
        {
            activeBubble.BubbleCanceled(this);

        }

        if (CurrentHealth <= 0)
        {
            Destroy(healthBarInstance);
            if(gameObject != null)
            Destroy(gameObject);

        }
    }

    public void ApplySlow()
    {
        if (isMage)
            GetComponent<MageAttack>().ApplySlow(0.5f);
        else if (isKnight)
            GetComponent<KnightAttack>().ApplySlow(0.5f);
        else if (isArcher)
            GetComponent<ArcherAttack>().ApplySlow(0.5f);

    }
    public void DenySlow()
    {
        if (isMage)
            GetComponent<MageAttack>().ApplySlow(0f);
        else if (isKnight)
            GetComponent<KnightAttack>().ApplySlow(0f);
        else if (isArcher)
            GetComponent<ArcherAttack>().ApplySlow(0f);
    }

}