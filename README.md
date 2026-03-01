# 🌍 CityTrip Planner

A modern web application built with Blazor for planning and organizing city trips with friends. Create detailed itineraries, manage events, and collaborate on travel plans.

## 📖 Full Story

Read the full background story behind this project in [docs/Article developer.md](https://github.com/SergeDePooter/AI-Blazor-Demo/tree/main/docs/Article%20developer.md).

![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)
![Blazor](https://img.shields.io/badge/Blazor-Server-512BD4?logo=blazor)
![License](https://img.shields.io/badge/license-MIT-green)

## ✨ Features

### 🗓️ Trip Management
- **Create City Trips** - Plan trips with a user-friendly 3-step wizard
- **Detailed Itineraries** - Organize your trip day by day
- **Event Scheduling** - Add events with times, descriptions, and locations
- **Google Maps Integration** - Select and visualize locations on interactive maps
- **Image Support** - Add cover images to make your trips more appealing

### 👥 Social Features
- **Share Trips** - Share your itineraries with others
- **Participant Management** - Set maximum participants and manage enrollments
- **Like & Favorite** - Save trips you're interested in
- **My Trips** - View trips you've created or enrolled in

### 👤 User Profiles
- Personal profile management
- Store your preferences and information
- Track your travel history

## 🏗️ Architecture

The application follows **Clean Architecture** principles with a feature-driven structure:

```
CitytripPlanner/
├── CitytripPlanner.Web/           # Blazor Server UI
│   ├── Components/
│   │   ├── Pages/                 # Routable pages
│   │   ├── Layout/                # Layout components
│   │   ├── Citytrips/             # Feature-specific components
│   │   └── Shared/                # Reusable components
│   └── wwwroot/                   # Static assets
│
├── CitytripPlanner.Features/      # Business logic & domain
│   ├── Citytrips/
│   │   ├── Domain/                # Domain entities
│   │   ├── CreateTrip/            # Use cases
│   │   ├── UpdateTrip/
│   │   ├── DeleteTrip/
│   │   └── ...
│   └── UserProfiles/
│
├── CitytripPlanner.Infrastructure/ # Data access & external services
│   ├── Citytrips/
│   └── UserProfiles/
│
└── CitytripPlanner.Tests/         # Unit tests
```

### Key Technologies

- **[.NET 10](https://dotnet.microsoft.com/)** - Latest .NET framework
- **[Blazor Server](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)** - Interactive server-side rendering
- **[MediatR](https://github.com/jbogard/MediatR)** - CQRS and mediator pattern implementation
- **[FluentValidation](https://fluentvalidation.net/)** - Input validation
- **[Google Maps JavaScript API](https://developers.google.com/maps/documentation/javascript)** - Map integration

### Design Patterns

- **CQRS** (Command Query Responsibility Segregation) via MediatR
- **Repository Pattern** for data access abstraction
- **Feature-driven structure** for better organization
- **Clean Architecture** for maintainability and testability

## 🚀 Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (17.12 or later) or [Visual Studio Code](https://code.visualstudio.com/)
- [Google Maps API Key](https://developers.google.com/maps/documentation/javascript/get-api-key) (optional, for map features)

### Local Development

1. **Clone the repository**
   ```bash
   git clone https://github.com/SergeDePooter/demo-speckit-ai.git
   cd demo-speckit-ai/CitytripPlanner
   ```

2. **Configure Google Maps (Optional)**
   
   Edit `CitytripPlanner.Web/appsettings.json` and add:
   ```json
   {
     "GoogleMaps": {
       "ApiKey": "YOUR_API_KEY_HERE"
     }
   }
   ```

3. **Run the application**
   ```bash
   dotnet run --project CitytripPlanner.Web
   ```

4. **Open in browser**
   ```
   https://localhost:5001
   ```

### Running with Visual Studio

1. Open `CitytripPlanner.sln`
2. Set `CitytripPlanner.Web` as the startup project
3. Press F5 to run

## 🐳 Docker Deployment

For production deployment or containerized development, see [Docker Setup Guide](README.Docker.md).

**Quick Start:**
```bash
# Set up environment
cp .env.example .env
# Edit .env and add your Google Maps API key

# Run with Docker Compose
docker-compose up --build

# Access at http://localhost:8080
```

## 🧪 Testing

Run the test suite:
```bash
dotnet test
```

The test project uses:
- xUnit for unit testing
- FluentAssertions for readable assertions
- In-memory implementations for testing

## 📁 Project Structure

### CitytripPlanner.Web
The Blazor Server frontend with:
- **Interactive Server Components** for real-time updates
- **Component-based architecture** for reusability
- **Responsive design** for mobile and desktop

### CitytripPlanner.Features
Business logic organized by feature:
- **Domain Models** - Core entities (Citytrip, DayPlan, ScheduledEvent)
- **Commands** - Write operations (Create, Update, Delete)
- **Queries** - Read operations (List, Get, Filter)
- **Validators** - Input validation using FluentValidation

### CitytripPlanner.Infrastructure
Data persistence and external services:
- **In-Memory Repositories** (development)
- Repository implementations
- External service integrations

### CitytripPlanner.Tests
Comprehensive test coverage:
- Domain model tests
- Handler tests
- Validation tests

## 🎨 Features Showcase

### Trip Creation Wizard

**Step 1: Basic Information**
- Title and destination
- Start and end dates
- Description and participant limit
- Cover image URL

**Step 2: Day-by-Day Itinerary**
- Add events for each day
- Schedule with start/end times
- Event types (Activity, Meal, Transport, Accommodation)
- Location picker with Google Maps

**Step 3: Review & Confirm**
- Review all trip details
- Edit if needed
- Save trip

### Trip Management
- View all public trips
- Filter and search
- View detailed itineraries
- Edit your own trips
- Delete trips you created
- Enroll in trips

## 🔐 Data Storage

**Current Implementation:**
- In-memory storage (data is not persisted)
- Suitable for development and demonstration

## 📝 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🙏 Acknowledgments

- Built with [Blazor](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)
- Uses [Google Maps Platform](https://developers.google.com/maps)
- Inspired by modern travel planning apps

---

**Note:** This is a demonstration project built for educational purposes. The current implementation uses in-memory storage and does not include production-ready authentication or authorization.
