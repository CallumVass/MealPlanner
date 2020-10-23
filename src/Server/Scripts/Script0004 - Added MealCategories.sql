CREATE TABLE [dbo].[MealCategories](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [nvarchar](250) NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
 CONSTRAINT [PK_MealCategories] PRIMARY KEY CLUSTERED
(
	[Id] ASC
))
GO

ALTER TABLE [dbo].[MealCategories] ADD  CONSTRAINT [DF_MealCategories_Id]  DEFAULT (newsequentialid()) FOR [Id]
GO

ALTER TABLE [dbo].[Meals] ADD CategoryId [uniqueidentifier] NULL
GO

ALTER TABLE [dbo].[Meals]  WITH CHECK ADD  CONSTRAINT [FK_Meals_MealCategorys] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[MealCategories] ([Id])
GO

CREATE UNIQUE NONCLUSTERED INDEX IX_UserId_Name_MealCategories
   ON [dbo].[MealCategories] ([UserId], [Name])
GO