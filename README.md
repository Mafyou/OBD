# OBD : On-Boarding Day

Application mobile pensée pour accompagner une personne en CDI pendant son onboarding. La promesse produit est simple : aider à réussir son arrivée dans l'entreprise sans se perdre dans les informations, les personnes à retenir et les habitudes internes.

## Positionnement

- Public cible : une personne en CDI, en phase d'arrivée dans une entreprise.
- Usage principal : centraliser les repères utiles dès les premiers jours puis garder une base simple de suivi dans le temps.
- Promesse : un onboard réussi.

## Problème à résoudre

Lors d'un onboarding, tout est nouveau :
- Les collègues et leurs rôles.
- Les secteurs et les interlocuteurs.
- Les habitudes de réunion.
- Les informations RH et managériales.
- Les remarques personnelles qu'on n'ose pas toujours exposer.

L'application doit donc rester simple, personnelle et rapide à utiliser au quotidien.

## Fonctionnalités retenues

### 1. Fiches contacts simplifiées

Chaque personne enregistrée contient uniquement :
- Nom.
- Poste.
- Secteur.
- Note courte.

Objectif : aller à l'essentiel pour retenir rapidement qui est qui, sans créer une fiche trop lourde à remplir.

### 2. Notes par secteurs

Les notes sont organisées par secteurs définis dans les paramètres de l'application.

Chaque secteur peut contenir :
- Des notes texte courtes ou plus détaillées.
- Des croquis rapides avec mots-clés associés pour pouvoir les retrouver facilement.

Objectif : ne pas mélanger les sujets et retrouver vite une information liée à un domaine précis.

### 3. Repères de fonctionnement

L'application stocke les informations pratiques récurrentes :
- Heures des réunions habituelles.
- Jours ou règles de télétravail (TT).
- Informations liées au N+1.

Objectif : garder sous la main les rythmes de travail et les repères de fonctionnement quotidiens.

### 4. Section de notes critiques

Une section permet de conserver des notes plus critiques ou plus sensibles sur des processus globaux déjà en place depuis longtemps.

Contraintes produit :
- Cette section reste disponible à tout moment.
- Elle doit être discrète ou cachée dans l'interface.
- Elle ne doit pas être exposée de manière évidente, car l'entreprise ne sera pas forcément prête à lire ce type de remarques.

Objectif : protéger un espace de réflexion personnelle sans en faire une zone visible par défaut.

## Principes UX

- Saisie rapide, car l'utilisateur note souvent une information entre deux réunions.
- Structure simple, car l'app sert d'outil personnel avant d'être un outil collaboratif.
- Classement clair par secteurs, pour éviter les mélanges.
- Recherche facile sur les mots-clés des notes et des croquis.
- Discrétion sur les contenus sensibles.

## Structure fonctionnelle

Navigation principale en 3 onglets (bottom navigation).

### Onglet 1 : Personnes

- Liste des contacts.
- Fiche minimale : nom, poste, secteur, note courte.
- FAB "+" pour créer un contact rapidement.
- Recherche rapide intégrée.

### Onglet 2 : Notes

- Liste des secteurs définis dans les paramètres.
- Drill-down : secteur → liste des notes → note.
- Chaque note est soit un texte, soit un croquis avec mots-clés.
- FAB "+" pour créer une note dans le secteur actif.

### Onglet 3 : Paramètres

- Définition des secteurs.
- Repères de travail : réunions habituelles, TT, N+1.
- Réglages de visibilité et gestion de la section sensible.

### Recherche globale

Barre persistante accessible depuis n'importe quel onglet. Recherche simultanément dans les contacts et les notes (texte et mots-clés des croquis).

### Notes sensibles

Non exposées dans la navigation principale. Accessibles via un geste discret (à définir : long-press sur un élément neutre, double-tap sur le logo...). Contenu jamais affiché en dehors de cette section.

## MVP

- Fiches contacts minimales.
- Secteurs personnalisables.
- Notes texte avec mots-clés.
- Repères de travail : réunions habituelles, TT, N+1.
- Recherche globale contacts + notes.

## Valeur produit

Cette application n'essaie pas de remplacer les outils internes de l'entreprise. Elle sert plutôt de mémoire personnelle structurée pour traverser l'onboarding avec plus de clarté, plus d'autonomie et moins de charge mentale.

## Technique

- Utiliser Community Toolkit pour la gestion de l'état, les commandes, le MVVM.
- `DrawingView` de CommunityToolkit.Maui pour les croquis (déjà inclus, pas de dépendance supplémentaire).
- Plugin.Notifications pour les rappels de réunions habituelles.
- Mettre tout dans OBD.Lib si ce n'est pas des ViewModels.
- Pas de synchronisation cloud, tout est local pour garantir la confidentialité des données.
- SQLite pour stocker les contacts, les notes, les secteurs et les repères de travail.
- Pas de localisation pour l'instant, mais prévoir une architecture qui permettrait de l'ajouter facilement plus tard.
