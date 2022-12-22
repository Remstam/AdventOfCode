using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode._2020
{
    public class Day8
    {
        public enum CommandType
        {
            Nop, Acc, Jump
        }

        public class Command
        {
            public CommandType CmdType { get; private set; }
            public int Argument { get; private set; }

            private readonly CommandType _originalCmdType;

            public Command(CommandType cmdType, int argument)
            {
                CmdType = cmdType;
                Argument = argument;

                _originalCmdType = cmdType;
            }

            public void SwitchTo(CommandType cmdType)
            {
                CmdType = cmdType;
            }

            public void Reset()
            {
                CmdType = _originalCmdType;
            }
        }

        public static void Main()
        {
            var input = File.ReadAllLines("2020/input8.txt");
            var commands = Parse(input);

            Execute(commands, out var ip, out var acc, out var executedCmds);

            var newAcc = 0;
            for (var i = 0; i < commands.Count; ++i)
            {
                var cmd = commands[i];

                bool wasAbleToFinish;
                switch (cmd.CmdType)
                {
                    case CommandType.Acc:
                        break;

                    case CommandType.Nop:
                        if (cmd.Argument == 0)
                        {
                            break;
                        }

                        if (executedCmds.Contains(i + cmd.Argument))
                        {
                            break;
                        }

                        cmd.SwitchTo(CommandType.Jump);
                        wasAbleToFinish = Execute(commands, out _, out newAcc, out _);
                        if (wasAbleToFinish)
                        {
                            goto printResults;
                        }

                        cmd.Reset();
                        break;

                    case CommandType.Jump:
                        if (executedCmds.Contains(i + 1))
                        {
                            break;
                        }

                        cmd.SwitchTo(CommandType.Nop);
                        wasAbleToFinish = Execute(commands, out _, out newAcc, out _);
                        if (wasAbleToFinish)
                        {
                            goto printResults;
                        }

                        cmd.Reset();
                        break;
                }
            }

            printResults:
            Console.WriteLine(acc);
            Console.WriteLine(newAcc);
            
            Console.ReadLine();
        }

        private static List<Command> Parse(string[] input)
        {
            return (from str in input
                        select str.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                        into parts 
                            let cmdType = GetCmdType(parts[0])
                            let arg = int.Parse(parts[1])
                        select new Command(cmdType, arg))
                .ToList();
        }

        private static CommandType GetCmdType(string str)
        {
            switch (str)
            {
                case "nop":
                    return CommandType.Nop;
                case "acc":
                    return CommandType.Acc;
                case "jmp":
                    return CommandType.Jump;
                default:
                    return CommandType.Nop;
            }
        }

        private static bool Execute(List<Command> commands, out int ip, out int acc, out HashSet<int> executedCmds)
        {
            ip = 0;
            acc = 0;
            executedCmds = new HashSet<int>();

            loopStart:
            while (!executedCmds.Contains(ip))
            {
                if (ip == commands.Count)
                {
                    return true;
                }

                executedCmds.Add(ip);
                var cmd = commands[ip];

                switch (cmd.CmdType)
                {
                    case CommandType.Acc:
                        acc += cmd.Argument;
                        break;

                    case CommandType.Jump:
                        ip += cmd.Argument;
                        goto loopStart;

                    case CommandType.Nop:
                        break;
                }

                ++ip;
            }

            return false;
        }
    }
}