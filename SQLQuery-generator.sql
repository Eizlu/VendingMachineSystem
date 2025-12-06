-- 1. Vymazání nudných uniformních dat
DELETE FROM ZasobaAutomatu;

-- 2. Chytré naskladnění podle typu automatu a náhody
DECLARE @automatId INT;
DECLARE @typAutomatu NVARCHAR(50);

-- Projdeme všechny automaty jeden po druhém
DECLARE cursor_automaty CURSOR FOR 
SELECT Id, Typ FROM Automat;

OPEN cursor_automaty;
FETCH NEXT FROM cursor_automaty INTO @automatId, @typAutomatu;

WHILE @@FETCH_STATUS = 0
BEGIN
    -- Vložíme produkty, které odpovídají typu automatu
    INSERT INTO ZasobaAutomatu (AutomatId, ProduktId, Mnozstvi, MinimaleLimit, DatumPoslednihoDoplneni)
    SELECT 
        @automatId,
        p.Id,
        -- Náhodné množství mezi 0 a 25. 
        -- (ABS(CHECKSUM(NEWID())) generuje náhodné číslo)
        ABS(CHECKSUM(NEWID()) % 25), 
        5, -- Limit pro varování
        GETDATE()
    FROM Produkt p
    WHERE 
        -- Logika: Co do automatu patří?
        (@typAutomatu = 'Mix') -- Mix bere všechno
        OR 
        (@typAutomatu = 'Napoje' AND p.Kategorie = 'Napoj') -- Jen pití
        OR 
        (@typAutomatu = 'Jidlo' AND p.Kategorie = 'Jidlo') -- Jen jídlo
        OR
        (@typAutomatu NOT IN ('Mix', 'Napoje', 'Jidlo')); -- Záchrana pro překlepy (bere vše)

    FETCH NEXT FROM cursor_automaty INTO @automatId, @typAutomatu;
END;

CLOSE cursor_automaty;
DEALLOCATE cursor_automaty;

-- 3. Korekce pro "Demo efekt" (Abychom měli jistotu, že aspoň něco bude červené)
-- Nastavíme, že v automatu č. 1 (Nová Karolina) dochází Cola, abys to mohla hned ukázat
UPDATE ZasobaAutomatu 
SET Mnozstvi = 1 
WHERE AutomatId = 1 AND ProduktId IN (SELECT Id FROM Produkt WHERE Nazev LIKE '%Cola%');

-- Výpis výsledku pro kontrolu
SELECT a.Lokalita, a.Typ, p.Nazev, z.Mnozstvi 
FROM ZasobaAutomatu z
JOIN Automat a ON z.AutomatId = a.Id
JOIN Produkt p ON z.ProduktId = p.Id
ORDER BY a.Id, p.Kategorie;