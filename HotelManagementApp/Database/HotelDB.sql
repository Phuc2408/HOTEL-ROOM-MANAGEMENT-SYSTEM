DROP DATABASE IF EXISTS HotelDB;
GO

CREATE DATABASE HotelDB;
GO
USE HotelDB;
GO

-- TẠO BẢNG KHÁCH HÀNG
CREATE TABLE Customer (
    CID INT PRIMARY KEY IDENTITY(1,1),
    CName NVARCHAR(100),
    CPhone NVARCHAR(20),
    CPersonalID NVARCHAR(20),
    CMail NVARCHAR(100),
    CCountry NVARCHAR(50)
);

-- TẠO BẢNG PHÒNG
CREATE TABLE Room (
    RID INT PRIMARY KEY IDENTITY(1,1),
    RType NVARCHAR(50),
    RStatus NVARCHAR(20),
    RPrice DECIMAL(10, 2),
    RFloor INT
);

-- TẠO BẢNG THUÊ PHÒNG (RENT)
CREATE TABLE Rent (
    RelID INT PRIMARY KEY IDENTITY(1,1),
    RID INT FOREIGN KEY REFERENCES Room(RID),
    CID INT FOREIGN KEY REFERENCES Customer(CID),
    CheckInDate DATE,
    CheckOutDate DATE,
    CheckInTime TIME,
    CheckOutTime TIME,
    NumberOfPeople INT
);

-- TẠO BẢNG HÓA ĐƠN
CREATE TABLE Invoice (
    IID INT PRIMARY KEY IDENTITY(1,1),
    CID INT FOREIGN KEY REFERENCES Customer(CID),
    RelID INT FOREIGN KEY REFERENCES Rent(RelID),
    IDate DATE,
    RoomTotal DECIMAL(10,2),
    ServiceTotal DECIMAL(10,2),
    Total DECIMAL(10,2)
);

-- TẠO BẢNG DỊCH VỤ
CREATE TABLE Service (
    SID INT PRIMARY KEY IDENTITY(1,1),
    SName NVARCHAR(100),
    SUnit NVARCHAR(20),
    SPrice DECIMAL(10,2)
);

-- TẠO BẢNG SỬ DỤNG DỊCH VỤ
CREATE TABLE ServiceUsage (
    UID INT PRIMARY KEY IDENTITY(1,1),
    SID INT FOREIGN KEY REFERENCES Service(SID),
    CID INT FOREIGN KEY REFERENCES Customer(CID),
    IID INT FOREIGN KEY REFERENCES Invoice(IID),
    Quantity INT,
    ServiceTotal DECIMAL(10,2)
);
INSERT INTO Customer (CName, CPhone, CPersonalID, CMail, CCountry)
VALUES 
(N'Nguyễn Văn A', '0901234567', '123456789', 'vana@gmail.com', 'Vietnam'),
(N'Lê Thị B', '0912345678', '987654321', 'leb@gmail.com', 'Vietnam');
GO