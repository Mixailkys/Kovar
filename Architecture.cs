using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Generics.Robots
{
    public interface IRobotAI<out T>
        where T : IMoveCommand
    {
        T GetCommand();
    }

    public class ShooterAI<T> : IRobotAI<T>
        where T: IMoveCommand
    {
        int counter = 1;

        public T GetCommand()
        {
            return (T)(IMoveCommand)ShooterCommand.ForCounter(counter++);
        }
    }

    public class BuilderAI<T> : IRobotAI<T>
        where T: IMoveCommand
    {
        int counter = 1;

        public T GetCommand()
        {
            return (T)(IMoveCommand)BuilderCommand.ForCounter(counter++);
        }
    }

    public interface IDevice<in T>
        where T : IMoveCommand
    {
        string ExecuteCommand(T command);
    }

    public class Mover<T> : IDevice<T>
        where T: IMoveCommand
    {
        public string ExecuteCommand(T _command)
        {
            var command = _command as IMoveCommand;
            if (command == null)
                throw new ArgumentException();
            return $"MOV {command.Destination.X}, {command.Destination.Y}";
        }
    }

    public class ShooterMover<T> : IDevice<T>
        where T : IMoveCommand
    {
        public string ExecuteCommand(T _command)
        {
            var command = _command as IShooterMoveCommand;
            if (command == null)
                throw new ArgumentException();
            var hide = command.ShouldHide ? "YES" : "NO";
            return $"MOV {command.Destination.X}, {command.Destination.Y}, USE COVER {hide}";
        }
    }

    public static class Robot
    {
        public static Robot<TCommand> Create<TCommand>(IRobotAI<TCommand> ai, IDevice<TCommand> executor)
            where TCommand : IMoveCommand
        {
            return new Robot<TCommand>(ai, executor);
        }
    }

    public class Robot<T>
        where T : IMoveCommand
    {
        private readonly IRobotAI<T> ai;
        private readonly IDevice<T> device;

        public Robot(IRobotAI<T> ai, IDevice<T> executor)
        {
            this.ai = ai;
            this.device = executor;
        }

        public IEnumerable<string> Start(int steps)
        {
            for (int i = 0; i < steps; i++)
            {
                var command = ai.GetCommand();
                if (command == null)
                    break;
                yield return device.ExecuteCommand(command);
            }
        }

    }
}
