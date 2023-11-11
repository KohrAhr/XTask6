-- ----------------------------
-- Table structure for Data_Identifiers
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[Data_Identifiers]') AND type IN ('U'))
	DROP TABLE [dbo].[Data_Identifiers]
GO

CREATE TABLE [dbo].[Data_Identifiers] (
  [InId] bigint  IDENTITY(1,1) NOT NULL,
  [InSupplierId] nvarchar(50) COLLATE Cyrillic_General_CI_AS  NULL,
  [InIdentifier] nvarchar(50) COLLATE Cyrillic_General_CI_AS  NULL,
  [TrackFileId] bigint  NULL
)
GO

ALTER TABLE [dbo].[Data_Identifiers] SET (LOCK_ESCALATION = TABLE)
GO

-- ----------------------------
-- Primary Key structure for table Data_Identifiers
-- ----------------------------
ALTER TABLE [dbo].[Data_Identifiers] ADD CONSTRAINT [PK_Identifiers] PRIMARY KEY CLUSTERED ([InId])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO

