using System.Collections.Generic;
using MrSwampMonster;
using PerilousSwamp.MapClasses;

namespace PerilousSwamp;

public class GameProperties
{
    public bool PrincessIsPickedUp { get; set; }
    public int PlayerCombatStrength { get; set; }
    public List<Treasure> Treasures { get; }

    public GameProperties()
    {
        PrincessIsPickedUp = false;
        PlayerCombatStrength = 800 + MapGenerator.Rnd.Next(0, 600);
        Treasures = [];
    }
}