using UnityEngine;
using Bubbles;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class Spelling : MonoBehaviour
{
    public List<Bubble> bubbles = new List<Bubble>();
    public List<Bubble> currentBubbles = new List<Bubble>();
    public GameObject[] Icons;
    public Character toUseOn;

    public int MaxMana = 100;
    public int currentMana = 100;
    public int ManaPerSec = 2;

    public Image manaImage;
    public TextMeshProUGUI manaText;

    public SpellsText spellingText;

    private IEnumerator RegenerateMana()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            // Regenerate mana
            currentMana = Mathf.Clamp(currentMana + ManaPerSec, 0, MaxMana);
        }
    }

    private void Start()
    {
        StartCoroutine(RegenerateMana());
    }

    public void AddBubles(List<Bubble> _bubbles)
    {
        foreach (Bubble bubble in _bubbles)
        {
            if (bubble != null && !bubbles.Contains(bubble))
            {
                bubbles.Add(bubble);
            }
        }
    }

    public void AddBubles(Bubble bubble)
    {
        bubbles.Add(bubble);
    }

    public void CastElement(Bubble element)
    {
        currentBubbles.Add(element);

        // Activate the GameObject with the name of the element
        foreach (GameObject icon in Icons)
        {
            if (icon.name == element.Name) // Assuming Bubble has a property 'Name'
            {
                icon.SetActive(true);
            }
        }
    }
    public void CastBubble(Bubble _bubble)
    {
        currentBubbles.Add(_bubble);

        // Activate the GameObject with the name of the element
        foreach (GameObject icon in Icons)
        {
            if (icon.name == _bubble.Name) // Assuming Bubble has a property 'Name'
            {
                icon.SetActive(true);
            }
        }
    }

    private bool isCombining = false;
private void CombineElements()
{
    if (currentBubbles.Count == 2)
    {
        DisableAllIcons();
        Bubble first = currentBubbles[0];
        Bubble second = currentBubbles[1];

        Bubble combinedBubble = null;

        // Combination logic
        if ((first is FireBubble && second is WaterBubble) || (first is WaterBubble && second is FireBubble))
        {
            combinedBubble = new SteamBubble();
        }
        else if ((first is WaterBubble && second is StoneBubble) || (first is StoneBubble && second is WaterBubble))
        {
            combinedBubble = new IceBubble();
        }
        else if ((first is StoneBubble && second is FireBubble) || (first is FireBubble && second is StoneBubble))
        {
            combinedBubble = new MetalBubble();
        }
        else if ((first is ArcaneBubble && second is WaterBubble) || (first is WaterBubble && second is ArcaneBubble))
        {
            combinedBubble = new ManaBubble();
        }
        else if ((first is ArcaneBubble && second is FireBubble) || (first is FireBubble && second is ArcaneBubble))
        {
            combinedBubble = new PlasmaBubble();
        }
        else if ((first is ArcaneBubble && second is StoneBubble) || (first is StoneBubble && second is ArcaneBubble))
        {
            combinedBubble = new ReflectionBubble();
        }

        if (combinedBubble != null)
        {
            currentBubbles.Clear();
            CastBubble(combinedBubble);
        }
    }

    isCombining = false; // Reset combining flag
}



    private void DisableAllIcons()
    {
        foreach (GameObject icon in Icons)
        {
            icon?.SetActive(false);
        }
    }
    private void FixedUpdate()
    {
        // Combine only if there are exactly two bubbles
        if (currentBubbles.Count == 2 && !isCombining)
        {
            isCombining = true;
            Invoke("CombineElements", 0.5f);
        }
    }
    void UpdateMana()
    {
        manaImage.fillAmount = (float)currentMana / MaxMana;
        manaText.text = currentMana.ToString();
    }

    private void Update()
    {
        UpdateMana();
        #region Chose Character
        if (UnityEngine.Input.GetMouseButtonDown(0)) // Left mouse button
        {
            Ray ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Character character = hit.collider.GetComponent<Character>();
                if (character != null)
                {
                    toUseOn = character;
                    return;
                }
                else
                {
                    toUseOn = null;
                }
            }
            else
            {
                toUseOn = null;
            }
        }
        #endregion

        if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1))
        {
            Bubble bubble = new FireBubble();
            if (!currentBubbles.Exists(b => b.GetType() == bubble.GetType()) &&
                currentBubbles.Count <= 1 &&
                currentBubbles.All(b => !b.isBubble))
            {
                CastElement(bubble);
            }
        }
        if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha2))
        {
            Bubble bubble = new StoneBubble();
            if (!currentBubbles.Exists(b => b.GetType() == bubble.GetType()) &&
                currentBubbles.Count <= 1 &&
                currentBubbles.All(b => !b.isBubble))
            {
                CastElement(bubble);
            }
        }
        if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha3))
        {
            Bubble bubble = new ArcaneBubble();
            if (!currentBubbles.Exists(b => b.GetType() == bubble.GetType()) &&
                currentBubbles.Count <= 1 &&
                currentBubbles.All(b => !b.isBubble))
            {
                CastElement(bubble);
            }
        }
        if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha4))
        {
            Bubble bubble = new WaterBubble();
            if (!currentBubbles.Exists(b => b.GetType() == bubble.GetType()) &&
                currentBubbles.Count <= 1 &&
                currentBubbles.All(b => !b.isBubble))
            {
                CastElement(bubble);
            }
        }

        if (UnityEngine.Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E key pressed");
            Debug.Log(isCombining);
            if (!isCombining && currentBubbles.Count > 0)
            {
                Debug.Log("Combining is false and bubbles exist");
                int ManaCost = currentBubbles[0].ManaCost;
                Debug.Log($"ManaCost: {ManaCost}, currentMana: {currentMana}");
                if (ManaCost <= currentMana && toUseOn != null)
                {
                    PerformBubble(currentBubbles[0], toUseOn);
                }
            }
        }

        if (UnityEngine.Input.GetKeyDown(KeyCode.Q))
        {
            currentBubbles.Clear();
            DisableAllIcons();
        }


    }
    
    public void UseMana(int mana)
    {
        currentMana = Mathf.Max(currentMana - mana, 0);
        Debug.Log("ManaTook");
    }
    public void AddMana(int mana)
    {
        currentMana = Mathf.Clamp(currentMana + mana, 0, MaxMana);
    }

    private void PerformBubble(Bubble _bubble, Character chr)
    {
        UseMana(_bubble.ManaCost);
        chr.AddShield(_bubble);
        spellingText.ChangeTextAndFade(_bubble.Name + " Bubble Casted");

        Debug.Log($"SPAWNED bubble {(string)_bubble.Name}");
        currentBubbles.Clear();
        DisableAllIcons();
    }

}
