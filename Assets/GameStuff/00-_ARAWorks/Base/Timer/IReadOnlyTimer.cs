using System;
namespace ARAWorks.Base.Timer
{
    public interface IReadOnlyTimer
    {
        event Action Started;
        event Action<float> Tick;
        event Action Finished;

        float Elapsed { get; }

        float Goal { get; }

        bool IsRunning { get; }

        bool IsFinished { get; }

        bool AutoStop { get; }

        float PercentComplete { get; }

        float RemainingTime { get; }
    }
}
