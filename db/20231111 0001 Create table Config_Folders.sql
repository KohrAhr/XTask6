
-- ----------------------------
-- Table structure for Config_Folders
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[Config_Folders]') AND type IN ('U'))
	DROP TABLE [dbo].[Config_Folders]
GO

CREATE TABLE [dbo].[Config_Folders] (
  [FolderId] bigint  IDENTITY(1,1) NOT NULL,
  [FolderToObserver] nvarchar(500) COLLATE Cyrillic_General_CI_AS  NOT NULL,
  [FilePattern] nvarchar(50) COLLATE Cyrillic_General_CI_AS DEFAULT N'*.*' NOT NULL,
  [FolderIsActive] bit DEFAULT 0 NOT NULL,
  [AssignToObserver] bigint DEFAULT 0 NOT NULL,
  [Template_HeaderStartWith] nvarchar(50) COLLATE Cyrillic_General_CI_AS  NULL,
  [Template_LineStartWith] nvarchar(50) COLLATE Cyrillic_General_CI_AS  NULL,
  [Template_HeaderDataRegex] nvarchar(100) COLLATE Cyrillic_General_CI_AS  NULL,
  [Template_HeaderLineRegex] nvarchar(100) COLLATE Cyrillic_General_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[Config_Folders] SET (LOCK_ESCALATION = TABLE)
GO

-- ----------------------------
-- Primary Key structure for table Config_Folders
-- ----------------------------
ALTER TABLE [dbo].[Config_Folders] ADD CONSTRAINT [PK_Folders] PRIMARY KEY CLUSTERED ([FolderId])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO

