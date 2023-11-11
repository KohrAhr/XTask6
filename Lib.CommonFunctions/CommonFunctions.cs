using Lib.CommonFunctions.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lib.CommonFunctions
{
    public class CommonFunctions : ICommonFunctions
    {
        private readonly ILogger _logger;

        public CommonFunctions(ILogger logger) 
        {
            _logger = logger;
        }

        /// <summary>
        ///     Read Critical parameter value from config file
        ///     Throw exception, if parameter is missed
        /// </summary>
        /// <param name="configurationRoot"></param>
        /// <param name="aParamName"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string ReadCriticalParameter(IConfigurationRoot configurationRoot, string aParamName)
        {
            string? lValue = configurationRoot[aParamName];

            if (string.IsNullOrEmpty(lValue))
            {
                string err = $"{aParamName} cannot be empty";
                _logger.LogCritical(err);
                throw new Exception(err);
            }

            return lValue;
        }

        public int ReadIntParameter(IConfigurationRoot configurationRoot, string aParamName, int aDefaultValue = 0)
        {
            int lResult;

            string? lValue = configurationRoot[aParamName];

            if (!int.TryParse(lValue, out lResult))
            {
                lResult = aDefaultValue;
            }

            return lResult;
        }

        //public IConfigurationRoot GetConfigFile()
        //{
        //    return new ConfigurationBuilder().AddConfiguration().JsonFile("appsettings.json").Build();
        //}

        public bool FileIsAccessible(string aFilePath, int aMaxWaitTime = 120000, int aSleepBetweenAttempt = 500)
        {
            if (!File.Exists(aFilePath)) 
            {
                _logger.LogInformation($"File \"{aFilePath}\" does not exist.");
                return false;
            }
            
            // Record the start time
            DateTime startTime = DateTime.Now;

            while (true)
            {
                try
                {
                    using (FileStream fileStream = File.Open(aFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        // File is accessible, you can now read from it
                        return true;
                    }
                }
                catch (IOException ex)
                {
                    // File is not yet accessible

                    // Check if the maximum wait time is reached
                    if ((DateTime.Now - startTime).TotalMilliseconds >= aMaxWaitTime)
                    {
                        return false;
                    }

                    _logger.LogInformation($"File {aFilePath} is locked by another user.");

                    // Wait for a short period before trying again
                    Thread.Sleep(aSleepBetweenAttempt);
                }
            }
        }
    }
}