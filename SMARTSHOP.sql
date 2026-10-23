-- ===================================================================
-- DATABASE CREATION SCRIPT: SMARTSHOP
-- Generative AI for .NET Developers with Google AI - SET01
-- ===================================================================

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = N'SMARTSHOP')
BEGIN
    CREATE DATABASE SMARTSHOP;
END
GO

USE SMARTSHOP;
GO

-- 1. Create Product Table
IF OBJECT_ID(N'dbo.Review', N'U') IS NOT NULL
    DROP TABLE dbo.Review;
GO

IF OBJECT_ID(N'dbo.Product', N'U') IS NOT NULL
    DROP TABLE dbo.Product;
GO

CREATE TABLE dbo.Product (
    ProductId INT PRIMARY KEY,
    ProductName NVARCHAR(128) NOT NULL,
    Category NVARCHAR(64) NOT NULL,
    Price DECIMAL(18, 2) NOT NULL,
    Features NVARCHAR(512) NOT NULL,
    Description NVARCHAR(MAX) NULL
);
GO

-- 2. Create Review Table
CREATE TABLE dbo.Review (
    ReviewId INT IDENTITY(1,1) PRIMARY KEY,
    ProductId INT NOT NULL,
    CustomerName NVARCHAR(64) NOT NULL,
    Content NVARCHAR(1000) NOT NULL,
    Rating INT NOT NULL CHECK (Rating >= 1 AND Rating <= 5),
    Sentiment NVARCHAR(16) NOT NULL, -- Positive / Negative / Neutral
    Score FLOAT NOT NULL,             -- 0.0 to 1.0
    Summary NVARCHAR(256) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Review_Product FOREIGN KEY (ProductId) REFERENCES dbo.Product(ProductId) ON DELETE CASCADE
);
GO

-- 3. Seed Initial Data for Product Table (As specified in Exam Paper)
INSERT INTO dbo.Product (ProductId, ProductName, Category, Price, Features, Description)
VALUES
(1, N'Galaxy S25', N'Phone', 22990000.00, N'6.2 inch AMOLED, Snapdragon 8 Elite, 50MP camera', NULL),
(2, N'MacBook Air M4', N'Laptop', 28490000.00, N'13.6 inch, 16GB RAM, 18 hours battery', NULL),
(3, N'AirPods Pro 3', N'Accessory', 6790000.00, N'Active noise cancellation, USB-C, IP57', NULL),
(4, N'Xiaomi Redmi Note 15', N'Phone', 6490000.00, N'120Hz display, 5500mAh battery, 108MP camera', NULL);
GO

-- 4. Seed Sample Reviews to match Requirement 4 Report Table in Exam Paper
-- Galaxy S25 (5 reviews: 3 Positive, 1 Negative, 1 Neutral; Avg: 4.0)
INSERT INTO dbo.Review (ProductId, CustomerName, Content, Rating, Sentiment, Score, Summary, CreatedAt)
VALUES
(1, N'Anh Tuấn', N'Màn hình AMOLED cực đẹp, chip Snapdragon 8 Elite chơi game mượt mà không nóng.', 5, N'Positive', 0.95, N'Màn hình đẹp, hiệu năng Snapdragon 8 Elite rất mượt.', DATEADD(day, -5, GETDATE())),
(1, N'Minh Hoàng', N'Camera 50MP chụp đêm rất sắc nét, thiết kế cầm nắm chắc tay.', 5, N'Positive', 0.92, N'Camera 50MP chụp đêm tốt, thiết kế cao cấp.', DATEADD(day, -4, GETDATE())),
(1, N'Hương Giang', N'Pin dùng cả ngày ổn định, sạc nhanh tiện lợi.', 4, N'Positive', 0.88, N'Thời lượng pin và tốc độ sạc đáp ứng tốt nhu cầu.', DATEADD(day, -3, GETDATE())),
(1, N'Quốc Bảo', N'Giá hơi cao so với phân khúc, không có củ sạc kèm theo hộp.', 3, N'Neutral', 0.70, N'Giá bán cao, thiếu củ sạc trong hộp.', DATEADD(day, -2, GETDATE())),
(1, N'Thanh Tùng', N'Mặt lưng dễ bám vân tay, loa ngoài âm bass chưa thực sự đã.', 3, N'Negative', 0.81, N'Bám vân tay và âm bass loa ngoài chưa đạt kỳ vọng.', DATEADD(day, -1, GETDATE()));

-- MacBook Air M4 (3 reviews: 3 Positive, 0 Negative, 0 Neutral; Avg: ~4.7)
INSERT INTO dbo.Review (ProductId, CustomerName, Content, Rating, Sentiment, Score, Summary, CreatedAt)
VALUES
(2, N'Đức Thịnh', N'Chip M4 siêu mạnh mẽ, 16GB RAM đa nhiệm mượt, pin 18 tiếng dùng 2 ngày không cần sạc.', 5, N'Positive', 0.98, N'Hiệu năng M4 ấn tượng, pin trâu 18 tiếng.', DATEADD(day, -4, GETDATE())),
(2, N'Khánh Linh', N'Màn hình 13.6 inch hiển thị màu sắc rực rỡ, bàn phím gõ êm, máy rất mỏng nhẹ.', 5, N'Positive', 0.95, N'Mỏng nhẹ, màn hình đẹp và bàn phím êm.', DATEADD(day, -3, GETDATE())),
(2, N'Huy Nam', N'Chất lượng hoàn thiện tuyệt hảo, dùng đồ họa cơ bản cực mát mẻ.', 4, N'Positive', 0.89, N'Hoàn thiện cao cấp, máy chạy mát.', DATEADD(day, -2, GETDATE()));

-- AirPods Pro 3 (2 reviews: 0 Positive, 2 Negative, 0 Neutral; Avg: ~1.5)
INSERT INTO dbo.Review (ProductId, CustomerName, Content, Rating, Sentiment, Score, Summary, CreatedAt)
VALUES
(3, N'Văn Đức', N'Chống ồn chủ động không như kỳ vọng, đeo lâu bị cấn tai đau buốt.', 2, N'Negative', 0.85, N'Chống ồn kém kỳ vọng, đeo lâu gây đau tai.', DATEADD(day, -2, GETDATE())),
(3, N'Thảo My', N'Tai nghe hay bị rớt kết nối một bên, pin tụt nhanh bất thường.', 1, N'Negative', 0.93, N'Lỗi kết nối một bên tai, pin tụt nhanh.', DATEADD(day, -1, GETDATE()));
GO

PRINT 'SMARTSHOP Database initialized successfully!';
GO

