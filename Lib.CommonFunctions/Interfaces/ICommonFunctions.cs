using Microsoft.Extensions.Configuration;

namespace Lib.CommonFunctions.Interfaces
{
    public interface ICommonFunctions
    {
        string ReadCriticalParameter(IConfigurationRoot configurationRoot, string aParamName);

        int ReadIntParameter(IConfigurationRoot configurationRoot, string aParamName, int aDefaultValue = 0);

        bool FileIsAccessible(string aFilePath, int aMaxWaitTime = 120000, int aSleepBetweenAttempt = 500);
    }
}
