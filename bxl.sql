GO
/****** Object:  Schema [bxl]    Script Date: 1/12/2018 10:44:38 AM ******/
CREATE SCHEMA [bxl]
GO
/****** Object:  Table [bxl].[Errors]    Script Date: 1/12/2018 10:44:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [bxl].[Errors](
	[ErrorId] [int] IDENTITY(1,1) NOT NULL,
	[TemplateId] [int] NOT NULL,
	[ColumnIndex] [int] NULL,
	[RowIndex] [int] NULL,
	[ColumnName] [nvarchar](50) NULL,
	[Issue] [nvarchar](500) NULL,
	[ImportFileName] [nvarchar](500) NULL,
	[Value] [nvarchar](2000) NULL,
 CONSTRAINT [PK_Errors] PRIMARY KEY CLUSTERED 
(
	[ErrorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [bxl].[Logs]    Script Date: 1/12/2018 10:44:39 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [bxl].[Logs](
	[LogId] [int] IDENTITY(1,1) NOT NULL,
	[TemplateId] [int] NOT NULL,
	[RanBy] [nvarchar](100) NULL,
	[Occurred] [datetime] NOT NULL,
	[Results] [nvarchar](100) NULL,
	[Notes] [nvarchar](255) NULL,
 CONSTRAINT [PK_FileProcessLog] PRIMARY KEY CLUSTERED 
(
	[LogId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [bxl].[Template]    Script Date: 1/12/2018 10:44:39 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [bxl].[Template](
	[TemplateId] [int] IDENTITY(1,1) NOT NULL,
	[PurgeWorkingOnEveryRun] [bit] NOT NULL,
	[ProcessName] [nvarchar](50) NOT NULL,
	[ProjectGroup] [nvarchar](50) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[ConnectionString] [nvarchar](300) NULL,
	[LoadTransformProcedure] [nvarchar](100) NULL,
	[FileNamePattern] [nvarchar](255) NULL,
	[OnErrorEmail] [nvarchar](500) NULL,
	[Created] [datetime] NOT NULL,
	[HasHeaders] [bit] NOT NULL,
	[Delimiter] [nvarchar](10) NULL,
	[ImportDirectory] [nvarchar](255) NULL,
	[ReferenceFile] [nvarchar](500) NOT NULL,
	[EmailProfile] [nvarchar](100) NULL,
	[ImportDirectoryHandling] [tinyint] NULL,
	[RowBehaviorOnFailure] [tinyint] NULL,
	[WorksheetName] [nvarchar](100) NULL,
	[StartRow] [int] NULL,
 CONSTRAINT [PK_Template] PRIMARY KEY CLUSTERED 
(
	[TemplateId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [bxl].[TemplateFields]    Script Date: 1/12/2018 10:44:39 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [bxl].[TemplateFields](
	[TemplateFieldId] [int] IDENTITY(1,1) NOT NULL,
	[TemplateId] [int] NOT NULL,
	[FieldName] [nvarchar](50) NOT NULL,
	[FieldType] [nvarchar](30) NOT NULL,
	[Created] [datetime] NOT NULL,
	[IsNullable] [bit] NOT NULL,
	[ColumnIndex] [int] NOT NULL,
	[ExcelColumnReference] [nvarchar](10) NULL,
	[ColumnLength] [int] NOT NULL,
 CONSTRAINT [PK_TemplateFields] PRIMARY KEY CLUSTERED 
(
	[TemplateFieldId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [bxl].[Logs] ADD  CONSTRAINT [DF_FileProcessLog_Occurred]  DEFAULT (getdate()) FOR [Occurred]
GO
ALTER TABLE [bxl].[Template] ADD  CONSTRAINT [DF_Template_Created]  DEFAULT (getdate()) FOR [Created]
GO
ALTER TABLE [bxl].[TemplateFields] ADD  CONSTRAINT [DF_TemplateFields_Created]  DEFAULT (getdate()) FOR [Created]
GO
ALTER TABLE [bxl].[Errors]  WITH CHECK ADD  CONSTRAINT [FK_Errors_Errors] FOREIGN KEY([TemplateId])
REFERENCES [bxl].[Template] ([TemplateId])
GO
ALTER TABLE [bxl].[Errors] CHECK CONSTRAINT [FK_Errors_Errors]
GO
ALTER TABLE [bxl].[Logs]  WITH CHECK ADD  CONSTRAINT [FK_Logs_Template] FOREIGN KEY([TemplateId])
REFERENCES [bxl].[Template] ([TemplateId])
GO
ALTER TABLE [bxl].[Logs] CHECK CONSTRAINT [FK_Logs_Template]
GO
ALTER TABLE [bxl].[TemplateFields]  WITH CHECK ADD  CONSTRAINT [FK_TemplateFields_Template] FOREIGN KEY([TemplateId])
REFERENCES [bxl].[Template] ([TemplateId])
GO
ALTER TABLE [bxl].[TemplateFields] CHECK CONSTRAINT [FK_TemplateFields_Template]
GO
/****** Object:  StoredProcedure [bxl].[EmailOnError]    Script Date: 1/12/2018 10:44:39 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [bxl].[EmailOnError] 
	@OnErrorEmail nvarchar(500),
	@Subject nvarchar(100),
	@Body nvarchar(500),
	@StartErrorId int,
	@EndErrorId int
AS
BEGIN
	
	SET NOCOUNT ON;

	DECLARE @errorSelect nvarchar(max);
	SET @errorSelect = 'SET NOCOUNT ON; SELECT        
		ColumnIndex, RowIndex [Row], ColumnName [Column], [Value], Issue, ImportFileName
	FROM            
		[' + (SELECT db_name()) +'].bxl.Errors
	WHERE
		ErrorId BETWEEN ' + CAST(@StartErrorId AS nvarchar) + ' AND ' + CAST(@EndErrorId AS nvarchar) +';';

	DECLARE @tab char(1);
	SET @tab =	CHAR(9);

	SELECT @errorSelect;
		EXEC msdb.dbo.sp_send_dbmail  
			@recipients = @OnErrorEmail,  			
			@query = @errorSelect,  
			@subject = @Subject,  
			@body= @Body,
			@body_format = 'HTML',
			@query_attachment_filename = 'errors.txt',
			@query_result_separator = @tab,
			@attach_query_result_as_file = 1,
			@query_result_no_padding= 1,
			@exclude_query_output =1,
			@append_query_error = 0,
			@query_result_header =1,
			@query_result_width=32767;


END
GO
/****** Object:  StoredProcedure [bxl].[Error_Insert]    Script Date: 1/12/2018 10:44:39 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [bxl].[Error_Insert] 	
	@ErrorId int output,
	@TemplateId int,
	@ColumnIndex int,
	@RowIndex int,
	@ColumnName nvarchar(50),
	@Issue nvarchar(500),
	@ImportFileName nvarchar(500),
	@Value nvarchar(2000)
AS BEGIN
   
	INSERT INTO bxl.Errors
	(
       TemplateId,
       ColumnIndex,
       RowIndex,
       ColumnName,
       Issue,
       ImportFileName,
	   [Value]
	)
	VALUES
	(
		@TemplateId,
		@ColumnIndex,
		@RowIndex,
		@ColumnName,
		@Issue,
		@ImportFileName,
		@Value
	);
	SET @ErrorId = SCOPE_IDENTITY();

END 
GO
/****** Object:  StoredProcedure [bxl].[Log_List]    Script Date: 1/12/2018 10:44:39 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [bxl].[Log_List] 
    @LogId int = null,
	@ProjectGroup nvarchar(50) = NULL,
	@ProcessName nvarchar(50) = NULL,
    @Top int = 30
AS BEGIN

	SET @LogId = CASE WHEN @LogId IS NULL THEN 0 ELSE @LogId END;

	SELECT TOP(@Top)
		l.LogId,
		v.ProjectGroup,
		v.ProcessName,
		l.RanBy,
		l.Occurred,
		l.Results,
		l.Notes
	FROM 
		bxl.Logs l INNER JOIN
		bxl.Template v ON l.TemplateId = v.TemplateId
	WHERE
		(
			@LogId = 0 OR
			LogId < @LogId
		) AND
		(
			@ProcessName IS NULL OR
			v.ProcessName = @ProcessName
		) AND
		(
			@ProjectGroup IS NULL OR
			v.ProjectGroup = @ProjectGroup
		)
	ORDER BY 
		l.LogId DESC;

   
END
GO
/****** Object:  StoredProcedure [bxl].[Log_Update]    Script Date: 1/12/2018 10:44:39 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [bxl].[Log_Update]
    @LogId int output,	
    @TemplateId int,
	@RanBy nvarchar(100),	
	@Results nvarchar(100),
	@Notes nvarchar(255)
AS BEGIN

	IF @LogId > 0 BEGIN

		UPDATE bxl.Logs
		SET 
		    RanBy = @RanBy,		    		    
		    Notes = @Notes
		WHERE
			LogId = @LogId;

	END ELSE BEGIN

		INSERT INTO bxl.Logs
		(
		    TemplateId,
		    RanBy,
		    Results,
		    Notes
		)
		VALUES
		(
			@TemplateId,
		    @RanBy,
		    @Results,
		    @Notes
		);
		SET @LogId = SCOPE_IDENTITY();
	END
END
GO
/****** Object:  StoredProcedure [bxl].[Template_First]    Script Date: 1/12/2018 10:44:39 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [bxl].[Template_First] 
    @TemplateId int
AS BEGIN

    SELECT        
		TemplateId, ProcessName, 
		ProjectGroup, ConnectionString, 
		LoadTransformProcedure, FileNamePattern, 
		OnErrorEmail, PurgeWorkingOnEveryRun,
		Created, HasHeaders, Delimiter, 
		ImportDirectory,  
		ReferenceFile,
		EmailProfile,
		ImportDirectoryHandling,
		RowBehaviorOnFailure,
		b.WorksheetName,
		b.StartRow
	FROM            
		bxl.Template b
	WHERE
		TemplateId = @TemplateId;

END	
GO
/****** Object:  StoredProcedure [bxl].[Template_List]    Script Date: 1/12/2018 10:44:39 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [bxl].[Template_List] 
    @ProjectGroup nvarchar(50)
AS BEGIN

    SELECT        
		TemplateId, ProcessName, 
		ProjectGroup, ConnectionString, 
		LoadTransformProcedure, FileNamePattern, 
		OnErrorEmail, PurgeWorkingOnEveryRun,
		Created, HasHeaders, Delimiter, 
		ImportDirectory,  
		ReferenceFile,
		EmailProfile,
		ImportDirectoryHandling,
		RowBehaviorOnFailure,
		b.WorksheetName,
		b.StartRow
	FROM            
		bxl.Template b
	WHERE
		ProjectGroup = @ProjectGroup;

END	
GO
/****** Object:  StoredProcedure [bxl].[Template_Update]    Script Date: 1/12/2018 10:44:39 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [bxl].[Template_Update]
    @TemplateId int output, 
	@ProcessName nvarchar(50),
	@ProjectGroup nvarchar(50), 
	@ConnectionString nvarchar(300), 
	@LoadTransformProcedure nvarchar(100), 
	@FileNamePattern nvarchar(255), 
	@OnErrorEmail nvarchar(500),
	@IsActive bit,
	@PurgeWorkingOnEveryRun bit,
	@HasHeaders bit,
	@Delimiter nvarchar(10),
	@ImportDirectory nvarchar(255),
	@ReferenceFile nvarchar(500),
	@EmailProfile nvarchar(100),
	@ImportDirectoryHandling tinyint,
	@RowBehaviorOnFailure tinyint,
	@WorksheetName nvarchar(100),
	@StartRow int
AS BEGIN

	IF @TemplateId > 0 BEGIN

		UPDATE bxl.Template
		SET 
			ProcessName = @ProcessName,
			ProjectGroup = @ProjectGroup,
			IsActive = @IsActive, 
			ConnectionString = @ConnectionString,
			LoadTransformProcedure = @LoadTransformProcedure, 
			FileNamePattern = @FileNamePattern,
			OnErrorEmail = @OnErrorEmail,
			PurgeWorkingOnEveryRun = @PurgeWorkingOnEveryRun,
			HasHeaders = @HasHeaders,
			Delimiter = @Delimiter,
			ImportDirectory = @ImportDirectory,
			ReferenceFile = @ReferenceFile,
			EmailProfile = @EmailProfile,
			ImportDirectoryHandling = @ImportDirectoryHandling,
			RowBehaviorOnFailure = @RowBehaviorOnFailure,
			WorksheetName = @WorksheetName,
			StartRow = @StartRow
		WHERE 
			TemplateId = @TemplateId;

	END ELSE BEGIN

		INSERT INTO bxl.Template
        (
			ProcessName,
			ProjectGroup,
			IsActive,
			ConnectionString,
			LoadTransformProcedure,
			FileNamePattern,
			OnErrorEmail,
			PurgeWorkingOnEveryRun,
			HasHeaders,
			Delimiter,
			ImportDirectory,
			ReferenceFile,
			EmailProfile,
			ImportDirectoryHandling,
			RowBehaviorOnFailure,
			WorksheetName,
			StartRow
		)
		VALUES
        (
			@ProcessName,
			@ProjectGroup,
			@IsActive,
			@ConnectionString, 
			@LoadTransformProcedure,
			@FileNamePattern,
			@OnErrorEmail,
			@PurgeWorkingOnEveryRun,
			@HasHeaders,
			@Delimiter,
			@ImportDirectory,
			@ReferenceFile,
			@EmailProfile,
			@ImportDirectoryHandling,
			@RowBehaviorOnFailure,
			@WorksheetName,
			@StartRow
		);
		SET @TemplateId = SCOPE_IDENTITY();
	END

END	
GO
/****** Object:  StoredProcedure [bxl].[TemplateField_List]    Script Date: 1/12/2018 10:44:39 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create PROCEDURE [bxl].[TemplateField_List] 
    @TemplateId int
AS BEGIN
	SELECT
		TemplateFieldId, TemplateId,
		FieldName, FieldType, Created, IsNullable, ColumnIndex,
		ExcelColumnReference, ColumnLength
	FROM            
		bxl.TemplateFields
	WHERE
		TemplateId = @TemplateId;
END
GO
/****** Object:  StoredProcedure [bxl].[TemplateField_Update]    Script Date: 1/12/2018 10:44:39 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create PROCEDURE [bxl].[TemplateField_Update] 
    @TemplateFieldId int output,
	@TemplateId int,
	@FieldName nvarchar(50),
	@FieldType nvarchar(30),
	@IsNullable bit,
	@ColumnIndex int,
	@ExcelColumnReference nvarchar(10),
	@ColumnLength int

AS begin
	
	IF @TemplateFieldId > 0 BEGIN

		UPDATE bxl.TemplateFields
		SET 
		    FieldName = @FieldName,
		    FieldType = @FieldType,		    
		    IsNullable = @IsNullable,
			ColumnIndex = @ColumnIndex,
			ExcelColumnReference = @ExcelColumnReference,
			ColumnLength = @ColumnLength
		WHERE
			TemplateFieldId = @TemplateFieldId;

	END ELSE BEGIN
		INSERT INTO bxl.TemplateFields
		(		
		    TemplateId,
		    FieldName,
		    FieldType,		    
		    IsNullable,
			ColumnIndex,
			ExcelColumnReference,
			ColumnLength
		)
		VALUES
		(		 
		    @TemplateId,
		    @FieldName,
		    @FieldType,
		    @IsNullable,
			@ColumnIndex,
			@ExcelColumnReference,
			@ColumnLength
		);
		SET @TemplateFieldId = SCOPE_IDENTITY();

	END

END 
GO
