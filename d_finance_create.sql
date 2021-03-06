USE [d_finance]
GO
/****** Object:  Table [dbo].[AnnualBalance]    Script Date: 2020-4-24 13:58:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AnnualBalance](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ProjectID] [int] NOT NULL,
	[Year] [int] NOT NULL,
	[BalanceCategory] [int] NOT NULL,
	[Balance] [money] NOT NULL,
 CONSTRAINT [PK_AnnualBalance] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Organization]    Script Date: 2020-4-24 13:58:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Organization](
	[OrganizationID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Organization] PRIMARY KEY CLUSTERED 
(
	[OrganizationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Perform]    Script Date: 2020-4-24 13:58:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Perform](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Year] [int] NOT NULL,
	[Month] [int] NOT NULL,
	[ProjectID] [int] NOT NULL,
	[Incoming] [money] NOT NULL,
	[Settlement] [money] NOT NULL,
	[Retrieve] [money] NOT NULL,
 CONSTRAINT [PK_Perform] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PerformTerms]    Script Date: 2020-4-24 13:58:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PerformTerms](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[PerformID] [int] NOT NULL,
	[TermsID] [int] NOT NULL,
	[Comments] [nvarchar](max) NULL,
 CONSTRAINT [PK_PerformTerms] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Plan]    Script Date: 2020-4-24 13:58:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Plan](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Year] [int] NOT NULL,
	[Month] [int] NOT NULL,
	[ProjectID] [int] NOT NULL,
	[Incoming] [money] NOT NULL,
	[Settlement] [money] NOT NULL,
	[Retrieve] [money] NOT NULL,
 CONSTRAINT [PK_Plan] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PlanTerms]    Script Date: 2020-4-24 13:58:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PlanTerms](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[PlanID] [int] NOT NULL,
	[TermsID] [int] NOT NULL,
	[Comments] [nvarchar](max) NULL,
 CONSTRAINT [PK_PlanTerms] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Project]    Script Date: 2020-4-24 13:58:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Project](
	[ProjectID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[ShortName] [nvarchar](20) NOT NULL,
	[ContractCode] [nvarchar](200) NOT NULL,
	[ContractAmount] [money] NOT NULL,
	[PM] [nvarchar](20) NOT NULL,
	[OrganizationID] [int] NOT NULL,
	[StartDate] [date] NULL,
	[EndDate] [date] NULL,
	[Status] [int] NOT NULL,
	[HasProblem] [bit] NOT NULL,
	[ActiveStatus] [bit] NOT NULL,
	[CloseDate] [date] NULL,
 CONSTRAINT [PK_Project] PRIMARY KEY CLUSTERED 
(
	[ProjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Terms]    Script Date: 2020-4-24 13:58:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Terms](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ProjectID] [int] NOT NULL,
	[TermsCategory] [int] NOT NULL,
	[TargetAmount] [money] NULL,
	[TargetDate] [date] NULL,
	[Provision] [nvarchar](max) NULL,
	[Remarks] [nvarchar](max) NULL,
 CONSTRAINT [PK_Terms] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 2020-4-24 13:58:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[Login] [nvarchar](50) NOT NULL,
	[Password] [nvarchar](200) NULL,
	[UserName] [nvarchar](50) NOT NULL,
	[Role] [int] NOT NULL,
	[ActiveStatus] [bit] NOT NULL,
	[OrganizationId] [int] NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AnnualBalance]  WITH CHECK ADD  CONSTRAINT [FK_AB_Proj] FOREIGN KEY([ProjectID])
REFERENCES [dbo].[Project] ([ProjectID])
GO
ALTER TABLE [dbo].[AnnualBalance] CHECK CONSTRAINT [FK_AB_Proj]
GO
ALTER TABLE [dbo].[Perform]  WITH CHECK ADD  CONSTRAINT [FK_Perform_Proj] FOREIGN KEY([ProjectID])
REFERENCES [dbo].[Project] ([ProjectID])
GO
ALTER TABLE [dbo].[Perform] CHECK CONSTRAINT [FK_Perform_Proj]
GO
ALTER TABLE [dbo].[PerformTerms]  WITH CHECK ADD  CONSTRAINT [FK_PerformTerms_Perform] FOREIGN KEY([PerformID])
REFERENCES [dbo].[Perform] ([ID])
GO
ALTER TABLE [dbo].[PerformTerms] CHECK CONSTRAINT [FK_PerformTerms_Perform]
GO
ALTER TABLE [dbo].[PerformTerms]  WITH CHECK ADD  CONSTRAINT [FK_PerformTerms_Terms] FOREIGN KEY([TermsID])
REFERENCES [dbo].[Terms] ([ID])
GO
ALTER TABLE [dbo].[PerformTerms] CHECK CONSTRAINT [FK_PerformTerms_Terms]
GO
ALTER TABLE [dbo].[Plan]  WITH CHECK ADD  CONSTRAINT [FK_Plan_Proj] FOREIGN KEY([ProjectID])
REFERENCES [dbo].[Project] ([ProjectID])
GO
ALTER TABLE [dbo].[Plan] CHECK CONSTRAINT [FK_Plan_Proj]
GO
ALTER TABLE [dbo].[PlanTerms]  WITH CHECK ADD  CONSTRAINT [FK_PlanTerms_Plan] FOREIGN KEY([PlanID])
REFERENCES [dbo].[Plan] ([ID])
GO
ALTER TABLE [dbo].[PlanTerms] CHECK CONSTRAINT [FK_PlanTerms_Plan]
GO
ALTER TABLE [dbo].[PlanTerms]  WITH CHECK ADD  CONSTRAINT [FK_PlanTerms_Terms] FOREIGN KEY([TermsID])
REFERENCES [dbo].[Terms] ([ID])
GO
ALTER TABLE [dbo].[PlanTerms] CHECK CONSTRAINT [FK_PlanTerms_Terms]
GO
ALTER TABLE [dbo].[Project]  WITH CHECK ADD  CONSTRAINT [FK_Proj_Org] FOREIGN KEY([OrganizationID])
REFERENCES [dbo].[Organization] ([OrganizationID])
GO
ALTER TABLE [dbo].[Project] CHECK CONSTRAINT [FK_Proj_Org]
GO
ALTER TABLE [dbo].[Terms]  WITH CHECK ADD  CONSTRAINT [FK_Terms_Proj] FOREIGN KEY([ProjectID])
REFERENCES [dbo].[Project] ([ProjectID])
GO
ALTER TABLE [dbo].[Terms] CHECK CONSTRAINT [FK_Terms_Proj]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_Org] FOREIGN KEY([OrganizationId])
REFERENCES [dbo].[Organization] ([OrganizationID])
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_Users_Org]
GO
