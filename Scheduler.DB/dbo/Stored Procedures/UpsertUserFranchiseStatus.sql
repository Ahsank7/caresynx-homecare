CREATE     PROCEDURE dbo.UpsertUserFranchiseStatus
(
    @UserId        UNIQUEIDENTIFIER,
    @FranchiseId   UNIQUEIDENTIFIER,
    @IsActive      BIT
)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF EXISTS (
            SELECT 1
            FROM dbo.tbUserFranchise
            WHERE UserId = @UserId
              AND FranchiseId = @FranchiseId
        )
        BEGIN
            -- UPDATE only tbUserFranchise
            UPDATE dbo.tbUserFranchise
            SET IsActive = @IsActive
            WHERE UserId = @UserId
              AND FranchiseId = @FranchiseId;
        END
        ELSE
        BEGIN
            -- INSERT new row
            INSERT INTO dbo.tbUserFranchise
            (UserId, FranchiseId, IsActive)
            VALUES
            (@UserId, @FranchiseId, @IsActive);
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH
END