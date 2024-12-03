using UnityEngine;

[CreateAssetMenu(fileName = "GunnerParams.asset", menuName = "Params/GunnerParams", order = 1)]
public class EnemyParams : ScriptableObject
{
    [Tooltip("普通攻撃のクールタイム"), Header("普通攻撃のクールタイム")]
    public float BasicAttackCooldown = 3f;

    [Tooltip("特殊攻撃のクールタイム"), Header("特殊攻撃のクールタイム")]
    public float SpecialAttackCooldown = 5f;

    [Tooltip("普通攻撃範囲"), Header("攻撃範囲")]
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
    public float KnockbackDistance = 1.5f;

    [Tooltip("特殊攻撃までの通常攻撃回数（特殊攻撃を行う前に通常攻撃を行う回数）"), Header("特殊攻撃までの通常攻撃回数（特殊攻撃を行う前に通常攻撃を行う回数）")]
    public int AttacksBeforeSpecialMove = 3;

    [Tooltip("攻撃前の移動時間（攻撃前に敵が移動する時間）"), Header("攻撃前の移動時間（攻撃前に敵が移動する時間）")]
    public float MoveDurationBeforeAttack = 5f;

    [Tooltip("アクション後に待機状態に戻るまでの遅延時間"), Header("アクション後に待機状態に戻るまでの遅延時間")]
    public float ReturnToIdleDelay = 2f;

    [Tooltip("プレーヤーが背後にいると判断する角度閾値"), Header("プレーヤーが背後にいると判断する角度閾値")]
    public float BackAngleThreshold = 120f;

    [Tooltip("近距離攻撃のクールダウン時間"), Header("近距離攻撃のクールダウン時間")]
    public float MeleeAttackCooldown = 2f;

    [Tooltip("遠距離攻撃のクールダウン時間"), Header("遠距離攻撃のクールダウン時間")]
    public float RangedAttackCooldown = 3f;

    [Tooltip("フライ攻撃の弾薬数"), Header("フライ攻撃の弾薬数")]
    public int FlyAttackAmmo = 3;

    [Tooltip("特殊攻撃の弾薬数"), Header("特殊攻撃の弾薬数")]
    public int SpecialAttackAmmo = 3;

    [Tooltip("遠距離攻撃範囲（50）"), Header("遠距離攻撃範囲（50）")]
    public float OverAttackRange = 50;

    [Tooltip("遠距離範囲（30）"), Header("遠距離範囲（30）")]
    public int FarDistanceRange = 30;

    [Tooltip("中距離範囲（18）"), Header("中距離範囲（18）")]
    public int MidDistanceRange = 18;

    [Tooltip("フライ攻撃のクールダウン"), Header("フライ攻撃のクールダウン")]
    public int FlyAttackColdTime = 18;

    [Tooltip("前方距離"), Header("前方距離")]
    public float ForwardDistance = 1.5f;

    [Tooltip("弾速"), Header("弾速")]
    public float BulletSpeed = 1.5f;

    [Tooltip("弾のダメージ"), Header("弾のダメージ")]
    public uint Damage = 10;

    [Tooltip("弾の寿命"), Header("弾の寿命")]
    public float BulletLifeTime = 5f;

    [Tooltip("レーザーの寿命"), Header("レーザーの寿命")]
    public float RayLifeTime = 1f;

    [Tooltip("レーザーの幅"), Header("レーザーの幅")]
    public float RayW = 0.5f;
}