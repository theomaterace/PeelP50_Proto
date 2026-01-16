# BPMN – proces end-to-end (kiosk Peel P50)

Dokument opisuje uproszczony proces end-to-end działania interaktywnej aplikacji muzealnej Peel P50, zgodnie z ideą BPMN (Business Process Model and Notation).

Proces obejmuje pełną ścieżkę od uruchomienia aplikacji na kiosku, poprzez interakcję zwiedzającego, aż do automatycznego powrotu systemu do stanu początkowego.

## 1. Uczestnicy procesu (Pool / Lanes)

- Zwiedzający (użytkownik kiosku)
- System (aplikacja Unity uruchomiona na kiosku)

## 2. Opis procesu głównego (end-to-end)

### Start procesu
Zdarzenie początkowe:
- Aplikacja uruchomiona na kiosku
- Wyświetlany ekran startowy (Tap to Start)

### Przebieg procesu

1. Zwiedzający dotyka ekranu startowego  
   → System przechodzi do menu głównego

2. Zwiedzający wybiera tryb działania  
   (prezentacja 3D / widok 360 / jazda testowa)

3. System uruchamia wybrany moduł:
   - prezentacja pojazdu
   - widok 360 wnętrza
   - tryb jazdy testowej

4. Zwiedzający korzysta z wybranego modułu:
   - interakcja dotykowa
   - sterowanie pojazdem
   - eksploracja widoku

5. Zwiedzający kończy interakcję:
   - używa przycisku „Wstecz”
   LUB
   - opuszcza kiosk (brak aktywności)

6. System:
   - wraca do menu głównego
   - lub po czasie bezczynności automatycznie resetuje widok

### Zdarzenie końcowe
- Aplikacja znajduje się w stanie początkowym (menu)
- Kiosk gotowy dla kolejnego zwiedzającego

## 3. Decyzje i zdarzenia (bramki BPMN – uproszczone)

- Decyzja: „Czy użytkownik jest aktywny?”
  - TAK → kontynuuj interakcję
  - NIE → automatyczny powrót do menu

- Decyzja: „Czy użytkownik wybrał tryb?”
  - TAK → uruchom moduł
  - NIE → pozostań w menu

## 4. Charakterystyka procesu

- Proces jest w pełni autonomiczny (brak backendu).
- Proces może być wykonywany wielokrotnie w pętli.
- Projektowany pod środowisko publiczne (muzeum).

Uwaga:
Diagram BPMN został uproszczony do formy opisowej ze względu na charakter projektu i środowisko akademickie.
