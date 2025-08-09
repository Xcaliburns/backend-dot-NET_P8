![Build Status](https://github.com/Xcaliburns/backend-dot-NET_P8/actions/workflows/dotnet.yml/badge.svg)

# 🧭 TourGuide API (.NET)

Cette API simule un service de guide touristique. Elle intègre plusieurs services externes fictifs (GPS, récompenses, tarification) et met l’accent sur la performance, l’asynchronisme et la modularité.

---

## 🚀 Objectifs

- Implémenter une API REST en C# avec ASP.NET Core
- Simuler des services tiers via des modules internes
- Utiliser le code asynchrone pour améliorer la scalabilité
- Mettre en place des tests unitaires et de performance
- Automatiser les builds et tests via GitHub Actions

---

## 🧱 Structure du projet

```text
├── Api/               # Contrôleurs et logique API
├── GpsUtil/           # Service simulé de géolocalisation
├── RewardCentral/     # Service simulé de récompenses
├── TripPricer/        # Service simulé de tarification
├── TourGuideTest/     # Tests unitaires
├── TourGuide.sln      # Solution Visual Studio

## ⚙️ Technologies

- **ASP.NET Core (.NET 6+)**
- **C#**
- **xUnit** (tests unitaires)
- **GitHub Actions** (CI/CD)
- **Services simulés en mémoire**
- **Programmation asynchrone** (`async/await`)

---

## 🔄 Asynchronisme

L’API utilise des méthodes `async` pour :

- Traiter les requêtes de manière non bloquante
- Améliorer les performances sous charge
- Simuler des appels réseau vers les services GPS, récompenses et tarification

Exemple :
```csharp
public async Task<List<Attraction>> GetNearbyAttractionsAsync(Location userLocation)
{
    var attractions = await _gpsService.GetAttractionsAsync();
    return attractions.Where(a => IsNearby(a.Location, userLocation)).ToList();
}

## 🧪 Tests & Performance

- Tests automatisés via GitHub Actions  
- Artefacts de test publiés et téléchargeables  
- Optimisation du code pour la performance (`Thread.Sleep` supprimé)

---

## 📦 Fonctionnalités de workflow GitHub Actions

- ✅ Build automatique  
- 🧪 Tests unitaires  
- 📤 Publication des artefacts  
- 📥 Téléchargement des résultats de tests

---

## 📚 Utilisation

Aucune base de données n’est requise pour ce projet.  
Les services sont simulés en mémoire, ce qui permet une exécution rapide et légère.  
L’API peut être testée directement via Swagger.

---

## 📂 Instructions de démarrage

1. Forkez la branche `master`  
2. Assurez-vous que le projet build correctement  
3. Lancez l’API et accédez à Swagger pour tester les endpoints


