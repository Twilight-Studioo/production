using UnityEngine;

[CreateAssetMenu(fileName = "GunnerParams.asset", menuName = "Params/GunnerParams", order = 1)]
public class EnemyParams : ScriptableObject
{
    [Tooltip("攻撃範囲（プレイヤーに攻撃できる距離）"),Header("攻撃範囲（プレイヤーに攻撃できる距離）")]
    public float AttackRange = 10f;

    [Tooltip("検知範囲（プレイヤーを発見できる距離）"), Header("検知範囲（プレイヤーを発見できる距離）")]
    public float DetectionRange = 20f;

    [Tooltip("最大弾薬数（使用できる弾薬の最大数）"), Header("最大弾薬数（使用できる弾薬の最大数）")]
    public int MaxAmmo = 6;

    [Tooltip("リロード時間（弾薬をリロードするのにかかる時間）"), Header("リロード時間（弾薬をリロードするのにかかる時間）")]
    public float ReloadTime = 2f;

    [Tooltip("移動速度（敵の通常の移動速度）"), Header("移動速度（敵の通常の移動速度）")]
    public float MoveSpeed = 3.5f;

    [Tooltip("ジャンプの高さ（敵がジャンプできる最大の高さ）"), Header("ジャンプの高さ（敵がジャンプできる最大の高さ）")]
    public float JumpHeight = 8f;

    [Tooltip("特殊移動速度（特殊攻撃中の移動速度）"), Header("特殊移動速度（特殊攻撃中の移動速度）")]
    public float SpecialMoveSpeed = 6f;

    [Tooltip("ノックバック距離（攻撃時にプレイヤーを押し返す距離）"), Header("ノックバック距離（攻撃時にプレイヤーを押し返す距離）")]
    public float KnockbackDistance = 3f;

    [Tooltip("攻撃クールダウン（次の攻撃までの待機時間）"), Header("攻撃クールダウン（次の攻撃までの待機時間）")]
    public float AttackCooldown = 5f;

    [Tooltip("特殊攻撃までの通常攻撃回数（特殊攻撃を行う前に通常攻撃を行う回数）"), Header("特殊攻撃までの通常攻撃回数（特殊攻撃を行う前に通常攻撃を行う回数）")]
    public int AttacksBeforeSpecialMove = 3;

    [Tooltip("瞬間移動距離（敵が前方に瞬間移動する距離）"), Header("瞬間移動距離（敵が前方に瞬間移動する距離）")]
    public float ForwardTeleportDistance = 5f;

    [Tooltip("攻撃前の移動時間（攻撃前に敵が移動する時間）"), Header("攻撃前の移動時間（攻撃前に敵が移動する時間）")]
    public float MoveDurationBeforeAttack = 5f;
}
