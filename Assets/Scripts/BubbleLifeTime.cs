
using UnityEngine;

namespace Bubbles
{
    public class BubbleLifetime : MonoBehaviour
    {
        public Bubble thisBubble;
        public Character character;

        public float lifeTime;

        public void InitializeBubble(Bubble bub, Character chr)
        {
            thisBubble = bub;
            character = chr;
            lifeTime = thisBubble.LifeTime;
            Invoke(nameof(BubbleLifeTime), lifeTime);
        }

        private void BubbleLifeTime()
        {
            thisBubble.FinishBubble(character);
            character.activeBubble = null;
            Destroy(gameObject);
        }
        public void CancelBubble()
        {
            CancelInvoke(nameof(BubbleLifeTime));
            Destroy(gameObject);
        }


    }

}
