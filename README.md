
Mon projet implémente une API pour la gestion de posts et de commentaires avec stockage d'images sur Azure Blob Storage via Azurite. L'API est documentée avec Swagger et utilise Entity Framework Core en mémoire.

Program.cs: Configure les services, définit les routes et initialise l'application.

IRepository_mini.cs: Définit les interfaces des méthodes pour interagir avec les données.

EFRepository_mini.cs: Implémente les méthodes définies dans IRepository_mini.cs en utilisant Entity Framework.

Résolution des erreurs: J'ai eu plusieurs erreurs depuis l'installation de azurite et pour les résoudre j'ai utilisé stackoverflow et chatgpt

Exécution:(Dans la remise, vous avez pas marqué de rendre tout le dossier donc cette partie ne fonctionnera pas de votre coté)

Installation de azurite: npm install -g azurite (dans le terminal)

Lancer Azurite: azurite

Tester via Swagger: /swagger (sur l'url du site web)
