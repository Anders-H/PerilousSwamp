using PerilousSwamp.MapClasses;

namespace PerilousSwamp;

public class GameProperties
{
    public bool PrincessIsPickedUp { get; set; }
    public int PlayerCombatStrength { get; set; }

    public GameProperties()
    {
        PrincessIsPickedUp = false;
        PlayerCombatStrength = 600 + MapGenerator.Rnd.Next(0, 700);
    }
}