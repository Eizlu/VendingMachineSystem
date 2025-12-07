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
DBCC CHECKIDENT ( N'Automat', RESEED, 0);
DBCC CHECKIDENT ( N'Produkt', RESEED, 0);
DBCC CHECKIDENT ( N'Uzivatel', RESEED, 0);
DBCC CHECKIDENT ( N'ZasobaAutomatu', RESEED, 0);
DBCC CHECKIDENT ( N'Objednavka', RESEED, 0);

-- 2. IMPORT UŽIVATELŮ (Mapujeme staré 'spravce' na nové 'Uzivatel')
-- Data převzata z
INSERT INTO Uzivatel (Jmeno, Login, Heslo, Role) VALUES 
( N'Adam Novák', N'adam', N'admin123', N'Administrator'), -- Původní ID 1
( N'Jan Pospíšil', N'jan', N'jan123', N'Skladnik'),      -- Původní ID 2
( N'Dana Kalusová', N'dana', N'dana123', N'Provozovatel'),-- Původní ID 3
( N'Marie Pastorková', N'marie', N'marie123', N'Pracovnik'), -- Původní ID 4
( N'Jakub Fitřík', N'jakub', N'jakub123', N'Pracovnik');  -- Původní ID 5

-- 3. IMPORT AUTOMATŮ
-- Data převzata z, přidáváme Typ a Stav
INSERT INTO Automat (Lokalita, Stav, Typ) VALUES 
( N'Ostrava, Forum Nová Karolina, 0.patro', N'Online', N'Mix'),
( N'Ostrava, Forum Nová Karolina, 2.patro', N'Online', N'Napoje'),
( N'Ostrava, Obchodní dům OC Laso, 0.patro', N'Offline', N'Jidlo'),
( N'Ostrava, Karolina nákupní centrum', N'Online', N'Mix'),
( N'Ostrava, OC Futurum, 0.patro', N'Online', N'Napoje'),
( N'Ostrava, Čez Korporátní Služby, 2.patro', N'Porucha', N'Jidlo'),
( N'Ostrava, Supermarket BILLA, 1. máje', N'Online', N'Mix'),
( N'Ostrava, Obchodní centrum Galerie', N'Online', N'Napoje'),
( N'Ostrava, Nákupní centrum Géčko', N'Offline', N'Mix'),
( N'Ostrava, Avion Shopping Park', N'Online', N'Jidlo'),
( N'Ostrava, Nákupní středisko KOTVA', N'Online', N'Mix'),
( N'Ostrava, Poliklinika Hrabůvka', N'Online', N'Napoje'),
( N'Ostrava, Albert Hypermarket Dubina', N'Online', N'Mix');

-- 4. IMPORT PRODUKTŮ
-- Data převzata z, ceny a kategorie odhadujeme
INSERT INTO Produkt (Nazev, Cena, Kategorie, Ean) VALUES 
( N'Tatranka lísková', 15.00, N'Jidlo', N'8591234567890'), -- ID 1
( N'Tatranka mléčná', 15.00, N'Jidlo', N'8591234567891'),
( N'Fidorka mléčná kokos', 20.00, N'Jidlo', N'8591234567892'),
( N'Fidorka bílá', 20.00, N'Jidlo', N'8591234567893'),
( N'Mila řezy', 18.00, N'Jidlo', N'8591234567894'),
( N'Zlaté Polomáčené', 22.00, N'Jidlo', N'8591234567895'),
( N'Twix', 25.00, N'Jidlo', N'8591234567896'),
( N'Brambůrky solené', 30.00, N'Jidlo', N'8591234567897'),
( N'Brambůrky česnekové', 30.00, N'Jidlo', N'8591234567898'),
( N'Tuc mini krekry', 25.00, N'Jidlo', N'8591234567899'),
( N'Coca Cola 0.5l', 35.00, N'Napoj', N'5449000000996'), -- ID 11
( N'Coca Cola Zero 0.5l', 35.00, N'Napoj', N'5449000000997'),
( N'Voda neperlivá 0.5l', 20.00, N'Napoj', N'8590000000001'),
( N'Voda jemně perlivá', 20.00, N'Napoj', N'8590000000002'),
( N'Ledový čaj citron', 32.00, N'Napoj', N'8590000000003'),
( N'Ledový čaj broskev', 32.00, N'Napoj', N'8590000000004'),
( N'Monster Energy', 45.00, N'Napoj', N'8590000000005'),
( N'Monster Zero', 45.00, N'Napoj', N'8590000000006'),
( N'Tiger Energy', 30.00, N'Napoj', N'8590000000007');

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