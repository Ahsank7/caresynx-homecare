CREATE   PROCEDURE [dbo].[sp_Seed_MasterData]
    @OrganizationId uniqueIdentifier
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        /* ================================
           tblServicesType
        ================================ */
        SET IDENTITY_INSERT dbo.tblServicesType ON;

        IF NOT EXISTS (SELECT 1 FROM dbo.tblServicesType WHERE Id = 1)
            INSERT dbo.tblServicesType (Id, Name, Description, IsActive, OrganizationId)
            VALUES (1, N'Day Care', N'Day Care', 1, @OrganizationId);

        IF NOT EXISTS (SELECT 1 FROM dbo.tblServicesType WHERE Id = 2)
            INSERT dbo.tblServicesType (Id, Name, Description, IsActive, OrganizationId)
            VALUES (2, N'Cleaning', N'Cleaning', 1, @OrganizationId);

        SET IDENTITY_INSERT dbo.tblServicesType OFF;

        /* ================================
           tblServices
        ================================ */
        SET IDENTITY_INSERT dbo.tblServices ON;

        IF NOT EXISTS (SELECT 1 FROM dbo.tblServices WHERE Id = 1)
            INSERT dbo.tblServices (Id, ServiceTypeId, Name, Description, Rate, IsActive)
            VALUES (1, 1, N'Child Day Care', NULL, 0, 1);

        IF NOT EXISTS (SELECT 1 FROM dbo.tblServices WHERE Id = 2)
            INSERT dbo.tblServices (Id, ServiceTypeId, Name, Description, Rate, IsActive)
            VALUES (2, 2, N'House Cleaning', N'House Cleaning', 0, 1);

        IF NOT EXISTS (SELECT 1 FROM dbo.tblServices WHERE Id = 3)
            INSERT dbo.tblServices (Id, ServiceTypeId, Name, Description, Rate, IsActive)
            VALUES (3, 2, N'Garden Cleaning', N'Garden Cleaning', 0, 1);

        SET IDENTITY_INSERT dbo.tblServices OFF;

      --  /* ================================
      --     tblLookups
      --  ================================ */
      --  SET IDENTITY_INSERT dbo.tblLookups ON;

      --  INSERT INTO dbo.tblLookups (Id, Name, Description, DisplayName, IsActive, IsVisible)
      --  SELECT v.Id, v.Name, v.Description, v.DisplayName, 1, 1
      --  FROM (VALUES
      --      (1,'AddressType','tblAddressType','Address Type'),
      --      (2,'ContactType','tblContactType','Contact Type'),
      --      (3,'Country','tblCountry','Country'),
      --      (4,'County','tblCounty','County'),
      --      (5,'Ethnicity','tblEthnicity','Ethnicity'),
      --      (6,'ExpenseType','tblExpenseType','Expense Type'),
      --      (7,'Gender','tblGender','Gender'),
      --      (8,'Languages','tblLanguages','Languages'),
      --      (9,'LeaveStatus','tblLeaveStatus','Leave Status'),
      --      (10,'LeaveType','tblLeaveType','Leave Type'),
      --      (11,'MaritalStatus','tblMaritalStatus','Marital Status'),
      --      (12,'Nationality','tblNationality','Nationality'),
      --      (13,'Role','tblRole','Role'),
      --      (14,'TaskStatus','tblTaskStatus','Task Status'),
      --      (15,'UserStatus','tblUserStatus','User Status'),
      --      (16,'UserType','tblUserType','User Type'),
      --      (17,'State','tblState','State'),
      --      (18,'Department','tblDepartment','Department'),
      --      (19,'Currency','tblCurrency','Currency'),
      --      (20,'DocumentType','tblDocumentType','Document Type'),
      --      (21,'Title','tblTitle','Title'),
      --      (22,'TimeZones','tblTimeZones','Time Zones'),
      --      (23,'CardType','tblCardType','Card Type'),
      --      (24,'TransactionType','tblTransactionType','Transaction Type'),
      --      (25,'TransactionStatus','tblTransactionStatus','Transaction Status'),
      --      (26,'Banks','tblBanks','Banks'),
      --      (27,'ContractType','tblContractType','Contract Type'),
      --      (28,'Frequency','tblFrequency','Frequency'),
      --      (29,'CurrencySign','CurrencySign','Currency Sign')
      --  ) v(Id,Name,Description,DisplayName)
      --  WHERE NOT EXISTS (
      --      SELECT 1 FROM dbo.tblLookups l WHERE l.Id = v.Id
      --  );

      --  SET IDENTITY_INSERT dbo.tblLookups OFF;

      --  /* ================================
      --     tblLookupItems (sample pattern)
      --  ================================ */
      --SET IDENTITY_INSERT dbo.tblLookupItems ON;

      --  INSERT INTO dbo.tblLookupItems
      --  (Id, LookupType, Name, [Description], IsActive, InsertedById, InsertedAt, UpdatedById, UpdatedAt)
      --  VALUES
      --  (1, 'AddressType', 'Primary', 'Primary', 1, NULL, NULL, NULL, NULL),
      --  (2, 'AddressType', 'Shipping', 'Shipping', 1, NULL, NULL, NULL, NULL),
      --  (3, 'AddressType', 'Billing', 'Billing', 1, NULL, NULL, NULL, NULL),
      --  (4, 'UserStatus', 'Temporary', 'Temporary', 1, NULL, NULL, NULL, NULL),
      --  (5, 'UserStatus', 'Primary', 'Primary', 1, NULL, NULL, NULL, NULL),
      --  (6, 'Gender', 'Male', 'Male', 1, NULL, NULL, NULL, NULL),
      --  (7, 'Gender', 'Female', 'Female', 1, NULL, NULL, NULL, NULL),
      --  (8, 'Language', 'English', 'English', 1, NULL, NULL, NULL, NULL),
      --  (9, 'Ethnicity', 'Punjabi', 'Punjabi', 1, NULL, NULL, NULL, NULL),
      --  (10, 'Nationality', 'Pakistani', 'Pakistani', 1, NULL, NULL, NULL, NULL),
      --  (11, 'Title', 'MR', 'MR', 1, NULL, NULL, NULL, NULL),
      --  (12, 'Title', 'MRS', 'MRS', 1, NULL, NULL, NULL, NULL),
      --  (13, 'County', 'Rawalpindi', 'Rawalpindi', 1, NULL, NULL, NULL, NULL),
      --  (14, 'State', 'Punjab', 'Punjab', 1, NULL, NULL, NULL, NULL),
      --  (15, 'Country', 'Pakistan', 'Pakistan', 1, NULL, NULL, NULL, NULL),
      --  (16, 'MaritalStatus', 'Married', 'Married', 1, NULL, NULL, NULL, NULL),
      --  (17, 'MaritalStatus', 'UnMarried', 'UnMarried', 1, NULL, NULL, NULL, NULL),
      --  (18, 'UserStatus', 'Active', 'Active', 1, NULL, NULL, NULL, NULL),
      --  (19, 'UserStatus', 'InActive', 'InActive', 1, NULL, NULL, NULL, NULL),
      --  (20, 'ContactType', 'Primary', 'ContactType', 1, NULL, NULL, NULL, NULL),
      --  (21, 'ExpenseType', 'Travel', 'Travel', 1, NULL, NULL, NULL, NULL),
      --  (22, 'Languages', 'English', 'English', 1, NULL, NULL, NULL, NULL),
      --  (23, 'Languages', 'Urdu', 'Urdu', 1, NULL, NULL, NULL, NULL),
      --  (24, 'LeaveType', 'Annual', 'Annual', 1, NULL, NULL, NULL, NULL),
      --  (25, 'LeaveType', 'Casual', 'Casual', 1, NULL, NULL, NULL, NULL),
      --  (26, 'LeaveType', 'Sick', 'Sick', 1, NULL, NULL, NULL, NULL),
      --  (27, 'LeaveStatus', 'Approved', 'Approved', 1, NULL, NULL, NULL, NULL),
      --  (28, 'LeaveStatus', 'Pending', 'Pending', 1, NULL, NULL, NULL, NULL),
      --  (31, 'Role', 'Staff', 'Staff', 1, NULL, NULL, NULL, NULL),
      --  (32, 'Role', 'Admin', 'Admin', 1, NULL, NULL, NULL, NULL),
      --  (33, 'TaskStatus', 'Scheduled', 'Scheduled', 1, NULL, NULL, NULL, NULL),
      --  (34, 'TaskStatus', 'Delayed', 'Delayed', 1, NULL, NULL, NULL, NULL),
      --  (35, 'TaskStatus', 'In-Progress', 'In-Progress', 1, NULL, NULL, NULL, NULL),
      --  (36, 'TaskStatus', 'Completed', 'Completed', 1, NULL, NULL, NULL, NULL),
      --  (37, 'TaskStatus', 'Cancelled', 'Cancelled', 1, NULL, NULL, NULL, NULL),
      --  (38, 'Department', 'HR', 'HR', 1, NULL, NULL, NULL, NULL),
      --  (39, 'Department', 'Other', 'Other', 1, NULL, NULL, NULL, NULL),
      --  (40, 'Currency', 'Dollar', 'Dollar', 1, NULL, NULL, NULL, NULL),
      --  (41, 'Currency', 'PKR', 'PKR', 1, NULL, NULL, NULL, NULL),
      --  (42, 'DocumentType', 'ID Card', 'ID Card', 1, NULL, NULL, NULL, NULL),
      --  (43, 'DocumentType', 'Educational', 'Educational', 1, NULL, NULL, NULL, NULL),
      --  (44, 'DocumentType', 'Other', 'Other', 1, NULL, NULL, NULL, NULL),
      --  (45, 'TimeZones', 'Pakistan Standard Time', 'Pakistan Standard Time', 1, NULL, NULL, NULL, NULL),
      --  (46, 'TimeZones', 'Irish Standard Time', 'Irish Standard Time', 1, NULL, NULL, NULL, NULL),
      --  (47, 'CardType', 'Credit Card', 'Credit Card', 1, NULL, NULL, NULL, NULL),
      --  (48, 'CardType', 'Debit Card', 'Debit Card', 1, NULL, NULL, NULL, NULL),
      --  (49, 'TransactionStatus', 'Pending', 'Pending', 1, NULL, NULL, NULL, NULL),
      --  (50, 'TransactionStatus', 'Completed', 'Completed', 1, NULL, NULL, NULL, NULL),
      --  (51, 'TransactionStatus', 'Declined', 'Declined', 1, NULL, NULL, NULL, NULL),
      --  (52, 'ContractType', 'Permanent', 'Permanent', 1, NULL, NULL, NULL, NULL),
      --  (53, 'ContractType', 'Contractual', 'Contractual', 1, NULL, NULL, NULL, NULL),
      --  (54, 'ContractType', 'Other', 'Other', 1, NULL, NULL, NULL, NULL),
      --  (55, 'Frequency', 'Daily', 'Daily', 1, NULL, NULL, NULL, NULL),
      --  (56, 'Frequency', 'Monthly', 'Monthly', 1, NULL, NULL, NULL, NULL),
      --  (57, 'Frequency', 'Weekly', 'Weekly', 1, NULL, NULL, NULL, NULL),
      --  (58, 'CurrencySign', '$', 'Dollar Sign', 1, NULL, GETDATE(), NULL, NULL),
      --  (59, 'CurrencySign', '€', 'Euro Sign', 1, NULL, GETDATE(), NULL, NULL),
      --  (60, 'CurrencySign', '£', 'Pound Sign', 1, NULL, GETDATE(), NULL, NULL),
      --  (61, 'CurrencySign', '₨', 'Rupee Sign', 1, NULL, GETDATE(), NULL, NULL),
      --  (62, 'CurrencySign', '¥', 'Yen Sign', 1, NULL, GETDATE(), NULL, NULL),
      --  (63, 'CurrencySign', '₹', 'Indian Rupee Sign', 1, NULL, GETDATE(), NULL, NULL),
      --  (64, 'CurrencySign', '₿', 'Bitcoin Sign', 1, NULL, GETDATE(), NULL, NULL);

      --  SET IDENTITY_INSERT dbo.tblLookupItems OFF;



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


		EXEC [dbo].[PopulateDefaultMenus] @OrganizationId

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH
END