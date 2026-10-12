# OBD : On-Boarding Day

> **Version :** 2.0 (build 11) · **Technologie :** .NET 11, .NET MAUI 11 · Android 7.0+ (API 24) / iOS 15+ · C# 15 (`LangVersion` latest)

[Google Play](https://play.google.com/store/apps/details?id=fr.mafyou.obd)
[App Store](https://apps.apple.com/fr/app/obd-on-boarding-day/id6773454327)

Application mobile pensée pour accompagner une personne en entreprise pendant son onboarding. La promesse produit est simple : aider à réussir son arrivée dans l'entreprise sans se perdre dans les informations, les personnes à retenir et les habitudes internes.

## Fiche Play Store

### Titre de l'application
OBD Assistant Onboarding

### Description courte
Réussis ton onboarding en entreprise avec contacts, notes, croquis et repères essentiels.

### Description longue
Commencer un nouveau contrat peut vite devenir stressant : trop d'informations, trop de nouvelles personnes, et peu de temps pour tout retenir. OBD t'aide à organiser ton onboarding pas à pas pour rester serein, structuré et efficace dès les premiers jours.

Fonctionnalités principales :
- 👥 Contacts onboarding : enregistre collègues, postes, secteurs et notes rapides.
- 🗂️ Notes par secteurs : structure tes informations avec notes texte et croquis.
- #️⃣ Hashtags cliquables : relie un mot-clé à un croquis en un tap.
- 🔒 Notes sensibles : protège tes contenus personnels avec la biométrie.
- 🔎 Recherche globale : retrouve vite contacts, notes, titres et mots-clés.
- ⚙️ Repères de travail : centralise réunions, télétravail et informations N+1.

Public cible :
OBD s'adresse aux personnes en entreprise qui rejoignent une nouvelle entreprise et veulent une mémoire personnelle claire, simple et confidentielle.

Appel à l'action :
Télécharge OBD sur Google Play et transforme ton onboarding en parcours clair, organisé et maîtrisé.

## Fiche App Store

### Titre de l'application
OBD Assistant Onboarding

### Description courte
Réussis ton arrivée en entreprise avec contacts, notes sectorisées, croquis et repères de travail.

### Description longue
Ton arrivée en entreprise mérite un vrai support personnel : entre nouveaux collègues, règles internes et habitudes de travail, il est facile d'oublier l'essentiel. OBD t'aide à garder une vision claire de ton onboarding et à mémoriser ce qui compte, au bon moment.

Fonctionnalités principales :
-  Contacts utiles : conserve noms, postes, secteurs et notes courtes.
-  Organisation par secteurs : classe les informations pour les retrouver facilement.
-  Notes et croquis : combine texte, dessins et mots-clés selon tes besoins.
-  Liens intelligents : associe des hashtags à des croquis existants.
-  Confidentialité : protège les notes sensibles avec Face ID ou empreinte.
-  Recherche rapide : accède immédiatement aux infos clés de ton onboarding.

Public cible :
OBD est conçue pour les nouveaux collaborateurs en entreprise qui veulent gagner en autonomie, réduire leur charge mentale et réussir leur intégration.

Appel à l'action :
Télécharge OBD sur l'App Store et démarre ton onboarding avec une méthode simple, structurée et efficace.

## 🍎 App Store

> **Paquet :** `fr.mafyou.obd` · **Fiche :** <https://apps.apple.com/fr/app/obd-on-boarding-day/id6773454327> · **Version :** 2.0 (build 11)

L'App Store n'interprète pas le Markdown et refuse les emojis : les textes ci-dessous se collent tels quels.

### Nouveautés de la version 2.0 — Français *(4000 caractères max, ici moins de 500)*
```
OBD parle maintenant français et anglais. Choisissez votre langue dans Paramètres > Langue : tout l'écran bascule aussitôt.

Si l'application s'est arrêtée sur une erreur, elle ouvre au lancement suivant un e-mail déjà rédigé avec le rapport. Vous décidez de l'envoyer ou non.

Nouvelle page Diagnostics : secouez deux fois le téléphone pour voir le dernier incident, l'envoyer par e-mail ou l'effacer.
```

### What's New in version 2.0 — English
```
OBD now speaks French and English. Pick your language in Settings > Language: the whole screen switches at once.

If the app stopped on an error, the next launch opens an e-mail already written with the report. You decide whether to send it.

New Diagnostics page: shake the phone twice to see the last incident, send it by e-mail or clear it.
```

### Notes pour la revue Apple *(App Review Information → Notes, en anglais)*
```
OBD is a personal notebook for the first weeks in a new job: people, notes and sketches sorted by sector, and work habits. Everything is stored on the device, in a local database.

No account and no sign-in: none exists in the app. No purchase, no subscription, no ads. The app has no server and makes no network request. The links of Settings > About (privacy policy, source code, overview page, help) only open the browser, and the crash report only opens the Mail composer: nothing leaves the device unless the user presses Send.

FIRST LAUNCH
The app asks once how to start: "Pre-filled OBD" creates sample people, sectors and notes, "Empty OBD" starts from scratch. Pre-filled is the quickest way to review.

LANGUAGE
French and English. Settings tab > Language switches the whole interface at once.

PERMISSIONS (their purpose strings are written in French)
- Face ID: Notes tab > the lock button at the top opens the "Sensitive notes" section after Face ID or Touch ID. A note goes there with a long press on it in the list of its sector. On a device without biometrics the app says the section is not available.
- Camera: while editing a sketch (Notes tab > a sector > a sketch), under "Photo background", the camera button takes a photo used as the background of the drawing.
- Photo library: same place, the gallery button picks one photo as the background of the drawing.
The photo stays on the device.

DIAGNOSTICS
Shaking the device twice opens the "Diagnostics" page: last crash recorded, "Send report by email", "Clear crash data", "Get help".
```

La version App Store se construit avec le pipeline manuel [`.github/workflows/CICD-iOS.yml`](.github/workflows/CICD-iOS.yml) (macOS 26, Xcode 26.6, SDK .NET 11), qui signe l'IPA et l'envoie sur TestFlight. Le numéro de build vient du csproj (`ApplicationVersion`), le pipeline ne l'incrémente pas.

## Positionnement

- **Public cible** : une personne en entreprise, en phase d'arrivée dans une nouvelle entreprise.
- **Usage principal** : centraliser les repères utiles dès les premiers jours puis garder une base simple de suivi dans le temps.
- **Promesse** : un onboarding réussi.

## Problème à résoudre

Lors d'un onboarding, tout est nouveau :
- Les collègues et leurs rôles.
- Les secteurs et les interlocuteurs.
- Les habitudes de réunion.
- Les informations RH et managériales.
- Les remarques personnelles qu'on n'ose pas toujours exposer.

L'application doit donc rester simple, personnelle et rapide à utiliser au quotidien.

## Fonctionnalités

### 0. Premier lancement

Au premier démarrage, l'utilisateur choisit entre :
- **OBD pré-rempli** : jeu de données d'exemple (personnes, secteurs, notes) pour découvrir l'application.
- **OBD vide** : démarrage à zéro.

Ce choix n'est proposé qu'une seule fois (mémorisé via les préférences de l'appareil).

### 1. Fiches contacts

Chaque personne enregistrée contient :
- Nom, poste, secteur, note courte.

### 2. Notes par secteurs

Les notes sont organisées par secteurs définis dans les paramètres.

Chaque secteur peut contenir :
- Des **notes texte** courtes ou détaillées, avec support des `#hashtags` liés à des croquis.
- Des **croquis nommés** avec mots-clés associés.

Les `#hashtags` dans une note texte sont cliquables : un tap permet de lier le mot à un croquis existant, ou de naviguer directement vers le croquis lié.

### 3. Repères de fonctionnement

- Réunions habituelles.
- Jours ou règles de télétravail.
- Informations liées au N+1.

### 4. Notes sensibles

Une section protégée par authentification biométrique (empreinte / Face ID).

- Accès via le bouton 🔒 dans l'onglet Notes.
- Marquer une note comme sensible : appui long dans la liste du secteur.
- Démarquer : appui long dans la section sensible.
- Contenu jamais affiché en dehors de cette section.

### 5. Recherche globale

Accessible depuis n'importe quel onglet. Recherche simultanément dans les contacts, les notes (contenu, titre, mots-clés) et les secteurs.

## Principes UX

- Saisie rapide entre deux réunions.
- Structure simple et personnelle.
- Classement clair par secteurs.
- Icônes Material Symbols Outlined cohérentes avec le thème émeraude foncé.
- Discrétion sur les contenus sensibles.

## Structure fonctionnelle

Navigation principale en 3 onglets (bottom navigation).

### Onglet 1 : Personnes

- Liste des contacts avec filtre local.
- FAB pour créer un contact rapidement.

### Onglet 2 : Notes

- Liste des secteurs.
- Drill-down : secteur → liste des notes → note.
- Appui long sur une note → la marque comme sensible (elle disparaît de la liste).
- Bouton 🔒 dans le header → accès aux notes sensibles après auth biométrique.

### Onglet 3 : Paramètres

- Définition des secteurs.
- Repères de travail : réunions habituelles, télétravail, N+1.

### Recherche globale

Accessible via l'icône loupe dans chaque header d'onglet. Clavier affiché automatiquement à l'ouverture.

## Valeur produit

Cette application n'essaie pas de remplacer les outils internes de l'entreprise. Elle sert plutôt de mémoire personnelle structurée pour traverser l'onboarding avec plus de clarté, plus d'autonomie et moins de charge mentale.

## Technique

- **.NET 11 MAUI** (SDK `11.0.100-rc.1.26425.128`, `Microsoft.Maui.Controls` 11.0.0-rc.1.26451.6) — `net11.0-android` (API 24 minimum) et `net11.0-ios` (iOS 15 minimum).
- **CommunityToolkit.Mvvm 8.4.2** — `[ObservableProperty]`, `[RelayCommand]`, source generators AOT-friendly.
- **CommunityToolkit.Maui 15.0.1** — `DrawingView` pour les croquis, `TouchBehavior` pour les appuis longs, popups et Toast.
- **sqlite-net-pcl 1.11.285** et **SQLitePCLRaw.bundle_e_sqlite3 3.0.5** — stockage local, migration automatique des colonnes.
- **Plugin.Fingerprint 3.0.0-beta.1** — authentification biométrique (empreinte / Face ID).
- **Material Symbols Outlined** — icônes via font intégrée.
- Release Android : runtime CoreCLR et précompilation ReadyToRun (défauts de .NET 11), pour `arm64-v8a` et `x86_64`.
- Pas de synchronisation cloud — tout est local pour garantir la confidentialité.
- Interface en français et en anglais (`AppResources.resx` / `AppResources.en.resx`), langue au choix dans les Paramètres.
- Tests : xUnit v3, Shouldly, Moq (`dotnet test tests/OBD.Mobile.Lib.UnitTests/OBD.Mobile.Lib.UnitTests.csproj`).
- Compilation : `dotnet build src/Fronts/OBD.Mobile/OBD.Mobile.csproj -f net11.0-android -c Release` (ou `-f net11.0-ios` ; sous Windows sans Mac, seul le code managé iOS est compilé).

## Architecture

```
src/Fronts/
  OBD.Mobile               → Application MAUI (pages, ViewModels, extensions DI)
  OBD.Mobile.Lib           → Modèles, services, DatabaseContext
    ├── Platforms/Android  → DI platform-specific
    └── Platforms/iOS      → DI platform-specific
tests/
  OBD.Mobile.Lib.UnitTests → Tests xUnit / Shouldly / Moq (cible OBD.Mobile.Lib)
```

Règle : tout ce qui n'est pas ViewModel ou page va dans `OBD.Mobile.Lib`.

## Contact

- 🌐 Site Web : [https://mafyouit.tech/apps](https://mafyouit.tech/apps)
- 📧 Email : [contact@mafyouit.tech](mailto:contact@mafyouit.tech)
- 🆘 Support : [https://mafyouit.tech/AskHelp](https://mafyouit.tech/AskHelp)
