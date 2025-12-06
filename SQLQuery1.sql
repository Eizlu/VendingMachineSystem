-- 1. Tabulka Uživatelů (Administrátor, Provozovatel, Skladník, Pracovník)
CREATE TABLE Uzivatel (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Jmeno NVARCHAR(100) NOT NULL,
    Login NVARCHAR(50) UNIQUE NOT NULL,
    Heslo NVARCHAR(100) NOT NULL, -- V reálu hash, pro školu stačí text
    Role NVARCHAR(50) NOT NULL -- 'Admin', 'Provozovatel', 'Skladnik', 'Pracovnik'
);

-- 2. Tabulka Automatů
CREATE TABLE Automat (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Lokalita NVARCHAR(200) NOT NULL,
    Stav NVARCHAR(50) NOT NULL, -- 'Online', 'Offline', 'Porucha'
    Typ NVARCHAR(50) NOT NULL -- 'Nápoje', 'Jídlo', 'Mix'
);

-- 3. Tabulka Produktů
CREATE TABLE Produkt (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nazev NVARCHAR(100) NOT NULL,
    Cena DECIMAL(18, 2) NOT NULL,
    Kategorie NVARCHAR(50),
    Ean NVARCHAR(50)
);

-- 4. Vazební tabulka: Zásoby v automatu (M:N vztah mezi Automatem a Produktem)
CREATE TABLE ZasobaAutomatu (
    Id INT PRIMARY KEY IDENTITY(1,1),
    AutomatId INT NOT NULL FOREIGN KEY REFERENCES Automat(Id),
    ProduktId INT NOT NULL FOREIGN KEY REFERENCES Produkt(Id),
    Mnozstvi INT NOT NULL DEFAULT 0,
    MinimaleLimit INT NOT NULL DEFAULT 5, -- Hranice pro notifikaci
    DatumPoslednihoDoplneni DATETIME
);

-- 5. Zásoby na centrálním skladě
CREATE TABLE ZasobaSkladu (
    Id INT PRIMARY KEY IDENTITY(1,1),
    ProduktId INT NOT NULL FOREIGN KEY REFERENCES Produkt(Id),
    Mnozstvi INT NOT NULL DEFAULT 0
);

-- 6. Objednávky (hlavička)
CREATE TABLE Objednavka (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Dodavatel NVARCHAR(100),
    DatumVytvoreni DATETIME DEFAULT GETDATE(),
    Stav NVARCHAR(50) NOT NULL -- 'Nova', 'Schvalena', 'Odeslana', 'Dorucena'
);

-- 7. Položky objednávky
CREATE TABLE PolozkaObjednavky (
    Id INT PRIMARY KEY IDENTITY(1,1),
    ObjednavkaId INT NOT NULL FOREIGN KEY REFERENCES Objednavka(Id),
    ProduktId INT NOT NULL FOREIGN KEY REFERENCES Produkt(Id),
    Mnozstvi INT NOT NULL
);

-- 8. Prodeje (pro statistiky a reporty)
CREATE TABLE Prodej (
    Id INT PRIMARY KEY IDENTITY(1,1),
    AutomatId INT NOT NULL FOREIGN KEY REFERENCES Automat(Id),
    ProduktId INT NOT NULL FOREIGN KEY REFERENCES Produkt(Id),
    Cena DECIMAL(18, 2) NOT NULL,
    DatumProdeje DATETIME DEFAULT GETDATE()
);

-- VLOŽENÍ TESTOVACÍCH DAT (abychom měli s čím pracovat)
INSERT INTO Uzivatel (Jmeno, Login, Heslo, Role) VALUES 
('Jan Novák', 'admin', 'admin123', 'Administrator'),
('Petr Skladník', 'sklad', 'sklad123', 'Skladnik'),
('Eva Servisní', 'servis', 'servis123', 'Pracovnik');

INSERT INTO Produkt (Nazev, Cena, Kategorie) VALUES 
('Cola 0.5l', 35.00, 'Napoj'),
('Voda 0.5l', 20.00, 'Napoj'),
('Horalky', 15.00, 'Jidlo');

INSERT INTO Automat (Lokalita, Stav, Typ) VALUES 
('Hlavní nádraží', 'Online', 'Mix'),
('Univerzita VŠB', 'Online', 'Napoje');

-- Naskladnění do automatu 1
INSERT INTO ZasobaAutomatu (AutomatId, ProduktId, Mnozstvi, MinimaleLimit) VALUES 
(1, 1, 2, 5),  -- Málo Coly (pod limitem) -> Měl by hlásit doplnění
(1, 2, 20, 5), -- Dost Vody
(1, 3, 10, 5); -- Dost Horalek