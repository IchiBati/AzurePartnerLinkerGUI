# Azure Partner Linker GUI

Dieses Projekt ist eine Desktop-Anwendung zur Verwaltung von Partner Admin Links (PAL) in Microsoft Azure. Die Anwendung ermöglicht die Authentifizierung mit Azure, das Abrufen und Verwalten von Partnerinformationen sowie das Verknüpfen, Aktualisieren und Löschen von PALs.

## Funktionen

- Authentifizierung mit Azure über Client Secret Credentials
- Anzeige und Verwaltung von Partner Admin Links (PAL)
- Erstellen, Aktualisieren (PUT/PATCH) und Löschen von PALs über die Azure Management API
- Protokollierung von Aktionen und Statusmeldungen in der Benutzeroberfläche
- Benutzerfreundliche Oberfläche mit Login- und Dashboard-Seiten

## Technologien

- .NET 9.0 (WPF)
- Azure SDK für .NET (Azure.Identity, Azure.ResourceManager)
- JSON-Serialisierung mit System.Text.Json
- MVVM-ähnliche Struktur mit Events zur Kommunikation zwischen Views

## Projektstruktur

- `LoginPage.xaml(.cs)`: Login-Seite mit Azure-Authentifizierung
- `DashboardPage.xaml(.cs)`: Dashboard zur Anzeige und Verwaltung von PALs
- `PartnerManagementClient.cs`: Client zur Kommunikation mit der Azure Partner Management API
- `ILogger.cs`: Interface für Logging
- `MainWindow.xaml(.cs)`: Hauptfenster und Logger-Implementierung
- `ApiError.cs`, `PartnerInfo.cs`, `LoginSuccessEventArgs.cs`: Datenmodelle und EventArgs

## Voraussetzungen

- Azure Active Directory Tenant mit entsprechenden Berechtigungen
- Registrierte Azure AD Anwendung mit Client ID, Tenant ID und Client Secret
- .NET 9.0 SDK installiert

## Installation und Nutzung

1. Repository klonen:
   ```bash
   git clone <repository-url>
   ```

2. Projekt in einer IDE (z.B. JetBrains Rider, Visual Studio) öffnen.

3. Azure AD Anmeldeinformationen in der Login-Seite eingeben.

4. Anwendung starten und mit Azure verbinden.

5. PALs im Dashboard verwalten.

### Alternative: Erstellen einer ausführbaren Datei

Um eine eigenständige ausführbare Datei zu erstellen, können Sie das Projekt mit dem .NET CLI-Befehl `dotnet publish` bauen:

```bash
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeAllContentForSelfExtract=true
```

- `-c Release`: Baut das Projekt im Release-Modus.
- `-r win-x64`: Zielplattform Windows 64-bit.
- `--self-contained true`: Erstellt eine eigenständige ausführbare Datei ohne Abhängigkeit von installiertem .NET.
- `/p:PublishSingleFile=true`: Packt alle Dateien in eine einzelne EXE.
- `/p:IncludeAllContentForSelfExtract=true`: Stellt sicher, dass alle Inhalte enthalten sind.

Die ausführbare Datei finden Sie anschließend im Ordner `bin\Release\net9.0-windows\win-x64\publish\`.

Diese Datei kann direkt ausgeführt werden, ohne dass eine IDE oder das .NET SDK installiert sein muss.

## Lizenz

Dieses Projekt ist unter der MIT-Lizenz lizenziert.

## Kontakt

Für Fragen oder Anregungen wenden Sie sich bitte an den Projektverantwortlichen.
