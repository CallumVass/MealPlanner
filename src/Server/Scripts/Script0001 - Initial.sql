CREATE TABLE [dbo].[MealRules](
	[MealId] [uniqueidentifier] NOT NULL,
	[RuleId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_MealRules] PRIMARY KEY CLUSTERED
(
	[MealId] ASC,
	[RuleId] ASC
))
GO
CREATE TABLE [dbo].[Meals](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [nvarchar](250) NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
 CONSTRAINT [PK_Meals] PRIMARY KEY CLUSTERED
(
	[Id] ASC
))
GO
CREATE TABLE [dbo].[RuleDays](
	[DayOfWeek] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_RuleDays] PRIMARY KEY CLUSTERED
(
	[DayOfWeek] ASC
))
GO
CREATE TABLE [dbo].[RuleRuleDays](
	[RuleId] [uniqueidentifier] NOT NULL,
	[DayOfWeek] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_RuleRuleDays] PRIMARY KEY CLUSTERED
(
	[RuleId] ASC,
	[DayOfWeek] ASC
))
GO
CREATE TABLE [dbo].[Rules](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
 CONSTRAINT [PK_Rules] PRIMARY KEY CLUSTERED
(
	[Id] ASC
))
GO
ALTER TABLE [dbo].[Meals] ADD  CONSTRAINT [DF_Meals_Id]  DEFAULT (newsequentialid()) FOR [Id]
GO
ALTER TABLE [dbo].[Rules] ADD  CONSTRAINT [DF_Rules_Id]  DEFAULT (newsequentialid()) FOR [Id]
GO
ALTER TABLE [dbo].[MealRules]  WITH CHECK ADD  CONSTRAINT [FK_MealRules_Meals] FOREIGN KEY([MealId])
REFERENCES [dbo].[Meals] ([Id])
GO
ALTER TABLE [dbo].[MealRules] CHECK CONSTRAINT [FK_MealRules_Meals]
GO
ALTER TABLE [dbo].[MealRules]  WITH CHECK ADD  CONSTRAINT [FK_MealRules_Rules] FOREIGN KEY([RuleId])
REFERENCES [dbo].[Rules] ([Id])
GO
ALTER TABLE [dbo].[MealRules] CHECK CONSTRAINT [FK_MealRules_Rules]
GO
ALTER TABLE [dbo].[RuleRuleDays]  WITH CHECK ADD  CONSTRAINT [FK_RuleRuleDays_RuleDays] FOREIGN KEY([DayOfWeek])
REFERENCES [dbo].[RuleDays] ([DayOfWeek])
GO
ALTER TABLE [dbo].[RuleRuleDays] CHECK CONSTRAINT [FK_RuleRuleDays_RuleDays]
GO
ALTER TABLE [dbo].[RuleRuleDays]  WITH CHECK ADD  CONSTRAINT [FK_RuleRuleDays_Rules] FOREIGN KEY([RuleId])
REFERENCES [dbo].[Rules] ([Id])
GO
ALTER TABLE [dbo].[RuleRuleDays] CHECK CONSTRAINT [FK_RuleRuleDays_Rules]
GO
