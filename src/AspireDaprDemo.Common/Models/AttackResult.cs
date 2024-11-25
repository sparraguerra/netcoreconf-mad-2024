namespace AspireDaprDemo.Common.Models;

public class AttackResult
{
    public AttackResult() { }
    public AttackResult(int damage, int healthPoints) => (Damage, HealthPoints) = (damage, healthPoints);

    public int Damage { get; set; }
    public int HealthPoints { get; set; } 
}
