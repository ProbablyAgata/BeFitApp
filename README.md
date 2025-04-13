# BeFit

BeFit to aplikacja ASP.NET Core MVC do zarządzania treningami siłowymi. Pozwala użytkownikom rejestrować typy ćwiczeń, sesje treningowe, wykonane ćwiczenia i przeglądać statystyki z ostatnich 4 tygodni.

## 🧰 Wymagania
- [.NET SDK 6.0 lub nowszy](https://dotnet.microsoft.com/en-us/download)
- Visual Studio Code lub dowolny edytor
- (opcjonalnie) SQLite jako baza danych lokalna (brak potrzeby instalacji SQL Server)

## 🚀 Uruchomienie projektu

### 1. Sklonuj repozytorium
```bash
git clone https://github.com/twoj-login/befit.git
cd befit
```

### 2. Zainstaluj pakiety
```bash
dotnet restore
```

### 3. (REKOMENDOWANE) Użycie SQLite zamiast SQL Server

#### 3.1 Zainstaluj pakiet SQLite
```bash
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
```

#### 3.2 W `Program.cs` zamień:
```csharp
// builder.Services.AddDbContext<ApplicationDbContext>(options =>
//     options.UseSqlServer(...));
```
na:
```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=befit.db"));
```

#### 3.3 Dodaj SQLite tools (jeśli chcesz podglądać bazę)
- Zainstaluj np. rozszerzenie **SQLite Viewer** do VS Code lub użyj [DB Browser for SQLite](https://sqlitebrowser.org/)

### 4. Dodaj migrację i stwórz bazę danych
```bash
dotnet ef migrations add InitialCreate

dotnet ef database update
```

### 5. Dodaj dane testowe
W `Program.cs`, przed `app.Run();`, dodaj:
```csharp
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await DbInitializer.Seed(services);
}
```

Plik `DbInitializer.cs` powinien zawierać przykładowe dane:
- 3 typy ćwiczeń: Przysiady, Martwy ciąg, Wyciskanie na ławce
- Konto admina: `admin@befit.pl` / `Test123!`

### 6. Uruchom projekt
```bash
dotnet run
```

Aplikacja będzie dostępna na:
```
https://localhost:5001
```

## 🧪 Dane testowe do logowania
- **Email:** `admin@befit.pl`
- **Hasło:** `Test123!`

## 📂 Moduły aplikacji
- Typy ćwiczeń – globalna lista ćwiczeń (CRUD dostępny tylko dla administratora)
- Sesje treningowe – przypisywane do zalogowanego użytkownika
- Wykonane ćwiczenia – powiązane z sesją i typem ćwiczenia
- Statystyki – podsumowanie ćwiczeń z ostatnich 4 tygodni

## 📌 Uwagi
- Wszystkie widoki zawierają polskie etykiety
- Nawigacja znajduje się w `_Layout.cshtml`
- Projekt obsługuje rejestrację, logowanie, role (Admin)

---
**Autor:** [Twoje Imię]  
**Rok:** 2025  
**Technologie:** ASP.NET Core MVC, EF Core, Identity, Bootstrap, SQLite


// -----------------------------
// .gitignore dla ASP.NET Core + SQLite
// -----------------------------
# .gitignore
bin/
obj/
*.db
*.db-journal
.env
.vscode/
appsettings.*.json
secrets.json
*.user
*.suo
*.swp
.idea/
.DS_Store

# VS Code
.vscode/

# Logs
*.log

# EF Migrations (opcjonalnie)
Migrations/
