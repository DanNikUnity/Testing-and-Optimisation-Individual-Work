using UnityEngine;
using Bubbles;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Bubbles
{
    public class BubbleSystem : MonoBehaviour
    {

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

    public class Bubble
    {
        public string Name;
        public int ManaCost;
        public bool isBubble;
        public float LifeTime = 3f;

        public int Damage;

        public bool PhysicalDefence;
        public bool ElementalDefence;
        public bool Reflection;
        public int HealthRegeneration;
        public int ManaRegeneration;

        public GameObject BubblePrefab;

        public GameObject spawnedBubbleInstance;

        public virtual void PerformBubble(Character chr)
        {
            if (BubblePrefab != null && chr != null && chr.projectilePivot != null)
            {
                spawnedBubbleInstance = GameObject.Instantiate(BubblePrefab, chr.projectilePivot.position, Quaternion.identity);
                BubbleLifetime blft = spawnedBubbleInstance.GetComponent<BubbleLifetime>();
                if (blft != null)
                {
                    blft.InitializeBubble(this, chr);
                }
                else
                {
                    Debug.LogError("BubbleLifetime component is missing on the BubblePrefab.");
                }

                spawnedBubbleInstance.transform.SetParent(chr.projectilePivot); // Attach to the Shooter's projectile pivot
                chr.activeBubble = this;
            }
            else
            {
                Debug.LogWarning("BubblePrefab, chr, or chr.ProjectilePivot is null. Ensure everything is properly assigned.");
            }
        }

        public virtual void BubbleCanceled(Character chr)
        {
            if (spawnedBubbleInstance != null)
            {
                BubbleLifetime blft = spawnedBubbleInstance.GetComponent<BubbleLifetime>();
                if (blft != null)
                {
                    blft.CancelBubble(); // Cancel the bubble's effects
                }
                GameObject.Destroy(spawnedBubbleInstance); // Destroy the bubble instance
                spawnedBubbleInstance = null; // Clear the reference
                if(chr != null)
                chr.activeBubble = null;
            }
            else
            {
                Debug.LogWarning("No active bubble instance to cancel.");
            }
        }

        public virtual void FinishBubble(Character chr)
        {

        }

        public void AttackedReflectionShield(Character chr, Character attacker)
        {
            Debug.Log("tookDamage");
            if(attacker != null)
                attacker.TakeDamage(attacker.Damage, chr);
        }

        public virtual void JustDelete(Character chr)
        {
            // If the spawned bubble instance exists, destroy it
            if (spawnedBubbleInstance != null)
            {
                // Destroy the bubble object
                GameObject.Destroy(spawnedBubbleInstance);
                spawnedBubbleInstance = null; // Clear the reference
                chr.activeBubble = null;
            }
            else
            {
                Debug.LogWarning("No bubble instance to delete.");
            }
        }

    }
    public class FireBubble : Bubble
    {
        private Coroutine damageCoroutine; // To store the coroutine

        public FireBubble()
        {
            Name = "Fire";
            ManaCost = 20;
            Damage = 1;
            BubblePrefab = (GameObject)Resources.Load("Bubbles/FireShield");
        }

        public override void PerformBubble(Character chr)
        {
            base.PerformBubble(chr);

            // Start the damage over time coroutine
            damageCoroutine = chr.StartCoroutine(DealDamageOverTime(chr));
        }

        private IEnumerator DealDamageOverTime(Character chr)
        {
            // Damage every second until the lifetime expires
            float timeElapsed = 0f;

            while (timeElapsed < LifeTime)
            {
                if (chr != null)
                {
                    // Apply damage to the character
                    chr.TakeDamage(Damage, chr); // Assuming you have a `TakeDamage` method in the `Character` class
                }

                timeElapsed += 1f;
                yield return new WaitForSeconds(1f); // Wait for 1 second

            }
        }

        public override void BubbleCanceled(Character chr)
        {
            // FireBubble cannot be canceled, so no action needed here.
        }

        public override void JustDelete(Character chr)
        {
            // Stop the damage over time effect by stopping the coroutine
            if (damageCoroutine != null)
            {
                // If the coroutine is running, stop it
                chr.StopCoroutine(damageCoroutine);
                damageCoroutine = null; // Clear the reference
            }

            base.JustDelete(chr); // Call base method to ensure the bubble instance is destroyed
        }
    }


    public class StoneBubble : Bubble
    {
        public StoneBubble()
        {
            Name = "Stone";
            ManaCost = 15;
            PhysicalDefence = true;
            BubblePrefab = (GameObject)Resources.Load("Bubbles/StoneShield");
        }

        public override void PerformBubble(Character chr)
        {
            base.PerformBubble(chr);
        }
        public void HandleAttack(Character chr, Character attacker)
        {

            if (attacker.isPhysical)
            {
                Debug.Log("Attacked");

                 base.BubbleCanceled(chr);
            }
            else
            {
                base.BubbleCanceled(chr);
                chr.TakeDamage(attacker.Damage, attacker);
                Debug.Log("Attacked Magical");
            }
        }
    }

    public class ArcaneBubble : Bubble
    {
        public ArcaneBubble()
        {
            Name = "Arcane";
            ManaCost = 15;
            ElementalDefence = true;
            BubblePrefab = (GameObject)Resources.Load("Bubbles/ArcaneShield");
        }

        public override void PerformBubble(Character chr)
        {
            base.PerformBubble(chr);
        }

        public void HandleAttack(Character chr, Character attacker)
        {

            if (!attacker.isPhysical)
            {
                Debug.Log("Attacked");

                base.BubbleCanceled(chr);
            }
            else
            {
                base.BubbleCanceled(chr);
                chr.TakeDamage(attacker.Damage, attacker);
                Debug.Log("Attacked Magical");
            }
        }
    }

    public class WaterBubble : Bubble
    {
        private int HealAmount = 2;
        public WaterBubble()
        {
            Name = "Water";
            ManaCost = 25;
            HealthRegeneration = 1;
            BubblePrefab = (GameObject)Resources.Load("Bubbles/WaterShield");
        }

        public override void PerformBubble(Character chr)
        {
            base.PerformBubble(chr);
        }

        public override void FinishBubble(Character chr)
        {
            chr.Heal(HealAmount);
            base.FinishBubble(chr);
        }
    }

    public class IceBubble : Bubble
    {
        public IceBubble()
        {
            Name = "Ice";
            ManaCost = 40;
            isBubble = true;
            LifeTime = 5f;
            BubblePrefab = (GameObject)Resources.Load("Bubbles/IceShield");
        }

        public override void PerformBubble(Character chr)
        {
            chr.ApplySlow();
            base.PerformBubble(chr);
            
        }

        public override void BubbleCanceled(Character chr)
        {
            //base.BubbleCanceled(chr);
        }

        public override void FinishBubble(Character chr)
        {
            chr.DenySlow();
            base.FinishBubble(chr);
        }
        public override void JustDelete(Character chr)
        {
            chr.DenySlow();
            base.JustDelete(chr);
        }

    }

    public class SteamBubble : Bubble
    {
        public SteamBubble()
        {
            Name = "Steam";
            ManaCost = 45;
            Damage = 6;
            LifeTime = 1f;
            isBubble = true;
            BubblePrefab = (GameObject)Resources.Load("Bubbles/SteamShield");
        }

        public override void PerformBubble(Character chr)
        {
            base.PerformBubble(chr);
        }

        public override void FinishBubble(Character chr)
        {
            base.FinishBubble(chr);

            if (chr != null)
            {
                chr.TakeDamage(Damage, chr);
                Debug.Log("Steam Bubble finished. Dealt 6 damage to the character.");
            }
        }

        public override void BubbleCanceled(Character chr)
        {
            // cannot be canceled, so leave this empty or you can add any logic if needed
        }
    }


    public class MetalBubble : Bubble
    {

        public int HoldAttacks = 3;
        public MetalBubble()
        {
            Name = "Metal";
            ManaCost = 35;
            Reflection = true;
            PhysicalDefence = true;
            isBubble = true;
            BubblePrefab = (GameObject)Resources.Load("Bubbles/MetalicShield");
        }

        public virtual void PerformBubble(Character chr)
        {
            base.PerformBubble(chr);
        }

        public virtual void FinishBubble(Character chr)
        {

        }

        public virtual void BubbleCanceled(Character chr)
        {
            //special interaction
        }
        
        public void HandleAttack(Character chr, Character attacker)
        {

            if (attacker.isPhysical)
            {
                Debug.Log("Attacked");
                HoldAttacks--;
                if(HoldAttacks == 0)
                    base.BubbleCanceled(chr);
            }
            else
            {
                base.BubbleCanceled(chr);
                Debug.Log("Attacked Magical");
            }
        }
    }

    public class ManaBubble : Bubble
    {
        private bool isManaDeducted = false; // Track if mana has been deducted

        public ManaBubble()
        {
            Name = "Mana";
            ManaCost = 40;
            ManaRegeneration = 100;
            isBubble = true;
            BubblePrefab = (GameObject)Resources.Load("Bubbles/ManaShield");
        }

        public override void PerformBubble(Character chr)
        {
            base.PerformBubble(chr);
            isManaDeducted = false; // Reset the mana deduction flag each time the bubble is performed
        }

        public override void BubbleCanceled(Character chr)
        {
            if (chr.CompareTag("PlayerCharacter"))
            {
                Spelling spelling = Object.FindAnyObjectByType<Spelling>();
                if (spelling != null && !isManaDeducted)
                {
                    spelling.UseMana(20); // Only deduct mana if it hasn't been deducted yet
                    isManaDeducted = true; // Flag that mana has been deducted
                }
            }

            base.BubbleCanceled(chr); // Call the base cancel behavior
        }

        public override void FinishBubble(Character chr)
        {
            if (chr.CompareTag("PlayerCharacter"))
            {
                Spelling spelling = Object.FindAnyObjectByType<Spelling>();
                if (spelling != null)
                {
                    spelling.AddMana(ManaRegeneration); // Fully regenerate mana
                    Debug.Log("Mana fully regenerated!");
                }
            }
        }

        public override void JustDelete(Character chr)
        {
            // Ensure the mana deduction flag is reset if the bubble is deleted directly
            isManaDeducted = false;
            base.JustDelete(chr);
        }
    }


    public class PlasmaBubble : Bubble
    {
        private Coroutine plasmaEffectCoroutine;
        public PlasmaBubble()
        {
            Name = "Plasma";
            ManaCost = 35;
            Damage = 2;
            isBubble = true;
            BubblePrefab = (GameObject)Resources.Load("Bubbles/PlasmaShield");
        }

        public override void PerformBubble(Character chr)
        {
            base.PerformBubble(chr);
             // Start the coroutine to handle the effect
        }

        public override void BubbleCanceled(Character chr)
        {
            StopPlasmaEffect(chr);
            base.BubbleCanceled(chr);
        }

        private IEnumerator PlasmaBubbleEffect(Character chr,EnemyHorde eh)
        {
            yield return new WaitForSeconds(LifeTime);
            if (eh != null)
            {
                foreach (Character ally in eh.enemies)
                {
                    if (ally != null)
                    {
                        ally.TakeDamage(Damage, chr);
                        Debug.Log($"{Name} dealt {Damage} damage to {ally.name}");
                    }
                }
            }
        }

        private void StopPlasmaEffect(Character chr)
        {
            // Stop the coroutine if it is running
            if (plasmaEffectCoroutine != null)
            {
                chr.StopCoroutine(plasmaEffectCoroutine);
                plasmaEffectCoroutine = null;
            }
        }

        public override void JustDelete(Character chr)
        {
            StopPlasmaEffect(chr);
            base.JustDelete(chr);
        }

        public void PerformPlasmaBubble(Character chr, EnemyHorde enemyHorde)
        {
            
            PerformBubble(chr); // Perform the bubble logic
            plasmaEffectCoroutine = chr.StartCoroutine(PlasmaBubbleEffect(chr, enemyHorde));
        }
    }

    public class ReflectionBubble : Bubble
    {
        public ReflectionBubble()
        {
            Name = "Reflection";
            ManaCost = 30;
            Reflection = true;
            isBubble = true;
            BubblePrefab = (GameObject)Resources.Load("Bubbles/ReflectionShield");
        }

        public override void PerformBubble(Character chr)
        {
            base.PerformBubble(chr);
        }


        public override void BubbleCanceled(Character chr)
        {
            
            base.BubbleCanceled(chr);
        }

        public override void FinishBubble(Character chr)
        {
            //does nothing if not canceled
        }

    }
}