-- =============================================
-- Author:		Scheduler System
-- Create date: 2026-01-21
-- Description:	Get user information by UserNo
-- =============================================
CREATE PROCEDURE [User].[uspGetUserByUserNo]
    @pUserNo NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        U.Id AS UserId,
        U.FirstName,
        U.SurName,
        U.LastName,
        U.Alias,
        U.Age,
        U.Gender AS GenderId,
        U.MaritalStatus AS MaritalStatusId,
        U.Title AS TitleId,
        U.Ethnicity AS EthnicityId,
        U.BirthDate,
        U.JoiningDate,
        U.PassportNo,
        U.IdentityNo,
        U.MobileNo,
        U.PhoneNo,
        U.Email,
        U.Status AS StatusId,
        U.CreatedDate,
        U.UpdatedDate,
        U.CreatedBy,
        U.UpdatedBy,
        U.IsActive,
        U.UserType,
        U.NationalityId,
        U.FranchiseId,
        U.UserNo,
        U.Notes,
        U.UserName,
        U.Password,
        U.RoleId,
        U.ProfileImagePath
    FROM [dbo].[tblUser] U
    WHERE U.UserNo = @pUserNo
        AND U.IsActive = 1;
END

