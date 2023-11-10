namespace WorkerService_Executor.Interfaces
{
    public interface IWorkerExecutor
    {
        void StartWithParametersAsync(string parameter1, CancellationToken cancellationToken);
    }
}
