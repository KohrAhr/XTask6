using Lib.CommonFunctions;
using Lib.DataTypes;
using Lib.DataTypes.EF;
using System.Text.RegularExpressions;
using WorkerService_Executor.Core;
using WorkerService_Executor.EF;

namespace WorkerService_Executor.Functions
{
    public class ParseHelper
    {
        // TODO: Move them away
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
        public async Task<DataFileResult> ProceedDataFile(string aFileName, Int64 aFileId)
        {
            DataFileResult result = new()
            {
                Suceeded = false
            };

            Box? currentBox = null;

            int lineId = 0;

            string onlyFileName = Path.GetFileName(aFileName);

            // Can we access data file?

            // File is accessible and exist
            if (!new CommonFunctions(_logger).FileIsAccessible(aFileName, AppData.FileMaxAccessWait, AppData.SleepBetweenFileAccessAttempt))
            {
                result.ErrorMessaget = "File is locked over limited time by another process or does not exist.";
                return result;
            }

            // OK file is accessible now
            try
            {
                using (FileStream fileStream = new FileStream(aFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        // Read data from the file
                        string? line;
                        while ((line = await reader.ReadLineAsync()) != null)
                        {
                            line = line.Trim();
                            lineId++;

                            // Ok, log for all might be too expensive?
                            if (lineId % 1000000 == 0)
                            {
                                _logger.LogInformation($"File: \"{onlyFileName}\". Completed lines in file {lineId}. Total boxes saved {result.EntriesInFilesOK}");
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
                                    SaveBoxToDb(currentBox, aFileId);

                                    //

                                    result.EntriesInFilesOK++;

                                    currentBox = null;
                                }

                                // Start a new aBox
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

                        // Leftovers or Last aBox entry
                        if (currentBox != null)
                        {
                            // TODO: Save Box to db
                            SaveBoxToDb(currentBox, aFileId);
                            //

                            result.EntriesInFilesOK++;

                            currentBox = null;
                        }

                        // Leftovers 
                        _logger.LogInformation($"File: \"{onlyFileName}\". Total lines in file {lineId}. Total boxes saved {result.EntriesInFilesOK}. Total failed boxes: {result.EntriesInFilesFailed}");
                    }
                }

                // All entries from files are completed. Consider as Succcess
                result.Suceeded = true;
            }
            catch (Exception ex)
            {
                result.ErrorMessaget = $"Exteption. Error message: {ex.InnerException}";

                _logger.LogError($"Exteption has occured. Data file is \"{aFileName}\". Error message: {ex.InnerException}");

                result.Suceeded = false;
            }

            return result;
        }


        private void SaveBoxToDb(Box aBox, Int64 aFileId)
        {
            // Main entry
            Data_Identifiers data_Identifiers = new Data_Identifiers
            {
                InIdentifier = aBox.Identifier,
                InSupplierId = aBox.SupplierIdentifier,
                TrackFileId = aFileId
            };

            IList<Data_IdentifiersDetails> identifiersDetailsList = new List<Data_IdentifiersDetails>();

            using (AppDbContext context = new AppDbContext())
            {
                // Main entry
                context.Data_Identifiers.Add(data_Identifiers);
                context.SaveChanges();

                // Addons (details)
                foreach(Box.Content item in aBox.Contents) 
                {
                    Data_IdentifiersDetails data_IdentifiersDetails = new Data_IdentifiersDetails
                    {
                        IdentifierId = data_Identifiers.InId,
                        PoNumber = item.PoNumber,
                        ISBN = item.Isbn,
                        Qty = item.Quantity
                    };

                    identifiersDetailsList.Add(data_IdentifiersDetails);
                }

                // Add all objects to the context in one go and save changes
                context.Data_IdentifiersDetails.AddRange(identifiersDetailsList);
                context.SaveChanges();
            }
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
