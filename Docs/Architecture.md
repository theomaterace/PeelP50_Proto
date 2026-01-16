# Architektura logiczna i komponenty
Interaktywna aplikacja muzealna Peel P50 (Unity)

## 1. Charakterystyka ogólna

Aplikacja została zaprojektowana jako kioskowa aplikacja standalone uruchamiana na dedykowanym urządzeniu multimedialnym w muzeum. System nie wykorzystuje backendu ani komunikacji sieciowej i działa w pełni lokalnie.

Architektura aplikacji oparta jest na podziale funkcjonalnym na sceny Unity oraz moduły logiczne realizowane przez skrypty C#.

## 2. Warstwy logiczne aplikacji

### 2.1 Warstwa prezentacji (UI)
Odpowiada za interfejs użytkownika oraz interakcję dotykową.

Główne komponenty:
- MenuController – obsługa menu głównego i nawigacji
- MainMenuController – wybór trybów aplikacji
- UI Button Controllers – obsługa przycisków dotykowych
- CanvasAutoMatch / SafeAreaFitter – dopasowanie UI do ekranu kiosku

### 2.2 Warstwa logiki aplikacji
Odpowiada za sterowanie przebiegiem aplikacji oraz reguły działania poszczególnych trybów.

Główne komponenty:
- TapToStartController – przejście z ekranu startowego do menu
- IdleReturn – automatyczny powrót do menu po bezczynności
- InstructionHint – podpowiedzi dla użytkownika
- RaceManager – logika wyścigu, czasu, start/meta
- StartFinishTrigger / RaceTrigger – zdarzenia startu i mety

### 2.3 Warstwa symulacji / interakcji 3D
Odpowiada za zachowanie pojazdu, kamery i fizykę.

Główne komponenty:
- PeelController_Lite – sterowanie pojazdem i fizyka jazdy
- NSU_AutoDrive – tryb automatycznej jazdy (opcjonalny)
- CameraFollow / CarCameraFollowNSU – kamera podążająca
- NSU_WheelSpin – animacja kół
- BrakeLightController – światła hamowania
- SpeedometerAnalog / GearIndicatorUI – HUD pojazdu

### 2.4 Warstwa multimedialna
Odpowiada za dźwięk i treści multimedialne.

Główne komponenty:
- PeelEngineSound – dźwięk silnika
- PeelSoundMenu – dźwięki interakcji
- WenaWebView – osadzona treść webowa (jeśli aktywna)

## 3. Struktura scen Unity

Aplikacja składa się z odrębnych scen, m.in.:
- Menu główne (Hub aplikacji)
- Scena prezentacji pojazdu
- Scena widoku 360 wnętrza
- Scena jazdy testowej
- Sceny pomocnicze (dźwięki, prezentacje)

Każda scena realizuje jeden jasno określony cel funkcjonalny.

## 4. Komunikacja między komponentami

- Komunikacja odbywa się lokalnie poprzez referencje komponentów Unity oraz zdarzenia (metody publiczne).
- Brak komunikacji sieciowej i API.
- Dane tymczasowe (np. rekordy czasów) przechowywane są lokalnie (PlayerPrefs).

## 5. Uzasadnienie przyjętej architektury

Zastosowana architektura:
- upraszcza utrzymanie aplikacji,
- zwiększa stabilność pracy na kiosku,
- umożliwia łatwe dodawanie kolejnych pojazdów i scen,
- jest adekwatna do skali projektu wdrożeniowego.

Architektura została dobrana świadomie do warunków sprzętowych oraz charakteru ekspozycji muzealnej.
