USE [master]
GO
/****** Object:  Database [RestaurantAppDB]    Script Date: 23/05/2025 06:22:24 ******/
CREATE DATABASE [RestaurantAppDB]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'RestaurantAppDB', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\RestaurantAppDB.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'RestaurantAppDB_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\RestaurantAppDB_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [RestaurantAppDB] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [RestaurantAppDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [RestaurantAppDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [RestaurantAppDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [RestaurantAppDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [RestaurantAppDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [RestaurantAppDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [RestaurantAppDB] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [RestaurantAppDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [RestaurantAppDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [RestaurantAppDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [RestaurantAppDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [RestaurantAppDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [RestaurantAppDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [RestaurantAppDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [RestaurantAppDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [RestaurantAppDB] SET  DISABLE_BROKER 
GO
ALTER DATABASE [RestaurantAppDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [RestaurantAppDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [RestaurantAppDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [RestaurantAppDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [RestaurantAppDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [RestaurantAppDB] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [RestaurantAppDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [RestaurantAppDB] SET RECOVERY FULL 
GO
ALTER DATABASE [RestaurantAppDB] SET  MULTI_USER 
GO
ALTER DATABASE [RestaurantAppDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [RestaurantAppDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [RestaurantAppDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [RestaurantAppDB] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [RestaurantAppDB] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [RestaurantAppDB] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'RestaurantAppDB', N'ON'
GO
ALTER DATABASE [RestaurantAppDB] SET QUERY_STORE = ON
GO
ALTER DATABASE [RestaurantAppDB] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [RestaurantAppDB]
GO
/****** Object:  UserDefinedTableType [dbo].[OrderItemTableType]    Script Date: 23/05/2025 06:22:25 ******/
CREATE TYPE [dbo].[OrderItemTableType] AS TABLE(
	[DishID] [int] NULL,
	[Quantity] [int] NULL,
	[UnitPrice] [decimal](10, 2) NULL
)
GO
/****** Object:  Table [dbo].[Allergens]    Script Date: 23/05/2025 06:22:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Allergens](
	[AllergenID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[AllergenID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Categories]    Script Date: 23/05/2025 06:22:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Categories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DishAllergens]    Script Date: 23/05/2025 06:22:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DishAllergens](
	[DishID] [int] NOT NULL,
	[AllergenID] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[DishID] ASC,
	[AllergenID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Dishes]    Script Date: 23/05/2025 06:22:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Dishes](
	[DishID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](255) NULL,
	[Price] [decimal](10, 2) NOT NULL,
	[Available] [bit] NULL,
	[QuantityPerPortion] [varchar](20) NULL,
	[TotalQuantity] [varchar](20) NULL,
	[CategoryId] [int] NULL,
	[IsPartOfMenu] [bit] NOT NULL,
	[ImagePath] [varchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[DishID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MenuItems]    Script Date: 23/05/2025 06:22:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MenuItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MenuId] [int] NOT NULL,
	[DishId] [int] NOT NULL,
	[QuantityPerMenuPortion] [decimal](10, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Menus]    Script Date: 23/05/2025 06:22:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Menus](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[CategoryId] [int] NOT NULL,
	[Description] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderItems]    Script Date: 23/05/2025 06:22:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderItems](
	[OrderItemID] [int] IDENTITY(1,1) NOT NULL,
	[OrderID] [int] NOT NULL,
	[DishID] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[UnitPrice] [decimal](10, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[OrderItemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Orders]    Script Date: 23/05/2025 06:22:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Orders](
	[OrderID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NOT NULL,
	[OrderDate] [datetime] NULL,
	[Status] [nvarchar](20) NOT NULL,
	[TotalAmount] [decimal](10, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[OrderID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 23/05/2025 06:22:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserID] [int] IDENTITY(1,1) NOT NULL,
	[Username] [nvarchar](50) NOT NULL,
	[PasswordHash] [nvarchar](255) NOT NULL,
	[FullName] [nvarchar](100) NULL,
	[Email] [nvarchar](100) NULL,
	[Role] [nvarchar](20) NOT NULL,
	[CreatedAt] [datetime] NULL,
	[PhoneNumber] [nvarchar](20) NULL,
	[Address] [nvarchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Dishes] ADD  DEFAULT ((1)) FOR [Available]
GO
ALTER TABLE [dbo].[Dishes] ADD  DEFAULT ((0)) FOR [IsPartOfMenu]
GO
ALTER TABLE [dbo].[Orders] ADD  DEFAULT (getdate()) FOR [OrderDate]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[DishAllergens]  WITH CHECK ADD FOREIGN KEY([AllergenID])
REFERENCES [dbo].[Allergens] ([AllergenID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DishAllergens]  WITH CHECK ADD FOREIGN KEY([DishID])
REFERENCES [dbo].[Dishes] ([DishID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Dishes]  WITH CHECK ADD  CONSTRAINT [FK_Dishes_Categories] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Categories] ([Id])
GO
ALTER TABLE [dbo].[Dishes] CHECK CONSTRAINT [FK_Dishes_Categories]
GO
ALTER TABLE [dbo].[MenuItems]  WITH CHECK ADD FOREIGN KEY([DishId])
REFERENCES [dbo].[Dishes] ([DishID])
GO
ALTER TABLE [dbo].[MenuItems]  WITH CHECK ADD FOREIGN KEY([MenuId])
REFERENCES [dbo].[Menus] ([Id])
GO
ALTER TABLE [dbo].[Menus]  WITH CHECK ADD FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Categories] ([Id])
GO
ALTER TABLE [dbo].[OrderItems]  WITH CHECK ADD FOREIGN KEY([DishID])
REFERENCES [dbo].[Dishes] ([DishID])
GO
ALTER TABLE [dbo].[OrderItems]  WITH CHECK ADD FOREIGN KEY([OrderID])
REFERENCES [dbo].[Orders] ([OrderID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[Dishes]  WITH CHECK ADD CHECK  (([Price]>=(0)))
GO
ALTER TABLE [dbo].[OrderItems]  WITH CHECK ADD CHECK  (([Quantity]>(0)))
GO
ALTER TABLE [dbo].[OrderItems]  WITH CHECK ADD CHECK  (([UnitPrice]>=(0)))
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD CHECK  (([TotalAmount]>=(0)))
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [CK_Orders_Status] CHECK  (([Status]='Canceled' OR [Status]='Delivered' OR [Status]='Out for delivery' OR [Status]='In preparation' OR [Status]='Registered'))
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [CK_Orders_Status]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD CHECK  (([Role]='Admin' OR [Role]='Staff' OR [Role]='Customer'))
GO
/****** Object:  StoredProcedure [dbo].[CancelOrder]    Script Date: 23/05/2025 06:22:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CancelOrder]  
    @OrderID INT  
AS  
BEGIN  
    SET NOCOUNT ON;  
  
    IF NOT EXISTS (  
        SELECT 1 FROM Orders WHERE OrderID = @OrderID AND UPPER(LTRIM(RTRIM(Status))) = 'REGISTERED'  
    )  
    BEGIN  
        RAISERROR('Only registered orders can be canceled.', 16, 1);  
        RETURN;  
    END  

    -- Obține lista cu DishID și Quantity  
    DECLARE @Items TABLE (DishID INT, Quantity INT);  

    INSERT INTO @Items (DishID, Quantity)  
    SELECT DishID, Quantity  
    FROM OrderItems  
    WHERE OrderID = @OrderID;  

    -- Pentru fiecare dish, apelează procedura care crește ingredientele  
    DECLARE @DishID INT, @Quantity INT;  
    DECLARE item_cursor CURSOR FOR  
        SELECT DishID, Quantity FROM @Items;  

    OPEN item_cursor;  
    FETCH NEXT FROM item_cursor INTO @DishID, @Quantity;  

    WHILE @@FETCH_STATUS = 0  
    BEGIN  
        EXEC CresteCantitatiIngredient @DishID, @Quantity;  
        FETCH NEXT FROM item_cursor INTO @DishID, @Quantity;  
    END  

    CLOSE item_cursor;  
    DEALLOCATE item_cursor;  

    -- Actualizează statusul comenzii  
    UPDATE Orders  
    SET Status = 'Canceled'  
    WHERE OrderID = @OrderID;  
END  
GO
/****** Object:  StoredProcedure [dbo].[CreateDish]    Script Date: 23/05/2025 06:22:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CreateDish]
    @Name NVARCHAR(100),
    @Price DECIMAL(10,2),
    @QuantityPerPortion VARCHAR(20) = NULL,
    @Description NVARCHAR(255) = NULL,
    @Available BIT = 1,
    @TotalQuantity VARCHAR(20) = NULL,
    @CategoryId INT = NULL,
    @IsPartOfMenu BIT = 0,
    @AllergenIDs NVARCHAR(MAX) = NULL
AS
BEGIN
    BEGIN TRANSACTION
    BEGIN TRY
        -- Insert the dish with all required fields
        INSERT INTO Dishes (
            Name, 
            Price, 
            IsPartOfMenu,
            QuantityPerPortion,
            Description,
            Available,
            TotalQuantity,
            CategoryId
        )
        VALUES (
            @Name,
            @Price,
            @IsPartOfMenu,
            @QuantityPerPortion,
            @Description,
            @Available,
            @TotalQuantity,
            @CategoryId
        )
        
        DECLARE @DishID INT = SCOPE_IDENTITY()
        
        -- Insert allergens if provided
        IF @AllergenIDs IS NOT NULL
        BEGIN
            CREATE TABLE #TempAllergens (AllergenID INT)

            INSERT INTO #TempAllergens
            SELECT value FROM STRING_SPLIT(@AllergenIDs, ',')

            INSERT INTO DishAllergens (DishID, AllergenID)
            SELECT @DishID, AllergenID FROM #TempAllergens

            DROP TABLE #TempAllergens
        END
        
        COMMIT TRANSACTION
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION
        THROW
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[CresteCantitatiIngredient]    Script Date: 23/05/2025 06:22:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CresteCantitatiIngredient]  
    @DishID INT,  
    @Multiplier INT  
AS  
BEGIN  
    SET NOCOUNT ON;  
  
    DECLARE @QuantityPerPortion NVARCHAR(50);  
    DECLARE @TotalQuantity NVARCHAR(50);  
  
    SELECT   
        @QuantityPerPortion = QuantityPerPortion,  
        @TotalQuantity = TotalQuantity  
    FROM Dishes  
    WHERE DishID = @DishID;  
  
    IF @QuantityPerPortion IS NOT NULL AND @TotalQuantity IS NOT NULL  
    BEGIN  
        -- Extragem valoarea numerică și unitatea din QuantityPerPortion  
        DECLARE @Value FLOAT = TRY_CAST(LEFT(@QuantityPerPortion, PATINDEX('%[^0-9.]%', @QuantityPerPortion + 'X') - 1) AS FLOAT);  
        DECLARE @Unit NVARCHAR(10) = LTRIM(RIGHT(@QuantityPerPortion, LEN(@QuantityPerPortion) - LEN(CAST(@Value AS NVARCHAR))));  
  
        DECLARE @ToAdd FLOAT = @Value * @Multiplier;  
  
        -- Extragem valoarea numerică și unitatea din TotalQuantity  
        DECLARE @TotalValue FLOAT = TRY_CAST(LEFT(@TotalQuantity, PATINDEX('%[^0-9.]%', @TotalQuantity + 'X') - 1) AS FLOAT);  
        DECLARE @TotalUnit NVARCHAR(10) = LTRIM(RIGHT(@TotalQuantity, LEN(@TotalQuantity) - LEN(CAST(@TotalValue AS NVARCHAR))));  
  
        IF @Unit = @TotalUnit  
        BEGIN  
            DECLARE @NewTotal FLOAT = @TotalValue + @ToAdd;  
            DECLARE @UpdatedTotal NVARCHAR(50) = CONCAT(@NewTotal, @Unit);  
  
            UPDATE Dishes  
            SET TotalQuantity = @UpdatedTotal  
            WHERE DishID = @DishID;  
        END  
    END  
END
GO
/****** Object:  StoredProcedure [dbo].[DeleteDish]    Script Date: 23/05/2025 06:22:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[DeleteDish]
    @DishID INT
AS
BEGIN
    BEGIN TRANSACTION
    BEGIN TRY
        -- Pasul 1: Ștergere explicită a alergenilor asociați
        DELETE FROM DishAllergens WHERE DishID = @DishID;

        -- Pasul 2: Ștergere fel de mâncare
        DELETE FROM Dishes WHERE DishID = @DishID;

        COMMIT TRANSACTION
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;

        -- Propagarea erorii pentru debugging
        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[GetAllAllergens]    Script Date: 23/05/2025 06:22:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- Get all available allergens
CREATE PROCEDURE [dbo].[GetAllAllergens]
AS
BEGIN
    SELECT 
        AllergenID,
        Name AS AllergenName
    FROM Allergens
    ORDER BY Name
END
GO
/****** Object:  StoredProcedure [dbo].[GetAllCategories]    Script Date: 23/05/2025 06:22:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetAllCategories]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Name
    FROM Categories;
END
GO
/****** Object:  StoredProcedure [dbo].[GetAllDishesWithDetails]    Script Date: 23/05/2025 06:22:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetAllDishesWithDetails]  
AS  
BEGIN  
    -- Get all dish information, including ImagePath  
    SELECT   
        d.DishID,  
        d.Name,  
        d.QuantityPerPortion,  
        d.Description,  
        d.Price,  
        d.Available,  
        d.TotalQuantity,  
        d.CategoryId,  
        d.IsPartOfMenu,
        d.ImagePath  -- added column  
    FROM Dishes d  
  
    -- Get allergens for each dish  
    SELECT   
        da.DishID,  
        a.AllergenID,  
        a.Name AS AllergenName  
    FROM DishAllergens da  
    JOIN Allergens a ON da.AllergenID = a.AllergenID  
END  
GO
/****** Object:  StoredProcedure [dbo].[GetAllDishesWithStock]    Script Date: 23/05/2025 06:22:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetAllDishesWithStock]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        d.DishID,
        d.Name,
        d.Description,
        d.Price,
        d.Available,
        d.QuantityPerPortion,
        d.TotalQuantity,
        c.Name AS CategoryName,
        d.IsPartOfMenu
    FROM Dishes d
    LEFT JOIN Categories c ON d.CategoryId = c.Id
    ORDER BY d.Name
END
GO
/****** Object:  StoredProcedure [dbo].[GetAllItemsByCategoryId]    Script Date: 23/05/2025 06:22:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetAllItemsByCategoryId]  
    @CategoryId INT  
AS  
BEGIN  
    -- Dishes that are not part of a menu  
    SELECT   
        d.DishID,   
        d.Name,   
        d.Price,   
        d.Description,   
        -- Agregare alergeni într-un singur câmp (ex: 'Gluten, Lactoză')
        STRING_AGG(a.Name, ', ') AS Allergens,  
        d.TotalQuantity,   
        d.QuantityPerPortion  
    FROM Dishes d
    LEFT JOIN DishAllergens da ON d.DishID = da.DishID  
    LEFT JOIN Allergens a ON da.AllergenID = a.AllergenID  
    WHERE d.CategoryId = @CategoryId AND d.IsPartOfMenu = 0  
    GROUP BY   
        d.DishID, d.Name, d.Price, d.Description, d.TotalQuantity, d.QuantityPerPortion;  
  
    -- Menus for the category  
    SELECT   
        Id,   
        Name,   
        Description  
    FROM Menus  
    WHERE CategoryId = @CategoryId;  
END
GO
/****** Object:  StoredProcedure [dbo].[GetAllOrders]    Script Date: 23/05/2025 06:22:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetAllOrders]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        o.OrderID,
        o.Status,
        o.OrderDate,
        o.TotalAmount,
        u.FullName,
        u.Email,
        d.Name AS DishName,
        oi.Quantity,
        oi.UnitPrice
    FROM Orders o
    JOIN Users u ON o.UserID = u.UserID
    JOIN OrderItems oi ON o.OrderID = oi.OrderID
    JOIN Dishes d ON oi.DishID = d.DishID
    ORDER BY o.OrderDate DESC, o.OrderID;
END
GO
/****** Object:  StoredProcedure [dbo].[GetDishAllergens]    Script Date: 23/05/2025 06:22:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[GetDishAllergens]
    @DishID INT
AS
BEGIN
    SELECT 
        a.AllergenID,
        a.Name AS AllergenName
    FROM DishAllergens da
    JOIN Allergens a ON da.AllergenID = a.AllergenID
    WHERE da.DishID = @DishID
END
GO
/****** Object:  StoredProcedure [dbo].[GetDishesByCategory]    Script Date: 23/05/2025 06:22:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetDishesByCategory]
    @CategoryId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        d.DishID,
        d.Name,
        d.Description,
        d.Price,
        d.Available,
        d.QuantityPerPortion,
        d.TotalQuantity,
        d.IsPartOfMenu
    FROM Dishes d
    WHERE d.CategoryId = @CategoryId
    ORDER BY d.Name
END
GO
/****** Object:  StoredProcedure [dbo].[GetDishPrice]    Script Date: 23/05/2025 06:22:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetDishPrice]
    @DishID INT,
    @Price DECIMAL(10, 2) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT @Price = Price
    FROM Dishes
    WHERE DishID = @DishID;
END;
GO
/****** Object:  StoredProcedure [dbo].[GetItemsByCategoryId]    Script Date: 23/05/2025 06:22:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetItemsByCategoryId]  
    @CategoryId INT  
AS  
BEGIN  
    IF @CategoryId = -1  
    BEGIN  
        SELECT   
            m.id AS Id,  
            m.name AS Name,  
            0.0 AS Price,  
            m.description AS Description,  
            1 AS IsMenu,  
            '0' AS TotalQuantity,  
            NULL AS Allergens  
        FROM Menus m;  
    END  
    ELSE  
    BEGIN  
        -- Dishes fără meniu, cu alergeni
        SELECT   
            d.DishID AS Id,  
            d.Name,  
            d.Price,  
            d.Description,  
            0 AS IsMenu,  
            d.TotalQuantity,  
            STRING_AGG(a.Name, ', ') AS Allergens  
        FROM Dishes d  
        LEFT JOIN DishAllergens da ON d.DishID = da.DishID  
        LEFT JOIN Allergens a ON da.AllergenID = a.AllergenID  
        WHERE d.CategoryId = @CategoryId AND d.IsPartOfMenu = 0  
        GROUP BY d.DishID, d.Name, d.Price, d.Description, d.TotalQuantity  
  
        UNION ALL  
  
        -- Menus din categoria respectivă
        SELECT   
            m.id AS Id,  
            m.name AS Name,  
            0.0 AS Price,  
            m.description AS Description,  
            1 AS IsMenu,  
            '0' AS TotalQuantity,  
            NULL AS Allergens  
        FROM Menus m  
        WHERE m.CategoryId = @CategoryId;  
    END  
END
GO
/****** Object:  StoredProcedure [dbo].[GetMenuItems]    Script Date: 23/05/2025 06:22:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetMenuItems]
    @MenuID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DishID
    FROM MenuItems
    WHERE MenuID = @MenuID;
END
GO
/****** Object:  StoredProcedure [dbo].[GetNormalUsers]    Script Date: 23/05/2025 06:22:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetNormalUsers]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM Users
    WHERE Email NOT LIKE '%@admin%' AND Email NOT LIKE '%@employee%';
END;
GO
/****** Object:  StoredProcedure [dbo].[GetSubItemsForMenu]    Script Date: 23/05/2025 06:22:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetSubItemsForMenu]      
    @MenuId INT      
AS      
BEGIN      
    SELECT       
        d.DishID,      
        d.Name,      
        d.Price,      
        d.Description,      
        ISNULL(STRING_AGG(a.Name, ', '), '') AS Allergens,      
        d.QuantityPerPortion,      
        d.TotalQuantity,
        d.ImagePath
    FROM MenuItems mi      
    INNER JOIN Dishes d ON mi.DishId = d.DishID      
    LEFT JOIN DishAllergens da ON d.DishID = da.DishID      
    LEFT JOIN Allergens a ON da.AllergenID = a.AllergenID      
    WHERE mi.MenuId = @MenuId      
    GROUP BY   
        d.DishID,   
        d.Name,   
        d.Price,   
        d.Description,   
        d.QuantityPerPortion,   
        d.TotalQuantity,
        d.ImagePath;      
END  
GO
/****** Object:  StoredProcedure [dbo].[GetUserByEmailAndPassword]    Script Date: 23/05/2025 06:22:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetUserByEmailAndPassword]
    @Email NVARCHAR(255),
    @PasswordHash NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM Users
    WHERE Email = @Email AND PasswordHash = @PasswordHash;
END;
GO
/****** Object:  StoredProcedure [dbo].[GetUserOrders]    Script Date: 23/05/2025 06:22:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetUserOrders]
    @UserID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        o.OrderID, 
        o.Status, 
        o.OrderDate, 
        o.TotalAmount,
        d.Name AS DishName, 
        oi.Quantity,
        oi.UnitPrice
    FROM Orders o
    JOIN OrderItems oi ON o.OrderID = oi.OrderID
    JOIN Dishes d ON oi.DishID = d.DishID
    WHERE o.UserID = @UserID
    ORDER BY o.OrderDate DESC, o.OrderID;
END
GO
/****** Object:  StoredProcedure [dbo].[RegisterUser]    Script Date: 23/05/2025 06:22:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[RegisterUser]
    @Username NVARCHAR(100),
    @FullName NVARCHAR(255),
    @Email NVARCHAR(255),
    @PhoneNumber NVARCHAR(50),
    @Address NVARCHAR(500),
    @PasswordHash NVARCHAR(255),
    @Role NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Users (Username, FullName, Email, PhoneNumber, Address, PasswordHash, Role, CreatedAt)
    VALUES (@Username, @FullName, @Email, @PhoneNumber, @Address, @PasswordHash, @Role, GETDATE());
END;
GO
/****** Object:  StoredProcedure [dbo].[SaveOrder]    Script Date: 23/05/2025 06:22:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SaveOrder]  
    @UserID INT,  
    @TotalAmount DECIMAL(10,2),  
    @OrderItems OrderItemTableType READONLY  
AS  
BEGIN  
    SET NOCOUNT ON;  
    BEGIN TRY  
        BEGIN TRANSACTION;  
  
        -- Inserare în tabelul Orders cu status "Registered"
        INSERT INTO Orders (UserID, OrderDate, Status, TotalAmount)  
        VALUES (@UserID, GETDATE(), 'Registered', @TotalAmount);  
  
        DECLARE @OrderID INT = SCOPE_IDENTITY();  
  
        -- Inserare în tabelul OrderItems  
        INSERT INTO OrderItems (OrderID, DishID, Quantity, UnitPrice)  
        SELECT @OrderID, DishID, Quantity, UnitPrice  
        FROM @OrderItems;  
  
        COMMIT TRANSACTION;  
    END TRY  
    BEGIN CATCH  
        IF @@TRANCOUNT > 0  
            ROLLBACK TRANSACTION;  
  
        THROW;  
    END CATCH  
END
GO
/****** Object:  StoredProcedure [dbo].[ScadeCantitatiIngredient]    Script Date: 23/05/2025 06:22:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ScadeCantitatiIngredient]  
    @DishID INT,  
    @Multiplier INT  
AS  
BEGIN  
    SET NOCOUNT ON;  
  
    DECLARE @QuantityPerPortion NVARCHAR(50);  
    DECLARE @TotalQuantity NVARCHAR(50);  
  
    SELECT   
        @QuantityPerPortion = QuantityPerPortion,  
        @TotalQuantity = TotalQuantity  
    FROM Dishes  
    WHERE DishID = @DishID;  
  
    IF @QuantityPerPortion IS NOT NULL AND @TotalQuantity IS NOT NULL  
    BEGIN  
        -- Extragem valoarea numerică și unitatea din QuantityPerPortion  
        DECLARE @Value FLOAT = TRY_CAST(LEFT(@QuantityPerPortion, PATINDEX('%[^0-9.]%', @QuantityPerPortion + 'X') - 1) AS FLOAT);  
        DECLARE @Unit NVARCHAR(10) = LTRIM(RIGHT(@QuantityPerPortion, LEN(@QuantityPerPortion) - LEN(CAST(@Value AS NVARCHAR))));  
  
        DECLARE @ToSubtract FLOAT = @Value * @Multiplier;  
  
        -- Extragem valoarea numerică și unitatea din TotalQuantity  
        DECLARE @TotalValue FLOAT = TRY_CAST(LEFT(@TotalQuantity, PATINDEX('%[^0-9.]%', @TotalQuantity + 'X') - 1) AS FLOAT);  
        DECLARE @TotalUnit NVARCHAR(10) = LTRIM(RIGHT(@TotalQuantity, LEN(@TotalQuantity) - LEN(CAST(@TotalValue AS NVARCHAR))));  
  
        IF @Unit = @TotalUnit  
        BEGIN  
            DECLARE @NewTotal FLOAT = IIF(@TotalValue - @ToSubtract > 0, @TotalValue - @ToSubtract, 0);  
            DECLARE @UpdatedTotal NVARCHAR(50) = CONCAT(@NewTotal, @Unit);  
  
            UPDATE Dishes  
            SET TotalQuantity = @UpdatedTotal  
            WHERE DishID = @DishID;  
        END  
    END  
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateDish]    Script Date: 23/05/2025 06:22:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[UpdateDish]
    @DishID INT,
    @Name NVARCHAR(100),
    @Price DECIMAL(10,2),
    @IsPartOfMenu BIT,
    @QuantityPerPortion VARCHAR(20) = NULL,
    @Description NVARCHAR(255) = NULL,
    @Available BIT = 1,
    @TotalQuantity VARCHAR(20) = NULL,
    @CategoryId INT = NULL,
    @AllergenIDs NVARCHAR(MAX) = NULL
AS
BEGIN
    BEGIN TRANSACTION
    BEGIN TRY
        -- Update dish information
        UPDATE Dishes
        SET 
            Name = @Name,
            Description = @Description,
            Price = @Price,
            Available = @Available,
            QuantityPerPortion = @QuantityPerPortion,
            TotalQuantity = @TotalQuantity,
            CategoryId = @CategoryId,
            IsPartOfMenu = @IsPartOfMenu
        WHERE DishID = @DishID;

        -- Update allergens (delete + reinsert)
        DELETE FROM DishAllergens WHERE DishID = @DishID;

        IF @AllergenIDs IS NOT NULL
        BEGIN
            CREATE TABLE #TempAllergens (AllergenID INT);

            INSERT INTO #TempAllergens
            SELECT value FROM STRING_SPLIT(@AllergenIDs, ',');

            INSERT INTO DishAllergens (DishID, AllergenID)
            SELECT @DishID, AllergenID FROM #TempAllergens;

            DROP TABLE #TempAllergens;
        END

        COMMIT TRANSACTION
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION
        THROW
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateDishStock]    Script Date: 23/05/2025 06:22:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[UpdateDishStock]
    @DishID INT,
    @QuantityToAdd NVARCHAR(50)  -- poate conține și unități, ex: '200g'
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @CurrentTotal NVARCHAR(50);
        DECLARE @CurrentPerPortion NVARCHAR(50);

        SELECT 
            @CurrentTotal = TotalQuantity,
            @CurrentPerPortion = QuantityPerPortion
        FROM Dishes
        WHERE DishID = @DishID;

        IF @CurrentTotal IS NOT NULL AND @QuantityToAdd IS NOT NULL
        BEGIN
            -- Extragem valoarea numerică și unitatea din TotalQuantity
            DECLARE @CurrentValue FLOAT = TRY_CAST(LEFT(@CurrentTotal, PATINDEX('%[^0-9.]%', @CurrentTotal + 'X') - 1) AS FLOAT);
            DECLARE @CurrentUnit NVARCHAR(10) = LTRIM(RIGHT(@CurrentTotal, LEN(@CurrentTotal) - LEN(CAST(@CurrentValue AS NVARCHAR))));

            -- Extragem valoarea numerică și unitatea din QuantityToAdd
            DECLARE @AddValue FLOAT = TRY_CAST(LEFT(@QuantityToAdd, PATINDEX('%[^0-9.]%', @QuantityToAdd + 'X') - 1) AS FLOAT);
            DECLARE @AddUnit NVARCHAR(10) = LTRIM(RIGHT(@QuantityToAdd, LEN(@QuantityToAdd) - LEN(CAST(@AddValue AS NVARCHAR))));

            -- Dacă unitățile sunt la fel sau una dintre ele lipsește, facem update
            IF @CurrentUnit = @AddUnit OR @AddUnit = ''
            BEGIN
                DECLARE @NewTotal FLOAT = @CurrentValue + @AddValue;
                DECLARE @UpdatedTotal NVARCHAR(50) = CONCAT(@NewTotal, @CurrentUnit);

                UPDATE Dishes
                SET 
                    TotalQuantity = @UpdatedTotal,
                    Available = CASE 
                        WHEN 
                            ISNUMERIC(LEFT(@UpdatedTotal, PATINDEX('%[^0-9.]%', @UpdatedTotal + 'X') - 1)) = 1 AND
                            ISNUMERIC(LEFT(QuantityPerPortion, PATINDEX('%[^0-9.]%', QuantityPerPortion + 'X') - 1)) = 1 AND
                            TRY_CAST(LEFT(@UpdatedTotal, PATINDEX('%[^0-9.]%', @UpdatedTotal + 'X') - 1) AS FLOAT) >=
                            TRY_CAST(LEFT(QuantityPerPortion, PATINDEX('%[^0-9.]%', QuantityPerPortion + 'X') - 1) AS FLOAT)
                        THEN 1 ELSE 0
                    END
                WHERE DishID = @DishID;
            END
        END

        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK;

        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateOrderStatus]    Script Date: 23/05/2025 06:22:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[UpdateOrderStatus]
    @OrderID INT,
    @NewStatus NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Orders
    SET Status = @NewStatus
    WHERE OrderID = @OrderID;
END;
GO
USE [master]
GO
ALTER DATABASE [RestaurantAppDB] SET  READ_WRITE 
GO
