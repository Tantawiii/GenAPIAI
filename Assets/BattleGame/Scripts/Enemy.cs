using UnityEngine;
using UnityEngine.UI;
using BattleGame.Scripts;

// Enemy class for handling different enemy types and their behaviors
namespace BattleGame.Scripts
{
    public class Enemy : MonoBehaviour
    {
        public enum EnemyType
        {
            Circular,
            Triangular,
            MiniBoss,
            Boss
        }

        [Header("Enemy Settings")]
        public EnemyType enemyType;
        public float health = 100f;
        
        [Header("Movement Settings")]
        public RectTransform playerTarget;
        private float moveSpeed;
        private RectTransform rectTransform;
        private Canvas canvas;
        private BoxCollider2D boxCollider;

        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            canvas = GetComponentInParent<Canvas>();
            
            // Ensure we have a collider
            boxCollider = GetComponent<BoxCollider2D>();
            if (boxCollider == null)
            {
                boxCollider = gameObject.AddComponent<BoxCollider2D>();
                boxCollider.isTrigger = true;
            }
            
            // Set health and speed based on enemy type
            switch (enemyType)
            {
                case EnemyType.Circular:
                    health = 50f;
                    moveSpeed = 20f; // Fastest
                    break;
                case EnemyType.Triangular:
                    health = 75f;
                    moveSpeed = 15f;  // Second fastest
                    break;
                case EnemyType.MiniBoss:
                    health = 150f;
                    moveSpeed = 10f;  // Slow
                    break;
                case EnemyType.Boss:
                    health = 300f;
                    moveSpeed = 5f;  // Slowest
                    break;
            }

            Debug.Log($"Enemy {enemyType} initialized with health: {health}, speed: {moveSpeed}");
            if (playerTarget == null)
            {
                Debug.LogError($"Enemy {enemyType}: No player target assigned!");
            }
        }

        private void Update()
        {
            if (playerTarget != null && canvas != null)
            {
                // Get screen positions
                Vector2 playerScreenPoint = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, playerTarget.position);
                Vector2 enemyScreenPoint = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, rectTransform.position);
                
                // Calculate direction in screen space
                Vector2 direction = (playerScreenPoint - enemyScreenPoint).normalized;
                
                // Move towards player
                rectTransform.anchoredPosition += direction * moveSpeed * Time.deltaTime;
                
                // Rotate to face movement direction
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
                rectTransform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }

        public void TakeDamage(float damage)
        {
            Debug.Log($"Enemy {enemyType} taking damage: {damage}, Current health: {health}");
            health -= damage;
            if (health <= 0)
            {
                DestroyEnemy();
            }
        }

        private void DestroyEnemy()
        {
            Debug.Log($"Enemy {enemyType} destroyed!");
            // Add any death effects or score updates here
            Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log($"Enemy {enemyType} collided with {other.tag}");
            
            if (other.CompareTag("PlayerBullet"))
            {
                BulletBehavior bullet = other.GetComponent<BulletBehavior>();
                if (bullet != null)
                {
                    TakeDamage(bullet.GetDamage());
                }
            }
            else if (other.CompareTag("Player"))
            {
                SpaceshipController spaceship = other.GetComponent<SpaceshipController>();
                if (spaceship != null && spaceship.shieldVisual != null && spaceship.shieldVisual.activeInHierarchy)
                {
                    Debug.Log($"Enemy {enemyType} hit player with active shield - destroying enemy!");
                    DestroyEnemy();
                }
                else
                {
                    Debug.Log($"Enemy {gameObject.name} hit unshielded player - game over!");
                    var gameManager = FindObjectOfType<GameManager>();
                    if (gameManager != null)
                    {
                        gameManager.GameOver();
                    }
                    else
                    {
                        Debug.LogError("GameManager not found in scene!");
                    }
                }
            }
        }
    }
} 