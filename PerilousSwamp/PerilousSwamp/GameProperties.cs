using System.Collections.Generic;
using MrSwampMonster;
using PerilousSwamp.MapClasses;

namespace PerilousSwamp;

public class GameProperties
{
    public int MoveCount { get; set; } = 0;
    public bool PrincessIsPickedUp { get; set; } = false;
    public int PlayerCombatStrength { get; set; } = 800 + MapGenerator.Rnd.Next(0, 600);
    public List<Treasure> Treasures { get; } = [];
}