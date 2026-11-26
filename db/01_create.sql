-- ПР 1.1. Варіант 15 «Ремонтна майстерня» (ремонт електроніки та побутової техніки)
-- Фізична модель: створення бази даних і таблиць (MS SQL Server)
-- Виконав: Гнетило Іван Іванович, ТЦР-31

USE master;
GO
IF DB_ID(N'RepairShop') IS NOT NULL
BEGIN
    ALTER DATABASE RepairShop SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE RepairShop;
END
GO
CREATE DATABASE RepairShop COLLATE Cyrillic_General_CI_AS;
GO
USE RepairShop;
GO

-- Клієнти майстерні
CREATE TABLE Clients (
    ClientID     INT IDENTITY(1,1) PRIMARY KEY,
    FullName     NVARCHAR(100) NOT NULL,
    Phone        VARCHAR(13)   NOT NULL UNIQUE
                 CHECK (Phone LIKE '+380[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'),
    Email        NVARCHAR(100) NULL,
    RegisteredAt DATE          NOT NULL DEFAULT CAST(GETDATE() AS DATE)
);
-- e-mail необов'язковий, але якщо вказаний — унікальний
CREATE UNIQUE INDEX UX_Clients_Email ON Clients(Email) WHERE Email IS NOT NULL;

-- Майстри
CREATE TABLE Masters (
    MasterID       INT IDENTITY(1,1) PRIMARY KEY,
    FullName       NVARCHAR(100) NOT NULL,
    Specialization NVARCHAR(30)  NOT NULL
                   CHECK (Specialization IN (N'смартфони', N'ноутбуки', N'побутова техніка', N'телевізори')),
    Phone          VARCHAR(13)   NOT NULL UNIQUE,
    HireDate       DATE          NOT NULL,
    CommissionPct  DECIMAL(4,2)  NOT NULL DEFAULT 20 CHECK (CommissionPct BETWEEN 0 AND 50),
    IsActive       BIT           NOT NULL DEFAULT 1
);

-- Пристрої клієнтів
CREATE TABLE Devices (
    DeviceID     INT IDENTITY(1,1) PRIMARY KEY,
    ClientID     INT          NOT NULL REFERENCES Clients(ClientID),
    DeviceType   NVARCHAR(30) NOT NULL
                 CHECK (DeviceType IN (N'смартфон', N'ноутбук', N'планшет', N'телевізор', N'пральна машина', N'холодильник', N'мікрохвильова піч')),
    Brand        NVARCHAR(50) NOT NULL,
    Model        NVARCHAR(50) NOT NULL,
    SerialNumber VARCHAR(40)  NOT NULL UNIQUE
);

-- Замовлення на ремонт
CREATE TABLE RepairOrders (
    OrderID          INT IDENTITY(1,1) PRIMARY KEY,
    DeviceID         INT           NOT NULL REFERENCES Devices(DeviceID),
    -- якщо майстра звільнено й видалено, замовлення лишається без виконавця
    MasterID         INT           NULL REFERENCES Masters(MasterID) ON DELETE SET NULL,
    ReceivedAt       DATETIME2(0)  NOT NULL DEFAULT SYSDATETIME(),
    CompletedAt      DATETIME2(0)  NULL,
    IssueDescription NVARCHAR(500) NOT NULL,
    Status           NVARCHAR(20)  NOT NULL DEFAULT N'прийнято'
                     CHECK (Status IN (N'прийнято', N'діагностика', N'в роботі', N'очікує запчастини', N'готово', N'видано', N'скасовано')),
    WarrantyDays     INT           NOT NULL DEFAULT 30 CHECK (WarrantyDays BETWEEN 0 AND 365),
    CONSTRAINT CK_RepairOrders_Dates CHECK (CompletedAt IS NULL OR CompletedAt >= ReceivedAt)
);

-- Довідник послуг
CREATE TABLE Services (
    ServiceID       INT IDENTITY(1,1) PRIMARY KEY,
    Name            NVARCHAR(100) NOT NULL UNIQUE,
    BasePrice       DECIMAL(10,2) NOT NULL CHECK (BasePrice > 0),
    DurationMinutes INT           NOT NULL CHECK (DurationMinutes > 0)
);

-- Запчастини на складі
CREATE TABLE Parts (
    PartID   INT IDENTITY(1,1) PRIMARY KEY,
    Name     NVARCHAR(100) NOT NULL,
    Article  VARCHAR(30)   NOT NULL UNIQUE,
    Price    DECIMAL(10,2) NOT NULL CHECK (Price >= 0),
    StockQty INT           NOT NULL DEFAULT 0 CHECK (StockQty >= 0)
);

-- Роботи, виконані в межах замовлення (асоціація «Замовлення — Послуги», M:N)
CREATE TABLE OrderServices (
    OrderServiceID INT IDENTITY(1,1) PRIMARY KEY,
    OrderID        INT           NOT NULL REFERENCES RepairOrders(OrderID) ON DELETE CASCADE,
    ServiceID      INT           NOT NULL REFERENCES Services(ServiceID),
    Price          DECIMAL(10,2) NOT NULL CHECK (Price >= 0),
    Quantity       INT           NOT NULL DEFAULT 1 CHECK (Quantity > 0),
    CONSTRAINT UQ_OrderServices UNIQUE (OrderID, ServiceID)
);

-- Запчастини, використані в замовленні (асоціація «Замовлення — Запчастини», M:N)
CREATE TABLE OrderParts (
    OrderPartID INT IDENTITY(1,1) PRIMARY KEY,
    OrderID     INT           NOT NULL REFERENCES RepairOrders(OrderID) ON DELETE CASCADE,
    PartID      INT           NOT NULL REFERENCES Parts(PartID),
    Quantity    INT           NOT NULL CHECK (Quantity > 0),
    UnitPrice   DECIMAL(10,2) NOT NULL CHECK (UnitPrice >= 0),
    CONSTRAINT UQ_OrderParts UNIQUE (OrderID, PartID)
);

-- Оплати за замовленнями
CREATE TABLE Payments (
    PaymentID INT IDENTITY(1,1) PRIMARY KEY,
    OrderID   INT           NOT NULL REFERENCES RepairOrders(OrderID),
    Amount    DECIMAL(10,2) NOT NULL CHECK (Amount > 0),
    PaidAt    DATETIME2(0)  NOT NULL DEFAULT SYSDATETIME(),
    Method    NVARCHAR(10)  NOT NULL CHECK (Method IN (N'готівка', N'картка', N'переказ'))
);
GO
