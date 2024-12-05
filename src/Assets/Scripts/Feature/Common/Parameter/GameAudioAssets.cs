using System;
using System.Collections.Generic;
using Core.Utilities;
using Core.Utilities.Parameter;
using Feature.Common.Constants;
using UnityEngine;

namespace Feature.Common.Parameter
{
    [CreateAssetMenu(fileName = "GameAudioAssets.asset", menuName = "GameAudioAssets", order = 0)]
    public class GameAudioAssets: BaseParameter
    {
        
        public AudioClip GetAudioClip(AudioAssetType type)
        {
            return type switch
            {
                AudioAssetType.BGM => bgm,
                AudioAssetType.Slashing => slashing.RandomElement(),
                AudioAssetType.SlashingHit => slashingHit.RandomElement(),
                var _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
            };
        }
        
        [Tooltip("BGM")]
        public AudioClip bgm;

        [Tooltip("Slashing(通常攻撃)")] public List<AudioClip> slashing;
        [Tooltip("SlashingHit(攻撃のhit)")] public List<AudioClip> slashingHit;

    }
}