# Percue

> Play audio files on cue for live performances.

Percue ist eine Windows-Desktopanwendung zur Organisation und Wiedergabe von Audiodateien während Live-Auftritten, Proben und Veranstaltungen. Audio-Cues können in einer Setlist strukturiert, gespeichert und während der Performance gezielt abgespielt werden.

Die Anwendung basiert auf C#, WPF und .NET und verwendet NAudio für die Audioverarbeitung.

---

## Inhaltsverzeichnis

- [Über Percue](#über-percue)
- [Funktionen](#funktionen)
- [Technologie](#technologie)
- [Systemanforderungen](#systemanforderungen)
- [Installation](#installation)
- [Projekt aus dem Quellcode starten](#projekt-aus-dem-quellcode-starten)
- [Verwendung](#verwendung)
- [Projektstruktur](#projektstruktur)
- [Tests](#tests)
- [Build und Veröffentlichung](#build-und-veröffentlichung)
- [Bekannte Einschränkungen](#bekannte-einschränkungen)
- [Mitwirken](#mitwirken)
- [Lizenz](#lizenz)

---

## Über Percue

Bei Live-Auftritten müssen häufig mehrere Audiodateien zuverlässig und in einer bestimmten Reihenfolge abgespielt werden. Percue soll diesen Ablauf vereinfachen, indem Audiodateien in Shows, Kanälen und Setlists organisiert werden können.

Die Anwendung ist insbesondere für folgende Einsatzbereiche gedacht:

- Theater- und Musicalaufführungen
- Live-Musik und Bands
- Veranstaltungen und Präsentationen
- Hörspiele und Performances
- Proben und technische Durchläufe
- Soundeffekte und Hintergrundmusik

Percue wird als native Windows-Anwendung mit einer grafischen Benutzeroberfläche bereitgestellt.

---

## Funktionen

Die aktuelle Anwendung enthält unter anderem folgende Funktionen:

- Erstellen einer neuen Show
- Speichern einer Show
- Öffnen einer bestehenden Show
- Hinzufügen zusätzlicher Audiokanäle
- Organisation von Audioinhalten in einer Setlist
- Wiedergabe von Audiodateien während einer Live-Performance
- Scrollbare Darstellung der Setlist
- Windows-Oberfläche auf Basis von WPF
- Audioverarbeitung über NAudio
- Erstellung einer eigenständigen Windows-x64-Anwendung

Die genaue Verfügbarkeit einzelner Wiedergabe- und Bearbeitungsfunktionen kann vom aktuellen Entwicklungsstand des Projekts abhängen.

---

## Technologie

Percue verwendet folgende Technologien und Bibliotheken:

| Komponente | Technologie |
|---|---|
| Programmiersprache | C# |
| Benutzeroberfläche | WPF |
| Zielplattform | Windows |
| Zielframework | .NET 5.0 for Windows |
| Audioverarbeitung | NAudio |
| UI-Framework | MahApps.Metro |
| Tests | NUnit |
| Projektformat | Visual Studio Solution / SDK-style `.csproj` |

Verwendete NuGet-Pakete sind unter anderem:

- [NAudio](https://github.com/naudio/NAudio)
- [NAudio.WaveFormRenderer](https://github.com/naudio/NAudio)
- [MahApps.Metro](https://github.com/MahApps/MahApps.Metro)
- [MahApps.Metro.IconPacks](https://github.com/MahApps/MahApps.Metro.IconPacks)
- [Octokit](https://github.com/octokit/octokit.net)
- [NUnit](https://nunit.org/)

---

## Systemanforderungen

Für die Ausführung der aktuellen Version werden voraussichtlich folgende Voraussetzungen benötigt:

- Windows 64-Bit
- x64-kompatibler Prozessor
- Unterstützte Audioausgabegeräte
- Ausreichend Speicherplatz für die Anwendung und die verwendeten Audiodateien

Das Projekt ist für `win-x64` konfiguriert und wird als selbstständige Anwendung veröffentlicht. Dadurch sollte auf dem Zielsystem keine separate .NET-Runtime erforderlich sein, wenn eine veröffentlichte Self-contained-Version verwendet wird.

> Hinweis: Die tatsächlich unterstützten Windows-Versionen sollten vor einer offiziellen Veröffentlichung noch eindeutig festgelegt und dokumentiert werden.

---

## Installation

### Variante 1: Veröffentlichte Version

Falls eine veröffentlichte Version vorhanden ist:

1. Lade das aktuelle Release aus dem Bereich **Releases** herunter.
2. Entpacke das Archiv in einen beliebigen Ordner.
3. Starte `Percue.exe`.
4. Falls Windows eine Sicherheitswarnung anzeigt, überprüfe die Herkunft der Datei und bestätige die Ausführung nur, wenn du der Quelle vertraust.

### Variante 2: Aus dem Quellcode

Alternativ kann Percue direkt aus dem Quellcode erstellt und gestartet werden.

```bash
git clone https://github.com/w3lk/Percue.git
cd Percue