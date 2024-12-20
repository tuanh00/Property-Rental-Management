USE [master]
GO
/****** Object:  Database [DbPropertyRental]    Script Date: 2024-11-27 5:01:14 AM ******/
CREATE DATABASE [DbPropertyRental]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'DbPropertyRental', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\DbPropertyRental.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'DbPropertyRental_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\DbPropertyRental_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [DbPropertyRental] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [DbPropertyRental].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [DbPropertyRental] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [DbPropertyRental] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [DbPropertyRental] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [DbPropertyRental] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [DbPropertyRental] SET ARITHABORT OFF 
GO
ALTER DATABASE [DbPropertyRental] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [DbPropertyRental] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [DbPropertyRental] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [DbPropertyRental] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [DbPropertyRental] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [DbPropertyRental] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [DbPropertyRental] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [DbPropertyRental] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [DbPropertyRental] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [DbPropertyRental] SET  DISABLE_BROKER 
GO
ALTER DATABASE [DbPropertyRental] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [DbPropertyRental] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [DbPropertyRental] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [DbPropertyRental] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [DbPropertyRental] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [DbPropertyRental] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [DbPropertyRental] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [DbPropertyRental] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [DbPropertyRental] SET  MULTI_USER 
GO
ALTER DATABASE [DbPropertyRental] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [DbPropertyRental] SET DB_CHAINING OFF 
GO
ALTER DATABASE [DbPropertyRental] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [DbPropertyRental] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [DbPropertyRental] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [DbPropertyRental] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'DbPropertyRental', N'ON'
GO
ALTER DATABASE [DbPropertyRental] SET QUERY_STORE = OFF
GO
USE [DbPropertyRental]
GO
/****** Object:  Table [dbo].[apartment]    Script Date: 2024-11-27 5:01:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[apartment](
	[apartmentId] [int] IDENTITY(1,1) NOT NULL,
	[apartmentNo] [int] NOT NULL,
	[nbRooms] [int] NOT NULL,
	[price] [money] NOT NULL,
	[status] [nvarchar](11) NOT NULL,
	[buildingId] [int] NOT NULL,
	[tenantId] [int] NULL,
 CONSTRAINT [PK_apartment] PRIMARY KEY CLUSTERED 
(
	[apartmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[appointment]    Script Date: 2024-11-27 5:01:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[appointment](
	[appointmentId] [int] IDENTITY(1,1) NOT NULL,
	[managerId] [int] NOT NULL,
	[tenantId] [int] NOT NULL,
	[appointmentDate] [datetime] NOT NULL,
	[description] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_appointment] PRIMARY KEY CLUSTERED 
(
	[appointmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[building]    Script Date: 2024-11-27 5:01:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[building](
	[buildingId] [int] IDENTITY(1,1) NOT NULL,
	[address] [nvarchar](50) NOT NULL,
	[city] [nvarchar](50) NOT NULL,
	[province] [nvarchar](50) NOT NULL,
	[postalCode] [nvarchar](10) NOT NULL,
	[ownerId] [int] NOT NULL,
	[managerId] [int] NOT NULL,
 CONSTRAINT [PK_building] PRIMARY KEY CLUSTERED 
(
	[buildingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[eventOwner]    Script Date: 2024-11-27 5:01:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[eventOwner](
	[eventId] [int] IDENTITY(1,1) NOT NULL,
	[managerId] [int] NOT NULL,
	[ownerId] [int] NOT NULL,
	[apartmentId] [int] NOT NULL,
	[eventDate] [datetime] NOT NULL,
	[description] [nvarchar](255) NOT NULL,
	[status] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_eventOwner] PRIMARY KEY CLUSTERED 
(
	[eventId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[manager]    Script Date: 2024-11-27 5:01:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[manager](
	[managerId] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](50) NOT NULL,
	[email] [nvarchar](50) NOT NULL,
	[password] [nvarchar](64) NOT NULL,
	[phoneNumber] [nvarchar](10) NOT NULL,
 CONSTRAINT [PK_manager] PRIMARY KEY CLUSTERED 
(
	[managerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[messageManager]    Script Date: 2024-11-27 5:01:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[messageManager](
	[messageId] [int] IDENTITY(1,1) NOT NULL,
	[managerId] [int] NOT NULL,
	[tenantId] [int] NOT NULL,
	[message] [nvarchar](100) NOT NULL,
	[responseMessage] [nvarchar](100) NULL,
 CONSTRAINT [PK_messageManager] PRIMARY KEY CLUSTERED 
(
	[messageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[messageOwner]    Script Date: 2024-11-27 5:01:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[messageOwner](
	[messageId] [int] IDENTITY(1,1) NOT NULL,
	[ownerId] [int] NOT NULL,
	[managerId] [int] NOT NULL,
	[message] [nvarchar](100) NOT NULL,
	[responseMessage] [nvarchar](100) NULL,
 CONSTRAINT [PK_messageOwner] PRIMARY KEY CLUSTERED 
(
	[messageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[owner]    Script Date: 2024-11-27 5:01:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[owner](
	[ownerId] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](50) NOT NULL,
	[email] [nvarchar](50) NOT NULL,
	[password] [nvarchar](64) NOT NULL,
	[phoneNumber] [nvarchar](10) NOT NULL,
 CONSTRAINT [PK_owner] PRIMARY KEY CLUSTERED 
(
	[ownerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tenant]    Script Date: 2024-11-27 5:01:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tenant](
	[tenantId] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](50) NOT NULL,
	[email] [nvarchar](50) NOT NULL,
	[password] [nvarchar](64) NOT NULL,
	[phoneNumber] [nvarchar](10) NOT NULL,
 CONSTRAINT [PK_tenant] PRIMARY KEY CLUSTERED 
(
	[tenantId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[apartment] ON 

INSERT [dbo].[apartment] ([apartmentId], [apartmentNo], [nbRooms], [price], [status], [buildingId], [tenantId]) VALUES (1, 16, 2, 789.0000, N'Available', 4, NULL)
INSERT [dbo].[apartment] ([apartmentId], [apartmentNo], [nbRooms], [price], [status], [buildingId], [tenantId]) VALUES (2, 2, 3, 1111.0000, N'Available', 5, 1)
SET IDENTITY_INSERT [dbo].[apartment] OFF
GO
SET IDENTITY_INSERT [dbo].[appointment] ON 

INSERT [dbo].[appointment] ([appointmentId], [managerId], [tenantId], [appointmentDate], [description]) VALUES (1, 1, 1, CAST(N'2024-05-21T00:00:00.000' AS DateTime), N'First meeting with the tenant.')
INSERT [dbo].[appointment] ([appointmentId], [managerId], [tenantId], [appointmentDate], [description]) VALUES (2, 1, 2, CAST(N'2024-05-25T00:00:00.000' AS DateTime), N'Schedule to review documents.')
SET IDENTITY_INSERT [dbo].[appointment] OFF
GO
SET IDENTITY_INSERT [dbo].[building] ON 

INSERT [dbo].[building] ([buildingId], [address], [city], [province], [postalCode], [ownerId], [managerId]) VALUES (4, N'123 Nguyen Trai', N'Hanoi', N'Hanoi', N'H3P 1Z6', 1, 1)
INSERT [dbo].[building] ([buildingId], [address], [city], [province], [postalCode], [ownerId], [managerId]) VALUES (5, N'456 Le Duan', N'Da Nang', N'Da Nang', N'H2Z 1R5', 1, 1)
INSERT [dbo].[building] ([buildingId], [address], [city], [province], [postalCode], [ownerId], [managerId]) VALUES (1004, N'789 Tran Hung Dao', N'Ho Chi Minh', N'Ho Chi Minh', N'H4P 1X4', 1, 1)
INSERT [dbo].[building] ([buildingId], [address], [city], [province], [postalCode], [ownerId], [managerId]) VALUES (1005, N'88 Kha Van Can', N'Ho Chi Minh', N'Ho Chi Minh', N'H8H 1P6', 1, 1)
SET IDENTITY_INSERT [dbo].[building] OFF
GO
SET IDENTITY_INSERT [dbo].[eventOwner] ON 

INSERT [dbo].[eventOwner] ([eventId], [managerId], [ownerId], [apartmentId], [eventDate], [description], [status]) VALUES (1, 1, 1, 1, CAST(N'2024-11-27T04:59:47.767' AS DateTime), N'Water leakage in apartment 16.', N'Pending')
INSERT [dbo].[eventOwner] ([eventId], [managerId], [ownerId], [apartmentId], [eventDate], [description], [status]) VALUES (2, 1, 1, 2, CAST(N'2024-11-27T04:59:47.767' AS DateTime), N'Broken heater in apartment 2.', N'Resolved')
SET IDENTITY_INSERT [dbo].[eventOwner] OFF
GO
SET IDENTITY_INSERT [dbo].[manager] ON 

INSERT [dbo].[manager] ([managerId], [name], [email], [password], [phoneNumber]) VALUES (1, N'Edna Mode', N'ednamode@gmail.com', N'ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f', N'4388888788')
SET IDENTITY_INSERT [dbo].[manager] OFF
GO
SET IDENTITY_INSERT [dbo].[messageManager] ON 

INSERT [dbo].[messageManager] ([messageId], [managerId], [tenantId], [message], [responseMessage]) VALUES (4, 1, 1, N'8AM Appointment. It would be great to get a confirmed date in the diary.', NULL)
INSERT [dbo].[messageManager] ([messageId], [managerId], [tenantId], [message], [responseMessage]) VALUES (1002, 1, 2, N'10AM Appointment. I appreciate rescheduling this appointment may cause you some disruption.', NULL)
SET IDENTITY_INSERT [dbo].[messageManager] OFF
GO
SET IDENTITY_INSERT [dbo].[messageOwner] ON 

INSERT [dbo].[messageOwner] ([messageId], [ownerId], [managerId], [message], [responseMessage]) VALUES (1, 1, 1, N'You can contact me at any point.', NULL)
INSERT [dbo].[messageOwner] ([messageId], [ownerId], [managerId], [message], [responseMessage]) VALUES (4, 1, 1, N'I will be happy to answer any questions.', NULL)
SET IDENTITY_INSERT [dbo].[messageOwner] OFF
GO
SET IDENTITY_INSERT [dbo].[owner] ON 

INSERT [dbo].[owner] ([ownerId], [name], [email], [password], [phoneNumber]) VALUES (1, N'Tyler Durden', N'tylerdurden@gmail.com', N'ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f', N'4387777896')
SET IDENTITY_INSERT [dbo].[owner] OFF
GO
SET IDENTITY_INSERT [dbo].[tenant] ON 

INSERT [dbo].[tenant] ([tenantId], [name], [email], [password], [phoneNumber]) VALUES (1, N'Michael Corleone', N'michaelcorleone@gmail.com', N'ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f', N'4383456777')
INSERT [dbo].[tenant] ([tenantId], [name], [email], [password], [phoneNumber]) VALUES (2, N'Forrest Gump', N'forrestgump@gmail.com', N'ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f', N'4381110055')
SET IDENTITY_INSERT [dbo].[tenant] OFF
GO
ALTER TABLE [dbo].[eventOwner] ADD  DEFAULT ('Pending') FOR [status]
GO
ALTER TABLE [dbo].[apartment]  WITH CHECK ADD  CONSTRAINT [FK_apartment_building] FOREIGN KEY([buildingId])
REFERENCES [dbo].[building] ([buildingId])
GO
ALTER TABLE [dbo].[apartment] CHECK CONSTRAINT [FK_apartment_building]
GO
ALTER TABLE [dbo].[apartment]  WITH CHECK ADD  CONSTRAINT [FK_apartment_tenant] FOREIGN KEY([tenantId])
REFERENCES [dbo].[tenant] ([tenantId])
GO
ALTER TABLE [dbo].[apartment] CHECK CONSTRAINT [FK_apartment_tenant]
GO
ALTER TABLE [dbo].[appointment]  WITH CHECK ADD  CONSTRAINT [FK_appointment_manager] FOREIGN KEY([managerId])
REFERENCES [dbo].[manager] ([managerId])
GO
ALTER TABLE [dbo].[appointment] CHECK CONSTRAINT [FK_appointment_manager]
GO
ALTER TABLE [dbo].[appointment]  WITH CHECK ADD  CONSTRAINT [FK_appointment_tenant] FOREIGN KEY([tenantId])
REFERENCES [dbo].[tenant] ([tenantId])
GO
ALTER TABLE [dbo].[appointment] CHECK CONSTRAINT [FK_appointment_tenant]
GO
ALTER TABLE [dbo].[building]  WITH CHECK ADD  CONSTRAINT [FK_building_manager] FOREIGN KEY([managerId])
REFERENCES [dbo].[manager] ([managerId])
GO
ALTER TABLE [dbo].[building] CHECK CONSTRAINT [FK_building_manager]
GO
ALTER TABLE [dbo].[building]  WITH CHECK ADD  CONSTRAINT [FK_building_owner] FOREIGN KEY([ownerId])
REFERENCES [dbo].[owner] ([ownerId])
GO
ALTER TABLE [dbo].[building] CHECK CONSTRAINT [FK_building_owner]
GO
ALTER TABLE [dbo].[eventOwner]  WITH CHECK ADD  CONSTRAINT [FK_eventOwner_apartment] FOREIGN KEY([apartmentId])
REFERENCES [dbo].[apartment] ([apartmentId])
GO
ALTER TABLE [dbo].[eventOwner] CHECK CONSTRAINT [FK_eventOwner_apartment]
GO
ALTER TABLE [dbo].[eventOwner]  WITH CHECK ADD  CONSTRAINT [FK_eventOwner_manager] FOREIGN KEY([managerId])
REFERENCES [dbo].[manager] ([managerId])
GO
ALTER TABLE [dbo].[eventOwner] CHECK CONSTRAINT [FK_eventOwner_manager]
GO
ALTER TABLE [dbo].[eventOwner]  WITH CHECK ADD  CONSTRAINT [FK_eventOwner_owner] FOREIGN KEY([ownerId])
REFERENCES [dbo].[owner] ([ownerId])
GO
ALTER TABLE [dbo].[eventOwner] CHECK CONSTRAINT [FK_eventOwner_owner]
GO
ALTER TABLE [dbo].[messageManager]  WITH CHECK ADD  CONSTRAINT [FK_messageManager_manager] FOREIGN KEY([managerId])
REFERENCES [dbo].[manager] ([managerId])
GO
ALTER TABLE [dbo].[messageManager] CHECK CONSTRAINT [FK_messageManager_manager]
GO
ALTER TABLE [dbo].[messageManager]  WITH CHECK ADD  CONSTRAINT [FK_messageManager_tenant] FOREIGN KEY([tenantId])
REFERENCES [dbo].[tenant] ([tenantId])
GO
ALTER TABLE [dbo].[messageManager] CHECK CONSTRAINT [FK_messageManager_tenant]
GO
ALTER TABLE [dbo].[messageOwner]  WITH CHECK ADD  CONSTRAINT [FK_messageOwner_manager] FOREIGN KEY([managerId])
REFERENCES [dbo].[manager] ([managerId])
GO
ALTER TABLE [dbo].[messageOwner] CHECK CONSTRAINT [FK_messageOwner_manager]
GO
ALTER TABLE [dbo].[messageOwner]  WITH CHECK ADD  CONSTRAINT [FK_messageOwner_owner] FOREIGN KEY([ownerId])
REFERENCES [dbo].[owner] ([ownerId])
GO
ALTER TABLE [dbo].[messageOwner] CHECK CONSTRAINT [FK_messageOwner_owner]
GO
USE [master]
GO
ALTER DATABASE [DbPropertyRental] SET  READ_WRITE 
GO
