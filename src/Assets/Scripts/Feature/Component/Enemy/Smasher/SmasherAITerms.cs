using System;
using System.Linq;
using Core.Utilities;
using Feature.Component.Enemy.Smasher.State;
using UnityEngine;

namespace Feature.Component.Enemy.Smasher
{
    public partial class SmasherAI
    {
        /// <summary>
        /// 突進攻撃が可能か
        /// </summary>
        private partial bool CanChargeForward()
        {
            if (spawnedMine == null || !spawnedMine.isActiveAndEnabled)
            {
                return false;
            }

            // 自機と上下位置が近いか
            var nearY = Math.Abs(transform.position.y - spawnedMine.transform.position.y) < 1f;
            if (!nearY)
            {
                return false;
            }
            
            // 地雷と自機の間にplayerがいるか
            var player = ObjectFactory.Instance.FindPlayer().CheckNull();
            var minePosition = spawnedMine.transform.position;
            return  ExistsBetweenTwoPoints(transform.position, minePosition, player.transform);
        }

        /// <summary>
        /// 地雷を投げることが可能か
        /// </summary>
        private partial bool CanThrowMines()
        {
            return minePrefab != null && spawnedMine == null;
        }

        private partial bool CanForwardBlow()
        {
            var player = ObjectFactory.Instance.FindPlayer().CheckNull();
            var toPlayerDistance = Vector3.Distance(transform.position, player.transform.position);
            return Math.Abs(toPlayerDistance - MoveToPlayerTargetDistance) < 1f;
        }
        
        private partial bool CanDropAttack()
        {
            var player = ObjectFactory.Instance.FindPlayer().CheckNull();
            var toPlayerDistance = Vector3.Distance(transform.position, player.transform.position);
            return Math.Abs(toPlayerDistance - MoveToKeepDistanceTargetDistance) < 1f;
        }
    }
}