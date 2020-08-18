USE [master]
GO

CREATE DATABASE [EliasMVCSample]
GO

USE [EliasMVCSample]
GO

CREATE SCHEMA [Elias]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [Elias].[CustomItem](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[TypeId] [int] NOT NULL,
	[Image] [image] NULL,
	[ParentId] [int] NULL,
	[IsDeleted] [bit] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [Elias].[CustomItem] ADD  CONSTRAINT [DF_CustomItem_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO

CREATE TABLE [Elias].[CustomType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[IsDeleted] [bit] NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [Elias].[CustomType] ADD  CONSTRAINT [DF_CustomType_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO

CREATE TABLE [Elias].[User](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Password] [nvarchar](50) NOT NULL,
	[IsActive] [bit] NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [Elias].[User] ADD  CONSTRAINT [DF_User_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO

-- =============================================
-- Author:		Elias Ramos
-- =============================================
CREATE PROCEDURE [Elias].[AuthenticateUser]
	@UserName nvarchar(50),
	@Password nvarchar(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    select top 1
		isnull([IsActive], 0) as IsAuthenticated
	from [Elias].[User]
	where
		[Name] = @UserName
		and [Password] = @Password
END
GO

-- =============================================
-- Author:		Elias Ramos
-- =============================================
Create PROCEDURE [Elias].[DeleteCustomItem]
	@CustomItemId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	update Elias.CustomItem set
		[IsDeleted] = cast(1 as bit)
	where
		[Id] =@CustomItemId
		
END
GO

-- =============================================
-- Author:		Elias Ramos
-- =============================================
CREATE PROCEDURE [Elias].[GetCustomItem]
	@CustomItemId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	select
		 i.[Id]
		,i.[Name]
		,[TypeId]
		,t.[Name] as [Type]
		,[Image]
		,[ParentId]
		,i.[IsDeleted]
	from [Elias].[CustomItem] as i
		join [Elias].[CustomType] as t on t.Id= i.TypeId
	where
		i.[Id] = @CustomItemId
		
END
GO

-- =============================================
-- Author:		Elias Ramos
-- =============================================
CREATE PROCEDURE [Elias].[GetCustomItemCount]
	@Filter nvarchar(50) = ''
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    select
		count(i.[Id]) as CustomItemTotal
	from [Elias].CustomItem as i
		left join [Elias].[CustomItem] as p on p.[Id] = i.[ParentId]
	where
		i.[IsDeleted] = 0
		and isnull(p.[IsDeleted], 0) = 0
		and i.[Name] like '%' + @Filter + '%' 

END
GO

-- =============================================
-- Author:		Elias Ramos
-- =============================================
CREATE PROCEDURE [Elias].[GetCustomItems]
	@Page int = 0,
	@Filter nvarchar(50) = ''
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	declare @IdStart int
	declare @ParentStart int
	declare @SetSize int = 10

	select top 1 @IdStart = [Id], @ParentStart = [ParentId] from (
		select top (@Page * @SetSize) [Id], [ParentId] from (
			select top (@Page * @SetSize)
					i.[Id],
					i.[ParentId],
					ROW_NUMBER() over (order by ISNULL(i.[ParentId], i.[Id]), i.[Id]) as [Row]
			from [Elias].[CustomItem] as i
				left join [Elias].[CustomItem] as p on p.[Id] = i.[ParentId]
			where
				i.[IsDeleted] = 0
				and ISNULL(p.[IsDeleted], 0) = 0
				and i.[Name] like '%' + @Filter + '%' 
		) as f order by f.[Row] desc
	) as r

	select top (@SetSize)
		 i.[Id]
		,i.[Name]
		,i.[TypeId]
		,t.[Name] as [Type]
		,i.[Image]
		,i.[ParentId]
		,p.[Name] as [ParentName]
		,i.[IsDeleted]
	from [Elias].[CustomItem] as i
		join [Elias].[CustomType] as t on t.[Id] = i.[TypeId]
		left join [Elias].[CustomItem] as p on p.[Id] = i.[ParentId]
	where
		i.[IsDeleted] = 0
		and isnull(p.[IsDeleted], 0) = 0
		and i.[Name] like '%' + @Filter + '%' 
		and (
			(@ParentStart is null
			and i.[Id] > isnull(@IdStart, 0)
			and (i.[ParentId] is null
				or i.[ParentId] >= isnull(@IdStart, 0)))
			or
			(@ParentStart is not null
			and i.[Id] > @ParentStart
			and (i.[ParentId] is null
				or (i.[Id] > @IdStart
					and i.[ParentId] >= @ParentStart)))
			)
	order by
		isnull(i.[ParentId], i.[Id]),
		i.[Id]
		
END
GO

-- =============================================
-- Author:		Elias Ramos
-- =============================================
CREATE PROCEDURE [Elias].[GetCustomTypes]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	select
		 [Id]
		,[Name]
		,[IsDeleted]
	from [Elias].[CustomType]
	where
		[IsDeleted] = 0
		
END
GO

-- =============================================
-- Author:		Elias Ramos
-- =============================================
CREATE PROCEDURE [Elias].[InsertCustomItem]
	@Name nvarchar(50),
	@TypeId int,
	@Image Image = null,
	@ParentId int = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	insert into Elias.CustomItem (
		 [Name]
		,[TypeId]
		,[Image]
		,[ParentId])
	values (
		@Name,
		@TypeId,
		@Image,
		@ParentId)

	select SCOPE_IDENTITY() as CustomItemId
		
END
GO

-- =============================================
-- Author:		Elias Ramos
-- =============================================
CREATE PROCEDURE [Elias].[InsertUser]
	@UserName nvarchar(50),
	@Password nvarchar(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	insert into [Elias].[User] (
		[Name]
		,[Password])
	values (
		@UserName
		,@Password)

END
GO

-- =============================================
-- Author:		Elias Ramos
-- =============================================
CREATE PROCEDURE [Elias].[UpdateCustomItem]
	@CustomItemId int,
	@Name nvarchar(50),
	@TypeId int,
	@Image Image = null,
	@ParentId int = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	update Elias.CustomItem set
		 [Name] = @Name
		,[TypeId] = @TypeId
		,[ParentId] = @ParentId
	where
		[Id] = @CustomItemId

	if @Image is not null
		update Elias.CustomItem set
			[Image] = @Image
		where
			[Id] = @CustomItemId
		
END
GO

-- =============================================
-- Author:		Elias Ramos
-- =============================================
CREATE PROCEDURE [Elias].[UserExists]
	@Username nvarchar(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
    select
		cast(count([Id]) as bit) as Result
	from [Elias].[User]
	where
		[Name] = @UserName
END
GO

Insert into [Elias].[CustomType] ([Name]) Values
('Archaic')
,('Classic')
,('Contemporary')
,('Eldritch')
,('Generic')
,('Obsolete')
,('Post Modern')
GO