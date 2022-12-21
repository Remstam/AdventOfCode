using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Console = System.Console;

namespace AdventOfCode._2021
{
    public class Octopus
    {
        const int FLASH_ENERGY = 10;

        public event Action<Octopus> Flash;
        public int X { get; }
        public int Y { get; }
        public int Energy { get; private set; }

        public bool FlashEligible => Energy >= FLASH_ENERGY && !HasFlashed;
        private bool HasFlashed { get; set; }

        public Octopus(int x, int y, int energy)
        {
            X = x;
            Y = y;
            Energy = energy;
        }

        public void IncEnergy()
        {
            ++Energy;
        }

        public void MakeFlash()
        {
            HasFlashed = true;
            Flash?.Invoke(this);
        }

        public void ResetEnergy()
        {
            if (Energy >= FLASH_ENERGY)
            {
                Energy = 0;
            }

            HasFlashed = false;
        }
    }

    public static class OctopusMapExtensions
    {
        public static OctopusMap ParseOctopusMap(this string[] input)
        {
            var size = input.Length;
            var result = new OctopusMap(size);

            for (var i = 0; i < size; ++i)
            {
                var line = input[i];
                for (var j = 0; j < size; ++j)
                {
                    var ch = line[j];
                    var num = ch - '0';

                    var octopus = new Octopus(i, j, num);
                    result.PutOctopus(octopus);
                }
            }

            return result;
        }
    }

    public class OctopusMap
    {
        public int FlashCount { get; private set; }
        public int AllFlashStep { get; private set; }

        private readonly Octopus[,] _octopusMap;
        public OctopusMap(int size)
        {
            _octopusMap = new Octopus[size, size];
        }

        public void PutOctopus(Octopus octopus)
        {
            var x = octopus.X;
            var y = octopus.Y;
            _octopusMap[x, y] = octopus;

            octopus.Flash += ProceedFlash;
        }

        public void Inc()
        {
            foreach (var octopus in _octopusMap)
            {
                octopus.IncEnergy();
            }
        }

        public void Flash(int step)
        {
            var totalUnflashedOctopuses = 0;

            var unflashedOctopuses = GetUnflashedOctopuses();
            while (unflashedOctopuses.Count > 0)
            {
                totalUnflashedOctopuses += unflashedOctopuses.Count;

                foreach (var octopus in unflashedOctopuses)
                {
                    octopus.MakeFlash();
                }

                unflashedOctopuses = GetUnflashedOctopuses();
            }

            if (totalUnflashedOctopuses == _octopusMap.Length)
            {
                AllFlashStep = step;
            }
        }

        private List<Octopus> GetUnflashedOctopuses()
        {
            return _octopusMap
                .Cast<Octopus>()
                .Where(octopus => octopus.FlashEligible)
                .ToList();
        }

        public void ResetEnergy()
        {
            foreach (var octopus in _octopusMap)
            {
                octopus.ResetEnergy();
            }
        }

        public void Print()
        {
            for (var i = 0; i < _octopusMap.GetLength(0); ++i)
            {
                for (var j = 0; j < _octopusMap.GetLength(1); ++j)
                {
                    Console.Write(_octopusMap[i, j].Energy);
                }

                Console.WriteLine();
            }

            for (var i = 0; i < _octopusMap.GetLength(0); ++i)
            {
                Console.Write('=');
            }

            Console.WriteLine();
        }

        private void ProceedFlash(Octopus octopus)
        {
            ++FlashCount;

            var neighbours = GetNeighboursFor(octopus);
            foreach (var neighbour in neighbours)
            {
                neighbour.IncEnergy();
            }
        }

        private List<Octopus> GetNeighboursFor(Octopus octopus)
        {
            var result = new List<Octopus>();
            var x = octopus.X;
            var y = octopus.Y;

            if (x - 1 >= 0)
            {
                result.Add(_octopusMap[x - 1, y]);

                if (y - 1 >= 0)
                {
                    result.Add(_octopusMap[x - 1, y - 1]);
                }

                if (y + 1 < _octopusMap.GetLength(1))
                {
                    result.Add(_octopusMap[x - 1, y + 1]);
                }
            }

            if (x + 1 < _octopusMap.GetLength(0))
            {
                result.Add(_octopusMap[x + 1, y]);

                if (y - 1 >= 0)
                {
                    result.Add(_octopusMap[x + 1, y - 1]);
                }

                if (y + 1 < _octopusMap.GetLength(1))
                {
                    result.Add(_octopusMap[x + 1, y + 1]);
                }
            }

            if (y - 1 >= 0)
            {
                result.Add(_octopusMap[x, y - 1]);
            }

            if (y + 1 < _octopusMap.GetLength(1))
            {
                result.Add(_octopusMap[x, y + 1]);
            }

            return result;
        }
    }

    public class Day11
    {
        public static void Main()
        {
            const int STEPS = 100;

            var input = File.ReadAllLines("2021/input11.txt");
            var octopusMap = input.ParseOctopusMap();

            var step = 1;
            while (true)
            {
                octopusMap.Inc();
                octopusMap.Flash(step);
                octopusMap.ResetEnergy();

                if (step == STEPS)
                {
                    Console.WriteLine(octopusMap.FlashCount);
                }

                if (octopusMap.AllFlashStep != 0)
                {
                    Console.WriteLine(octopusMap.AllFlashStep);
                    break;
                }

                ++step;
            }

            Console.ReadLine();
        }
    }
}