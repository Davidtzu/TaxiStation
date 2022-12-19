--TABLES:
--1

USE [TaxiStation]
GO

SET ANSI_NULLS ON
GO


SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DriveHistory](
	[RequestID] [int] IDENTITY(30000,1) NOT NULL,
	[TaxiID] [int] NOT NULL,
	[UserID] [int] NOT NULL,
	[StartDate] [datetime] NULL,
	[FinishDate] [datetime] NULL,
	[Cost] [float] NULL,
	[StatusID] [int] NULL,
	[PickUp] [varchar](50) NULL,
	[TakenDown] [varchar](50) NULL
) ON [PRIMARY]
GO

--2

USE [TaxiStation]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Status_INF](
	[StatusID] [int] NULL,
	[StatusDesc] [varchar](100) NULL
) ON [PRIMARY]
GO

--3

USE [TaxiStation]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TaxiPool](
	[TaxiID] [int] NULL,
	[UserID] [int] NULL
) ON [PRIMARY]
GO


--4

USE [TaxiStation]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Taxis](
	[TaxiID] [int] NULL,
	[TaxiNumber] [varchar](50) NULL,
	[StatusID] [int] NULL,
	[FirstName] [varchar](50) NULL,
	[LastName] [varchar](150) NULL
) ON [PRIMARY]
GO

--5

USE [TaxiStation]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Users](
	[UserID] [int] NULL,
	[FirstName] [varchar](50) NULL,
	[LastName] [varchar](50) NULL,
	[Email] [varchar](100) NULL,
	[StatusID] [int] NULL
) ON [PRIMARY]
GO


--Stored Procedures:

--1

USE [TaxiStation]
GO
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[AddTaxiToPool] (
	@TaxiID int,
	@UserID int
)
AS
	BEGIN
		SET NOCOUNT ON;
		insert into [dbo].[TaxiPool]
		VALUES (@TaxiID, @UserID)
	END
GO


--2

USE [TaxiStation]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[FinishDrive] (
	@TaxiID int,
	@UserID int
)
AS
BEGIN

	SET NOCOUNT ON;

	update [dbo].[DriveHistory]
	set 
		[FinishDate] = (select dateadd(hh,+1,getdate()))
		,[Cost] = (SELECT CAST(RAND() * 200 AS INT) AS [RandomNumber])
		,[StatusID] = 8
	where 
		TaxiID = @TaxiID and StatusID <> 8

	update [dbo].[Taxis]
	set
		StatusID = 8
	where
		TaxiID = @TaxiID

	update [dbo].[Users]
	set 
		StatusID = 8
	where 
		UserID = @UserID

	delete from [dbo].[TaxiPool]
	where
		TaxiID = @TaxiID and 
		UserID = @UserID
END
GO

--3

USE [TaxiStation]
GO
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetDriveHistory](
	@UserID varchar(50),
	@UserType int
)
AS
BEGIN
	SET NOCOUNT ON;

	if @UserType = 1 --users
		begin
			SELECT 
				u.FirstName + ' ' + u.LastName as UserFullName,
				u.Email as UserEmail,
				t.TaxiNumber,
				t.FirstName + ' ' + t.LastName as TaxiDriverFullName,
				h.StartDate,
				h.FinishDate,
				h.Cost,
				h.PickUp,
				h.TakenDown
			from 
				[dbo].[DriveHistory] h
				left join [dbo].[Taxis] t on h.TaxiID = t.TaxiID
				left join [dbo].[Users] u on h.UserID = u.UserID
			where
				@UserID = u.UserID
			order by RequestID desc
		end
	else if @UserType = 2 --taxis
		begin
			SELECT 
				u.FirstName + ' ' + u.LastName as UserFullName,
				u.Email as UserEmail,
				t.TaxiNumber,
				t.FirstName + ' ' + t.LastName as TaxiDriverFullName,
				h.StartDate,
				h.FinishDate,
				h.Cost,
				h.PickUp,
				h.TakenDown
			from 
				[dbo].[DriveHistory] h
				left join [dbo].[Taxis] t on h.TaxiID = t.TaxiID
				left join [dbo].[Users] u on h.UserID = u.UserID
			where
				@UserID = t.TaxiID
			order by RequestID desc
		end
	else --station
		begin
			SELECT 
				u.FirstName + ' ' + u.LastName as UserFullName,
				u.Email as UserEmail,
				t.TaxiNumber,
				t.FirstName + ' ' + t.LastName as TaxiDriverFullName,
				h.StartDate,
				h.FinishDate,
				h.Cost,
				h.PickUp,
				h.TakenDown
			from 
				[dbo].[DriveHistory] h
				left join [dbo].[Taxis] t on h.TaxiID = t.TaxiID
				left join [dbo].[Users] u on h.UserID = u.UserID
			order by RequestID desc
		end
END
GO


--4

USE [TaxiStation]
GO
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetLisiningTaxis](
	@UserID int
)
AS
	BEGIN
		SET NOCOUNT ON;
		select * FROM  [dbo].[TaxiPool]
		where
			UserID = @UserID
	END
GO


--5


USE [TaxiStation]
GO
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[InsertDrive] (
	@IsAdded bit output,
	@TaxiID int,
	@UserID int
	)
AS
BEGIN

	SET NOCOUNT ON;

	  INSERT INTO [dbo].[DriveHistory]  (
		   [TaxiID]
		  ,[UserID]
		  ,[StartDate]
		  ,[FinishDate]
		  ,[Cost]
		  ,[StatusID]
		  ,[PickUp]
		  ,[TakenDown]
	  )
      VALUES 
	  (
		@TaxiID, 
		@UserID, 
		getDate(), 
		null, 
		null, 
		5,
		'X: ' +   (select Cast((SELECT FLOOR(RAND()*(50-1+1)+5)) as varchar(10))) + ' Y: ' +   (select Cast((SELECT FLOOR(RAND()*(50-1+1)+5)) as varchar(10))),
		'X: ' +   (select Cast((SELECT FLOOR(RAND()*(50-1+1)+5)) as varchar(10))) + ' Y: ' +   (select Cast((SELECT FLOOR(RAND()*(50-1+1)+5)) as varchar(10)))
	  )

	  update [dbo].[Taxis]
	  set 
		StatusID = 5
	  where
		TaxiID = @TaxiID

	  update [dbo].[Users]
	  set 
		StatusID = 5
	  where 
		UserID = @UserID


	  set 
		@IsAdded = 1
      RETURN  
		@IsAdded
END
GO







