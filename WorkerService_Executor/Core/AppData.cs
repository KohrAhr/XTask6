namespace WorkerService_Executor.Core
{
    public static class AppData
    {
        public const int CONST_DEF_FileMaxAccessWait = 60000;
        public const int CONST_DEF_SleepBetweenFileAccessAttempt = 1000;

        public static string ConnectionString { get; set; } = string.Empty;

        /// <summary>
        ///     1 minute default value
        /// </summary>
        public static int FileMaxAccessWait { get; set; } = CONST_DEF_FileMaxAccessWait;

        /// <summary>
        ///     1 second between access attempt if file locked
        /// </summary>
        public static int SleepBetweenFileAccessAttempt { get; set; } = CONST_DEF_SleepBetweenFileAccessAttempt;
    }
}
