CREATE PROCEDURE [Franchise].[CreateFranchiseAdminUser]
    @pFranchiseId uniqueidentifier,
    @pFranchiseName nvarchar(50),
    @pOrganizationName nvarchar(100),
    @pOrganizationId uniqueidentifier,
    @pUserName nvarchar(50),
    @pPassword nvarchar(MAX),
    @pOutUserId uniqueidentifier = null output
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @vUserId uniqueidentifier = NEWID()
    DECLARE @vStatusId int
    DECLARE @vRoleId int = 1  -- Role ID 1 as specified
    DECLARE @vUserNo nvarchar(20)
    DECLARE @vYear nvarchar(4) = CONVERT(NVARCHAR(4), YEAR(GETDATE()))
    DECLARE @vRandomNumber nvarchar(5) = RIGHT(CAST(ABS(CHECKSUM(NEWID())) AS NVARCHAR), 5)
    
    -- Generate UserNo
    SET @vUserNo = @vYear + '-3-' + @vRandomNumber  -- 3 = Staff UserType
    
    -- Get Active status ID
    SELECT @vStatusId = Id FROM tblLookupItems WHERE LookupType = 'UserStatus' AND [Name] = 'Active'
    
    -- Create the user
    INSERT INTO [dbo].[tblUser]
        (Id, [FirstName], [SurName], [LastName], [Alias], [Age], [Gender], [MaritalStatus], 
         [Title], [Ethnicity], [BirthDate], [JoiningDate], [PassportNo], [IdentityNo], 
         [MobileNo], [PhoneNo], [Email], [Status], [CreatedDate], [IsActive], [UserType], 
         [NationalityId], [FranchiseId], UserNo, Notes, UserName, [Password])
    VALUES
        (@vUserId, 'Admin', '', @pOrganizationName, '', 0, NULL, NULL, NULL, NULL, NULL, 
         GETDATE(), '', 'ORG-' + SUBSTRING(CAST(@pOrganizationId AS NVARCHAR(36)), 1, 8), 
         '', '', @pUserName + '@' + LOWER(REPLACE(@pOrganizationName, ' ', '')) + '.com', 
         @vStatusId, GETUTCDATE(), 1, 3, NULL, @pFranchiseId, @vUserNo, 
         'Auto-created admin user for franchise: ' + @pFranchiseName, @pUserName, @pPassword)
    
    -- Create Staff record
    INSERT INTO [dbo].[tblStaff]
    VALUES (NEWID(), @vUserId, 1, GETDATE())
    
    -- Link user to franchise
    INSERT INTO [dbo].[tbUserFranchise]
    VALUES (@vUserId, @pFranchiseId, 1)
    
    -- Create user login credentials (password should be encrypted by API before calling this SP)
    INSERT INTO [dbo].[tblUserLogin]
    VALUES (@vUserId, @pUserName, @pPassword, 1)
    
    -- Assign role ID 1
    IF @vRoleId > 0
    BEGIN
        INSERT INTO [dbo].[tblUserRole]
            ([Id], [UserId], [RoleId], [IsActive], [CreatedDate])
        VALUES
            (NEWID(), @vUserId, @vRoleId, 1, GETUTCDATE())
    END
    
    SET @pOutUserId = @vUserId
    
END