using KyleGame;

/// <summary>
/// 敵：ドローン
/// </summary>
public class Enemy_Drone : EnemyBase
{

    protected override void EnemyDead()
    {
        GetComponent<Drone>().Dead();    
    }
}