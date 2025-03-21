using UnityEngine;

namespace BattleGame.Scripts
{
    public class BulletBehavior : MonoBehaviour
    {
        private Vector2 velocity;
        private bool isPowered;
        private float damage;
        private RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        public void Initialize(Vector2 velocity, bool isPowered, float damage)
        {
            this.velocity = velocity;
            this.isPowered = isPowered;
            this.damage = damage;
            
            // If it's a powered shot, increase damage
            if (isPowered)
            {
                this.damage *= 2f;
            }
        }

        public float GetDamage()
        {
            return damage;
        }

        private void Update()
        {
            // Move the bullet
            rectTransform.anchoredPosition += velocity * Time.deltaTime;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Enemy"))
            {
                Enemy enemy = other.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
                Destroy(gameObject);
            }
        }
    }
} 