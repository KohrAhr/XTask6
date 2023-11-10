namespace WorkerService_Broker.Core
{
    public static class AppData
    {
        public static string QueuePath { get; set; } = "SimCity2023";

        public static string QueueServer { get; set; } = "localhost";

        /// <summary>
        ///     General dealy in seconds for ExecuteAsync
        /// </summary>
        public static int DelayInSeconds { get; set; } = 5;

        /// <summary>
        ///     2 minutes def.
        ///     How long we are waiting to get access to Locked file in total
        /// </summary>
        public static int FileMaxAccessWait { get; set; } = 120000;


        /// <summary>
        ///     0.5 second
        ///     How long we are wait between next attempt to get access to file
        /// </summary>
        public static int SleepBetweenFileAccessAttempt { get; set; } = 500;
    }
}
