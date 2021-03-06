ALTER TABLE Rules
ADD UserId NVARCHAR(250) NULL
GO

UPDATE Rules SET UserId = (SELECT TOP 1 UserId FROM Meals)

ALTER TABLE Rules
ALTER COLUMN UserId NVARCHAR(250) NOT NULL

CREATE INDEX IX_Meals_UserId
ON Meals (UserId)

CREATE INDEX IX_Rules_UserId
ON Rules (UserId)