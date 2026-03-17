# Brisbane Airport Management System (C#)

## What it is
A console application that models a small airport booking system. It supports three user roles (Traveller, Frequent Flyer, Flight Manager), seat selection with validation, flight creation & delays, and frequent-flyer points.

## How to run
- .NET SDK: 8.0 (or compatible)
- Build & run:
  dotnet build
  dotnet run

## Features
**Users & Auth**
- Register/Login as Traveller, Frequent Flyer, or Flight Manager.
- Password validation (≥8 chars, upper, lower, digit).
**Traveller**
- Book arrival and departure flights (one of each).
- Choose a seat (rows 1–10, cols A–D) with validation.
- View ticket details.
- Times are checked (arrival must be before departure, and vice-versa).
**Frequent Flyer**
- All traveller features plus:
- View current points and projected points from booked flights.
- If a Frequent Flyer chooses a seat already held by a Traveller,
-the Traveller is automatically reallocated to the next available seat
- scanning across the row, then subsequent rows (wrapping after row 10).
**Flight Manager**
- Create arrival and departure flights (airline, flight ID, plane ID, time).
- Pricing derived from flight ID bands (e.g., 100–199 → 1200, 200–299 → 1250, …).
- Delay a chosen arrival/departure flight by N minutes.
- List all flights (sorted by time).

## Project Structure
BrisbaneAirportSystem/
│
├── Program.cs
│
├── Core/
│   ├── App.cs
│   └── Screen.cs
│
├── Models/
│   ├── ArrivalFlight.cs
│   ├── DepartureFlight.cs
│   └── Traveller.cs
│
├── Users/
│   ├── UserBaseClass.cs
│   ├── FlightManager.cs
│   └── FrequentFlyer.cs
│
├── Services/
│   ├── FlightManagerService.cs
│   └── UserRepository.cs
│
├── Screens/
│   ├── StartupScreen.cs
│   ├── LoginScreen.cs
│   ├── RegisterScreen.cs
│   ├── TravellerMenuScreen.cs
│   ├── FlightManagerMenu.cs
│   └── FrequentFlyerMenuScreen.cs
│
└── README.md
