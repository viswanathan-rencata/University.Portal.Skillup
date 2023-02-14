DROP TABLE IF EXISTS UniversityPortal.[Notification]
DROP TABLE IF EXISTS UniversityPortal.[StudentDocument]
DROP TABLE IF EXISTS UniversityPortal.[SubjectResult]
DROP TABLE IF EXISTS UniversityPortal.[ExamResult]
DROP TABLE IF EXISTS UniversityPortal.[ExamSchedule]
DROP TABLE IF EXISTS UniversityPortal.[SubjectMaster]
DROP TABLE IF EXISTS UniversityPortal.[FeePayment]
DROP TABLE IF EXISTS UniversityPortal.[FeeDetails]
DROP TABLE IF EXISTS UniversityPortal.[AppUserRole]
DROP TABLE IF EXISTS UniversityPortal.[AppUser]
DROP TABLE IF EXISTS UniversityPortal.[Student]
DROP TABLE IF EXISTS UniversityPortal.[University]
DROP TABLE IF EXISTS UniversityPortal.[Role]
DROP TABLE IF EXISTS UniversityPortal.[Department]
DROP TABLE IF EXISTS UniversityPortal.[FeeMaster]
Drop TABLE IF EXISTS UniversityPortal.[DocumentMaster]  CREATE TABLE UniversityPortal.[DocumentMaster]
(
    ID INT IDENTITY(1,1) NOT NULL,
    DocCode VARCHAR(10) NOT NULL,    
    DocName VARCHAR(50) NOT NULL,    
    CreatedOn DATETIME NOT NULL
    PRIMARY KEY (ID)
) CREATE TABLE UniversityPortal.[FeeMaster]
(
    ID INT IDENTITY(1,1) NOT NULL,
    FeeType VARCHAR(30) NOT NULL,    
    CreatedOn DATETIME NOT NULL
    PRIMARY KEY (ID)
) CREATE TABLE UniversityPortal.[Department]
(
    ID INT IDENTITY(1,1) NOT NULL,
    DepartmentName VARCHAR(30) NOT NULL,    
    CreatedOn DATETIME NOT NULL
    PRIMARY KEY (ID)
) CREATE TABLE UniversityPortal.[Role]
(
    ID INT IDENTITY(1,1) NOT NULL,
    RoleName VARCHAR(30) NOT NULL,    
    CreatedOn DATETIME NOT NULL
    PRIMARY KEY (ID)
) CREATE TABLE UniversityPortal.[University]
(
    ID INT IDENTITY(1,1) NOT NULL,
    UniversityName VARCHAR(30) NOT NULL,    
    CreatedOn DATETIME NOT NULL
    PRIMARY KEY (ID)
) CREATE TABLE UniversityPortal.[Student]
(
    ID INT IDENTITY(1,1) NOT NULL,
    StudentCode VARCHAR(30) NULL,
    FirstName VARCHAR(30) NULL,    
    MiddleName VARCHAR(30) NULL,    
    LastName VARCHAR(30) NULL,    
    Gender CHAR(1) NULL,    
    Email VARCHAR(100) NULL,
    PhoneNumber VARCHAR(10) NULL,
    DOB Date NULL,
    DOJ Date NULL,
    UniversityId INT NULL, 
    DepartmentId INT NULL, 
    Year INT NULL, 
    [Status] BIT NOT NULL,    
    CreatedOn DATETIME NOT NULL,
    PRIMARY KEY (ID),
    CONSTRAINT FK_UnivId FOREIGN KEY (UniversityId) REFERENCES UniversityPortal.[University](ID),
    CONSTRAINT FK_DeptId FOREIGN KEY (DepartmentId) REFERENCES UniversityPortal.[Department](ID)
) CREATE TABLE UniversityPortal.[AppUser]
(
    ID INT IDENTITY(1,1) NOT NULL,
    UserName VARCHAR(20) NOT NULL,
    PasswordHash VARBINARY(max) NOT NULL,
    PasswordSalt VARBINARY(max) NOT NULL,
    StudentOrUniversity INT NOT NULL,
    UniversityId INT NULL,
    StudentId INT NULL,
    [Status] BIT NOT NULL,    
    [Email] varchar(100) NOT NULL,
    [PhoneNumber] varchar(15) NOT NULL,
    CreatedOn DATETIME NOT NULL    
    PRIMARY KEY (ID),
    CONSTRAINT FK_UniversityId FOREIGN KEY (UniversityId) REFERENCES UniversityPortal.[University](ID)
) CREATE TABLE UniversityPortal.[AppUserRole]
(
    ID INT IDENTITY(1,1) NOT NULL,
    AppUserID INT NOT NULL,    
    RoleID INT NOT NULL,    
    CreatedOn DATETIME NOT NULL
    PRIMARY KEY (ID),
    CONSTRAINT FK_AppUserID FOREIGN KEY (AppUserID) REFERENCES UniversityPortal.AppUser(ID),
    CONSTRAINT FK_RoleID FOREIGN KEY (RoleID) REFERENCES UniversityPortal.[Role](ID)
) CREATE TABLE UniversityPortal.[FeeDetails]
(
    ID INT IDENTITY(1,1) NOT NULL,
    FeeMasterId INT NOT NULL,
    UniversityId INT NOT NULL,
    DepartmentId INT NOT NULL, 
    Year INT NOT NULL,    
    Amount MONEY NOT NULL,    
    DueDate DATETIME NOT NULL,
    IsActive BIT NOT NULL,
    CreatedOn DATETIME NOT NULL
    PRIMARY KEY (ID),
    CONSTRAINT FK_TutionFee_UniversityId FOREIGN KEY (UniversityId) REFERENCES UniversityPortal.[University](ID),
    CONSTRAINT FK_TutionFee_DeptId FOREIGN KEY (DepartmentId) REFERENCES UniversityPortal.[Department](ID),
    CONSTRAINT FK_TutionFee_FeeMasterId FOREIGN KEY (FeeMasterId) REFERENCES UniversityPortal.[FeeMaster](ID)
) CREATE TABLE UniversityPortal.[FeePayment]
(
    ID INT IDENTITY(1,1) NOT NULL,
    FeeDetailsId INT NOT NULL,
    Amount MONEY NOT NULL,
    StudentID     INT NOT NULL,    
    PaymentDate DATE NOT NULL,
    PRIMARY KEY (ID),
    CONSTRAINT FK_FeeDetailsId FOREIGN KEY (FeeDetailsId) REFERENCES UniversityPortal.[FeeDetails](ID),
    CONSTRAINT FK_FeePayment_StudentID FOREIGN KEY (StudentID) REFERENCES UniversityPortal.Student(ID)    
) CREATE TABLE UniversityPortal.[SubjectMaster]
(
    ID INT IDENTITY(1,1) NOT NULL,
    UniversityId INT NOT NULL,
    DepartmentId INT NOT NULL,
    Year INT NOT NULL,
    SubjectCode VARCHAR(10) NOT NULL,    
    SubjectName VARCHAR(50) NOT NULL,    
    CreatedOn DATETIME NOT NULL
    PRIMARY KEY (ID)
    CONSTRAINT FK_SubjectMaster_UniversityId FOREIGN KEY (UniversityId) REFERENCES UniversityPortal.[University](ID),
    CONSTRAINT FK_SubjectMaster_DepartmentId FOREIGN KEY (DepartmentId) REFERENCES UniversityPortal.[Department](ID)
) CREATE TABLE UniversityPortal.[ExamSchedule]
(
    ID INT IDENTITY(1,1) NOT NULL,
    UniversityId INT NOT NULL,
    DepartmentId INT NOT NULL,
    Year INT NOT NULL,
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL,    
    IsActive BIT NOT NULL,
    PRIMARY KEY (ID),
    CONSTRAINT FK_ExamSchedule_DepartmentId FOREIGN KEY (DepartmentId) REFERENCES UniversityPortal.[Department](ID),
    CONSTRAINT FK_ExamSchedule_UniversityId FOREIGN KEY (UniversityId) REFERENCES UniversityPortal.[University](ID),
) CREATE TABLE UniversityPortal.[ExamResult]
(
    ID INT IDENTITY(1,1) NOT NULL,    
    StudentId INT NOT NULL,    
    ExamResult BIT NOT NULL,    
    CGPA decimal(10,2) NOT NULL,    
    IsActive BIT NOT NULL,
    PRIMARY KEY (ID),    
    CONSTRAINT FK_ExamResult_StudentID FOREIGN KEY (StudentID) REFERENCES UniversityPortal.Student(ID)    
) CREATE TABLE UniversityPortal.[SubjectResult]
(
    ID INT IDENTITY(1,1) NOT NULL,    
    StudentId INT NOT NULL,    
    SubjectMasterId INT NOT NULL,    
    ExamResult BIT NOT NULL,    
    Mark decimal(10,2) NOT NULL,    
    IsActive BIT NOT NULL,
    CreatedOn DATETIME NOT NULL
    PRIMARY KEY (ID),    
    CONSTRAINT FK_SubjectResult_SubjectMasterId FOREIGN KEY (SubjectMasterId) REFERENCES UniversityPortal.SubjectMaster(ID),    
    CONSTRAINT FK_SubjectResult_StudentID FOREIGN KEY (StudentID) REFERENCES UniversityPortal.Student(ID)    
) CREATE TABLE UniversityPortal.[StudentDocument]
(
    ID INT IDENTITY(1,1) NOT NULL,    
    StudentId INT NOT NULL,    
    DocumentMasterId INT NOT NULL,
    IsActive BIT NOT NULL,
	CreatedOn DATETIME NOT NULL,
    PRIMARY KEY (ID),    
    CONSTRAINT FK_StudentDocument_StudentID FOREIGN KEY (StudentID) REFERENCES UniversityPortal.Student(ID),    
    CONSTRAINT FK_StudentDocument_DocumentMasterId FOREIGN KEY (DocumentMasterId) REFERENCES UniversityPortal.DocumentMaster(ID)    
) 
CREATE TABLE UniversityPortal.[Notification]
(
    ID INT IDENTITY(1,1) NOT NULL,
    StudentOrUniversity INT NOT NULL,
    UniversityId INT NULL,
    StudentID     INT NULL,    
    Message VARCHAR(max) NOT NULL,
    CreatedOn DATETIME NOT NULL
    PRIMARY KEY (ID),
    CONSTRAINT FK_Notification_StudentID FOREIGN KEY (StudentID) REFERENCES UniversityPortal.Student(ID)    
) SET IDENTITY_INSERT UniversityPortal.[DocumentMaster] ON INSERT INTO UniversityPortal.[DocumentMaster] (ID, DocCode,DocName, CreatedOn) Values (1,'DOC_001','Hall Ticket', GETDATE())
INSERT INTO UniversityPortal.[DocumentMaster] (ID, DocCode,DocName, CreatedOn) Values (2,'DOC_002','Mark Sheet', GETDATE())
INSERT INTO UniversityPortal.[DocumentMaster] (ID, DocCode,DocName, CreatedOn) Values (3,'DOC_003','Provisional Certificate', GETDATE())
INSERT INTO UniversityPortal.[DocumentMaster] (ID, DocCode,DocName, CreatedOn) Values (4,'DOC_004','Degree Certificate', GETDATE()) SET IDENTITY_INSERT UniversityPortal.[DocumentMaster] OFF SET IDENTITY_INSERT UniversityPortal.[Role] ON INSERT INTO UniversityPortal.[Role] (ID, RoleName, CreatedOn) Values (1,'OfficeAdmin', GETDATE())
INSERT INTO UniversityPortal.[Role] (ID, RoleName, CreatedOn) Values (2,'ExaminationCoordinator', GETDATE()) SET IDENTITY_INSERT UniversityPortal.[Role] OFF SET IDENTITY_INSERT UniversityPortal.[Department] ON INSERT INTO UniversityPortal.[Department] (ID, DepartmentName, CreatedOn) Values (1,'CIVIL', GETDATE())
INSERT INTO UniversityPortal.[Department] (ID, DepartmentName, CreatedOn) Values (2,'ELECTRICAL', GETDATE())
INSERT INTO UniversityPortal.[Department] (ID, DepartmentName, CreatedOn) Values (3,'ELECTRONICS', GETDATE())
INSERT INTO UniversityPortal.[Department] (ID, DepartmentName, CreatedOn) Values (4,'COMPUTER', GETDATE())
INSERT INTO UniversityPortal.[Department] (ID, DepartmentName, CreatedOn) Values (5,'MECHANICAL', GETDATE()) SET IDENTITY_INSERT UniversityPortal.[Department] OFF SET IDENTITY_INSERT UniversityPortal.[FeeMaster] ON INSERT INTO UniversityPortal.[FeeMaster] (ID, FeeType, CreatedOn) Values (1,'TutionFee', GETDATE())
INSERT INTO UniversityPortal.[FeeMaster] (ID, FeeType, CreatedOn) Values (2,'ExamFee', GETDATE()) SET IDENTITY_INSERT UniversityPortal.[FeeMaster] OFF

select * from UniversityPortal.Role;


update UniversityPortal.[StudentDocument]
set CreatedOn = GETDATE()
where id = 2;

alter table UniversityPortal.[StudentDocument]
add CreatedOn DATETIME

select * from UniversityPortal.Student

select * from UniversityPortal.notification

select * from UniversityPortal.ExamSchedule

