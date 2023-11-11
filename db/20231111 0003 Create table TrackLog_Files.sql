
-- ----------------------------
-- Table structure for TrackLog_Files
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[TrackLog_Files]') AND type IN ('U'))
	DROP TABLE [dbo].[TrackLog_Files]
GO

CREATE TABLE [dbo].[TrackLog_Files] (
  [TrackFileId] bigint  IDENTITY(1,1) NOT NULL,
  [FileFullPath] nvarchar(500) COLLATE Cyrillic_General_CI_AS  NOT NULL,
  [FileStartProceedTime] datetime  NULL,
  [FileFinishProceedTime] datetime  NULL,
  [EntriesInFilesOK] bigint DEFAULT 0 NOT NULL,
  [EntriesInFilesFailed] bigint DEFAULT 0 NOT NULL,
  [OverallSuccessStatus] bit DEFAULT 0 NULL,
  [ErrorMessage] nvarchar(500) COLLATE Cyrillic_General_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[TrackLog_Files] SET (LOCK_ESCALATION = TABLE)
GO

-- ----------------------------
-- Primary Key structure for table TrackLog_Files
-- ----------------------------
ALTER TABLE [dbo].[TrackLog_Files] ADD CONSTRAINT [PK_Files] PRIMARY KEY CLUSTERED ([TrackFileId])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO

