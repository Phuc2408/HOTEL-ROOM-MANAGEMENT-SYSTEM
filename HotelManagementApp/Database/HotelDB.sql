DROP DATABASE IF EXISTS HotelDB;
GO

CREATE DATABASE HotelDB;
GO
USE HotelDB;
GO

-- Drop tables to avoid foreign key conflict on re-run
DROP TABLE IF EXISTS ServiceUsage;
DROP TABLE IF EXISTS Invoice;
DROP TABLE IF EXISTS Service;
DROP TABLE IF EXISTS Rent;
DROP TABLE IF EXISTS Customer;
DROP TABLE IF EXISTS Room;

-- Bảng Room
CREATE TABLE Room (
    RID INT PRIMARY KEY IDENTITY(1,1),
    RType NVARCHAR(50),
    RStatus NVARCHAR(20),
    RPrice DECIMAL(10,2),
    Rfloor INT
);

-- Bảng Customer
CREATE TABLE Customer (
    CID INT PRIMARY KEY IDENTITY(1,1),
    CName NVARCHAR(100),
    CPhone NVARCHAR(20),
    CPersonalID NVARCHAR(20),
    Cmail NVARCHAR(100),
    Ccountry NVARCHAR(50)
);

-- Bảng Rent
CREATE TABLE Rent (
    ReID INT PRIMARY KEY IDENTITY(1,1),
    RID INT,
    CID INT,
    CheckInDate DATE,
    CheckOutDate DATE,
    CheckInTime TIME,
    CheckOutTime TIME,
    NumberOfPeople INT,
    isDone BIT DEFAULT 0,
    FOREIGN KEY (RID) REFERENCES Room(RID),
    FOREIGN KEY (CID) REFERENCES Customer(CID)
);

-- Bảng Service
CREATE TABLE Service (
    SID INT PRIMARY KEY IDENTITY(1,1),
    SName NVARCHAR(100),
    SUnit NVARCHAR(20),
    SPrice DECIMAL(10,2)
);

-- Bảng ServiceUsage
CREATE TABLE ServiceUsage (
    UID INT PRIMARY KEY IDENTITY(1,1),
    SID INT,
    ReID INT,
    Quantity INT,
    TotalPerService DECIMAL(10,2),
    FOREIGN KEY (SID) REFERENCES Service(SID),
    FOREIGN KEY (ReID) REFERENCES Rent(ReID)
);

-- Bảng Invoice
CREATE TABLE Invoice (
    IID INT PRIMARY KEY IDENTITY(1,1),
    ReID INT,
    InvoiceDate DATE,
    RoomTotal DECIMAL(10,2),
    ServiceTotal DECIMAL(10,2),
    Total DECIMAL(10,2),
    FOREIGN KEY (ReID) REFERENCES Rent(ReID)
);


SET IDENTITY_INSERT Room ON;
INSERT INTO Room (RID, RType, RStatus, RPrice, RFloor)
VALUES
--tầng 1
(101, N'single', N'in_use', 400000, 1),
(102, N'vip', N'available', 1200000, 1),
(103, N'vip', N'available', 1200000, 1),
(104, N'vip', N'repairing', 1200000, 1),
(105, N'double', N'cleaning', 800000, 1),
(106, N'single', N'repairing', 400000, 1),
(107, N'single', N'cleaning', 400000, 1),
(108, N'vip', N'available', 1200000, 1),
(109, N'double', N'cleaning', 800000, 1),
(110, N'single', N'available', 400000, 1),
(111, N'single', N'repairing', 400000, 1),
(112, N'single', N'cleaning', 400000, 1),
(113, N'single', N'repairing', 400000, 1),
(114, N'twin', N'available', 600000, 1),
(115, N'double', N'in_use', 800000, 1),
(116, N'vip', N'in_use', 1200000, 1),
(117, N'double', N'cleaning', 800000, 1),
(118, N'twin', N'cleaning', 600000, 1),
(119, N'vip', N'repairing', 1200000, 1),
(120, N'vip', N'cleaning', 1200000, 1),
(121, N'single', N'in_use', 400000, 1),
(122, N'single', N'in_use', 400000, 1),
(123, N'double', N'available', 800000, 1),
(124, N'twin', N'available', 600000, 1),
(125, N'double', N'repairing', 800000, 1),
(126, N'vip', N'repairing', 1200000, 1),
(127, N'single', N'available', 400000, 1),
(128, N'single', N'cleaning', 400000, 1),
--tầng 2
(201, N'single', N'repairing', 400000, 2),
(202, N'double', N'in_use', 800000, 2),
(203, N'vip', N'in_use', 1200000, 2),
(204, N'single', N'in_use', 400000, 2),
(205, N'double', N'in_use', 800000, 2),
(206, N'single', N'repairing', 400000, 2),
(207, N'double', N'repairing', 800000, 2),
(208, N'double', N'in_use', 800000, 2),
(209, N'double', N'available', 800000, 2),
(210, N'twin', N'cleaning', 600000, 2),
(211, N'single', N'repairing', 400000, 2),
(212, N'double', N'cleaning', 800000, 2),
(213, N'single', N'repairing', 400000, 2),
(214, N'twin', N'available', 600000, 2),
(215, N'vip', N'available', 1200000, 2),
(216, N'double', N'in_use', 800000, 2),
(217, N'twin', N'in_use', 600000, 2),
(218, N'single', N'available', 400000, 2),
(219, N'single', N'in_use', 400000, 2),
(220, N'vip', N'available', 1200000, 2),
(221, N'double', N'in_use', 800000, 2),
(222, N'double', N'in_use', 800000, 2),
(223, N'vip', N'cleaning', 1200000, 2),
(224, N'twin', N'cleaning', 600000, 2),
(225, N'double', N'in_use', 800000, 2),
(226, N'vip', N'available', 1200000, 2),
(227, N'single', N'available', 400000, 2),
(228, N'vip', N'in_use', 1200000, 2),
--tầng 3
(301, N'double', N'in_use', 800000, 3),
(302, N'vip', N'in_use', 1200000, 3),
(303, N'double', N'in_use', 800000, 3),
(304, N'twin', N'in_use', 600000, 3),
(305, N'single', N'in_use', 400000, 3),
(306, N'vip', N'in_use', 1200000, 3),
(307, N'double', N'in_use', 800000, 3),
(308, N'twin', N'in_use', 600000, 3),
(309, N'single', N'in_use', 400000, 3),
(310, N'single', N'in_use', 400000, 3),
(311, N'single', N'in_use', 400000, 3),
(312, N'double', N'in_use', 800000, 3),
(313, N'vip', N'in_use', 1200000, 3),
(314, N'vip', N'in_use', 1200000, 3),
(315, N'twin', N'in_use', 600000, 3),
(316, N'vip', N'in_use', 1200000, 3),
(317, N'double', N'in_use', 800000, 3),
(318, N'vip', N'in_use', 1200000, 3),
(319, N'single', N'in_use', 400000, 3),
(320, N'twin', N'in_use', 600000, 3),
(321, N'single', N'in_use', 400000, 3),
(322, N'vip', N'in_use', 1200000, 3),
(323, N'twin', N'in_use', 600000, 3),
(324, N'twin', N'in_use', 600000, 3),
(325, N'vip', N'in_use', 1200000, 3),
(326, N'vip', N'in_use', 1200000, 3),
(327, N'double', N'in_use', 800000, 3),
(328, N'double', N'in_use', 800000, 3),
-- Tầng 4
(401, N'twin', N'available', 600000, 4),
(402, N'twin', N'available', 600000, 4),
(403, N'double', N'available', 800000, 4),
(404, N'vip', N'available', 1200000, 4),
(405, N'twin', N'available', 600000, 4),
(406, N'single', N'available', 400000, 4),
(407, N'twin', N'available', 600000, 4),
(408, N'vip', N'available', 1200000, 4),
(409, N'vip', N'available', 1200000, 4),
(410, N'single', N'available', 400000, 4),
(411, N'double', N'available', 800000, 4),
(412, N'double', N'available', 800000, 4),
(413, N'single', N'available', 400000, 4),
(414, N'double', N'available', 800000, 4),
(415, N'twin', N'available', 600000, 4),
(416, N'vip', N'available', 1200000, 4),
(417, N'double', N'available', 800000, 4),
(418, N'twin', N'available', 600000, 4),
(419, N'single', N'available', 400000, 4),
(420, N'vip', N'available', 1200000, 4),
(421, N'single', N'available', 400000, 4),
(422, N'single', N'available', 400000, 4),
(423, N'vip', N'available', 1200000, 4),
(424, N'twin', N'available', 600000, 4),
(425, N'single', N'available', 400000, 4),
(426, N'twin', N'available', 600000, 4),
(427, N'double', N'available', 800000, 4),
(428, N'vip', N'available', 1200000, 4),
-- Tầng 5
(501, N'double', N'available', 800000, 5),
(502, N'double', N'available', 800000, 5),
(503, N'single', N'available', 400000, 5),
(504, N'vip', N'available', 1200000, 5),
(505, N'twin', N'available', 600000, 5),
(506, N'vip', N'available', 1200000, 5),
(507, N'twin', N'available', 600000, 5),
(508, N'twin', N'available', 600000, 5),
(509, N'single', N'available', 400000, 5),
(510, N'twin', N'available', 600000, 5),
(511, N'vip', N'available', 1200000, 5),
(512, N'twin', N'available', 600000, 5),
(513, N'twin', N'available', 600000, 5),
(514, N'vip', N'available', 1200000, 5),
(515, N'twin', N'available', 600000, 5),
(516, N'vip', N'available', 1200000, 5),
(517, N'double', N'available', 800000, 5),
(518, N'twin', N'available', 600000, 5),
(519, N'single', N'available', 400000, 5),
(520, N'single', N'available', 400000, 5),
(521, N'vip', N'available', 1200000, 5),
(522, N'single', N'available', 400000, 5),
(523, N'double', N'available', 800000, 5),
(524, N'single', N'available', 400000, 5),
(525, N'twin', N'available', 600000, 5),
(526, N'twin', N'available', 600000, 5),
(527, N'single', N'available', 400000, 5),
(528, N'single', N'available', 400000, 5);

GO

INSERT INTO Service (SName, SUnit, SPrice)
VALUES
(N'Extra Bed', N'night', 200000),
(N'Late Checkout', N'hour', 100000),
(N'Early Check-in', N'hour', 100000),
(N'Printing Documents', N'page', 3000),
(N'Photocopying', N'page', 2000),
(N'Meeting Room Rental', N'hour', 400000),
(N'Babysitting Service', N'hour', 250000),
(N'Express Laundry', N'item', 80000),
(N'City Tour Booking', N'tour', 600000),
(N'Shoe Shine', N'pair', 30000),
(N'Room Decoration', N'request', 200000),
(N'Anniversary Package', N'package', 700000),
(N'Birthday Cake', N'cake', 300000),
(N'Flower Delivery', N'bouquet', 250000),
(N'Ironing Service', N'item', 40000),
(N'Pet Sitting', N'hour', 150000),
(N'Breakfast in Bed', N'portion', 100000);
GO
-- CHÈN DỮ LIỆU KHÁCH HÀNG (KHÔNG chỉ định CID vì là IDENTITY)
INSERT INTO Customer (CName, CPhone, CPersonalID, CMail, CCountry) VALUES
(N'Nguyễn Văn An', '0324222770', '129699576532', 'mnavarro@gmail.com', 'Vietnam'),
(N'Lê Thị Bình', '0326587983', '409640843367', 'wendygraham@gmail.com', 'Vietnam'),
(N'Trần Văn Cường', '0327688072', '691984438919', 'qmiller@gmail.com', 'Vietnam'),
(N'Phạm Thị Dung', '0332805396', '560636519096', 'andrea47@gmail.com', 'Vietnam'),
(N'Hoàng Văn Em', '0336302138', '936852633062', 'pvalentine@gmail.com', 'Vietnam'),
(N'Đặng Thị Phúc', '0338907477', '829990176814', 'mlee@gmail.com', 'Vietnam'),
(N'Bùi Văn Giang', '0351198700', '265989405528', 'jreed@gmail.com', 'Vietnam'),
(N'Vũ Thị Hiền', '0352230141', '675703288538', 'tylerjohnson@gmail.com', 'Vietnam'),
(N'Ngô Văn Ích', '0353301862', '709003870742', 'vsantos@gmail.com', 'Vietnam'),
(N'Dương Thị Giang', '0354607301', '870093223239', 'heatherberger@gmail.com', 'Vietnam'),
(N'Lương Văn Khoa', '0354791182', '643194608808', 'michaeljones@gmail.com', 'Vietnam'),
(N'Nguyễn Thị Hồng', '0365212821', '234045062862', 'lbyrd@gmail.com', 'Vietnam'),
(N'Phạm Văn Nam', '0367678572', '286535962262', 'glowe@gmail.com', 'Vietnam'),
(N'Bùi Thị Hoa', '0376879943', '755906156243', 'xcarr@gmail.com', 'Vietnam'),
(N'Lê Văn Phú', '0389296225', '746978396814', 'osbornejeffery@gmail.com', 'Vietnam'),
(N'Hoàng Thị Minh', '0389477582', '811596191948', 'usalazar@gmail.com', 'Vietnam'),
(N'Trần Văn Tiến', '0394604656', '311593615101', 'jimmy55@gmail.com', 'Vietnam'),
(N'Ngô Thị Mai', '0395310864', '200016021541', 'nperry@gmail.com', 'Vietnam'),
(N'Đặng Văn Lâm', '0398134117', '163291531936', 'joshuatucker@gmail.com', 'Vietnam'),
(N'Vũ Thị Lan', '0809531373', '285784434015', 'qwhite@gmail.com', 'Vietnam'),
(N'Nguyễn Minh Tuấn', '0816051892', '960623968935', 'debraharrington@gmail.com', 'Vietnam'),
(N'Phan Thị Ngọc', '0816329619', '232146200801', 'katie29@gmail.com', 'Vietnam'),
(N'Hồ Văn Long', '0821705027', '673845809750', 'curtisbarton@gmail.com', 'Vietnam'),
(N'Đỗ Thị Hạnh', '0823031553', '219548672278', 'cynthia72@gmail.com', 'Vietnam'),
(N'Trịnh Văn Khánh', '0823326041', '492801294807', 'qhudson@gmail.com', 'Vietnam'),
(N'Lâm Thị Xuân', '0828408152', '877075030821', 'christophercortez@gmail.com', 'Vietnam'),
(N'Mai Văn Hùng', '0828811531', '775571619117', 'ronald85@gmail.com', 'Vietnam'),
(N'Dương Thị Diễm', '0831706321', '887224917394', 'lhill@gmail.com', 'Vietnam'),
(N'Kiều Văn Hoàng', '0832594070', '340978382620', 'heidi27@gmail.com', 'Vietnam'),
(N'Tống Thị Thảo', '0839925928', '229505591630', 'shortrichard@gmail.com', 'Vietnam'),
(N'Triệu Văn Bình', '0841090385', '366096116940', 'yobrien@gmail.com', 'Vietnam'),
(N'Vương Thị Duyên', '0846570171', '243641476636', 'mezajared@gmail.com', 'Vietnam'),
(N'Cao Văn Thành', '0857000234', '814371432396', 'carlos88@gmail.com', 'Vietnam'),
(N'Lữ Thị Phượng', '0861346859', '208112074056', 'dennis58@gmail.com', 'Vietnam'),
(N'Nguyễn Văn Bảo', '0865253884', '484856586384', 'ihays@gmail.com', 'Vietnam'),
(N'Lê Thị Thảo', '0866083328', '900546540880', 'tylerjimenez@gmail.com', 'Vietnam'),
(N'Trần Văn Đông', '0866800959', '965971905070', 'cjackson@gmail.com', 'Vietnam'),
(N'Phạm Thị Hường', '0868121309', '965484731164', 'ejohnson@gmail.com', 'Vietnam'),
(N'Hoàng Văn Trí', '0878040709', '265850595554', 'berryalyssa@gmail.com', 'Vietnam'),
(N'Đặng Thị Linh', '0881625140', '681846172736', 'heather73@gmail.com', 'Vietnam'),
(N'Bùi Văn Khải', '0888288690', '646752311074', 'patricia22@gmail.com', 'Vietnam'),
(N'Vũ Thị Yến', '0893417500', '610016839610', 'cherylmiller@gmail.com', 'Vietnam'),
(N'Ngô Văn Đức', '0894555793', '706203924771', 'katrinaanderson@gmail.com', 'Vietnam'),
(N'Dương Thị Thắm', '0904812665', '888373219730', 'david49@gmail.com', 'Vietnam'),
(N'Đỗ Văn Toàn', '0905786525', '640130045949', 'bryananthony@gmail.com', 'Vietnam'),
(N'Phan Thị Hòa', '0907489254', '363260563541', 'timothy64@gmail.com', 'Vietnam'),
(N'Hồ Văn Kỳ', '0911874777', '838722287204', 'theresa25@gmail.com', 'Vietnam'),
(N'Tô Thị Tươi', '0913713037', '849878093642', 'sydneybarrera@gmail.com', 'Vietnam'),
(N'Đinh Văn Hoàng', '0936697964', '410273586633', 'brandywilliams@gmail.com', 'Vietnam'),
(N'Lâm Thị Hằng', '0942947946', '515394835598', 'hudsonjames@gmail.com', 'Vietnam'),
(N'Trịnh Văn Bích', '0967871648', '709734678374', 'longlaura@gmail.com', 'Vietnam'),
(N'Hà Thị Tuyết', '0977325395', '913861833054', 'catherinerobertson@gmail.com', 'Vietnam'),
(N'Nguyễn Văn Bình', '0978248204', '700866211908', 'user53@gmail.com', 'Vietnam'),
(N'Lê Văn Thành', '0986191942', '591913052169', 'user54@gmail.com', 'Vietnam');

--Chèn dữ liệu bảng thuê phòng
INSERT INTO Rent (RID, CID, CheckInDate, CheckOutDate, CheckInTime, CheckOutTime, NumberOfPeople, isDone)
VALUES
(101, 10, '2025-06-05', '2025-06-07', '14:00', '12:00', 2, 0),
(115, 5, '2025-06-05', '2025-06-07', '14:00', '12:00', 2, 0),
(116, 21, '2025-06-05', '2025-06-07', '14:00', '12:00', 2, 0),
(121, 14, '2025-06-05', '2025-06-08', '14:00', '12:00', 2, 0),
(122, 30, '2025-06-05', '2025-06-07', '14:00', '12:00', 2, 0),
(202, 2, '2025-06-05', '2025-06-09', '14:00', '12:00', 2, 0),
(203, 41, '2025-06-05', '2025-06-07', '14:00', '12:00', 2, 0),
(204, 26, '2025-06-05', '2025-06-07', '14:00', '12:00', 2, 0),
(205, 41, '2025-06-05', '2025-06-07', '14:00', '12:00', 2, 0),
(208, 41, '2025-06-05', '2025-06-08', '14:00', '12:00', 2, 0),
(216, 30, '2025-06-05', '2025-06-07', '14:00', '12:00', 2, 0),
(217, 13, '2025-06-05', '2025-06-07', '14:00', '12:00', 2, 0),
(219, 36, '2025-06-05', '2025-06-09', '14:00', '12:00', 2, 0),
(221, 16, '2025-06-05', '2025-06-07', '14:00', '12:00', 2, 0),
(222, 35, '2025-06-05', '2025-06-07', '14:00', '12:00', 2, 0),
(225, 27, '2025-06-05', '2025-06-07', '14:00', '12:00', 2, 0),
(228, 31, '2025-06-05', '2025-06-07', '14:00', '12:00', 2, 0),
(301, 35, '2025-06-05', '2025-06-07', '14:00', '12:00', 2, 0),
(302, 50, '2025-06-05', '2025-06-07', '14:00', '12:00', 2, 0),
(303, 46, '2025-06-05', '2025-06-07', '14:00', '12:00', 2, 0),
(304, 36, '2025-06-05', '2025-06-07', '14:00', '12:00', 2, 0),
(305, 32, '2025-06-05', '2025-06-07', '14:00', '12:00', 2, 0),
(306, 10, '2025-06-05', '2025-06-07', '14:00', '12:00', 2, 0),
(307, 29, '2025-06-05', '2025-06-07', '14:00', '12:00', 2, 0),
(308, 6, '2025-06-05', '2025-06-07', '14:00', '12:00', 2, 0),
(309, 19, '2025-06-05', '2025-06-07', '14:00', '12:00', 2, 0),
(310, 16, '2025-06-05', '2025-06-07', '14:00', '12:00', 2, 0),
(311, 31, '2025-06-05', '2025-06-07', '14:00', '12:00', 2, 0),
(312, 43, '2025-06-05', '2025-06-07', '14:00', '12:00', 2, 0),
(313, 4, '2025-06-05', '2025-06-07', '14:00', '12:00', 2, 0),
(314, 9, '2025-06-05', '2025-06-07', '14:00', '12:00', 2, 0),
(315, 13, '2025-06-05', '2025-06-07', '14:00', '12:00', 2, 0),
(316, 14, '2025-06-05', '2025-06-07', '14:00', '12:00', 2, 0),
(317, 41, '2025-06-05', '2025-06-07', '14:00', '12:00', 2, 0),
(318, 17, '2025-06-05', '2025-06-07', '14:00', '12:00', 2, 0),
(319, 30, '2025-06-05', '2025-06-07', '14:00', '12:00', 2, 0),
(320, 42, '2025-06-05', '2025-06-07', '14:00', '12:00', 2, 0),
(321, 2, '2025-06-05', '2025-06-07', '14:00', '12:00', 2, 0),
(322, 20, '2025-06-05', '2025-06-07', '14:00', '12:00', 2, 0),
(323, 11, '2025-06-05', '2025-06-07', '14:00', '12:00', 2, 0),
(324, 48, '2025-06-05', '2025-06-07', '14:00', '12:00', 2, 0),
(325, 27, '2025-06-05', '2025-06-07', '14:00', '12:00', 2, 0),
(326, 18, '2025-06-05', '2025-06-07', '14:00', '12:00', 2, 0),
(327, 22, '2025-06-05', '2025-06-07', '14:00', '12:00', 2, 0),
(328, 1, '2025-06-05', '2025-06-07', '14:00', '12:00', 2, 0);

Go

