# Ghost Bird integration (Unity)

1. Ouvrir `Assets/Scenes/SampleScene.unity`.
2. Sélectionner l'objet `player` (racine gameplay avec `Rigidbody`, `PlayerMovement`, `PlayerAim`, `PlayerShooting`, `Health`, `PlayerDeath`).
3. Vérifier que `GhostBirdAvatarBuilder` est présent, puis cliquer sur **Build Avatar** (menu contexte du composant).
4. Vérifier dans la Hierarchy qu'il n'existe qu'un seul enfant visuel nommé `GhostBirdModel`.
5. Conserver `FirePoint` comme enfant séparé de `player` (ne pas le déplacer sous `GhostBirdModel`).

Résultat attendu :

- Les matériaux sont créés/réutilisés dans `Assets/Materials/GhostBird` avec des shaders URP compatibles.
- Le bec pointe vers l'avant du joueur (axe +Z, aligné avec `PlayerAim`).
- Aucun collider n'est ajouté sur les parties visuelles générées.
- `GhostBirdTurnAnimation` anime le flottement seulement si `GhostBirdModel` existe, sans `NullReferenceException`.
