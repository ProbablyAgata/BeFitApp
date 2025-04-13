# 🏋️‍♀️ BeFit – Aplikacja do zarządzania treningami

BeFit to aplikacja stworzona w technologii ASP.NET MVC umożliwiająca użytkownikom tworzenie, edytowanie i analizowanie swoich sesji treningowych. Projekt zawiera funkcje zarządzania typami ćwiczeń, sesjami treningowymi oraz statystykami użytkownika.

---

## 📚 Funkcjonalności

✅ Rejestracja i logowanie użytkownika  
✅ Tworzenie i zarządzanie **typami ćwiczeń** (dostępne tylko dla administratora)  
✅ Tworzenie i zarządzanie **sesjami treningowymi** (widoczne tylko dla zalogowanego użytkownika)  
✅ Dodawanie ćwiczeń do sesji treningowej  
✅ Wyświetlanie **statystyk z ostatnich 4 tygodni**:  
   - suma powtórzeń  
   - średnie obciążenie  
   - maksymalne obciążenie

---

## 🚀 Uruchamianie aplikacji lokalnie

### Wymagania:
- Visual Studio 2022
- .NET 6 SDK lub wyższy

### Krok po kroku:

1. **Sklonuj repozytorium**
   ```bash
   git clone https://github.com/ProbablyAgata/BeFitApp.git
   ```

2. **Otwórz projekt w Visual Studio**  
   Plik rozwiązania: `BeFitApp.sln`

3. **Wykonaj migrację bazy danych (SQLite)**
   W konsoli Menedżera pakietów:
   ```
   Update-Database
   ```

4. **Uruchom projekt** (Ctrl + F5)

---

## 🧪 Dane testowe

Po zainstalowaniu aplikacji możesz zalogować się na testowe konto:

```
Login: admin@befit.pl
Hasło: Admin123!
```

Możesz również zarejestrować nowego użytkownika i przetestować aplikację z poziomu zwykłego konta.

---

## 📦 Użyte technologie

- ASP.NET MVC 6
- Entity Framework Core (SQLite)
- Identity do zarządzania użytkownikami i rolami
- Razor Views
- Bootstrap (UI)

---

## 📸 Zrzuty ekranu

*(opcjonalnie dodaj screeny z widokiem statystyk, sesji i formularza ćwiczenia)*

---

## 🛡️ Bezpieczeństwo

- Autoryzacja i uwierzytelnianie oparte na ASP.NET Identity
- Dostęp do edycji typów ćwiczeń ograniczony do roli `Admin`
- Dane widoczne tylko dla właściciela konta
- Zabezpieczenia CSRF (`[ValidateAntiForgeryToken]`) we wszystkich akcjach POST

---

## 📂 Struktura projektu

```
BeFitApp/
│
├── Controllers/          // Logika aplikacji
├── Models/               // Klasy danych i walidacje
├── Views/                // Widoki Razor
├── Data/                 // Kontekst bazy danych
├── Migrations/           // Migracje EF Core
├── wwwroot/              // Style i skrypty
├── appsettings.json      // Ustawienia aplikacji
└── Program.cs            // Konfiguracja aplikacji
```

---

## ✍️ Autor

Projekt wykonany jako część zadania zaliczeniowego.  
**Autor:** Agata Ochocińska (ProbablyAgata)  
2025
