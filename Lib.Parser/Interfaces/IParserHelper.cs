using Lib.AppDb.Interfaces;
using Lib.CommonFunctions.Interfaces;
using Lib.DataTypes;
using Microsoft.Extensions.Logging;

namespace Lib.Parser.Interfaces
{
    public interface IParserHelper
    {
        void Init
        (
            ILogger aLogger, IAppDbContext aAppDbContext, ICommonFunctions aCommonFunctions,
            int aFileMaxAccessWait, int aSleepBetweenFileAccessAttempt
        );

        Task<DataFileResult> ProceedDataFile(string aFileName, Int64 aFileId);

        Box? ParseHeaderLine(string line);

        Box.Content? ParseLine(string line);

        void SaveBoxToDb(Box aBox, Int64 aFileId);

        void ReportBadData(string aFileName, int aLineId, string aLineValue);
    }
}
