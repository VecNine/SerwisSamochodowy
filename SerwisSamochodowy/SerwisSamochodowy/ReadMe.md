## Warsztat samochodowy

Prosta aplikacja dla warsztatu samochodowego przetrzymująca zapisane naprawy, oraz dane konieczne do ich prawidłowego zarejestrowania.

1. **Start Aplikacji**
- Zostaje utworzony użytkownik: admin hasło: admin123 (można to zmienić w głównym menu aplikacji, jeżeli jesteśmy zalogowani jako administrator)
- Utworzone są puste bazy danych konieczne do prawidłowego funkcjonowania aplikacji.
2. **Funkcjonalności**
- Do dyspozycji mamy 4 tabele główne (widzialne w interfejsie graficznym w aplikacji webowej)
  - Vehicle (Pojazdy)
  - Mechanics (Mechanicy)
  - Repairments (Naprawy)
  - Parts (Części do napraw)
  - Users (Użytkownicy)
- Dodatkowo mamy tabelę niewidoczną dla użytkownika: łączącą Repairments z Parts (wiele do wielu)

#### **_ADMIN:_**

- Posiada wszystkie funkcjonalności User'a
- Dla admina wyświetlane są statystyki:
  - Zysk z napraw w przedziale czasowym
  - Łączne koszty części zamówionych do napraw w przedziale czasowym
  - Ile dany mechanik zarobił z napraw w przedziale czasowym, oraz jakie dostał łączne wynagrodzenie w przedziale czasowym
  - Ile napraw wykonał dany mechanik w przedziale czasowym
  - Który zarejestrowany pojazd dostał największą liczbę napraw
- Dodatkowo admin posiada funkcjonalności:
  - Dodawanie użytkowników, edycja oraz usuwanie
  - Dodawanie mechaników, edycja oraz usuwanie

#### **_USER_**

- Dodawanie, edycja oraz usuwanie
  - Części samochodowych
  - Pojazdów
  - Napraw

#### API

Przewidziane są funkcjonalności dodawania, edytowania oraz usuwania wszystkich rekordów bazy, o ile posiadamy odpowiedni token użytkownika
Przykład działania w `TestAPI/TestApi.cs`