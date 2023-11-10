using Lib.CommonFunctions;
using Lib.DataTypes;
using System.Text.RegularExpressions;

namespace WorkerService_Executor.Functions
{
    public class ParseHelper
    {
        private const string CONST_REGEX_HEADER = @"^HDR\s+(\S+)\s+(\S+)$";
        private const string CONST_REGEX_DATALINE = @"^LINE\s+(\S+)\s+(\d+)\s+(\d+)$";

        private const string CONST_HEADER_STARTWITH = "HDR";
        private const string CONST_LINE_STARTWITH = "LINE";

        private ILogger _logger;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="logger"></param>
        public ParseHelper(ILogger logger) 
        { 
            _logger = logger;
        }

        /// <summary>
        ///     Main function to proceed data file
        /// </summary>
        /// <param name="aFileName"></param>
        /// <param name="aExecutorId"></param>
        /// <returns></returns>
        public DataFileResult ProceedDataFile(string aFileName)
        {
            DataFileResult result = new()
            {
                Suceeded = false
            };

            Box? currentBox = null;

            int lineId = 0;

            string onlyFileName = Path.GetFileName(aFileName);

            // Can access data file?

            // File is accessible
            if (new CommonFunctions(_logger).FileIsAccessible(aFileName))
            {
                // OK file is accessible now
                try
                {
                    using (FileStream fileStream = new FileStream(aFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (StreamReader reader = new StreamReader(fileStream))
                        {
                            // Read data from the file
                            string? line;
                            while ((line = reader.ReadLine()?.Trim()) != null)
                            {
                                lineId++;

                                // Ok, log for all might be too expensive?
                                if (lineId % 10000 == 0)
                                {
                                    _logger.LogInformation($"File: \"{onlyFileName}\".Total boxes saved {lineId}");
                                }


                                // Nothing to do with empty line
                                if (String.IsNullOrEmpty(line))
                                {
                                    continue;
                                }

                                // Is it Header?
                                if (line.StartsWith(CONST_HEADER_STARTWITH))
                                {
                                    // If currentBox is not null, save it
                                    if (currentBox != null)
                                    {
                                        // TODO: Save Box to db

                                        //

                                        result.EntriesInFilesOK++;

                                        currentBox = null;
                                    }

                                    // Start a new box
                                    currentBox = ParseHeaderLine(line);

                                    if (currentBox == null)
                                    {
                                        // bad data

                                        ReportBadData(aFileName, lineId, line);
                                        result.EntriesInFilesFailed++;
                                    }

                                    // No reason to wait until end of loop
                                    continue;
                                }
                                // or data
                                else
                                if (line.StartsWith(CONST_LINE_STARTWITH) && currentBox != null)
                                {
                                    Box.Content? contentOfLine = ParseLine(line);

                                    if (contentOfLine != null)
                                    {
                                        currentBox.Contents.Add(contentOfLine);
                                    }
                                    else
                                    {
                                        // bad data

                                        ReportBadData(aFileName, lineId, line);
                                        result.EntriesInFilesFailed++;
                                    }
                                }
                                else
                                {
                                    // Handle invalid lines or other scenarios

                                    ReportBadData(aFileName, lineId, line);
                                    result.EntriesInFilesFailed++;
                                }
                            }

                            // Leftovers or Last box entry
                            if (currentBox != null)
                            {
                                // TODO: Save Box to db

                                //

                                result.EntriesInFilesOK++;

                                currentBox = null;
                            }

                            // Leftovers 
                            _logger.LogInformation($"File: \"{onlyFileName}\".Total boxes saved {lineId}");
                        }
                    }

                    // All entries from files are completed. Consider as Succcess
                    result.Suceeded = true;
                }
                catch (Exception ex)
                {
                    result.ErrorMessaget = "Exteption has occured. Error message: {ex.Message}";

                    _logger.LogError($"Exteption has occured. Data file is \"{aFileName}\". Error message: {ex.Message}");

                    result.Suceeded = false;
                }
            }
            else
            {
                result.ErrorMessaget = "File is locked by another process or user.";
            }

            return result;
        }


        private void ReportBadData(string aFileName, int aLineId, string aLineValue)
        {
            _logger.LogWarning($"File \"{aFileName}\" has failed at parsing. Unrecognized data. Issue in line {aLineId}, value \"{aLineValue}\"");
        }

        /// <summary>
        ///     Why its public? I want to make it accessible for tests.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public Box? ParseHeaderLine(string line)
        {
            string pattern = CONST_REGEX_HEADER;
            Match match = Regex.Match(line, pattern);

            if (match.Success && match.Groups.Count == 3)
            {
                string supplierIdentifier = match.Groups[1].Value.Trim();
                string identifier = match.Groups[2].Value.Trim();

                return new Box
                {
                    SupplierIdentifier = supplierIdentifier,
                    Identifier = identifier,
                };
            }

            // Return null for invalid lines
            return null;
        }

        /// <summary>
        ///     Why its public? I want to make it accessible for tests.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public Box.Content? ParseLine(string line)
        {
            string pattern = CONST_REGEX_DATALINE;
            Match match = Regex.Match(line, pattern);

            if (match.Success && match.Groups.Count == 4)
            {
                string poNumber = match.Groups[1].Value.Trim();
                string isbn = match.Groups[2].Value.Trim();
                int quantity;

                if (int.TryParse(match.Groups[3].Value.Trim(), out quantity))
                {
                    return new Box.Content
                    {
                        PoNumber = poNumber,
                        Isbn = isbn,
                        Quantity = quantity
                    };
                }
            }

            // Return null for invalid lines
            return null;
        }

    }
}
