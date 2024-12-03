using UnityEngine;

[CreateAssetMenu(fileName = "GunnerParams.asset", menuName = "Params/GunnerParams", order = 1)]
public class EnemyParams : ScriptableObject
{
    [Tooltip("���ʍU���̃N�[���^�C��"), Header("���ʍU���̃N�[���^�C��")]
    public float BasicAttackCooldown = 3f;

    [Tooltip("����U���̃N�[���^�C��"), Header("����U���̃N�[���^�C��")]
    public float SpecialAttackCooldown = 5f;

    [Tooltip("���ʍU���͈�"), Header("�U���͈�")]
    public float AttackRange = 10f;

    [Tooltip("CQB�U���͈�"), Header("CQB�U���͈�")]
    public float CQBAttackRange = 2f;

    [Tooltip("�v���[���[���m�͈�"), Header("�v���[���[���m�͈�")]
    public float DetectionRange = 20f;

    [Tooltip("�ő�e��"), Header("�ő�e��")]
    public int MaxAmmo = 6;

    [Tooltip("�����[�h����"), Header("�����[�h����")]
    public float ReloadTime = 2f;

    [Tooltip("�ړ����x�i�G�̒n�ʂňړ����鑬�x�j"), Header("�ړ����x�i�G�̒n�ʂňړ����鑬�x�j")]
    public float MoveSpeed = 3.5f;

    [Tooltip("�W�����v�̍���"), Header("�W�����v�̍���")]
    public float JumpHeight = 8f;

    [Tooltip("����ړ����x�i�󒆂ňړ����鑬�x�j"), Header("����ړ����x�i�󒆂ňړ����鑬�x�j")]
    public float SpecialMoveSpeed = 6f;

    [Tooltip("�m�b�N�o�b�N����"), Header("�m�b�N�o�b�N����")]
    public float KnockbackDistance = 1.5f;

    [Tooltip("����U���܂ł̒ʏ�U���񐔁i����U�����s���O�ɒʏ�U�����s���񐔁j"), Header("����U���܂ł̒ʏ�U���񐔁i����U�����s���O�ɒʏ�U�����s���񐔁j")]
    public int AttacksBeforeSpecialMove = 3;

    [Tooltip("�U���O�̈ړ����ԁi�U���O�ɓG���ړ����鎞�ԁj"), Header("�U���O�̈ړ����ԁi�U���O�ɓG���ړ����鎞�ԁj")]
    public float MoveDurationBeforeAttack = 5f;

    [Tooltip("�A�N�V������ɑҋ@��Ԃɖ߂�܂ł̒x������"), Header("�A�N�V������ɑҋ@��Ԃɖ߂�܂ł̒x������")]
    public float ReturnToIdleDelay = 2f;

    [Tooltip("�v���[���[���w��ɂ���Ɣ��f����p�x臒l"), Header("�v���[���[���w��ɂ���Ɣ��f����p�x臒l")]
    public float BackAngleThreshold = 120f;

    [Tooltip("�ߋ����U���̃N�[���_�E������"), Header("�ߋ����U���̃N�[���_�E������")]
    public float MeleeAttackCooldown = 2f;

    [Tooltip("�������U���̃N�[���_�E������"), Header("�������U���̃N�[���_�E������")]
    public float RangedAttackCooldown = 3f;

    [Tooltip("�t���C�U���̒e��"), Header("�t���C�U���̒e��")]
    public int FlyAttackAmmo = 3;

    [Tooltip("����U���̒e��"), Header("����U���̒e��")]
    public int SpecialAttackAmmo = 3;

    [Tooltip("�������U���͈́i50�j"), Header("�������U���͈́i50�j")]
    public float OverAttackRange = 50;

    [Tooltip("�������͈́i30�j"), Header("�������͈́i30�j")]
    public int FarDistanceRange = 30;

    [Tooltip("�������͈́i18�j"), Header("�������͈́i18�j")]
    public int MidDistanceRange = 18;

    [Tooltip("�t���C�U���̃N�[���_�E��"), Header("�t���C�U���̃N�[���_�E��")]
    public int FlyAttackColdTime = 18;

    [Tooltip("�O������"), Header("�O������")]
    public float ForwardDistance = 1.5f;

    [Tooltip("�e��"), Header("�e��")]
    public float BulletSpeed = 1.5f;

    [Tooltip("�e�̃_���[�W"), Header("�e�̃_���[�W")]
    public uint Damage = 10;

    [Tooltip("�e�̎���"), Header("�e�̎���")]
    public float BulletLifeTime = 5f;

    [Tooltip("���[�U�[�̎���"), Header("���[�U�[�̎���")]
    public float RayLifeTime = 1f;

    [Tooltip("���[�U�[�̕�"), Header("���[�U�[�̕�")]
    public float RayW = 0.5f;
}