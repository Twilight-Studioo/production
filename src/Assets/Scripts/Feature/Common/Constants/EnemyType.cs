namespace Feature.Common.Constants
{
    /// <summary>
    ///     type追加時は最後尾に追加すること
    ///     serializeField等でindex管理している部分がズレてしまうため
    /// </summary>
    public enum EnemyType
    {
        SimpleEnemy1,

        SimpleEnemy2,

        Drone,

        Smasher,

        None,

        SimpleEnemy2Fly,
    }
}