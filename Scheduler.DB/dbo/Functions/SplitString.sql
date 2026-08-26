CREATE FUNCTION [dbo].[SplitString]
(
    @String NVARCHAR(MAX),
    @Delimiter CHAR(1)
)
RETURNS TABLE
AS
RETURN (
    SELECT Value
    FROM STRING_SPLIT(@String, @Delimiter)
);