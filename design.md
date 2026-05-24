# Design

## Direction générale

Base flat avec accents glassmorphisme.
- Glassmorphisme uniquement sur les cartes et les modals, pas sur toute l'interface.
- La section sensible a un traitement volontairement plus opaque et plus sombre — elle doit visuellement se sentir séparée du reste.

## Mode

Dark mode first. Light mode à envisager plus tard.

## Palette

| Rôle                  | Valeur                    | Notes                                  |
|-----------------------|---------------------------|----------------------------------------|
| Fond principal        | `#0F1117`                 | Quasi-noir, légèrement froid           |
| Surface (cards)       | `#1A1D24`                 | Fond des éléments posés                |
| Glassmorphisme        | `rgba(26, 29, 36, 0.6)`   | + blur — modals, cartes flottantes     |
| Accent principal      | `#10B981`                 | Vert émeraude — FAB, highlights, tags  |
| Accent foncé          | `#065F46`                 | Hover, pressed states                  |
| Texte primaire        | `#F1F5F9`                 |                                        |
| Texte secondaire      | `#64748B`                 | Labels, placeholders                   |
| Séparateurs           | `#2A2D34`                 |                                        |
| Section sensible      | `#0A0C10`                 | Plus sombre que le fond, intentionnel  |

## Typographie

Geist.

## Iconographie

Material Symbols Outlined (Google). Font embarquée dans le projet.
