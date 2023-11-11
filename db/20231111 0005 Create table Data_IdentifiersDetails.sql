-- ----------------------------
-- Table structure for Data_IdentifiersDetails
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[Data_IdentifiersDetails]') AND type IN ('U'))
	DROP TABLE [dbo].[Data_IdentifiersDetails]
GO

CREATE TABLE [dbo].[Data_IdentifiersDetails] (
  [Id] bigint  IDENTITY(1,1) NOT NULL,
  [IdentifierId] bigint  NOT NULL,
  [PoNumber] nvarchar(50) COLLATE Cyrillic_General_CI_AS  NULL,
  [ISBN] nvarchar(50) COLLATE Cyrillic_General_CI_AS  NULL,
  [Qty] int DEFAULT 0 NOT NULL
)
GO

ALTER TABLE [dbo].[Data_IdentifiersDetails] SET (LOCK_ESCALATION = TABLE)
GO

-- ----------------------------
-- Primary Key structure for table Data_IdentifiersDetails
-- ----------------------------
ALTER TABLE [dbo].[Data_IdentifiersDetails] ADD CONSTRAINT [PK_IdentifiersDetails] PRIMARY KEY CLUSTERED ([Id])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO

