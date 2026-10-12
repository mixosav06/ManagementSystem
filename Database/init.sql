-- 1. Таблиця працівників парковки (Users -> Attendants)
CREATE TABLE IF NOT EXISTS Attendants (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Email TEXT NOT NULL UNIQUE,
    PasswordHash TEXT NOT NULL
);

-- 2. Таблиця паркувальних місць (Items -> ParkingSpots)
CREATE TABLE IF NOT EXISTS ParkingSpots (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    Number TEXT NOT NULL,
    Status TEXT NOT NULL DEFAULT 'Free',
    UserID INTEGER,
    FOREIGN KEY (UserID) REFERENCES Attendants(ID) ON DELETE SET NULL
);

-- 3. Таблиця бронювань (Actions -> Reservations)
CREATE TABLE IF NOT EXISTS Reservations (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    UserID INTEGER NOT NULL,
    SpotID INTEGER NOT NULL,
    ReservationDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Status TEXT NOT NULL DEFAULT 'Active',
    FOREIGN KEY (UserID) REFERENCES Attendants(ID) ON DELETE CASCADE,
    FOREIGN KEY (SpotID) REFERENCES ParkingSpots(ID) ON DELETE CASCADE
);

-- Вставка 2-3 тестових записів
INSERT INTO Attendants (Name, Email, PasswordHash) VALUES 
('Іван Петренко', 'attendant1@parking.com', '$2a$11$e8.10u3oVqj/O'),
('Олена Коваль', 'attendant2@parking.com', '$2a$11$e8.10u3oVqj/O');

INSERT INTO ParkingSpots (Number, Status, UserID) VALUES 
('A-101', 'Free', NULL),
('A-102', 'Occupied', 1),
('B-201', 'Free', NULL);

INSERT INTO Reservations (UserID, SpotID, ReservationDate, Status) VALUES 
(1, 2, CURRENT_TIMESTAMP, 'Active');