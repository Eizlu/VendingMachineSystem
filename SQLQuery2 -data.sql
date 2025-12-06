-- 1. ÚKLID (Smažeme stará data a resetujeme počítadla ID)
-- Musíme mazat v přesném pořadí kvůli cizím klíčům
DELETE FROM PolozkaObjednavky;
DELETE FROM Objednavka;
DELETE FROM Prodej;
DELETE FROM ZasobaAutomatu;
DELETE FROM ZasobaSkladu;
DELETE FROM Automat;
DELETE FROM Produkt;
DELETE FROM Uzivatel;

-- Resetování ID na 1 (aby číslování začínalo zase od jedničky)
DBCC CHECKIDENT ('Automat', RESEED, 0);
DBCC CHECKIDENT ('Produkt', RESEED, 0);
DBCC CHECKIDENT ('Uzivatel', RESEED, 0);
DBCC CHECKIDENT ('ZasobaAutomatu', RESEED, 0);
DBCC CHECKIDENT ('Objednavka', RESEED, 0);

-- 2. IMPORT UŽIVATELŮ (Mapujeme staré 'spravce' na nové 'Uzivatel')
-- Data převzata z
INSERT INTO Uzivatel (Jmeno, Login, Heslo, Role) VALUES 
('Adam Novák', 'adam', 'admin123', 'Administrator'), -- Původní ID 1
('Jan Pospíšil', 'jan', 'jan123', 'Skladnik'),      -- Původní ID 2
('Dana Kalusová', 'dana', 'dana123', 'Provozovatel'),-- Původní ID 3
('Marie Pastorková', 'marie', 'marie123', 'Pracovnik'), -- Původní ID 4
('Jakub Fitřík', 'jakub', 'jakub123', 'Pracovnik');  -- Původní ID 5

-- 3. IMPORT AUTOMATŮ
-- Data převzata z, přidáváme Typ a Stav
INSERT INTO Automat (Lokalita, Stav, Typ) VALUES 
('Ostrava, Forum Nová Karolina, 0.patro', 'Online', 'Mix'),
('Ostrava, Forum Nová Karolina, 2.patro', 'Online', 'Napoje'),
('Ostrava, Obchodní dům OC Laso, 0.patro', 'Offline', 'Jidlo'),
('Ostrava, Karolina nákupní centrum', 'Online', 'Mix'),
('Ostrava, OC Futurum, 0.patro', 'Online', 'Napoje'),
('Ostrava, Čez Korporátní Služby, 2.patro', 'Porucha', 'Jidlo'),
('Ostrava, Supermarket BILLA, 1. máje', 'Online', 'Mix'),
('Ostrava, Obchodní centrum Galerie', 'Online', 'Napoje'),
('Ostrava, Nákupní centrum Géčko', 'Offline', 'Mix'),
('Ostrava, Avion Shopping Park', 'Online', 'Jidlo'),
('Ostrava, Nákupní středisko KOTVA', 'Online', 'Mix'),
('Ostrava, Poliklinika Hrabuvka', 'Online', 'Napoje'),
('Ostrava, Albert Hypermarket Dubina', 'Online', 'Mix');

-- 4. IMPORT PRODUKTŮ
-- Data převzata z, ceny a kategorie odhadujeme
INSERT INTO Produkt (Nazev, Cena, Kategorie, Ean) VALUES 
('Tatranka lísková', 15.00, 'Jidlo', '8591234567890'), -- ID 1
('Tatranka mléčná', 15.00, 'Jidlo', '8591234567891'),
('Fidorka mléčná kokos', 20.00, 'Jidlo', '8591234567892'),
('Fidorka bílá', 20.00, 'Jidlo', '8591234567893'),
('Mila řezy', 18.00, 'Jidlo', '8591234567894'),
('Zlaté Polomáčené', 22.00, 'Jidlo', '8591234567895'),
('Twix', 25.00, 'Jidlo', '8591234567896'),
('Brambůrky solené', 30.00, 'Jidlo', '8591234567897'),
('Brambůrky česnekové', 30.00, 'Jidlo', '8591234567898'),
('Tuc mini krekry', 25.00, 'Jidlo', '8591234567899'),
('Coca Cola 0.5l', 35.00, 'Napoj', '5449000000996'), -- ID 11
('Coca Cola Zero 0.5l', 35.00, 'Napoj', '5449000000997'),
('Voda neperlivá 0.5l', 20.00, 'Napoj', '8590000000001'),
('Voda jemně perlivá', 20.00, 'Napoj', '8590000000002'),
('Ledový čaj citron', 32.00, 'Napoj', '8590000000003'),
('Ledový čaj broskev', 32.00, 'Napoj', '8590000000004'),
('Monster Energy', 45.00, 'Napoj', '8590000000005'),
('Monster Zero', 45.00, 'Napoj', '8590000000006'),
('Tiger Energy', 30.00, 'Napoj', '8590000000007');

-- 5. NASKLADNĚNÍ DO CENTRÁLNÍHO SKLADU
-- Dáme tam od všeho hodně, ale od Tatranek a Coly málo, ať můžeme testovat objednávky
INSERT INTO ZasobaSkladu (ProduktId, Mnozstvi)
SELECT Id, CASE WHEN Id IN (1, 11) THEN 5 ELSE 100 END -- 5 ks pro Tatranku a Colu, jinak 100
FROM Produkt;

-- 6. NASKLADNĚNÍ DO AUTOMATŮ (Generování zásob)
-- Naplníme každý automat náhodnými produkty
DECLARE @automatId INT = 1;
DECLARE @maxAutomatId INT = (SELECT MAX(Id) FROM Automat);

WHILE @automatId <= @maxAutomatId
BEGIN
    -- Do každého automatu dáme prvních 10 produktů
    INSERT INTO ZasobaAutomatu (AutomatId, ProduktId, Mnozstvi, MinimaleLimit, DatumPoslednihoDoplneni)
    SELECT 
        @automatId, 
        Id, 
        CASE WHEN Id IN (1, 3, 11) THEN 2 ELSE 20 END, -- Záměrně dáme málo (2 ks) Tatranek a Coly, ať svítí červeně
        5, -- Limit pro upozornění
        GETDATE()
    FROM Produkt
    WHERE Id <= 10; -- Jen prvních 10 produktů

    SET @automatId = @automatId + 1;
END

-- 7. GENEROVÁNÍ PRODEJŮ (Pro Analýzu - UC 18)
-- Vygenerujeme 100 náhodných prodejů za poslední měsíc
DECLARE @i INT = 0;
WHILE @i < 100
BEGIN
    INSERT INTO Prodej (AutomatId, ProduktId, Cena, DatumProdeje)
    VALUES (
        (SELECT TOP 1 Id FROM Automat ORDER BY NEWID()), -- Náhodný automat
        (SELECT TOP 1 Id FROM Produkt ORDER BY NEWID()), -- Náhodný produkt
        (SELECT TOP 1 Cena FROM Produkt ORDER BY NEWID()), -- Cena (zjednodušeně náhodná z ceníku)
        DATEADD(day, - (ABS(CHECKSUM(NEWID())) % 30), GETDATE()) -- Náhodné datum v posledních 30 dnech
    );
    SET @i = @i + 1;
END

-- KONTROLNÍ VÝPIS
SELECT 'Uživatelů' as Tabulka, COUNT(*) as Pocet FROM Uzivatel
UNION ALL
SELECT 'Automatů', COUNT(*) FROM Automat
UNION ALL
SELECT 'Produktů', COUNT(*) FROM Produkt
UNION ALL
SELECT 'Zásob v automatech', COUNT(*) FROM ZasobaAutomatu
UNION ALL
SELECT 'Prodejů (historie)', COUNT(*) FROM Prodej;