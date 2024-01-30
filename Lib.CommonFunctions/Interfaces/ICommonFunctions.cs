using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lib.CommonFunctions.Interfaces
{
    public interface ICommonFunctions
    {
        void SetLogger(ILogger aLogger);
        
        string ReadCriticalParameter(IConfigurationRoot configurationRoot, string aParamName);

        int ReadIntParameter(IConfigurationRoot configurationRoot, string aParamName, int aDefaultValue = 0);

        bool FileIsAccessible(string aFilePath, int aMaxWaitTime = 120000, int aSleepBetweenAttempt = 500);
    }
}
