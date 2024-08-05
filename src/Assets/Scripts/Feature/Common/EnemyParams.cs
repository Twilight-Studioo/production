using UnityEngine;

[CreateAssetMenu(fileName = "EnemyParameter", menuName = "EnemyParameter", order = 0)]
public class EnemyParams : ScriptableObject
{
    public float speed = 2.0f;
    public int damage = 1;
}
