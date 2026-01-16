# Use Case + aktorzy — Peel P50 (kiosk muzealny)

Dokument opisuje aktorów oraz przypadki użycia interaktywnej aplikacji Unity uruchamianej na kiosku multimedialnym w Muzeum Motoryzacji Wena. Aplikacja działa w trybie standalone, bez backendu.

## 1. Aktorzy

1) Zwiedzający (Użytkownik kiosku)
- osoba korzystająca z ekranu dotykowego kiosku
- cel: obejrzeć pojazd, poznać informacje i uruchomić tryby interakcji (prezentacja, widok 360, jazda testowa)

2) Administrator / Opiekun ekspozycji (pośredni)
- nie korzysta z aplikacji jako użytkownik końcowy, ale odpowiada za uruchomienie kiosku i zapewnienie ciągłej pracy
- cel: stabilne działanie aplikacji, minimalna potrzeba obsługi

## 2. Założenia i ograniczenia (kontekst)

- Aplikacja przeznaczona jest do pracy na kiosku dotykowym (autonomicznie, bez stałej obsługi).
- Brak logowania, brak kont użytkowników.
- Brak komunikacji z backendem / API.
- Aplikacja powinna samoczynnie wracać do menu po bezczynności (ochrona przed “zawieszeniem się” na ekranie po odejściu zwiedzającego).

## 3. Przypadki użycia (Use Cases)

### UC-01: Rozpoczęcie interakcji z aplikacją (Tap to Start)
Aktor: Zwiedzający  
Cel: wejść z ekranu powitalnego do menu.  
Warunek wstępny: aplikacja uruchomiona na kiosku, ekran powitalny widoczny.  
Scenariusz główny:
1. Zwiedzający dotyka ekran (“Dotknij, aby rozpocząć”).
2. Aplikacja ukrywa ekran startowy i wyświetla menu główne.
Rezultat: użytkownik widzi menu i może wybrać tryb.

---

### UC-02: Nawigacja po menu głównym
Aktor: Zwiedzający  
Cel: przejść do wybranego modułu (prezentacja/360/jazda/testy).  
Warunek wstępny: menu główne jest widoczne.  
Scenariusz główny:
1. Zwiedzający wybiera opcję w menu (przycisk/pozycja).
2. Aplikacja uruchamia odpowiednią scenę/moduł.
Rezultat: uruchomiony zostaje wybrany tryb.

---

### UC-03: Prezentacja modelu 3D pojazdu (oglądanie / obrót / detale)
Aktor: Zwiedzający  
Cel: obejrzeć pojazd Peel P50 w formie modelu 3D.  
Warunek wstępny: uruchomiono moduł prezentacji.  
Scenariusz główny:
1. Zwiedzający obraca pojazd / kamerę gestami (lub UI).
2. Zwiedzający ogląda detale pojazdu.
3. Zwiedzający może wrócić do menu.
Rezultat: użytkownik zapoznaje się z wizualnym modelem i wraca do wyboru trybów.

---

### UC-04: Widok 360 wnętrza pojazdu
Aktor: Zwiedzający  
Cel: rozejrzeć się we wnętrzu Peel P50 w trybie 360.  
Warunek wstępny: uruchomiono moduł widoku 360.  
Scenariusz główny:
1. Zwiedzający wykonuje gest przesunięcia (drag), aby obracać widok.
2. Aplikacja płynnie aktualizuje kierunek patrzenia.
3. Zwiedzający używa przycisku “Wstecz”, aby wrócić do menu.
Rezultat: użytkownik obejrzał wnętrze w 360 i wraca do menu.

---

### UC-05: Uruchomienie wirtualnego zwiedzania (WebView) – jeśli moduł występuje w buildzie
Aktor: Zwiedzający  
Cel: uruchomić stronę/treść 360 w osadzonym WebView.  
Warunek wstępny: aplikacja posiada w menu opcję “Wirtualne zwiedzanie”.  
Scenariusz główny:
1. Zwiedzający wybiera opcję “Wirtualne zwiedzanie”.
2. Aplikacja ukrywa menu i otwiera WebView z adresem URL.
3. Zwiedzający porusza się po treści w WebView.
4. Zwiedzający używa “Wstecz”, aby wrócić do menu (lub cofa w historii).
Rezultat: treść www została wyświetlona, użytkownik wraca do menu.

Uwaga: jeżeli w finalnej wersji ten moduł jest wyłączony/nieużywany, UC-05 oznaczyć jako “nie dotyczy” w kontekście finalnego wdrożenia.

---

### UC-06: Jazda testowa / tryb symulacyjny
Aktor: Zwiedzający  
Cel: uruchomić prostą jazdę testową i sterować pojazdem.  
Warunek wstępny: uruchomiono moduł jazdy testowej.  
Scenariusz główny:
1. Aplikacja wykonuje odliczanie startowe.
2. Zwiedzający steruje pojazdem (przyciski ekranowe / wejście).
3. Aplikacja zlicza czas przejazdu (start/meta) i prezentuje wynik.
4. Zwiedzający może rozpocząć ponownie lub wrócić do menu.
Rezultat: wykonano przejazd i wyświetlono wynik / rekordy.

---

### UC-07: Automatyczny powrót do menu po bezczynności (Idle Return)
Aktor: Zwiedzający (pośrednio), Administrator (pośrednio)  
Cel: przywrócić aplikację do stanu gotowości, gdy użytkownik odejdzie.  
Warunek wstępny: aplikacja jest uruchomiona w dowolnym module/scenie.  
Scenariusz główny:
1. Użytkownik nie wykonuje żadnych akcji przez określony czas.
2. Aplikacja automatycznie przełącza się do menu (scena główna).
Rezultat: kiosk wraca do stanu “dla następnego zwiedzającego”.

## 4. Kryteria akceptacji (skrót)

- Menu musi być czytelne na ekranie kiosku i umożliwiać wejście do modułów.
- Moduły muszą mieć możliwość powrotu do menu.
- Aplikacja powinna być odporna na “pozostawienie” bez obsługi (auto return).
- Brak elementów wymagających sieci/back-endu do podstawowego działania (poza opcjonalnym WebView).
