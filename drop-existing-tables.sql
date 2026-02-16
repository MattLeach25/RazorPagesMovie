-- Drop existing tables in RazorPagesMovieDB
-- Run this script against your Azure SQL Database before deploying the new migrations

USE RazorPagesMovieDB;
GO

-- Drop tables if they exist (in reverse order of dependencies)
IF OBJECT_ID('dbo.__EFMigrationsHistory', 'U') IS NOT NULL
    DROP TABLE [dbo].[__EFMigrationsHistory];
GO

IF OBJECT_ID('dbo.Documentary', 'U') IS NOT NULL
    DROP TABLE [dbo].[Documentary];
GO

IF OBJECT_ID('dbo.Movie', 'U') IS NOT NULL
    DROP TABLE [dbo].[Movie];
GO

PRINT 'All tables dropped successfully.';
GO

-- Verify tables are gone
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE';
GO
