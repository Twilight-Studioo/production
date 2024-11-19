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
    public float KnockbackDistance = 3f;

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
}