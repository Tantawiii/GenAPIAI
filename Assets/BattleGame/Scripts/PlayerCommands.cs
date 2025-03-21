using UnityEngine;

namespace BattleGame.Scripts
{
    public class PlayerCommands
    {
        public string ActivateShield()
        {
            return "Shield";
        }

        public string FirePowerBomb()
        {
            return "Power";
        }

        public string NoCommandMentioned()
        {
            return "NoCommandMentioned";
        }
    }
} 