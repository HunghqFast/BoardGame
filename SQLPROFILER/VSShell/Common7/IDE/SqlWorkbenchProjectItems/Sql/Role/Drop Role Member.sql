-- ============================
-- Drop Role Member template
-- ============================

USE <database_name, sysname, AdventureWorks>
GO

-- Create the database role
CREATE ROLE <role_name, sysname, Production_Owner> AUTHORIZATION [dbo]
GO

-- Grant access rights to a specific schema in the database
GRANT 
	ALTER, 
	CONTROL, 
	DELETE, 
	EXECUTE, 
	INSERT, 
	REFERENCES, 
	SELECT, 
	TAKE OWNERSHIP, 
	UPDATE, 
	VIEW DEFINITION 
ON SCHEMA::<schema_name, sysname, Production>
	TO <role_name, sysname, Production_Owner>
GO

-- Add an existing user to the role
EXEC sp_addrolemember N'<role_name, sysname, Production_Owner>', N'<user_name, sysname, user_name>'
GO

-- Drop the user from the role
EXEC sp_droprolemember N'<role_name, sysname, Production_Owner>', N'<user_name, sysname, user_name>'
