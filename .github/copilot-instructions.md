# Copilot Instructions

> Les conventions supplémentaires (ex: fiche Play Store) sont définies dans [CLAUDE.md](.github/CLAUDE.md) — s'y référer systématiquement.


## General Guidelines
- Utilise toujours cette stack:
  - dotnet 10 (https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-10/overview)
  - C#14.0 (https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-14) LangVersion = latest
- Toujours compiler uniquement le projet dans le quel on est.
- Compiler juste le projet où on a fait des changements, pas la solution entière.
- Quand l'utilisateur référence un projet ou une méthode spécifique, n'appliquer les modifications que dans ce périmètre exact.

## Code Style
- N'oublie pas:
  - GetFromJsonAsync, PostJsonAsync et IHttpClientFactory.
  - Encoding UTF8 pour supporter les é,è,ô,ù
  - is null et is not null à la place de == et !=
  - Pas de .ToString() pour afficher des objets, utilise l'interpolation de chaîne $"{}"
  - Les records structs pour les petits objets immuables (https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-10#record-structs)
  - Les primary constructors pour les classes (https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-12#primary-constructors-for-classes)
  - Utiliser field du C#14.0 (https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-14.0/field-keyword)
  - Utiliser les extensions members à la place des méthodes d'extensions (https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-14.0/extensions)
  - CancellationToken s'appelle toujours stoppingToken dans les méthodes async

## Logging and Testing
- Pour les logs, toujours utiliser Serilog (https://serilog.net) avec logs circulaires quand on dépasse 500 messages.
- Pour les tests unitaires et intégrations, toujours utiliser xUnit (https://xunit.net), Shouldly et Moq.

## Web Development
- En web:
  - Ne jamais mettre du CSS dans une page ! Toujours dans un fichier à part le CSS

## Mobile Development
- En mobile:
  - Toujours utiliser les animations FadeToAsync, TranslateToAsync et ScaleToAsync, pas FadeTo, TranslateTo ou ScaleTo.
  - Jamais de Frame, toujours des Border
  - Mettre les styles dans Styles.xaml
  - Ne jamais hardcoder les couleurs mais les mettre deans Colors.xaml

## Document Formatting
- Le CV ResumeFuture.cshtml doit tenir sur exactement une page A4 à l'impression, pas plus.