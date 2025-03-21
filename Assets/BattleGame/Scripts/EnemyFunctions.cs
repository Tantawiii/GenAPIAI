using UnityEngine;

namespace BattleGame.Scripts
{
    public static class EnemyFunctions
    {
        public static Vector2 GetDirectionToEnemy(RectTransform spaceship, RectTransform target)
        {
            if (target == null) return Vector2.up; // Default to pointing upward if no target

            // Get positions in screen space
            Vector2 spaceshipPos = spaceship.position;
            Vector2 targetPos = target.position;
            
            // Calculate direction in screen space
            Vector2 direction = targetPos - spaceshipPos;
            return direction.normalized;
        }

        public static float GetRotationAngle(Vector2 direction)
        {
            // Calculate angle in degrees, with 0 being up
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            return angle;
        }
    }
} 