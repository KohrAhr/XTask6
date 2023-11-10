namespace WorkerService_Observer.Core
{
    public static class AppData
    {
        public static string QueuePath { get; set; } = "SimCity2023";

        public static string QueueServer { get; set; } = "localhost";

        public static int MaxCountOfProceedProcesses { get; set; } = 5;

        /// <summary>
        ///     value from AssignToObserver
        /// </summary>
        public static int ScopeOfFolders { get; set; } = 35;
    }
}
