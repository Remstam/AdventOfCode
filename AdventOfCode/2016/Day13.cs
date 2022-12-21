using System;
using System.Collections.Generic;
using System.IO;

namespace AdventOfCode._2016
{
    public static class RoomExtensions
    {
        public static Day13.Room GetNeighbour(this Day13.Room[,] rooms, Day13.Room room, Day13.Direction dir)
        {
            var x = room.X;
            var y = room.Y;

            switch (dir)
            {
                case Day13.Direction.Left:
                    if (x - 1 < 0)
                    {
                        return null;
                    }

                    return rooms[x - 1, y];

                case Day13.Direction.Right:
                    if (x + 1 >= rooms.GetLength(0))
                    {
                        return null;
                    }

                    return rooms[x + 1, y];

                case Day13.Direction.Down:
                    if (y - 1 < 0)
                    {
                        return null;
                    }

                    return rooms[x, y - 1];

                case Day13.Direction.Up:
                    if (y + 1 >= rooms.GetLongLength(1))
                    {
                        return null;
                    }

                    return rooms[x, y + 1];

                default:
                    return null;
            }
        }
    }

    public class Day13
    {
        public enum Direction
        {
            Left, Up, Down, Right
        }

        public enum RoomType
        {
            Pass, Wall
        }

        public class Room
        {
            public RoomType Type { get; }
            public int X { get; }
            public int Y { get; }

            public int MinSteps { get; private set; }
            public int Depth { get; private set; }

            public Room(int x, int y, int input)
            {
                X = x;
                Y = y;

                var num = (x + y) * (x + y) + 3 * x + y + input;
                var popCount = PopCount(num);
                Type = popCount % 2 == 0 ? RoomType.Pass : RoomType.Wall;

                MinSteps = int.MaxValue;
            }

            public void MakeStart()
            {
                MinSteps = 0;
                Depth = 0;
            }

            public bool TryUpdateMinSteps(int minSteps)
            {
                var prevMinSteps = MinSteps;
                MinSteps = Math.Min(prevMinSteps, minSteps);

                return prevMinSteps != MinSteps;
            }

            public void UpdateDepth(int depth)
            {
                Depth = depth;
            }

            static int PopCount(int num)
            {
                var result = 0;
                while (num > 0)
                {
                    result += num & 1;
                    num >>= 1;
                }

                return result;
            }
        }

        public static void Main()
        {
            const int X = 31;
            const int Y = 39;
            const int STEPS = 50;

            var inputStr = File.ReadAllText("2016/input13.txt");
            var input = int.Parse(inputStr);

            var rooms = new Room[input, input];
            
            for (var i = 0; i < input; ++i)
            {
                for (var j = 0; j < input; ++j)
                {
                    rooms[i, j] = new Room(i, j, input);
                }
            }

            var distinctRooms = new HashSet<Room>();
            var queue = new Queue<Room>();

            var startRoom = rooms[1, 1];
            startRoom.MakeStart();
            
            queue.Enqueue(startRoom);

            while (queue.Count > 0)
            {
                var room = queue.Dequeue();
                
                if (room.Depth <= STEPS)
                {
                    distinctRooms.Add(room);
                }

                foreach (Direction dir in Enum.GetValues(typeof(Direction)))
                {
                    var neighbourRoom = rooms.GetNeighbour(room, dir);
                    if (neighbourRoom == null)
                    {
                        continue;
                    }

                    var neighbourRoomType = neighbourRoom.Type;
                    if (neighbourRoomType == RoomType.Wall)
                    {
                        continue;
                    }

                    var areMinStepsUpdated = neighbourRoom.TryUpdateMinSteps(room.MinSteps + 1);
                    if (areMinStepsUpdated)
                    {
                        neighbourRoom.UpdateDepth(room.Depth + 1);
                        queue.Enqueue(neighbourRoom);
                    }
                }
            }

            Console.WriteLine(rooms[X, Y].MinSteps);
            Console.WriteLine(distinctRooms.Count);

            Console.ReadLine();
        }
    }
}