using System;
using System.Collections.Generic;

namespace PerilousSwamp.MapClasses;

public static class MapGenerator
{
    private static readonly Random Rnd = new Random();
    private const int Wall = 1;
    private const int Free = 2;
    private const int Obstacle = 3;
    private const int Player = 4;
    private const int Princess = 5;
    private const int Size = 20;

    public static int[,] GenerateMap()
    {
        while (true)
        {
            int[,] map = CreateBaseMap();
            PlaceObstacles(map, obstacleFraction: 0.22f);

            List<(int x, int y)> freeCells = GetFreeCells(map);
            if (freeCells.Count < 2)
                continue;

            Shuffle(freeCells);
            var (px, py) = freeCells[0];
            var (tx, ty) = freeCells[1];

            // Verifiera att spelaren kan nå skatten via BFS
            if (!CanReach(map, px, py, tx, ty))
                continue;

            map[py, px] = Player;
            map[ty, tx] = Princess;

            return map;
        }
    }

    private static int[,] CreateBaseMap()
    {
        var map = new int[Size, Size];

        for (int y = 0; y < Size; y++)
        for (int x = 0; x < Size; x++)
            map[y, x] = (x == 0 || y == 0 || x == Size - 1 || y == Size - 1)
                ? Wall
                : Free;

        return map;
    }

    private static void PlaceObstacles(int[,] map, float obstacleFraction)
    {
        var inner = new List<(int x, int y)>();

        for (int y = 1; y < Size - 1; y++)
        for (int x = 1; x < Size - 1; x++)
            inner.Add((x, y));

        Shuffle(inner);

        int count = (int)(inner.Count * obstacleFraction);
        for (int i = 0; i < count; i++)
        {
            var (x, y) = inner[i];
            map[y, x] = Obstacle;
        }
    }

    private static List<(int x, int y)> GetFreeCells(int[,] map)
    {
        var list = new List<(int x, int y)>();

        for (int y = 1; y < Size - 1; y++)
        for (int x = 1; x < Size - 1; x++)
            if (map[y, x] == Free)
                list.Add((x, y));

        return list;
    }

    private static bool CanReach(int[,] map, int sx, int sy, int tx, int ty)
    {
        var visited = new bool[Size, Size];
        var queue = new Queue<(int x, int y)>();

        visited[sy, sx] = true;
        queue.Enqueue((sx, sy));

        int[] dx = { 0, 0, 1, -1 };
        int[] dy = { 1, -1, 0, 0 };

        while (queue.Count > 0)
        {
            var (x, y) = queue.Dequeue();

            if (x == tx && y == ty)
                return true;

            for (int d = 0; d < 4; d++)
            {
                int nx = x + dx[d];
                int ny = y + dy[d];

                if (nx < 0 || ny < 0 || nx >= Size || ny >= Size)
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
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Rnd.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}