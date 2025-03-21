using UnityEngine;
using UnityEngine.UI;
using BattleGame.Scripts;

namespace BattleGame.Scripts
{
    public class SpaceshipController : MonoBehaviour
    {
        [Header("Bullet Settings")]
        public GameObject bulletPrefab;
        public float fireRate = 0.5f;
        public float bulletSpeed = 300f;
        
        [Header("Power Bomb Settings")]
        public float powerBombSpeed = 450f;
        public float powerBombDamage = 30f;
        public Color powerBombColor = Color.red;
        
        [Header("Shield Settings")]
        public GameObject shieldVisual;
        public float shieldDuration = 5f;
        public float shieldCooldown = 10f;
        
        [Header("Spawn Point")]
        public Transform bulletSpawnPoint;

        [Header("Enemy References")]
        public RectTransform circularEnemy;
        public RectTransform triangularEnemy;
        public RectTransform miniBossEnemy;
        public RectTransform bossEnemy;

        private float nextFireTime;
        private float nextShieldTime;
        private bool isShieldActive;
        private RectTransform rectTransform;
        private RectTransform currentTarget;
        private float rotationOffset = -90f; // Offset to make the spaceship point upward at 0 degrees

        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            
            // Ensure we have a collider
            BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
            if (boxCollider == null)
            {
                boxCollider = gameObject.AddComponent<BoxCollider2D>();
                boxCollider.isTrigger = true;
            }

            // Set up bullet spawn point
            if (bulletSpawnPoint == null)
            {
                GameObject spawnPoint = new GameObject("BulletSpawnPoint");
                bulletSpawnPoint = spawnPoint.transform;
                bulletSpawnPoint.SetParent(transform);
                bulletSpawnPoint.localPosition = new Vector3(0, rectTransform.rect.height / 2, 0);
            }

            // Ensure shield is initially disabled and has proper setup
            if (shieldVisual != null)
            {
                shieldVisual.tag = "Shield";
                // Add collider to shield if it doesn't have one
                BoxCollider2D shieldCollider = shieldVisual.GetComponent<BoxCollider2D>();
                if (shieldCollider == null)
                {
                    shieldCollider = shieldVisual.AddComponent<BoxCollider2D>();
                    shieldCollider.isTrigger = true;
                }
                shieldVisual.SetActive(false);
            }

            // Set the player tag
            gameObject.tag = "Player";
        }

        private void Update()
        {
            if (currentTarget != null)
            {
                // Continuously update rotation to track the target
                Vector2 direction = EnemyFunctions.GetDirectionToEnemy(rectTransform, currentTarget);
                float angle = EnemyFunctions.GetRotationAngle(direction);
                rectTransform.rotation = Quaternion.Euler(0, 0, angle + rotationOffset);
            }
            else
            {
                rectTransform.rotation = Quaternion.Euler(0, 0, 0);
            }

            // Fire if it's time
            if (Time.time >= nextFireTime)
            {
                FireBullet(false);
                nextFireTime = Time.time + fireRate;
            }

            // Update shield status
            if (isShieldActive && Time.time >= nextShieldTime)
            {
                DeactivateShield();
            }
        }

        public void ActivateShield()
        {
            if (Time.time >= nextShieldTime && !isShieldActive)
            {
                isShieldActive = true;
                if (shieldVisual != null)
                {
                    shieldVisual.SetActive(true);
                }
                nextShieldTime = Time.time + shieldDuration;
            }
        }

        private void DeactivateShield()
        {
            isShieldActive = false;
            if (shieldVisual != null)
            {
                shieldVisual.SetActive(false);
            }
            nextShieldTime = Time.time + shieldCooldown;
        }

        public bool IsShieldAvailable()
        {
            return Time.time >= nextShieldTime && !isShieldActive;
        }

        public bool IsShieldActive()
        {
            return isShieldActive;
        }

        public void FirePowerBomb()
        {
            FireBullet(true);
        }

        private void FireBullet(bool isPowerBomb)
        {
            if (bulletPrefab != null)
            {
                GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, rectTransform.rotation);
                bullet.transform.SetParent(transform.parent);
                
                RectTransform bulletRect = bullet.GetComponent<RectTransform>();
                if (bulletRect == null)
                    bulletRect = bullet.AddComponent<RectTransform>();
                
                BulletBehavior bulletBehavior = bullet.GetComponent<BulletBehavior>();
                if (bulletBehavior == null)
                    bulletBehavior = bullet.AddComponent<BulletBehavior>();
                
                // Set bullet properties based on type
                float speed = isPowerBomb ? powerBombSpeed : bulletSpeed;
                float damage = isPowerBomb ? powerBombDamage : 10f;
                
                Vector2 direction;
                if (currentTarget != null)
                {
                    direction = EnemyFunctions.GetDirectionToEnemy(rectTransform, currentTarget);
                }
                else
                {
                    direction = Vector2.up;
                }

                bulletBehavior.Initialize(direction * speed, isPowerBomb, damage);
                
                // Set power bomb visual
                if (isPowerBomb)
                {
                    Image bulletImage = bullet.GetComponent<Image>();
                    if (bulletImage != null)
                    {
                        bulletImage.color = powerBombColor;
                        bulletRect.sizeDelta *= 2f; // Make power bombs bigger
                    }
                }

                Destroy(bullet, 3f);
            }
        }

        public void TargetEnemy(string enemyType)
        {
            currentTarget = null;
            switch (enemyType.ToLower())
            {
                case "circular":
                    currentTarget = circularEnemy;
                    break;
                case "triangular":
                    currentTarget = triangularEnemy;
                    break;
                case "miniboss":
                    currentTarget = miniBossEnemy;
                    break;
                case "boss":
                    currentTarget = bossEnemy;
                    break;
            }

            if (currentTarget != null)
            {
                // Initial rotation towards target
                Vector2 direction = EnemyFunctions.GetDirectionToEnemy(rectTransform, currentTarget);
                float angle = EnemyFunctions.GetRotationAngle(direction);
                rectTransform.rotation = Quaternion.Euler(0, 0, angle + rotationOffset);
            }
        }
    }
} 