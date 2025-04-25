using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;
using System.Text.Json;

var gsourcePath = FindProjectSourcePath();
var goutputPathEn = Path.Combine(Directory.GetCurrentDirectory(), "Docs", "en");
var goutputPathEs = Path.Combine(Directory.GetCurrentDirectory(), "Docs", "es");
var goutputPathSidebar = Path.Combine(Directory.GetCurrentDirectory(), "Docs");
var ggithubBaseUrl = "https://github.com/JuanMa7310/SOLTEC.Core/wiki";
var gclassNamespaceMap = new Dictionary<string, string>();
var genumNames = new List<string>();

Directory.CreateDirectory(goutputPathEn);
Directory.CreateDirectory(goutputPathEs);

Console.WriteLine($"> Searching in: {gsourcePath}");

foreach (var _sourceFile in Directory.GetFiles(gsourcePath, "*.cs", SearchOption.AllDirectories))
{
    var _fileContent = await File.ReadAllTextAsync(_sourceFile);
    var _syntaxTree = CSharpSyntaxTree.ParseText(_fileContent);
    var _syntaxRoot = await _syntaxTree.GetRootAsync();
    var _classDeclarations = _syntaxRoot.DescendantNodes().OfType<ClassDeclarationSyntax>()
        .Where(c => c.Modifiers.Any(SyntaxKind.PublicKeyword));
    var _enumDeclarations = _syntaxRoot.DescendantNodes().OfType<EnumDeclarationSyntax>()
        .Where(e => e.Modifiers.Any(SyntaxKind.PublicKeyword));

    foreach (var _classDeclaration in _classDeclarations)
    {
        gclassNamespaceMap[_classDeclaration.Identifier.Text] = _classDeclaration.FirstAncestorOrSelf<NamespaceDeclarationSyntax>()?.Name.ToString() ?? "Global";
        var _className = _classDeclaration.Identifier.Text;
        var _classDoc = ExtractXmlDoc(_classDeclaration);
        var _classSummary = _classDoc?.Split('.').FirstOrDefault()?.Trim() ?? "No summary available.";
        var _classMdEn = new StringBuilder();
        var _classMdEs = new StringBuilder();

        _classMdEn.AppendLine($"![SOLTEC Logo]({ggithubBaseUrl}/Images/logo.png)  \n**SOLTEC**  \n");
        _classMdEn.AppendLine($"# 📦 {_className}\n\n> {_classSummary}\n\n## Public Members\n");

        _classMdEs.AppendLine($"![SOLTEC Logo]({ggithubBaseUrl}/Images/logo.png)  \n**SOLTEC**  \n");
        _classMdEs.AppendLine($"# 📦 {_className}\n\n> {Translate(_classSummary)}\n\n## Miembros Públicos\n");

        foreach (var _member in _classDeclaration.Members.Where(m => m.Modifiers.Any(SyntaxKind.PublicKeyword)))
        {
            var _signature = _member switch
            {
                MethodDeclarationSyntax m => m.Identifier.Text + m.ParameterList.ToString(),
                PropertyDeclarationSyntax p => p.Identifier.Text,
                ConstructorDeclarationSyntax c => c.Identifier.Text + c.ParameterList.ToString(),
                _ => _member.ToString()
            };

            var _memberDoc = ExtractXmlDoc(_member);
            _classMdEn.AppendLine($"- `{_signature}` — {_memberDoc ?? "No XML documentation."}");
            var _translateDoc = await Translate(_memberDoc);
            _classMdEs.AppendLine($"- `{_signature}` — {_translateDoc ?? "Sin documentación XML."}");
        }
        _classMdEn.AppendLine($"\n---\n[Ver en Español]({ggithubBaseUrl}/{_className}_ES)");
        _classMdEs.AppendLine($"\n---\n[View in English]({ggithubBaseUrl}/{_className})");

        await File.WriteAllTextAsync(Path.Combine(goutputPathEn, $"{_className}.md"), _classMdEn.ToString());
        await File.WriteAllTextAsync(Path.Combine(goutputPathEs, $"{_className}_ES.md"), _classMdEs.ToString());
    }
    foreach (var _enumDeclaration in _enumDeclarations)
    {
        var _enumMdEn = new StringBuilder();
        var _enumMdEs = new StringBuilder();
        var _enumName = _enumDeclaration.Identifier.Text;
        var _enumSummary = ExtractXmlDoc(_enumDeclaration);
        var _enumDoc = ExtractXmlDoc(_enumDeclaration);

        genumNames.Add(_enumDeclaration.Identifier.Text);
        _enumMdEn.AppendLine($"# 🧷 {_enumName}\n\n> {_enumSummary}\n\n## Enum Members\n");
        _enumMdEs.AppendLine($"# 🧷 {_enumName}\n\n> {Translate(_enumSummary)}\n\n## Miembros del Enumerado\n");

        foreach (var _member in _enumDeclaration.Members)
        {
            var _memberDoc = ExtractXmlDoc(_member);
            var _translateDoc = await Translate(_memberDoc);

            _enumMdEn.AppendLine($"- `{_member.Identifier}` — {_memberDoc ?? "No XML documentation."}");
            _enumMdEs.AppendLine($"- `{_member.Identifier}` — {_translateDoc ?? "Sin documentación XML."}");
        }
        _enumMdEn.AppendLine($"\n---\n[Ver en Español]({ggithubBaseUrl}/{_enumName}_ES)");
        _enumMdEs.AppendLine($"\n---\n[View in English]({ggithubBaseUrl}/{_enumName})");

        await File.WriteAllTextAsync(Path.Combine(goutputPathEn, $"{_enumName}.md"), _enumMdEn.ToString());
        await File.WriteAllTextAsync(Path.Combine(goutputPathEs, $"{_enumName}_ES.md"), _enumMdEs.ToString());
    }
}

// TOC & HOME GENERATION
var _enFiles = Directory.GetFiles(goutputPathEn, "*.md")
    .Where(f => !Path.GetFileName(f).StartsWith("Home") && !Path.GetFileName(f).StartsWith("TOC"))
    .OrderBy(f => f);
var _esFiles = Directory.GetFiles(goutputPathEs, "*.md")
    .Where(f => !Path.GetFileName(f).StartsWith("Home") && !Path.GetFileName(f).StartsWith("TOC"))
    .OrderBy(f => f);
var _tocEn = "# Table of Contents\n\n" + string.Join("\n", _enFiles.Select(f =>
{
    var _page = Path.GetFileNameWithoutExtension(f);
    return $"- [{_page}]({ggithubBaseUrl}/{_page})";
}));
var _tocEs = "# Índice de Contenidos\n\n" + string.Join("\n", _esFiles.Select(f =>
{
    var _page = Path.GetFileNameWithoutExtension(f);
    return $"- [{_page.Replace("_ES", "")}]({ggithubBaseUrl}/{_page})";
}));
var _homeEn = "# 🌐 SOLTEC.Core Wiki (English)\n" +
              "Welcome to the official documentation for **SOLTEC.Core**, a .NET library that provides utilities for secure HTTP communication, file management, data encryption, response standardization, and more.\n" +
              "---\n" +
              "## 📌 Quick Start\n" +
              "- [📚 View All Components](TOC)\n" +
              "- 🇪🇸 [View this page in Spanish](Home_ES)\n" +
              "---\n" +
              "## 🧩 What You'll Find Here\n" +
              "- 📦 Public classes with IntelliSense-ready XML comments\n" +
              "- 🧪 Unit-tested methods with xUnit and NUnit\n" +
              "- 🔐 Custom exception handling\n" +
              "- 🧰 Helpers for cryptography, HTTP, and validation\n" +
              "---\n" +
              "## 🛠️ Current Version: 1.0.0\n" +
              "Check the [Changelog](https://github.com/JuanMa7310/SOLTEC.Core/wiki/CHANGELOG) or [Features](https://github.com/JuanMa7310/SOLTEC.Core/wiki/FEATURES) for full capabilities.\n";
var _homeEs = "# 🌐 Wiki de SOLTEC.Core (Español)\n" +
              "Bienvenido a la documentación oficial de **SOLTEC.Core**, una librería en .NET que proporciona utilidades para comunicación HTTP segura, manejo de archivos, encriptación de datos, estandarización de respuestas, y más.\n" +
              "---\n" +
              "## 📌 Inicio Rápido\n" +
              "- [📚 Ver todos los componentes](TOC_ES)\n" +
              "- 🇬🇧 [View this page in English](Home)\n" +
              "---\n" +
              "## 🧩 ¿Qué encontrarás aquí?\n" +
              "- 📦 Clases públicas con comentarios XML compatibles con IntelliSense\n" +
              "- 🧪 Métodos verificados con pruebas unitarias en xUnit y NUnit\n" +
              "- 🔐 Manejadores personalizados de excepciones\n" +
              "- 🧰 Herramientas de cifrado, HTTP y validación\n" +
              "---\n" +
              "## 🛠️ Versión actual: 1.0.0\n" +
              "Consulta el [Registro de Cambios](https://github.com/JuanMa7310/SOLTEC.Core/wiki/CHANGELOG_ES) o las [Características](https://github.com/JuanMa7310/SOLTEC.Core/wiki/FEATURES_ES) para conocer todas las capacidades.\n";

await File.WriteAllTextAsync(Path.Combine(goutputPathEn, "Home.md"), _homeEn);
await File.WriteAllTextAsync(Path.Combine(goutputPathEs, "Home_ES.md"), _homeEs);
await File.WriteAllTextAsync(Path.Combine(goutputPathEn, "TOC.md"), _tocEn);
await File.WriteAllTextAsync(Path.Combine(goutputPathEs, "TOC_ES.md"), _tocEs);
await File.WriteAllTextAsync(Path.Combine(goutputPathSidebar, "_Sidebar.md"), GenerateSidebar(gclassNamespaceMap, genumNames));

Console.WriteLine("✅ Wiki Markdown generation completed.");

// Helper methods
static string? ExtractXmlDoc(SyntaxNode node)
{
    var _trivia = node.GetLeadingTrivia()
                    .FirstOrDefault(t => t.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia) ||
                                         t.IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia));
    if (_trivia == default) 
        return null;
    var _xml = _trivia.ToFullString();
    return string.Join(" ", _xml.Replace("///", "").Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries).Select(l => l.Trim()));
}
static async Task<string> Translate(string? input)
{
    if (string.IsNullOrWhiteSpace(input))
    {
        Console.WriteLine("⚠️ No translation available for empty or null input.");
        return input ?? string.Empty;
    }

    using var _httpClient = new HttpClient();
    try
    {
        var _url = $"https://api.mymemory.translated.net/get?q={Uri.EscapeDataString(input)}&langpair=en|es";
        var _response = await _httpClient.GetStringAsync(_url);
        var _json = JsonDocument.Parse(_response);
        var _translatedText = _json.RootElement
            .GetProperty("responseData")
            .GetProperty("translatedText")
            .GetString();

        return _translatedText ?? "[Translation Error]";
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[Translation Error: {ex.Message}]");
        return "[ES] " + input;
    }
}
static string GenerateSidebar(Dictionary<string, string> classNamespaceMap, List<string> enumNames)
{
    var _sb = new StringBuilder();
    _sb.AppendLine("## 📦 Classes");

    // Agrupar clases por tercer nivel de namespace
    var _grouped = classNamespaceMap
        .GroupBy(c => c.Value.Split('.').Length >= 3 ? c.Value.Split('.')[2] : "Global")
        .OrderBy(g => g.Key);

    foreach (var _group in _grouped)
    {
        var _sectionName = _group.Key;
        var _sectionNameEs = _sectionName switch
        {
            "Connections" => "Conexiones",
            "Encryptions" => "Cifrados",
            "Exceptions" => "Excepciones",
            "DTOS" => "DTOS",
            "Enums" => "Enumeraciones",
            _ => _sectionName
        };

        _sb.AppendLine($"### 🔹 {_sectionName}");
        foreach (var _cls in _group.OrderBy(c => c.Key))
        {
            _sb.AppendLine($"- [{_cls.Key}](/en/{_cls.Key}) | [🇪🇸](/es/{_cls.Key}_ES)");
        }
    }

    _sb.AppendLine();
    _sb.AppendLine("## 🎛️ Enums");
    foreach (var _enum in enumNames.OrderBy(n => n))
    {
        _sb.AppendLine($"- [{_enum}](/en/{_enum}) | [🇪🇸](/es/{_enum}_ES)");
    }
    return _sb.ToString();
}
static string FindProjectSourcePath()
{
    var _dir = Directory.GetCurrentDirectory();
    while (_dir != null)
    {
        var _slnFile = Directory.GetFiles(_dir, "*.sln").FirstOrDefault();
        if (_slnFile != null)
        {
            var _solutionName = Path.GetFileNameWithoutExtension(_slnFile);
            var _candidate = Path.Combine(_dir, _solutionName, _solutionName + ".csproj");
            if (File.Exists(_candidate))
                return Path.Combine(_dir, _solutionName);
        }
        _dir = Directory.GetParent(_dir)?.FullName;
    }
    throw new DirectoryNotFoundException("❌ Could not locate the main project path.");
}
