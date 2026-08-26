      /* ================================
           tblLookups
        ================================ */
        SET IDENTITY_INSERT dbo.tblLookups ON;

        INSERT INTO dbo.tblLookups (Id, Name, Description, DisplayName, IsActive, IsVisible)
        SELECT v.Id, v.Name, v.Description, v.DisplayName, 1, 1
        FROM (VALUES
            (1,'AddressType','tblAddressType','Address Type'),
            (2,'ContactType','tblContactType','Contact Type'),
            (3,'Country','tblCountry','Country'),
            (4,'County','tblCounty','County'),
            (5,'Ethnicity','tblEthnicity','Ethnicity'),
            (6,'ExpenseType','tblExpenseType','Expense Type'),
            (7,'Gender','tblGender','Gender'),
            (8,'Language','tblLanguages','Languages'),
            (9,'LeaveStatus','tblLeaveStatus','Leave Status'),
            (10,'LeaveType','tblLeaveType','Leave Type'),
            (11,'MaritalStatus','tblMaritalStatus','Marital Status'),
            (12,'Nationality','tblNationality','Nationality'),
            (13,'Role','tblRole','Role'),
            (14,'TaskStatus','tblTaskStatus','Task Status'),
            (15,'UserStatus','tblUserStatus','User Status'),
            (16,'UserType','tblUserType','User Type'),
            (17,'State','tblState','State'),
            (18,'Department','tblDepartment','Department'),
            (19,'Currency','tblCurrency','Currency'),
            (20,'DocumentType','tblDocumentType','Document Type'),
            (21,'Title','tblTitle','Title'),
            (22,'TimeZones','tblTimeZones','Time Zones'),
            (23,'CardType','tblCardType','Card Type'),
            (24,'TransactionType','tblTransactionType','Transaction Type'),
            (25,'TransactionStatus','tblTransactionStatus','Transaction Status'),
            (26,'Banks','tblBanks','Banks'),
            (27,'ContractType','tblContractType','Contract Type'),
            (28,'Frequency','tblFrequency','Frequency'),
            (29,'CurrencySign','CurrencySign','Currency Sign'),
            (30,'SmokingStatus','SmokingStatus','Smoking Status'),
            (31,'PetFriendly','PetFriendly','Pet Friendly'),
            (32,'Certification','Certification','Certification'),
            (33,'TransportationMode','TransportationMode','Transportation Mode'),
            (34,'Experience','Experience','Experience'),
            (35,'AgeRange','AgeRange','Age Range'),
            (36,'ComplaintCategory','ComplaintCategory','ComplaintCategory'),
            (37,'ComplaintSeverity','ComplaintSeverity','ComplaintSeverity'),
            (38,'ComplaintStatus','ComplaintStatus','ComplaintStatus')
        ) v(Id,Name,Description,DisplayName)
        WHERE NOT EXISTS (
            SELECT 1 FROM dbo.tblLookups l WHERE l.Id = v.Id
        );

        SET IDENTITY_INSERT dbo.tblLookups OFF;

        /* ================================
           tblLookupItems (sample pattern)
        ================================ */
      SET IDENTITY_INSERT dbo.tblLookupItems ON;

        INSERT INTO dbo.tblLookupItems
        (Id, LookupType, Name, [Description], IsActive, InsertedById, InsertedAt, UpdatedById, UpdatedAt)
        VALUES
        (1, 'AddressType', 'Primary', 'Primary', 1, NULL, NULL, NULL, NULL),
        (2, 'AddressType', 'Shipping', 'Shipping', 1, NULL, NULL, NULL, NULL),
        (3, 'AddressType', 'Billing', 'Billing', 1, NULL, NULL, NULL, NULL),
        (4, 'UserStatus', 'Temporary', 'Temporary', 1, NULL, NULL, NULL, NULL),
        (5, 'UserStatus', 'Primary', 'Primary', 1, NULL, NULL, NULL, NULL),
        (6, 'Gender', 'Male', 'Male', 1, NULL, NULL, NULL, NULL),
        (7, 'Gender', 'Female', 'Female', 1, NULL, NULL, NULL, NULL),
        (8, 'Language', 'English', 'English', 1, NULL, NULL, NULL, NULL),
        (9, 'Ethnicity', 'Punjabi', 'Punjabi', 1, NULL, NULL, NULL, NULL),
        (10, 'Nationality', 'Pakistani', 'Pakistani', 1, NULL, NULL, NULL, NULL),
        (11, 'Title', 'MR', 'MR', 1, NULL, NULL, NULL, NULL),
        (12, 'Title', 'MRS', 'MRS', 1, NULL, NULL, NULL, NULL),
        (13, 'County', 'Rawalpindi', 'Rawalpindi', 1, NULL, NULL, NULL, NULL),
        (14, 'State', 'Punjab', 'Punjab', 1, NULL, NULL, NULL, NULL),
        (15, 'Country', 'Pakistan', 'Pakistan', 1, NULL, NULL, NULL, NULL),
        (16, 'MaritalStatus', 'Married', 'Married', 1, NULL, NULL, NULL, NULL),
        (17, 'MaritalStatus', 'UnMarried', 'UnMarried', 1, NULL, NULL, NULL, NULL),
        (18, 'UserStatus', 'Active', 'Active', 1, NULL, NULL, NULL, NULL),
        (19, 'UserStatus', 'InActive', 'InActive', 1, NULL, NULL, NULL, NULL),
        (20, 'ContactType', 'Primary', 'ContactType', 1, NULL, NULL, NULL, NULL),
        (21, 'ExpenseType', 'Travel', 'Travel', 1, NULL, NULL, NULL, NULL),
        (22, 'Language', 'Spanish', 'Spanish', 1, NULL, NULL, NULL, NULL),
        (23, 'Language', 'Urdu', 'Urdu', 1, NULL, NULL, NULL, NULL),
        (24, 'LeaveType', 'Annual', 'Annual', 1, NULL, NULL, NULL, NULL),
        (25, 'LeaveType', 'Casual', 'Casual', 1, NULL, NULL, NULL, NULL),
        (26, 'LeaveType', 'Sick', 'Sick', 1, NULL, NULL, NULL, NULL),
        (27, 'LeaveStatus', 'Approved', 'Approved', 1, NULL, NULL, NULL, NULL),
        (28, 'LeaveStatus', 'Pending', 'Pending', 1, NULL, NULL, NULL, NULL),
        (31, 'Role', 'Staff', 'Staff', 1, NULL, NULL, NULL, NULL),
        (32, 'Role', 'Admin', 'Admin', 1, NULL, NULL, NULL, NULL),
        (33, 'TaskStatus', 'Scheduled', 'Scheduled', 1, NULL, NULL, NULL, NULL),
        (34, 'TaskStatus', 'Delayed', 'Delayed', 1, NULL, NULL, NULL, NULL),
        (35, 'TaskStatus', 'In-Progress', 'In-Progress', 1, NULL, NULL, NULL, NULL),
        (36, 'TaskStatus', 'Completed', 'Completed', 1, NULL, NULL, NULL, NULL),
        (37, 'TaskStatus', 'Cancelled', 'Cancelled', 1, NULL, NULL, NULL, NULL),
        (38, 'TaskStatus', 'Unassigned', 'Unassigned', 1, NULL, NULL, NULL, NULL),
        (39, 'Department', 'Other', 'Other', 1, NULL, NULL, NULL, NULL),
        (40, 'Currency', 'Dollar', 'Dollar', 1, NULL, NULL, NULL, NULL),
        (41, 'Currency', 'PKR', 'PKR', 1, NULL, NULL, NULL, NULL),
        (42, 'DocumentType', 'ID Card', 'ID Card', 1, NULL, NULL, NULL, NULL),
        (43, 'DocumentType', 'Educational', 'Educational', 1, NULL, NULL, NULL, NULL),
        (44, 'DocumentType', 'Other', 'Other', 1, NULL, NULL, NULL, NULL),
        (45, 'TimeZones', 'Pakistan Standard Time', 'Pakistan Standard Time', 1, NULL, NULL, NULL, NULL),
        (46, 'TimeZones', 'Irish Standard Time', 'Irish Standard Time', 1, NULL, NULL, NULL, NULL),
        (47, 'CardType', 'Credit Card', 'Credit Card', 1, NULL, NULL, NULL, NULL),
        (48, 'CardType', 'Debit Card', 'Debit Card', 1, NULL, NULL, NULL, NULL),
        (49, 'TransactionStatus', 'Pending', 'Pending', 1, NULL, NULL, NULL, NULL),
        (50, 'TransactionStatus', 'Completed', 'Completed', 1, NULL, NULL, NULL, NULL),
        (51, 'TransactionStatus', 'Declined', 'Declined', 1, NULL, NULL, NULL, NULL),
        (52, 'ContractType', 'Permanent', 'Permanent', 1, NULL, NULL, NULL, NULL),
        (53, 'ContractType', 'Contractual', 'Contractual', 1, NULL, NULL, NULL, NULL),
        (54, 'ContractType', 'Other', 'Other', 1, NULL, NULL, NULL, NULL),
        (55, 'Frequency', 'Daily', 'Daily', 1, NULL, NULL, NULL, NULL),
        (56, 'Frequency', 'Monthly', 'Monthly', 1, NULL, NULL, NULL, NULL),
        (57, 'Frequency', 'Weekly', 'Weekly', 1, NULL, NULL, NULL, NULL),
        (58, 'CurrencySign', '$', 'Dollar Sign', 1, NULL, GETDATE(), NULL, NULL),
        (59, 'CurrencySign', '€', 'Euro Sign', 1, NULL, GETDATE(), NULL, NULL),
        (60, 'CurrencySign', '£', 'Pound Sign', 1, NULL, GETDATE(), NULL, NULL),
        (61, 'CurrencySign', '₨', 'Rupee Sign', 1, NULL, GETDATE(), NULL, NULL),
        (62, 'CurrencySign', '¥', 'Yen Sign', 1, NULL, GETDATE(), NULL, NULL),
        (63, 'CurrencySign', '₹', 'Indian Rupee Sign', 1, NULL, GETDATE(), NULL, NULL),
        (64, 'CurrencySign', '₿', 'Bitcoin Sign', 1, NULL, GETDATE(), NULL, NULL);

        SET IDENTITY_INSERT dbo.tblLookupItems OFF;



        /* ================================
           tblRole
           RoleLevel: Lower number = Higher authority
           1 = Super Admin (highest)
           6 = Staff (lowest)
        ================================ */
        SET IDENTITY_INSERT dbo.tblRole ON;

        INSERT INTO dbo.tblRole
        (Id, Name, Description, OrganizationId, IsActive, CreatedDate, RoleLevel)
        SELECT v.Id, v.Name, v.Description, NULL, 1, GETDATE(), v.RoleLevel
        FROM (VALUES
            (1,'Administrator','Full system access', 2),
            (2,'Manager','Management level access', 3),
            (3,'Supervisor','Supervisory level access', 4),
            (4,'Staff','Basic staff access', 6),
            (5,'Coordinator','Coordination level access', 5),
            (6,'Super Admin','Super administrator with highest privileges', 1)
        ) v(Id,Name,Description,RoleLevel)
        WHERE NOT EXISTS (SELECT 1 FROM dbo.tblRole r WHERE r.Id = v.Id);

        SET IDENTITY_INSERT dbo.tblRole OFF;

        /* ================================
           tblTimeZoneMapping
        ================================ */
        INSERT INTO dbo.tblTimeZoneMapping (DisplayName, SqlServerIdentifier, IsActive)
        SELECT v.DisplayName, v.SqlId, 1
        FROM (VALUES
            ('Pakistan Standard Time','Pakistan Standard Time'),
            ('Irish Standard Time','GMT Standard Time'),
            ('UTC','UTC'),
            ('Eastern Standard Time','Eastern Standard Time')
        ) v(DisplayName,SqlId)
        WHERE NOT EXISTS (
            SELECT 1 FROM dbo.tblTimeZoneMapping t
            WHERE t.DisplayName = v.DisplayName
        );




IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'SmokingStatus' AND [Name] = 'No Preference')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('SmokingStatus', 'No Preference', 'No smoking preference', 1, GETDATE());
END


-- Insert Age Range Items
IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'AgeRange' AND [Name] = '18-30')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('AgeRange', '18-30', 'Age range 18 to 30 years', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'AgeRange' AND [Name] = '31-45')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('AgeRange', '31-45', 'Age range 31 to 45 years', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'AgeRange' AND [Name] = '46-60')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('AgeRange', '46-60', 'Age range 46 to 60 years', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'AgeRange' AND [Name] = '60+')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('AgeRange', '60+', 'Age 60 years and above', 1, GETDATE());
END

-- Insert Experience Level Items
IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'Experience' AND [Name] = 'Entry Level (0-2 years)')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('Experience', 'Entry Level (0-2 years)', '0-2 years of experience', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'Experience' AND [Name] = 'Mid Level (3-5 years)')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('Experience', 'Mid Level (3-5 years)', '3-5 years of experience', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'Experience' AND [Name] = 'Senior Level (6-10 years)')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('Experience', 'Senior Level (6-10 years)', '6-10 years of experience', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'Experience' AND [Name] = 'Expert Level (10+ years)')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('Experience', 'Expert Level (10+ years)', '10 or more years of experience', 1, GETDATE());
END

-- Insert Pet Friendly Items
IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'PetFriendly' AND [Name] = 'Yes')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('PetFriendly', 'Yes', 'Comfortable working with pets', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'PetFriendly' AND [Name] = 'No')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('PetFriendly', 'No', 'Not comfortable working with pets', 1, GETDATE());
END

-- Insert Transportation Mode Items
IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'TransportationMode' AND [Name] = 'Own Vehicle')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('TransportationMode', 'Own Vehicle', 'Has own vehicle for transportation', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'TransportationMode' AND [Name] = 'Public Transport')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('TransportationMode', 'Public Transport', 'Uses public transportation', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'TransportationMode' AND [Name] = 'Bicycle')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('TransportationMode', 'Bicycle', 'Uses bicycle for transportation', 1, GETDATE());
END

-- Insert Certification Items
IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'Certification' AND [Name] = 'First Aid Certified')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('Certification', 'First Aid Certified', 'Has first aid certification', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'Certification' AND [Name] = 'CPR Certified')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('Certification', 'CPR Certified', 'Has CPR certification', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'Certification' AND [Name] = 'Licensed Professional')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('Certification', 'Licensed Professional', 'Has professional license', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'Certification' AND [Name] = 'Background Check Cleared')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('Certification', 'Background Check Cleared', 'Passed background verification', 1, GETDATE());
END
PRINT 'Lookup data inserted successfully.';
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'ComplaintStatus' AND [Name] = 'Submitted')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('ComplaintStatus', 'Submitted', 'Complaint has been submitted', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'ComplaintStatus' AND [Name] = 'Under Review')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('ComplaintStatus', 'Under Review', 'Complaint is being reviewed', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'ComplaintStatus' AND [Name] = 'In Progress')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('ComplaintStatus', 'In Progress', 'Action is being taken on the complaint', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'ComplaintStatus' AND [Name] = 'Resolved')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('ComplaintStatus', 'Resolved', 'Complaint has been resolved', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'ComplaintStatus' AND [Name] = 'Closed')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('ComplaintStatus', 'Closed', 'Complaint has been closed', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'ComplaintStatus' AND [Name] = 'Rejected')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('ComplaintStatus', 'Rejected', 'Complaint has been rejected', 1, GETDATE());
END

-- Insert Complaint Severity Lookup Items
IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'ComplaintSeverity' AND [Name] = 'Low')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('ComplaintSeverity', 'Low', 'Low severity complaint', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'ComplaintSeverity' AND [Name] = 'Medium')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('ComplaintSeverity', 'Medium', 'Medium severity complaint', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'ComplaintSeverity' AND [Name] = 'High')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('ComplaintSeverity', 'High', 'High severity complaint', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'ComplaintSeverity' AND [Name] = 'Critical')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('ComplaintSeverity', 'Critical', 'Critical severity complaint', 1, GETDATE());
END

-- Insert Complaint Category Lookup Items
IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'ComplaintCategory' AND [Name] = 'Unprofessional Behavior')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('ComplaintCategory', 'Unprofessional Behavior', 'Complaint about unprofessional conduct', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'ComplaintCategory' AND [Name] = 'Poor Service Quality')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('ComplaintCategory', 'Poor Service Quality', 'Complaint about service quality', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'ComplaintCategory' AND [Name] = 'Late Arrival')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('ComplaintCategory', 'Late Arrival', 'Complaint about late arrival', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'ComplaintCategory' AND [Name] = 'No Show')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('ComplaintCategory', 'No Show', 'Complaint about not showing up', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'ComplaintCategory' AND [Name] = 'Harassment')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('ComplaintCategory', 'Harassment', 'Complaint about harassment', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'ComplaintCategory' AND [Name] = 'Safety Concerns')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('ComplaintCategory', 'Safety Concerns', 'Complaint about safety issues', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'ComplaintCategory' AND [Name] = 'Contract Violation')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('ComplaintCategory', 'Contract Violation', 'Complaint about contract terms violation', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'ComplaintCategory' AND [Name] = 'Communication Issues')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('ComplaintCategory', 'Communication Issues', 'Complaint about poor communication', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'ComplaintCategory' AND [Name] = 'Other')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('ComplaintCategory', 'Other', 'Other type of complaint', 1, GETDATE());
END

PRINT 'Complaint lookup data inserted successfully!';
Go


--DECLARE @vRoleId int=6


--declare @p33 uniqueidentifier
--set @p33=null
--exec [User].[InsertUpdateUser] @pUserType=3,@pFirstName=N'Staff',@pSurName=N'Admin Staff',@pLastName=N'Admin',@pAlias=N''
--        ,@pUserName=N'',@pPhoneNo=N'034654545',@pMobileNo=N'54545454',@pPassportNo=N'5454545',@pIdentityNo=N'545454'
--		,@pEthnicityId=9,@pAge=0,@pBirthDate='2020-01-01 00:00:00',@pJoiningDate='2020-01-01 00:00:00',@pCountyId=0
--		,@pMaritalStatusId=16,@pEmail=N'test@test.com',@pAddressLine1=N'',@pAddressLine2=N'',@pAddressLine3=N''
--		,@pLatitude=0,@pLongitude=0,@pStateId=0,@pNationalityId=10,@pCountryId=0,@pGenderId=6,@pTitleId=11
--		,@pPasswordHash=N'',@pFranchiseId=null,@pAddressId=NULL,@pNotes=N'',@pId=NULL,@pOutId=@p33 output
--select @p33


--declare @p5 uniqueidentifier=null
--exec [dbo].[UpdateUserAuthenticationInfo] @pUserName=N'test',@pPassword=N'yLxCvky32Pfy2saEen+xeQ==',@pRoleId=@vRoleId
--         ,@pUserId=@p33,@pOutId=@p5 output
--select @p5

       --UPDATE tblUserRole 
       -- SET IsActive = 0, UpdatedDate = GETUTCDATE()
       -- WHERE UserId = @p33 AND IsActive = 1;
        
       -- -- Insert new role assignment
       -- INSERT INTO [dbo].[tblUserRole] 
       --     (Id, UserId, RoleId, IsActive, CreatedDate, CreatedBy)
       -- VALUES 
       --     (NEWID(), @p33, @vRoleId, 1, GETUTCDATE(), null);

----UserName : test
----Password :test1234