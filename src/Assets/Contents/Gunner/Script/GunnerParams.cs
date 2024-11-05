using UnityEngine;

[CreateAssetMenu(fileName = "GunnerParams.asset", menuName = "Params/GunnerParams", order = 1)]
public class EnemyParams : ScriptableObject
{
    [Tooltip("普通攻撃のクールタイム"), Header("普通攻撃のクールタイム")]
    public float BasicAttackCooldown = 3f;

    [Tooltip("特殊攻撃のクールタイム"), Header("特殊攻撃のクールタイム")]
    public float SpecialAttackCooldown = 5f; 

    [Tooltip("普通攻撃範囲"),Header("攻撃範囲")]
    public float AttackRange = 10f;

    [Tooltip("CQB攻撃範囲"), Header("CQB攻撃範囲")]
    public float CQBAttackRange = 2f;

    [Tooltip("プレーヤー検知範囲"), Header("プレーヤー検知範囲")]
    public float DetectionRange = 20f;

    [Tooltip("最大弾薬数"), Header("最大弾薬数")]
    public int MaxAmmo = 6;

    [Tooltip("リロード時間"), Header("リロード時間")]
    public float ReloadTime = 2f;

    [Tooltip("移動速度（敵の地面で移動する速度）"), Header("移動速度（敵の地面で移動する速度）")]
    public float MoveSpeed = 3.5f;

    [Tooltip("ジャンプの高さ"), Header("ジャンプの高さ")]
    public float JumpHeight = 8f;

    [Tooltip("特殊移動速度（空中で移動する速度）"), Header("特殊移動速度（空中で移動する速度）")]
    public float SpecialMoveSpeed = 6f;

    [Tooltip("ノックバック距離"), Header("ノックバック距離")]
    public float KnockbackDistance = 3f;

    [Tooltip("特殊攻撃までの通常攻撃回数（特殊攻撃を行う前に通常攻撃を行う回数）"), Header("特殊攻撃までの通常攻撃回数（特殊攻撃を行う前に通常攻撃を行う回数）")]
    public int AttacksBeforeSpecialMove = 3;

    [Tooltip("攻撃前の移動時間（攻撃前に敵が移動する時間）"), Header("攻撃前の移動時間（攻撃前に敵が移動する時間）")]
    public float MoveDurationBeforeAttack = 5f;
}
