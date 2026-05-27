# OBD : On-Boarding Day

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

- **.NET 10 MAUI** — Android, iOS, macCatalyst, Windows.
- **CommunityToolkit.Mvvm 8.4.2** — `[ObservableProperty]`, `[RelayCommand]`, source generators AOT-friendly.
- **CommunityToolkit.Maui 14.1.1** — `DrawingView` pour les croquis, `TouchBehavior` pour les appuis longs.
- **SQLite-net-PCL** — stockage local, migration automatique des colonnes.
- **Plugin.Fingerprint** — authentification biométrique (empreinte / Face ID).
- **Material Symbols Outlined** — icônes via font intégrée.
- **Profiled AOT** activé en Release Android.
- Pas de synchronisation cloud — tout est local pour garantir la confidentialité.
- Pas de localisation pour l'instant.

## Architecture

```
OBD.Mobile               → Application MAUI (pages, ViewModels, extensions DI)
OBD.Mobile.Lib           → Modèles, services, DatabaseContext
  ├── Platforms/Android  → DI platform-specific
  └── Platforms/iOS      → DI platform-specific
OBD.Mobile.Lib.UnitTests → Tests xUnit
```

Règle : tout ce qui n'est pas ViewModel ou page va dans `OBD.Mobile.Lib`.
