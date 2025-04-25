# 📘 SOLTEC.Core.WikiDocGen - Guía de Uso

Este proyecto genera automáticamente documentación Markdown bilingüe (inglés y español) para todas las clases públicas, propiedades, métodos y enumeraciones del proyecto `SOLTEC.Core`.

> **Nota:** El archivo `_Sidebar.md` se genera automáticamente y organiza las clases por namespace (tercer nivel), con enlaces bilingües.

## 🚀 Cómo Ejecutar

Asegúrate de compilar el proyecto antes de ejecutar el generador.

### ✅ Desde la línea de comandos de .NET
```
dotnet run --project ./Tools/SOLTEC.Core.WikiDocGen
```

### ✅ Desde Windows Terminal (con script de ayuda)
```
./Tools/SOLTEC.Core.WikiDocGen/run-wiki-generator.bat
```

### ✅ Desde Linux o terminal de Rider
```
sh ./Tools/SOLTEC.Core.WikiDocGen/run-wiki-generator.sh
```

## 📂 Salida

Los archivos Markdown generados se ubican en:
```
./Tools/SOLTEC.Core.WikiDocGen/DOCS/
```

## 🌐 Integración con la Wiki

Copia el contenido de `DOCS/en/` y `DOCS/es/` en el repositorio Wiki de GitHub, ya sea con `git clone` o manualmente.

---

Los archivos generados incluyen:
- `Home.md`, `Home_ES.md`
- `TOC.md`, `TOC_ES.md`
- Un archivo Markdown por cada clase o enumeración (con ejemplos y comentarios XML)

---

## 🌐 Traducción automática en la documentación de la Wiki

Desde la versión 1.0.0, la herramienta WikiDocGen traduce automáticamente los comentarios XML al **español en línea**, sin mostrar la versión original en inglés dentro de los archivos Markdown en español. Esta funcionalidad aplica a:

- Descripciones de clases
- Comentarios de métodos y propiedades
- Descripciones de enumeraciones

Para ello, se conecta con un servicio de traducción externo que garantiza resultados precisos y legibles.
