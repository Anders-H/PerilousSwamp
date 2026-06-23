using System;
using System.Collections.Generic;

namespace PerilousSwamp.MapClasses;

internal static class MapGenerator
{
    private static readonly Random Rnd = new();
    private const int Wall = 1;
    private const int Free = 2;
    private const int Obstacle = 3;
    private const int Player = 4;
    private const int Princess = 5;

    public static int[,] GenerateMap(Map map)
    {
        while (true)
        {
            var mapGrid = CreateBaseMap();
            PlaceObstacles(mapGrid, obstacleFraction: 0.22f);
            var freeCells = GetFreeCells(mapGrid);

            if (freeCells.Count < 2)
                continue;

            Shuffle(freeCells);
            var (px, py) = freeCells[0];
            var (tx, ty) = freeCells[1];

            var distance = Math.Abs(px - tx) + Math.Abs(py - ty);

            if (distance < 6)
                continue;

            if (!CanReach(mapGrid, px, py, tx, ty))
                continue;

            mapGrid[py, px] = Player;
            mapGrid[ty, tx] = Princess;
            map.SetPlayerPosition(px, py);
            map.SetPrincessPosition(tx, ty);
            return mapGrid;
        }
    }

    private static int[,] CreateBaseMap()
    {
        var map = new int[Map.Size, Map.Size];

        for (var y = 0; y < Map.Size; y++)
        {
            for (var x = 0; x < Map.Size; x++)
            {
                map[y, x] = x == 0 || y == 0 || x == Map.Size - 1 || y == Map.Size - 1
                    ? Wall
                    : Free;
            }
        }

        return map;
    }

    private static void PlaceObstacles(int[,] map, float obstacleFraction)
    {
        var inner = new List<(int x, int y)>();

        for (var y = 1; y < Map.Size - 1; y++)
        {
            for (var x = 1; x < Map.Size - 1; x++)
            {
                inner.Add((x, y));
            }
        }

        Shuffle(inner);
        var count = (int)(inner.Count * obstacleFraction);

        for (var i = 0; i < count; i++)
        {
            var (x, y) = inner[i];
            map[y, x] = Obstacle;
        }
    }

    private static List<(int x, int y)> GetFreeCells(int[,] map)
    {
        var list = new List<(int x, int y)>();

        for (var y = 1; y < Map.Size - 1; y++)
        {
            for (var x = 1; x < Map.Size - 1; x++)
            {
                if (map[y, x] == Free)
                    list.Add((x, y));
            }
        }

        return list;
    }

    private static bool CanReach(int[,] map, int sx, int sy, int tx, int ty)
    {
        var visited = new bool[Map.Size, Map.Size];
        var queue = new Queue<(int x, int y)>();
        visited[sy, sx] = true;
        queue.Enqueue((sx, sy));
        int[] dx = [0, 0, 1, -1];
        int[] dy = [1, -1, 0, 0];

        while (queue.Count > 0)
        {
            var (x, y) = queue.Dequeue();

            if (x == tx && y == ty)
                return true;

            for (var d = 0; d < 4; d++)
            {
                var nx = x + dx[d];
                var ny = y + dy[d];

                if (nx < 0 || ny < 0 || nx >= Map.Size || ny >= Map.Size)
                    continue;

                if (visited[ny, nx])
                    continue;

                if (map[ny, nx] == Wall || map[ny, nx] == Obstacle)
                    continue;

                visited[ny, nx] = true;
                queue.Enqueue((nx, ny));
            }
        }

        return false;
    }

    private static void Shuffle<T>(List<T> list)
    {
        for (var i = list.Count - 1; i > 0; i--)
        {
            var j = Rnd.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}