namespace AspireDaprDemo.Common.Models;

public class AttackHistory
{
    public AttackHistory() { }
    public AttackHistory(string attacker, string defender, int damage, int healthPoints) => 
                        (Attacker, Defender, Damage, HealthPoints) = (attacker, defender, damage, healthPoints);
    public string Attacker { get; set; }
    public string Defender { get; set; }
    public int Damage { get; set; }
    public int HealthPoints { get; set; }
}
