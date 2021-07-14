-- ==========================================
-- Create Login For SysAdmin User template
-- ==========================================

CREATE LOGIN [<domain_name,,domain>\<user_name,,windows_user>] FROM WINDOWS WITH DEFAULT_DATABASE= <default_database, sysname, master>
GO
EXEC master..sp_addsrvrolemember @loginame = N'<domain_name,,domain>\<user_name,,windows_user>', @rolename = N'sysadmin'
GO

 