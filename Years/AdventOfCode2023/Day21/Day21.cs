using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace AdventOfCode2023
{
    public static class Day21
    {
        private static bool[][] _rocks = [];
        private static readonly (int x, int y)[] _directions = [(-1,0),(0,-1),(1,0),(0,1)];
        private static HashSet<(int x, int y)> _reachableCoordinates = [];
        private static HashSet<(int x, int y)> _seenCoordinates = [];
        private static Queue<(int x, int y, int step)> next = [];
        private const int nbSteps = 64;
        public static void Solve(int part)
        { 
            string[] input = File.ReadAllLines(@"Day21\input.txt");

            _rocks = ParseMap(input);

            while(next.Count != 0) BFS(next.Dequeue(), part);

            Console.WriteLine(_reachableCoordinates.Count);
        }

        private static bool[][] ParseMap(string[] input)
        {
            bool[][] output = new bool[input.Length][];

            for (int y = 0; y < input.Length; y++)
            {
                output[y] = input[y]
                    .Select(c => c == '#')
                    .ToArray();
                    
                if (input[y].Contains('S')) 
                {
                    next.Enqueue((input[y].IndexOf('S'), y, nbSteps));
                    _seenCoordinates.Add((input[y].IndexOf('S'), y));
                }
            }

            return output;
        }

        private static void BFS((int x, int y, int step) path, int part)
        {
            (int x, int y, int step) = path;
            
            if (step % 2 == 0) _reachableCoordinates.Add((x,y));
            if (step == 0) return;

            foreach (var dir in _directions)
            {
                (int x2, int y2) = (x+dir.x, y+dir.y);
                
                if (!(x2,y2).IsGarden(part)) continue;
                _seenCoordinates.Add((x2,y2));
                next.Enqueue((x2,y2,step-1));
            }
        }

        private static bool IsGarden (this (int x, int y) coord, int part) => part switch {
            1 => (coord.x,coord.y).IsValid() && !(coord.x,coord.y).IsRock() && !_seenCoordinates.Contains((coord.x,coord.y)),
            2 => !(coord.x,coord.y).InOriginalGrid().IsRock() && !_seenCoordinates.Contains((coord.x,coord.y)),
            _ => throw new ArgumentOutOfRangeException("Part should be 1 or 2")
        };

        private static bool IsValid(this (int x, int y) coord) =>
            coord.x >= 0 
            && coord.y >= 0
            && coord.y < _rocks.Length            
            && coord.x < _rocks[coord.y].Length;

        private static bool IsRock(this (int x, int y) coord) => _rocks[coord.y][coord.x];

        private static (int x, int y) InOriginalGrid (this (int x, int y) coord) => (CoordinateInOriginalGrid(coord.x, _rocks.First().Length), CoordinateInOriginalGrid(coord.y, _rocks.Length));

        private static int CoordinateInOriginalGrid(int coord, int size) => (coord + size * (Math.Abs(coord)/size + 1)) % size;
    }
}