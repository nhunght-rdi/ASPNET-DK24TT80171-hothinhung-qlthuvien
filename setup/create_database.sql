IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'ThuVienSoDB')
    CREATE DATABASE ThuVienSoDB;
GO
USE ThuVienSoDB;
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Categories' AND xtype='U')
CREATE TABLE Categories (
    CategoryId INT PRIMARY KEY IDENTITY(1,1),
    CategoryName NVARCHAR(100) NOT NULL
);
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
CREATE TABLE Users (
    UserId NVARCHAR(20) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(150) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL,
    PhoneNumber NVARCHAR(15),
    Address NVARCHAR(255),
    RegisterDate DATETIME DEFAULT GETDATE(),
    Role NVARCHAR(20) DEFAULT 'Reader',
    Status NVARCHAR(20) DEFAULT 'Active',
    ViolationCount INT DEFAULT 0
);
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Books' AND xtype='U')
CREATE TABLE Books (
    BookId NVARCHAR(20) PRIMARY KEY,
    BookTitle NVARCHAR(255) NOT NULL,
    Author NVARCHAR(50),
    Publisher NVARCHAR(100),
    Quantity INT DEFAULT 0,
    Price DECIMAL(18,2) DEFAULT 0,
    Status NVARCHAR(50) DEFAULT N'Còn sách',
    Description NVARCHAR(MAX),
    CoverImage NVARCHAR(500),
    CategoryId INT FOREIGN KEY REFERENCES Categories(CategoryId)
);
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='BorrowRecords' AND xtype='U')
CREATE TABLE BorrowRecords (
    BorrowId NVARCHAR(20) PRIMARY KEY,
    BookId NVARCHAR(20) NOT NULL FOREIGN KEY REFERENCES Books(BookId),
    UserId NVARCHAR(20) NOT NULL FOREIGN KEY REFERENCES Users(UserId),
    BorrowDate DATETIME DEFAULT GETDATE(),
    DueDate DATETIME NOT NULL,
    ReturnDate DATETIME,
    CreatedBy NVARCHAR(200) NOT NULL,
    Status NVARCHAR(50) DEFAULT 'DangMuon'
);
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Violations' AND xtype='U')
CREATE TABLE Violations (
    ViolationId INT PRIMARY KEY IDENTITY(1,1),
    UserId NVARCHAR(20) NOT NULL FOREIGN KEY REFERENCES Users(UserId),
    BookId NVARCHAR(20) NOT NULL,
    ViolationDate DATETIME DEFAULT GETDATE(),
    ViolationName NVARCHAR(500),
    PenaltyType NVARCHAR(50) DEFAULT 'CanhCao',
    PenaltyAmount DECIMAL(18,2),
    Status NVARCHAR(20) DEFAULT 'ChuaXuLy'
);
GO

IF NOT EXISTS (SELECT * FROM Categories)
INSERT INTO Categories (CategoryName) VALUES
(N'Sách văn học'),(N'Sách thiếu nhi'),(N'Sách kinh tế'),
(N'Tâm lý - Kỹ năng sống'),(N'Khoa học - Công nghệ'),(N'Lịch sử - Địa lý');
GO

PRINT 'Database ThuVienSoDB created successfully!';
GO

-- Seed data for Users
IF NOT EXISTS (SELECT * FROM Users WHERE UserId = 'ADMIN001')
INSERT INTO Users (UserId, FullName, Email, Password, Role, Status)
VALUES (N'ADMIN001', N'Quản Trị Viên', N'170124893@rdi.edu.vn', N'$2a$11$j24O/noDHbIW7lQHqs8hAuuW3gsGxr/Q3LosQL16enDTEsqUAUS3i', N'Admin', N'Active');
GO

-- Seed data for Books
IF NOT EXISTS (SELECT * FROM Books WHERE BookId = 'VH001')
INSERT INTO Books (BookId, CategoryId, BookTitle, Author, Publisher, Quantity, Price, Status, Description)
VALUES (N'VH001', 1, N'Số Đỏ', N'Vũ Trọng Phụng', N'NXB Văn Học', 5, 65000, N'Còn sách', N'Tiểu thuyết trào phúng kinh điển của văn học Việt Nam hiện đại.');
GO
IF NOT EXISTS (SELECT * FROM Books WHERE BookId = 'VH002')
INSERT INTO Books (BookId, CategoryId, BookTitle, Author, Publisher, Quantity, Price, Status, Description)
VALUES (N'VH002', 1, N'Tắt Đèn', N'Ngô Tất Tố', N'NXB Văn Học', 4, 55000, N'Còn sách', N'Tác phẩm hiện thực xuất sắc về cuộc sống khổ cực của người nông dân.');
GO
IF NOT EXISTS (SELECT * FROM Books WHERE BookId = 'VH003')
INSERT INTO Books (BookId, CategoryId, BookTitle, Author, Publisher, Quantity, Price, Status, Description)
VALUES (N'VH003', 1, N'Chí Phèo', N'Nam Cao', N'NXB Giáo Dục', 6, 48000, N'Còn sách', N'Truyện ngắn về bi kịch tha hóa của người nông dân trong xã hội cũ.');
GO
IF NOT EXISTS (SELECT * FROM Books WHERE BookId = 'VH004')
INSERT INTO Books (BookId, CategoryId, BookTitle, Author, Publisher, Quantity, Price, Status, Description)
VALUES (N'VH004', 1, N'Dế Mèn Phiêu Lưu Ký', N'Tô Hoài', N'NXB Kim Đồng', 8, 72000, N'Còn sách', N'Cuộc phiêu lưu của chú dế mèn dũng cảm qua thế giới loài vật.');
GO
IF NOT EXISTS (SELECT * FROM Books WHERE BookId = 'VH005')
INSERT INTO Books (BookId, CategoryId, BookTitle, Author, Publisher, Quantity, Price, Status, Description)
VALUES (N'VH005', 1, N'Nhà Thờ Đức Bà Paris', N'Victor Hugo', N'NXB Văn Học', 3, 120000, N'Còn sách', N'Kiệt tác văn học lãng mạn với nhân vật Quasimodo bất hủ.');
GO
IF NOT EXISTS (SELECT * FROM Books WHERE BookId = 'TN001')
INSERT INTO Books (BookId, CategoryId, BookTitle, Author, Publisher, Quantity, Price, Status, Description)
VALUES (N'TN001', 2, N'Hoàng Tử Bé', N'Antoine de Saint-Exupéry', N'NXB Hội Nhà Văn', 10, 85000, N'Còn sách', N'Câu chuyện cảm động về tình bạn, tình yêu và những điều quan trọng trong cuộc sống.');
GO
IF NOT EXISTS (SELECT * FROM Books WHERE BookId = 'TN002')
INSERT INTO Books (BookId, CategoryId, BookTitle, Author, Publisher, Quantity, Price, Status, Description)
VALUES (N'TN002', 2, N'Không Gia Đình', N'Hector Malot', N'NXB Kim Đồng', 5, 90000, N'Còn sách', N'Hành trình cảm động của cậu bé Rémi tìm kiếm gia đình thật sự.');
GO
IF NOT EXISTS (SELECT * FROM Books WHERE BookId = 'TN003')
INSERT INTO Books (BookId, CategoryId, BookTitle, Author, Publisher, Quantity, Price, Status, Description)
VALUES (N'TN003', 2, N'Conan Thám Tử - Tập 1', N'Gosho Aoyama', N'NXB Kim Đồng', 7, 25000, N'Còn sách', N'Bộ truyện tranh trinh thám nổi tiếng nhất Nhật Bản.');
GO
IF NOT EXISTS (SELECT * FROM Books WHERE BookId = 'TN004')
INSERT INTO Books (BookId, CategoryId, BookTitle, Author, Publisher, Quantity, Price, Status, Description)
VALUES (N'TN004', 2, N'Doremon - Tập 1', N'Fujiko F. Fujio', N'NXB Kim Đồng', 9, 22000, N'Còn sách', N'Chú mèo máy thần kỳ đến từ tương lai cùng những bảo bối kỳ diệu.');
GO
IF NOT EXISTS (SELECT * FROM Books WHERE BookId = 'KT001')
INSERT INTO Books (BookId, CategoryId, BookTitle, Author, Publisher, Quantity, Price, Status, Description)
VALUES (N'KT001', 3, N'Cha Giàu Cha Nghèo', N'Robert T. Kiyosaki', N'NXB Trẻ', 6, 115000, N'Còn sách', N'Cuốn sách kinh điển về tài chính cá nhân và đầu tư.');
GO
IF NOT EXISTS (SELECT * FROM Books WHERE BookId = 'KT002')
INSERT INTO Books (BookId, CategoryId, BookTitle, Author, Publisher, Quantity, Price, Status, Description)
VALUES (N'KT002', 3, N'Đắc Nhân Tâm', N'Dale Carnegie', N'NXB Tổng Hợp TP.HCM', 8, 108000, N'Còn sách', N'Nghệ thuật giao tiếp và xây dựng mối quan hệ.');
GO
IF NOT EXISTS (SELECT * FROM Books WHERE BookId = 'KT003')
INSERT INTO Books (BookId, CategoryId, BookTitle, Author, Publisher, Quantity, Price, Status, Description)
VALUES (N'KT003', 3, N'Khởi Nghiệp Tinh Gọn', N'Eric Ries', N'NXB Lao Động', 4, 130000, N'Còn sách', N'Phương pháp xây dựng startup hiệu quả.');
GO
IF NOT EXISTS (SELECT * FROM Books WHERE BookId = 'KT004')
INSERT INTO Books (BookId, CategoryId, BookTitle, Author, Publisher, Quantity, Price, Status, Description)
VALUES (N'KT004', 3, N'Từ Tốt Đến Vĩ Đại', N'Jim Collins', N'NXB Trẻ', 3, 145000, N'Còn sách', N'Nghiên cứu về cách các công ty vượt qua ngưỡng từ tốt lên vĩ đại.');
GO
IF NOT EXISTS (SELECT * FROM Books WHERE BookId = 'TL001')
INSERT INTO Books (BookId, CategoryId, BookTitle, Author, Publisher, Quantity, Price, Status, Description)
VALUES (N'TL001', 4, N'Nhà Giả Kim', N'Paulo Coelho', N'NXB Văn Học', 9, 95000, N'Còn sách', N'Hành trình tìm kiếm giấc mơ và triết lý sâu sắc về cuộc đời.');
GO
IF NOT EXISTS (SELECT * FROM Books WHERE BookId = 'TL002')
INSERT INTO Books (BookId, CategoryId, BookTitle, Author, Publisher, Quantity, Price, Status, Description)
VALUES (N'TL002', 4, N'Dám Bị Ghét', N'Ichiro Kishimi & Fumitake Koga', N'NXB Lao Động', 7, 118000, N'Còn sách', N'Triết học Adler — bí quyết sống tự do và hạnh phúc.');
GO
IF NOT EXISTS (SELECT * FROM Books WHERE BookId = 'TL003')
INSERT INTO Books (BookId, CategoryId, BookTitle, Author, Publisher, Quantity, Price, Status, Description)
VALUES (N'TL003', 4, N'Đời Ngắn Đừng Ngủ Dài', N'Robin Sharma', N'NXB Tổng Hợp TP.HCM', 5, 99000, N'Còn sách', N'Bộ quy tắc vàng để sống trọn vẹn và phát triển bản thân.');
GO
IF NOT EXISTS (SELECT * FROM Books WHERE BookId = 'KH001')
INSERT INTO Books (BookId, CategoryId, BookTitle, Author, Publisher, Quantity, Price, Status, Description)
VALUES (N'KH001', 5, N'Lược Sử Thời Gian', N'Stephen Hawking', N'NXB Trẻ', 4, 135000, N'Còn sách', N'Khám phá vũ trụ từ Big Bang đến lỗ đen bởi Stephen Hawking.');
GO
IF NOT EXISTS (SELECT * FROM Books WHERE BookId = 'KH002')
INSERT INTO Books (BookId, CategoryId, BookTitle, Author, Publisher, Quantity, Price, Status, Description)
VALUES (N'KH002', 5, N'Sapiens: Lược Sử Loài Người', N'Yuval Noah Harari', N'NXB Tri Thức', 6, 185000, N'Còn sách', N'Khám phá toàn bộ lịch sử loài người qua góc nhìn khoa học.');
GO
IF NOT EXISTS (SELECT * FROM Books WHERE BookId = 'KH003')
INSERT INTO Books (BookId, CategoryId, BookTitle, Author, Publisher, Quantity, Price, Status, Description)
VALUES (N'KH003', 5, N'Lập Trình Python Căn Bản', N'Nguyễn Thanh Bình', N'NXB Khoa Học Kỹ Thuật', 5, 155000, N'Còn sách', N'Giáo trình Python từ cơ bản đến nâng cao cho sinh viên.');
GO
IF NOT EXISTS (SELECT * FROM Books WHERE BookId = 'LS001')
INSERT INTO Books (BookId, CategoryId, BookTitle, Author, Publisher, Quantity, Price, Status, Description)
VALUES (N'LS001', 6, N'Lịch Sử Việt Nam Bằng Tranh - Tập 1', N'Trần Bạch Đằng', N'NXB Trẻ', 6, 45000, N'Còn sách', N'Bộ sách tranh tái hiện lịch sử dựng nước và giữ nước hào hùng.');
GO
IF NOT EXISTS (SELECT * FROM Books WHERE BookId = 'LS002')
INSERT INTO Books (BookId, CategoryId, BookTitle, Author, Publisher, Quantity, Price, Status, Description)
VALUES (N'LS002', 6, N'Địa Lý Các Vùng Kinh Tế Việt Nam', N'Lê Thông', N'NXB Giáo Dục', 3, 78000, N'Còn sách', N'Phân tích đặc điểm tự nhiên, kinh tế - xã hội các vùng kinh tế Việt Nam.');
GO
IF NOT EXISTS (SELECT * FROM Books WHERE BookId = 'LS003')
INSERT INTO Books (BookId, CategoryId, BookTitle, Author, Publisher, Quantity, Price, Status, Description)
VALUES (N'LS003', 6, N'Quốc Sử Quán Triều Nguyễn', N'Nhiều Tác Giả', N'NXB Thuận Hóa', 2, 220000, N'Còn sách', N'Tư liệu lịch sử quý giá từ triều Nguyễn ghi chép các sự kiện lịch sử.');
GO
