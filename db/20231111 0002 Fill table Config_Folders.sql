
-- ----------------------------
-- Records of Config_Folders
-- ----------------------------
SET IDENTITY_INSERT [dbo].[Config_Folders] ON
GO

INSERT INTO [dbo].[Config_Folders] ([FolderId], [FolderToObserver], [FilePattern], [FolderIsActive], [AssignToObserver], [Template_HeaderStartWith], [Template_LineStartWith], [Template_HeaderDataRegex], [Template_HeaderLineRegex]) VALUES (N'1', N'E:\My\SDK\XTask6\data_in\s1\', N'*.txt', N'1', N'35', N'HDR', N'LINE', N'^HDR\s+(\S+)\s+(\S+)$', N'^LINE\s+(\S+)\s+(\d+)\s+(\d+)$')
GO

INSERT INTO [dbo].[Config_Folders] ([FolderId], [FolderToObserver], [FilePattern], [FolderIsActive], [AssignToObserver], [Template_HeaderStartWith], [Template_LineStartWith], [Template_HeaderDataRegex], [Template_HeaderLineRegex]) VALUES (N'2', N'E:\My\SDK\XTask6\data_in\s2\', N'*.txt', N'1', N'35', N'HDR', N'LINE', N'^HDR\s+(\S+)\s+(\S+)$', N'^LINE\s+(\S+)\s+(\d+)\s+(\d+)$')
GO

INSERT INTO [dbo].[Config_Folders] ([FolderId], [FolderToObserver], [FilePattern], [FolderIsActive], [AssignToObserver], [Template_HeaderStartWith], [Template_LineStartWith], [Template_HeaderDataRegex], [Template_HeaderLineRegex]) VALUES (N'3', N'E:\My\SDK\XTask6\data_in\s3\', N'*.*', N'1', N'35', N'HDR', N'LINE', N'^HDR\s+(\S+)\s+(\S+)$', N'^LINE\s+(\S+)\s+(\d+)\s+(\d+)$')
GO

SET IDENTITY_INSERT [dbo].[Config_Folders] OFF
GO


-- ----------------------------
-- Auto increment value for Config_Folders
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[Config_Folders]', RESEED, 3)
GO


