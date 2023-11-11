using Lib.DataTypes;

namespace WorkerService_Executor.Interfaces
{
    public interface IWorkerExecutor
    {
        void StartWithParametersAsync(Message aMessage, CancellationToken cancellationToken);
    }
}
