# Toon Tank Unity Network Game

Welcome to the repository for the Toon Tank Unity Network Game! This project is a multiplayer tank game built with Unity and Mirror for networking. Dive into a cartoonish world where players control tanks, battling it out to see which team emerges victorious.

<div align="center">
  <video src="https://github.com/ahmedafifiabodu/Unity-Network-Game/assets/74466733/da76eb3f-ec43-4b34-9878-13064801e0c4" width="400" />
</div>

## Features

- Multiplayer gameplay using Unity's Mirror networking.
- Cartoonish, engaging graphics and tank models.
- Dynamic battle arenas.
- Real-time player notifications for kills, revives, and team victories.

## Scripts Overview

This project is powered by several Unity C# scripts that handle everything from networking to player control. Here's a brief overview of the scripts used:

### Networking
- `NetworkingManager.cs` - Manages all network operations, including server start and player connections.

### UI Management
- `UIManager.cs` - Handles all UI notifications for player actions, such as kills, revives, and team victories.

### Player
- `PlayerController.cs` - Manages player movement, shooting mechanics, and health.
- `PlayerHealth.cs` - Handles player health, damage, and death.

### Utilities
- `Singleton.cs` - A generic singleton class for objects that should only have one instance.

## Getting Started

To get started with the Toon Tank Unity Network Game, follow these steps:

1. Clone this repository to your local machine.
2. Open the project in Unity (version 2022.1 or newer recommended).
3. Navigate to the `Assets/Lecture 3 - 4/Scripts` directory to explore the scripts.
4. Hit play in Unity to start the game in the editor, or build the game to play as a standalone application.

## Contributing

We welcome contributions to the Toon Tank Unity Network Game! If you have suggestions for improvements or new features, feel free to open an issue or submit a pull request.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.
