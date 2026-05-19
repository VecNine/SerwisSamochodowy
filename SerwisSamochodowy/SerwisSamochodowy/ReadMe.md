# Warsztat samochodowy

Prosta aplikacja dla warsztatu samochodowego, przechowująca zapisane naprawy oraz dane konieczne do ich prawidłowego zarejestrowania.

### 1. Start aplikacji
* **Zostaje utworzony domyślny użytkownik:** login: `admin`, hasło: `admin123`. Dane te można zmienić w głównym menu aplikacji po zalogowaniu się na konto administratora.
* **Tworzone są puste tabele bazy danych:** konieczne do prawidłowego funkcjonowania aplikacji.

### 2. Funkcjonalności
Do dyspozycji mamy główne tabele (widoczne w interfejsie graficznym aplikacji webowej):
* **Vehicle** (Pojazdy)
* **Mechanics** (Mechanicy)
* **Repairments** (Naprawy)
* **Parts** (Części do napraw)
* **Users** (Użytkownicy)

Dodatkowo istnieje tabela niewidoczna dla użytkownika, łącząca tabele **Repairments** z **Parts** (relacja wiele do wielu).

---

### **ADMIN**
Posiada wszystkie uprawnienia zwykłego użytkownika (Usera). Dla admina wyświetlane są dodatkowo statystyki obejmujące:
* Zysk z napraw w wybranym przedziale czasowym.
* Łączne koszty części zamówionych do napraw w przedziale czasowym.
* Kwotę, jaką dany mechanik wygenerował z napraw w przedziale czasowym oraz jego łączne wynagrodzenie w tym okresie.
* Liczbę napraw wykonanych przez danego mechanika w przedziale czasowym.
* Wskazanie zarejestrowanego pojazdu, który przeszedł największą liczbę napraw.

Dodatkowo admin ma możliwość:
* Dodawania, edycji oraz usuwania użytkowników.
* Dodawania, edycji oraz usuwania mechaników.

### **USER**
Posiada uprawnienia do dodawania, edycji oraz usuwania:
* Części samochodowych,
* Pojazdów,
* Napraw.

---

### API
API udostępnia funkcjonalność dodawania, edytowania oraz usuwania wszystkich rekordów z bazy danych, pod warunkiem posiadania odpowiedniego tokena użytkownika.

> **Uwaga:** Przykład działania znajduje się w pliku `TestAPI/TestApi.cs`.