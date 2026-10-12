using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public enum State
    {
        Walking,
        Idle,
        Fighting
    }
    public State state;

    public GameObject[] Characters;
    public Animator[] animators;


    public float speed = 3.0f;

    void Start()
    {
        // Инициализация при старте
        if (Characters == null || Characters.Length == 0)
        {
            Debug.LogWarning("Characters array is empty or null");
        }

        if (animators == null || animators.Length == 0)
        {
            Debug.LogWarning("Animators array is empty or null");
        }
    }

    public void StartFight()
    {
        state = State.Fighting;
    }
    public void StopFight()
    {
        state = State.Walking;
    }

    void Update()
    {
        switch (state)
        {
            case State.Walking:
                if (Characters != null)
                {
                    foreach (GameObject character in Characters)
                    {
                        if (character != null)
                        {
                            character.transform.Translate(Vector3.forward * speed * Time.deltaTime);
                        }
                    }
                }

                if (animators != null)
                {
                    foreach (Animator animator in animators)
                    {
                        if (animator != null)
                        {
                            animator.SetFloat("speed", 1f);
                            if (animator.GetBool("isWalking") == false)
                            {
                                animator.SetBool("isWalking", true);
                            }
                        }
                    }
                }
                break;

            case State.Idle:
                if (animators != null)
                {
                    foreach (Animator animator in animators)
                    {
                        if (animator != null)
                        {

                            if (animator.GetBool("isWalking") == true)
                            {
                                animator.SetBool("isWalking", false);
                                animator.SetFloat("speed", 0f);
                                animator.Play("Idle");
                            }
                        }
                    }
                }
                break;

            case State.Fighting:
                if (animators != null)
                {
                    foreach (Animator animator in animators)
                    {
                        if (animator != null)
                        {
                            
                            if (animator.GetBool("isWalking") == true)
                            {
                                animator.SetBool("isWalking", false);
                                animator.SetFloat("speed", 0f);
                                animator.Play("Idle");
                            }
                        }
                    }
                }
                break;
        }
    }
}
