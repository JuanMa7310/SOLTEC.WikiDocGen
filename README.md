# 📘 SOLTEC.Core.WikiDocGen - Usage Guide

This project automatically generates bilingual Markdown documentation (English and Spanish) for all public classes, properties, methods, and enums in the `SOLTEC.Core` library.

> **Note:** The file `_Sidebar.md` is automatically generated and organized by class namespace (third level), with bilingual links.

## 🚀 How to Run

Make sure the project is built before running this generator.

### ✅ From .NET CLI
```
dotnet run --project ./Tools/SOLTEC.Core.WikiDocGen
```

### ✅ From Windows Terminal (with helper script)
```
./Tools/SOLTEC.Core.WikiDocGen/run-wiki-generator.bat
```

### ✅ From Linux / Rider Terminal
```
sh ./Tools/SOLTEC.Core.WikiDocGen/run-wiki-generator.sh
```

## 📂 Output

Markdown files are generated into:
```
./Tools/SOLTEC.Core.WikiDocGen/DOCS/
```

## 🌐 Wiki Integration

Copy the contents of `DOCS/en/` and `DOCS/es/` to your GitHub Wiki repository via `git clone` or manually.

---

Generated files include:
- `Home.md`, `Home_ES.md`
- `TOC.md`, `TOC_ES.md`
- One Markdown file per class and enum (with examples and XML comments)

---

## 🌐 Inline Translation in Wiki

As of version 1.0.0, the WikiDocGen tool automatically translates XML comments into **Spanish inline**, without showing the original English version in the Spanish Markdown files. This applies to:

- Class summaries
- Method and property comments
- Enum descriptions

It connects to an external translation service to generate accurate and readable translations.
