using UnityEngine;

[CreateAssetMenu(fileName = "GunnerParams.asset", menuName = "Params/GunnerParams", order = 1)]
public class EnemyParams : ScriptableObject
{
    [Tooltip("�U���͈́i�v���C���[�ɍU���ł��鋗���j"),Header("�U���͈́i�v���C���[�ɍU���ł��鋗���j")]
    public float AttackRange = 10f;

    [Tooltip("���m�͈́i�v���C���[�𔭌��ł��鋗���j"), Header("���m�͈́i�v���C���[�𔭌��ł��鋗���j")]
    public float DetectionRange = 20f;

    [Tooltip("�ő�e�򐔁i�g�p�ł���e��̍ő吔�j"), Header("�ő�e�򐔁i�g�p�ł���e��̍ő吔�j")]
    public int MaxAmmo = 6;

    [Tooltip("�����[�h���ԁi�e��������[�h����̂ɂ����鎞�ԁj"), Header("�����[�h���ԁi�e��������[�h����̂ɂ����鎞�ԁj")]
    public float ReloadTime = 2f;

    [Tooltip("�ړ����x�i�G�̒ʏ�̈ړ����x�j"), Header("�ړ����x�i�G�̒ʏ�̈ړ����x�j")]
    public float MoveSpeed = 3.5f;

    [Tooltip("�W�����v�̍����i�G���W�����v�ł���ő�̍����j"), Header("�W�����v�̍����i�G���W�����v�ł���ő�̍����j")]
    public float JumpHeight = 8f;

    [Tooltip("����ړ����x�i����U�����̈ړ����x�j"), Header("����ړ����x�i����U�����̈ړ����x�j")]
    public float SpecialMoveSpeed = 6f;

    [Tooltip("�m�b�N�o�b�N�����i�U�����Ƀv���C���[�������Ԃ������j"), Header("�m�b�N�o�b�N�����i�U�����Ƀv���C���[�������Ԃ������j")]
    public float KnockbackDistance = 3f;

    [Tooltip("�U���N�[���_�E���i���̍U���܂ł̑ҋ@���ԁj"), Header("�U���N�[���_�E���i���̍U���܂ł̑ҋ@���ԁj")]
    public float AttackCooldown = 5f;

    [Tooltip("����U���܂ł̒ʏ�U���񐔁i����U�����s���O�ɒʏ�U�����s���񐔁j"), Header("����U���܂ł̒ʏ�U���񐔁i����U�����s���O�ɒʏ�U�����s���񐔁j")]
    public int AttacksBeforeSpecialMove = 3;

    [Tooltip("�u�Ԉړ������i�G���O���ɏu�Ԉړ����鋗���j"), Header("�u�Ԉړ������i�G���O���ɏu�Ԉړ����鋗���j")]
    public float ForwardTeleportDistance = 5f;

    [Tooltip("�U���O�̈ړ����ԁi�U���O�ɓG���ړ����鎞�ԁj"), Header("�U���O�̈ړ����ԁi�U���O�ɓG���ړ����鎞�ԁj")]
    public float MoveDurationBeforeAttack = 5f;
}
