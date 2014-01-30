--***************************************************
--Monitoring Data Change with SqlDependency using C#
--***************************************************

--## Check Service Broker Enabled / Disbled for database.
--USE [SampleDb]
--GO
--SELECT NAME, IS_BROKER_ENABLED FROM SYS.DATABASES


--## Enable / Disable Service Broker for existing database.
--USE [SampleDb]
--GO
--ALTER DATABASE SampleDb SET ENABLE_BROKER
--ALTER DATABASE SampleDb SET DISABLE_BROKER

--## Create sample table in SampleDb database
--USE [SampleDb]
--GO
--CREATE TABLE [dbo].[SampleTable02](
--	[SampleId] [bigint] IDENTITY(1,1) NOT NULL,
--	[SampleName] [nvarchar](50) NOT NULL,
--	[SampleCategory] [nvarchar](50) NOT NULL,
--	[SampleDateTime] [datetime] NOT NULL,
--	[IsSampleProcessed] [bit] NOT NULL)
-- ON [PRIMARY];

--## Create stored procedure for query notification
--USE [SampleDb]
--GO
--CREATE PROCEDURE uspGetSampleInformation
--AS
--BEGIN
--	SELECT
--		[SampleId],
--		[SampleName],
--		[SampleCategory],
--		[SampleDateTime],
--		[IsSampleProcessed]
--	FROM
--		[dbo].[SampleTable01];
--END

--## Create QUEUE and SERVICE for the query
--USE [SampleDb]
--GO
--CREATE QUEUE QueueSampleInformationDataChange
--CREATE SERVICE ServiceSampleInformationDataChange
--	ON QUEUE QueueSampleInformationDataChange
--	([http://schemas.microsoft.com/SQL/Notifications/PostQueryNotification]);

--GRANT SUBSCRIBE QUERY NOTIFICATIONS TO YourSqlUserName;

--## Give authorization for the Sql user to SampleDb database
--ALTER AUTHORIZATION ON DATABASE::SampleDb TO YourSqlUserName;

--## Grand send permission on Service broker service you created for user that will be used to run the client application
--GRANT SEND ON SERVICE:: ServiceSampleInformationDataChange TO [YourMachineNameOrDomain\UserName];

--## Grand send permission on Service broker queue you created for user that will be used to run the client application
--GRANT RECEIVE ON dbo.QueueSampleInformationDataChange TO [YourMachineNameOrDomain\UserName];

--## Use this query to see the current queue subscriptions
--SELECT * FROM SYS.DM_QN_SUBSCRIPTIONS