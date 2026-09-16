# Ghost Bird setup (Unity 6 URP)

## Objets requis sous `player`

- Composants gameplay à conserver sur **player** : `Rigidbody`, `PlayerMovement`, `PlayerAim`, `PlayerShooting`, `Health`, `PlayerDeath`.
- Enfant obligatoire : `FirePoint` (objet séparé du visuel, devant le joueur sur l'axe local **+Z**).
- Visuel généré : `GhostBirdModel` (créé/recréé par `GhostBirdAvatarBuilder`).

## Reconstruire le modèle

1. Sélectionner `player`.
2. Vérifier le composant `GhostBirdAvatarBuilder`.
3. Ouvrir le menu contexte du composant puis **Build Avatar**.
4. Vérifier qu'il ne reste qu'un seul enfant visuel `GhostBirdModel`.

Le builder recrée entièrement le modèle (corps blanc en flamme, tête noire, bec long, yeux blancs, mèches basses) et assigne des matériaux URP persistants dans `Assets/Materials/GhostBird`.

## Supprimer les anciens modèles

- Supprimer uniquement les anciens objets visuels (`GhostModel`, anciens doublons `GhostBirdModel`) s'ils existent.
- Ne pas supprimer `player`, `FirePoint` ni les composants gameplay.

## Vérifications gameplay après rebuild

- `PlayerShooting.firePoint` doit référencer `player/FirePoint`.
- `FirePoint` doit être placé devant le bec (local `z > 0`).
- `PlayerShooting.projectilePrefab`, `spell1`, `spell2` doivent être assignés.
- Tester en Play Mode : visuel non magenta, orientation du bec cohérente avec l'aim, tir depuis le `FirePoint`.

## Diagnostic scripts manquants

Si Unity affiche `Missing (Mono Script)` ou `The referenced script (Unknown) on this Behaviour is missing!` :

- Menu Unity : **Tools > SpellWorkshop > Report Missing Scripts In Open Scenes**.
- Le rapport liste chaque objet concerné sans supprimer automatiquement de composants.
