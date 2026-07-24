CREATE DATABASE FASTFEAST
GO
USE FASTFEAST
GO
-- Bảng Customers (Khách hàng)
CREATE TABLE Customers (
    CustomerID INT PRIMARY KEY IDENTITY(1,1),
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(256) NOT NULL, -- Lưu trữ password đã hash
    PhoneNumber NVARCHAR(20),
    Address NVARCHAR(255),
    City NVARCHAR(50),
    PostalCode NVARCHAR(10),
    RegistrationDate DATETIME DEFAULT GETDATE(),
    LastLoginDate DATETIME
);
GO

-- Bảng Categories (Danh mục sản phẩm, ví dụ: Burger, Gà rán, Đồ uống)
CREATE TABLE Categories (
    CategoryID INT PRIMARY KEY IDENTITY(1,1),
    CategoryName NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(500)
);
GO

-- Bảng Products (Sản phẩm/Món ăn)
CREATE TABLE Products (
    ProductID INT PRIMARY KEY IDENTITY(1,1),
    ProductName NVARCHAR(150) NOT NULL,
    Description NVARCHAR(MAX),
    Price DECIMAL(10, 2) NOT NULL,
    ImageURL NVARCHAR(MAX), -- Đường dẫn đến hình ảnh sản phẩm
    CategoryID INT NOT NULL,
    IsAvailable BIT DEFAULT 1, -- Trạng thái còn hàng/hết hàng
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID)
);
GO

-- Bảng ProductOptions (Tùy chọn cho sản phẩm, ví dụ: thêm phô mai, không hành tây)
CREATE TABLE ProductOptions (
    OptionID INT PRIMARY KEY IDENTITY(1,1),
    OptionName NVARCHAR(100) NOT NULL, -- Ví dụ: "Thêm Phô Mai", "Không Hành Tây"
    AdditionalPrice DECIMAL(10, 2) DEFAULT 0.00
);
GO

-- Bảng Product_ProductOptions (Bảng trung gian ánh xạ tùy chọn với sản phẩm nào)
CREATE TABLE Product_ProductOptions (
    ProductID INT NOT NULL,
    OptionID INT NOT NULL,
    PRIMARY KEY (ProductID, OptionID),
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID),
    FOREIGN KEY (OptionID) REFERENCES ProductOptions(OptionID)
);
GO

-- Bảng Orders (Đơn hàng)
CREATE TABLE Orders (
    OrderID INT PRIMARY KEY IDENTITY(1,1),
    CustomerID INT NOT NULL,
    OrderDate DATETIME DEFAULT GETDATE(),
    TotalAmount DECIMAL(10, 2) NOT NULL,
    OrderStatus NVARCHAR(50) NOT NULL, -- Ví dụ: "Pending", "Confirmed", "Preparing", "Delivering", "Delivered", "Cancelled"
    ShippingAddress NVARCHAR(255) NOT NULL,
    ShippingCity NVARCHAR(50),
    ShippingPostalCode NVARCHAR(10),
    PaymentMethod NVARCHAR(50), -- Ví dụ: "Cash on Delivery", "Credit Card"
    PaymentStatus NVARCHAR(50), -- Ví dụ: "Paid", "Unpaid", "Refunded"
    DeliveryFee DECIMAL(10, 2) DEFAULT 0.00,
    DiscountAmount DECIMAL(10, 2) DEFAULT 0.00,
    EstimatedDeliveryTime DATETIME,
    ActualDeliveryTime DATETIME,
    Notes NVARCHAR(MAX), -- Ghi chú của khách hàng
    FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID)
);
GO

-- Bảng OrderDetails (Chi tiết đơn hàng - các sản phẩm trong một đơn hàng)
CREATE TABLE OrderDetails (
    OrderDetailID INT PRIMARY KEY IDENTITY(1,1),
    OrderID INT NOT NULL,
    ProductID INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(10, 2) NOT NULL, -- Giá tại thời điểm đặt hàng
    Subtotal DECIMAL(10, 2) NOT NULL,
    Notes NVARCHAR(255), -- Ghi chú riêng cho sản phẩm này trong đơn hàng
    FOREIGN KEY (OrderID) REFERENCES Orders(OrderID),
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);
GO

-- Bảng OrderDetail_Options (Bảng trung gian lưu các tùy chọn được chọn cho từng sản phẩm trong OrderDetail)
CREATE TABLE OrderDetail_Options (
    OrderDetailID INT NOT NULL,
    OptionID INT NOT NULL,
    PRIMARY KEY (OrderDetailID, OptionID),
    FOREIGN KEY (OrderDetailID) REFERENCES OrderDetails(OrderDetailID),
    FOREIGN KEY (OptionID) REFERENCES ProductOptions(OptionID)
);
GO

-- Bảng DeliveryDrivers (Nhân viên giao hàng)
CREATE TABLE DeliveryDrivers (
    DriverID INT PRIMARY KEY IDENTITY(1,1),
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    PhoneNumber NVARCHAR(20) UNIQUE NOT NULL,
    Email NVARCHAR(100) UNIQUE,
    VehicleType NVARCHAR(50), -- Ví dụ: "Motorbike", "Car"
    LicensePlate NVARCHAR(20),
    IsAvailable BIT DEFAULT 1, -- Trạng thái sẵn sàng giao hàng
    CurrentLocation NVARCHAR(255) -- Có thể lưu trữ tọa độ GPS nếu có tích hợp bản đồ
);
GO

-- Bảng Deliveries (Lịch sử giao hàng)
CREATE TABLE Deliveries (
    DeliveryID INT PRIMARY KEY IDENTITY(1,1),
    OrderID INT NOT NULL UNIQUE, -- Mỗi Order chỉ có một Delivery
    DriverID INT,
    PickupTime DATETIME,
    DeliveredTime DATETIME,
    DeliveryStatus NVARCHAR(50) NOT NULL, -- Ví dụ: "Assigned", "PickedUp", "EnRoute", "Delivered", "Failed"
    Notes NVARCHAR(MAX), -- Ghi chú của tài xế
    FOREIGN KEY (OrderID) REFERENCES Orders(OrderID),
    FOREIGN KEY (DriverID) REFERENCES DeliveryDrivers(DriverID)
);
GO

-- Bảng Promotions (Khuyến mãi/Mã giảm giá)
CREATE TABLE Promotions (
    PromotionID INT PRIMARY KEY IDENTITY(1,1),
    PromotionCode NVARCHAR(50) UNIQUE NOT NULL,
    Description NVARCHAR(500),
    DiscountType NVARCHAR(50) NOT NULL, -- Ví dụ: "Percentage", "FixedAmount"
    DiscountValue DECIMAL(10, 2) NOT NULL,
    MinimumOrderAmount DECIMAL(10, 2) DEFAULT 0.00,
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL,
    UsageLimit INT, -- Tổng số lần sử dụng
    UsedCount INT DEFAULT 0, -- Số lần đã sử dụng
    IsActive BIT DEFAULT 1
);
GO

-- Bảng Reviews (Đánh giá sản phẩm/đơn hàng)
CREATE TABLE Reviews (
    ReviewID INT PRIMARY KEY IDENTITY(1,1),
    CustomerID INT NOT NULL,
    ProductID INT, -- Có thể đánh giá riêng sản phẩm
    OrderID INT, -- Hoặc đánh giá toàn bộ đơn hàng
    Rating INT NOT NULL CHECK (Rating >= 1 AND Rating <= 5), -- Điểm từ 1 đến 5
    Comment NVARCHAR(MAX),
    ReviewDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID),
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID),
    FOREIGN KEY (OrderID) REFERENCES Orders(OrderID)
);
GO

-- Bảng Admins (Tài khoản quản trị viên)
CREATE TABLE Admins (
    AdminID INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(256) NOT NULL,
    Email NVARCHAR(100) UNIQUE NOT NULL,
    Role NVARCHAR(50) NOT NULL, -- Ví dụ: "SuperAdmin", "OrderManager", "ProductManager"
    CreatedAt DATETIME DEFAULT GETDATE(),
    LastLogin DATETIME
);
GO


-- Bước 3: Thêm dữ liệu mẫu (Seed Data)

-- 1. Dữ liệu mẫu cho Categories (Danh mục sản phẩm)
INSERT INTO Categories (CategoryName, Description) VALUES
('Burgers', N'Các loại bánh burger thơm ngon với nhiều lựa chọn.'),
('Fried Chicken', N'Gà rán giòn rụm với công thức đặc biệt.'),
('Pasta & More', N'Các món mì Ý và nhiều lựa chọn khác.'),
('Sides', N'Món ăn kèm hoàn hảo cho bữa ăn của bạn.'),
('Drink', N'Các loại đồ uống giải khát.'),
('Pizza', N'Các loại Pizza.');
GO

-- 2. Dữ liệu mẫu cho Products (Sản phẩm/Món ăn)
INSERT INTO Products (ProductName, Description, Price, ImageURL, CategoryID, IsAvailable) VALUES
(N'Classic Beef Burger', N'Bánh burger bò truyền thống với rau diếp, cà chua, hành tây và sốt đặc biệt.', 85.000, '~/Content/images/f2.png', (SELECT CategoryID FROM Categories WHERE CategoryName = 'Burgers'), 1),
(N'Burger Gà Cay', N'Bánh burger gà cay với thịt gà giòn, sốt cay và rau.', 65.000, '~/Content/images/f7.png', (SELECT CategoryID FROM Categories WHERE CategoryName = 'Burgers'), 1),
(N'Double Cheese Burger', N'Bánh burger phô mai đôi với hai miếng bò, phô mai tan chảy và sốt.', 75.000, '~/Content/images/f9.png', (SELECT CategoryID FROM Categories WHERE CategoryName = 'Burgers'), 1),
(N'Crispy Fried Chicken (2pcs)', N'Hai miếng gà rán giòn rụm, tẩm ướp đậm đà.', 50.000, '~/Content/images/2miengga.jfif', (SELECT CategoryID FROM Categories WHERE CategoryName = 'Fried Chicken'), 1),
(N'Family Bucket (8pcs)', N'Gia đình gà rán 8 miếng, hoàn hảo cho cả nhà.', 90.000, '~/Content/images/8miengga.jfif', (SELECT CategoryID FROM Categories WHERE CategoryName = 'Fried Chicken'), 1),
(N'Mì ý sốt pho mai', N'Mì Ý sốt pho mai.', 45.000, '~/Content/images/f9.png', (SELECT CategoryID FROM Categories WHERE CategoryName = 'Pasta & More'), 1),
(N'Mì ống ngon', N'Mì ống chay.', 30.000, '~/Content/images/f9.png', (SELECT CategoryID FROM Categories WHERE CategoryName = 'Pasta & More'), 1),
(N'French Fries (Large)', N'Khoai tây chiên giòn rụm cỡ lớn.', 3.99, '~/Content/images/f5.png', (SELECT CategoryID FROM Categories WHERE CategoryName = 'Sides'), 1),
(N'Onion Rings', N'Vòng hành tây chiên giòn.', 35.000, '~/Content/images/hanhtaychien.jfif', (SELECT CategoryID FROM Categories WHERE CategoryName = 'Sides'), 1),
(N'Coca-Cola', N'Nước ngọt Coca-Cola.', 12.000, '~/Content/images/coca.jfif', (SELECT CategoryID FROM Categories WHERE CategoryName = 'Drink'), 1),
(N'Orange Juice', N'Nước cam ép tự nhiên.', 15.000, '~/Content/images/camep.jfif', (SELECT CategoryID FROM Categories WHERE CategoryName = 'Drink'), 1),
(N'Pizza rau', N'Pizza được nướng với lớp phô mai dày kèm rau củ.', 65.000, '~/Content/images/f6.png', (SELECT CategoryID FROM Categories WHERE CategoryName = 'Pizza'), 1),
(N'Pizza hải sản', N'Pizza với rất nhiều hải sản.', 85.000, '~/Content/images/f1.png', (SELECT CategoryID FROM Categories WHERE CategoryName = 'Pizza'), 1);
GO

-- 3. Dữ liệu mẫu cho ProductOptions (Tùy chọn cho sản phẩm)
INSERT INTO ProductOptions (OptionName, AdditionalPrice) VALUES
(N'Thêm Phô Mai', 15.000),
(N'Thêm Thịt Xông Khói', 25.000),
(N'Không Hành Tây', 0.00),
(N'Không Cà Chua', 0.00),
(N'Sốt Cay',5.000),
(N'Sốt BBQ',10.000);
GO

-- 4. Dữ liệu mẫu cho Product_ProductOptions (Ánh xạ tùy chọn với sản phẩm)
-- Giả sử Burger có thể thêm phô mai, thịt xông khói, không hành tây, không cà chua
-- Classic Beef Burger options
DECLARE @ClassicBurgerID INT = (SELECT ProductID FROM Products WHERE ProductName = N'Classic Beef Burger');
DECLARE @OptionCheese INT = (SELECT OptionID FROM ProductOptions WHERE OptionName = N'Thêm Phô Mai');
DECLARE @OptionBacon INT = (SELECT OptionID FROM ProductOptions WHERE OptionName = N'Thêm Thịt Xông Khói');
DECLARE @OptionNoOnion INT = (SELECT OptionID FROM ProductOptions WHERE OptionName = N'Không Hành Tây');
DECLARE @OptionNoTomato INT = (SELECT OptionID FROM ProductOptions WHERE OptionName = N'Không Cà Chua');

IF @ClassicBurgerID IS NOT NULL
BEGIN
    IF @OptionCheese IS NOT NULL
        INSERT INTO Product_ProductOptions (ProductID, OptionID) VALUES (@ClassicBurgerID, @OptionCheese);
    IF @OptionBacon IS NOT NULL
        INSERT INTO Product_ProductOptions (ProductID, OptionID) VALUES (@ClassicBurgerID, @OptionBacon);
    IF @OptionNoOnion IS NOT NULL
        INSERT INTO Product_ProductOptions (ProductID, OptionID) VALUES (@ClassicBurgerID, @OptionNoOnion);
    IF @OptionNoTomato IS NOT NULL
        INSERT INTO Product_ProductOptions (ProductID, OptionID) VALUES (@ClassicBurgerID, @OptionNoTomato);
END

-- Spicy Chicken Burger options
DECLARE @SpicyBurgerID INT = (SELECT ProductID FROM Products WHERE ProductName = N'Spicy Chicken Burger');
DECLARE @OptionCheeseSpicy INT = (SELECT OptionID FROM ProductOptions WHERE OptionName = N'Thêm Phô Mai');
DECLARE @OptionSpicy INT = (SELECT OptionID FROM ProductOptions WHERE OptionName = N'Sốt Cay');

IF @SpicyBurgerID IS NOT NULL
BEGIN
    IF @OptionCheeseSpicy IS NOT NULL
        INSERT INTO Product_ProductOptions (ProductID, OptionID) VALUES (@SpicyBurgerID, @OptionCheeseSpicy);
    IF @OptionSpicy IS NOT NULL
        INSERT INTO Product_ProductOptions (ProductID, OptionID) VALUES (@SpicyBurgerID, @OptionSpicy);
END

-- Double Cheese Burger options
DECLARE @DoubleCheeseID INT = (SELECT ProductID FROM Products WHERE ProductName = N'Double Cheese Burger');
DECLARE @OptionBaconDouble INT = (SELECT OptionID FROM ProductOptions WHERE OptionName = N'Thêm Thịt Xông Khói');

IF @DoubleCheeseID IS NOT NULL AND @OptionBaconDouble IS NOT NULL
BEGIN
    INSERT INTO Product_ProductOptions (ProductID, OptionID) VALUES (@DoubleCheeseID, @OptionBaconDouble);
END


-- 5. Dữ liệu mẫu cho Customers (Khách hàng)
-- Lưu ý: PasswordHash phải là giá trị đã được hash thực tế trong ứng dụng.
-- Ở đây tôi dùng giá trị mẫu, bạn sẽ cần hash password thật khi đăng ký.
INSERT INTO Customers (FirstName, LastName, Email, PasswordHash, PhoneNumber, Address, City, PostalCode, RegistrationDate) VALUES
(N'Nguyễn', N'Văn A', 'nguyenvana@example.com', '123123', '0901234567', N'123 Đường ABC', N'Hà Nội', '100000', GETDATE()),
(N'Trần', N'Thị B', 'tranthib@example.com', '123123', '0912345678', N'456 Phố XYZ', N'TP. Hồ Chí Minh', '700000', GETDATE()),
(N'Lê', N'Văn C', 'levanc@example.com', '123123', '0987654321', N'789 Đại lộ QWE', N'Đà Nẵng', '550000', GETDATE());
GO

-- 6. Dữ liệu mẫu cho Promotions (Khuyến mãi)
INSERT INTO Promotions (PromotionCode, Description, DiscountType, DiscountValue, MinimumOrderAmount, StartDate, EndDate, UsageLimit, UsedCount, IsActive) VALUES
('WELCOME10', N'Giảm 10% cho đơn hàng đầu tiên.', 'Percentage', 10.00, 20.00, GETDATE(), DATEADD(month, 1, GETDATE()), 100, 0, 1),
('FREESHIP', N'Miễn phí giao hàng cho đơn hàng trên $15.', 'FixedAmount', 50.000, 15.00, GETDATE(), DATEADD(month, 2, GETDATE()), NULL, 0, 1),
('BURGERSALE', N'Giảm $3 cho bất kỳ burger nào.', 'FixedAmount', 30.000, 0.00, DATEADD(day, -7, GETDATE()), DATEADD(day, 7, GETDATE()), 50, 10, 1);
GO

-- 7. Dữ liệu mẫu cho DeliveryDrivers (Nhân viên giao hàng)
INSERT INTO DeliveryDrivers (FirstName, LastName, PhoneNumber, Email, VehicleType, LicensePlate, IsAvailable) VALUES
(N'Phạm', N'Minh', '0934567890', 'phamminh@example.com', N'Xe máy', '29B1-12345', 1),
(N'Đỗ', N'Tấn', '0967890123', 'dotan@example.com', N'Xe máy', '51G2-67890', 1);
GO

-- 8. Dữ liệu mẫu cho Admins (Tài khoản quản trị viên)
-- Tương tự PasswordHash của Customer, bạn cần hash password thực tế.
INSERT INTO Admins (Username, PasswordHash, Email, Role, CreatedAt) VALUES
('admin', 'admin123', 'admin@fastbiteexpress.com', 'SuperAdmin', GETDATE()),
('ordermanager', '123123', 'order.manager@fastbiteexpress.com', 'OrderManager', GETDATE());
GO

UPDATE Admins
SET PasswordHash = '0192023a7bbd73250516f069df18b500' 
WHERE Username = 'admin';

-- Ví dụ về một đơn hàng đơn giản:
/*
INSERT INTO Orders (CustomerID, OrderDate, TotalAmount, OrderStatus, ShippingAddress, ShippingCity, ShippingPostalCode, PaymentMethod, PaymentStatus) VALUES
((SELECT CustomerID FROM Customers WHERE Email = 'nguyenvana@example.com'), GETDATE(), 13.98, 'Confirmed', N'123 Đường ABC', N'Hà Nội', '100000', 'Cash on Delivery', 'Unpaid');

INSERT INTO OrderDetails (OrderID, ProductID, Quantity, UnitPrice, Subtotal) VALUES
((SELECT OrderID FROM Orders WHERE CustomerID = (SELECT CustomerID FROM Customers WHERE Email = 'nguyenvana@example.com') AND OrderStatus = 'Confirmed' ORDER BY OrderDate DESC LIMIT 1), (SELECT ProductID FROM Products WHERE ProductName = N'Classic Beef Burger'), 1, 9.99, 9.99),
((SELECT OrderID FROM Orders WHERE CustomerID = (SELECT CustomerID FROM Customers WHERE Email = 'nguyenvana@example.com') AND OrderStatus = 'Confirmed' ORDER BY OrderDate DESC LIMIT 1), (SELECT ProductID FROM Products WHERE ProductName = N'Coca-Cola'), 1, 3.99, 3.99);

-- Thêm một bản ghi giao hàng cho đơn hàng trên
INSERT INTO Deliveries (OrderID, DriverID, PickupTime, DeliveryStatus) VALUES
((SELECT OrderID FROM Orders WHERE CustomerID = (SELECT CustomerID FROM Customers WHERE Email = 'nguyenvana@example.com') AND OrderStatus = 'Confirmed' ORDER BY OrderDate DESC LIMIT 1), (SELECT DriverID FROM DeliveryDrivers WHERE FirstName = N'Phạm'), DATEADD(minute, 10, GETDATE()), 'Assigned');
*/

EXEC sp_msforeachtable "ALTER TABLE ? NOCHECK CONSTRAINT ALL";
GO

DROP TABLE IF EXISTS OrderDetail_Options;
DROP TABLE IF EXISTS OrderDetails;
DROP TABLE IF EXISTS Deliveries;
DROP TABLE IF EXISTS Orders;
DROP TABLE IF EXISTS Product_ProductOptions;
DROP TABLE IF EXISTS Reviews;
DROP TABLE IF EXISTS Products;
DROP TABLE IF EXISTS ProductOptions;
DROP TABLE IF EXISTS Promotions;
DROP TABLE IF EXISTS DeliveryDrivers;
DROP TABLE IF EXISTS Categories;
DROP TABLE IF EXISTS Customers;
DROP TABLE IF EXISTS Admins;
GO