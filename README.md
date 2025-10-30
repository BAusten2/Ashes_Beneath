# Ashes Beneath

A first-person horror survival game built in Unity, where darkness is your greatest enemy.

![Unity Version](https://img.shields.io/badge/Unity-2021.3%2B-black?logo=unity)
![License](https://img.shields.io/badge/License-MIT-blue)
![Game Status](https://img.shields.io/badge/Status-In%20Development-yellow)

## 📖 Description

**Ashes Beneath** is an atmospheric horror game that challenges players to explore dark, foreboding environments while managing limited resources. Navigate through eerie locations with only your flashlight to guide you, but beware—your battery won't last forever.

## ✨ Features

### Core Gameplay
- **🔦 Dynamic Flashlight System** - Your only source of light in the darkness
- **🔋 Battery Management** - Scavenge for batteries to keep your flashlight powered
- **💾 Save/Load System** - Complete save system that preserves:
  - Player position
  - Flashlight state (on/off, battery level)
  - Collected items
  - Inventory (spare batteries)
  - World state (removed pickups)

### Technical Features
- First-person exploration mechanics
- Interactive item pickup system
- Persistent game state across play sessions
- Real-time battery depletion
- UI feedback for battery levels and item collection
- Collectible tracking system

## 🎮 How to Play

### Controls
- **WASD** - Move
- **Mouse** - Look around
- **E** - Interact/Pick up items
- **F** - Toggle flashlight (when equipped)
- **B** - Replace battery (when available)

### Gameplay Tips
- Keep your flashlight off when possible to conserve battery
- Collect spare batteries whenever you find them
- Save your progress frequently
- Listen carefully—sometimes you hear things before you see them

## 🛠️ Installation

### For Players
1. Download the latest release from the [Releases](../../releases) page
2. Extract the ZIP file
3. Run `Ashes_Beneath.exe` (Windows) or the equivalent for your platform
4. Enjoy!

### For Developers
1. Clone the repository:
   ```bash
   git clone https://github.com/BAusten2/Ashes_Beneath.git
   ```
2. Open the project in Unity (2021.3 or newer recommended)
3. Open the main scene located in `Assets/Scenes/`
4. Press Play to test

## 📋 Requirements

### Minimum System Requirements
- **OS:** Windows 7/8/10/11 (64-bit)
- **Processor:** Intel Core i3 or equivalent
- **Memory:** 4 GB RAM
- **Graphics:** NVIDIA GTX 660 or AMD Radeon HD 7850
- **DirectX:** Version 11
- **Storage:** 2 GB available space

### Recommended System Requirements
- **OS:** Windows 10/11 (64-bit)
- **Processor:** Intel Core i5 or equivalent
- **Memory:** 8 GB RAM
- **Graphics:** NVIDIA GTX 1060 or AMD Radeon RX 580
- **DirectX:** Version 12
- **Storage:** 2 GB available space

## 🏗️ Project Structure

```
Ashes_Beneath/
├── Assets/
│   ├── Scripts/          # Core game scripts
│   │   ├── PlayerLoadSave.cs
│   │   ├── FlashlightScript.cs
│   │   ├── BatteryPickup.cs
│   │   ├── SaveManager.cs
│   │   └── etc...
│   ├── Scenes/             # Game scenes
│   ├── Prefabs/            # Reusable game objects
│   ├── Materials/          # Visual materials
│   ├── Audio/              # Sound effects and music
│   └── UI/                 # User interface elements
├── ProjectSettings/
└── README.md
```

## 🔧 Key Systems

### Save System
The game features a robust save system that preserves the complete game state:
- Player position and orientation
- Flashlight ownership and battery level
- Flashlight on/off state
- Spare battery inventory
- All collected items in the world

### Flashlight Mechanics
- Realistic battery depletion over time
- Visual battery indicator with 20 charge levels
- Smooth light toggling
- Audio feedback for interactions

### Item Collection
- Unique ID system prevents duplicate collection after loading
- Persistent removal of collected items
- Inventory management for consumables

## 👥 Contributors

<table>
  <tr>
    <td align="center">
      <a href="https://github.com/BAusten2">
        <img src="https://github.com/BAusten2.png" width="100px;" alt="BAusten2"/>
        <br />
        <sub><b>BRODY</b></sub>
      </a>
    </td>
    <td align="center">
      <a href="https://github.com/Jasmi343">
        <img src="https://github.com/Jasmit343.png" width="100px;" alt="Jasmit343"/>
        <br />
        <sub><b>Jasmit</b></sub>
      </a>
    </td>
    <td align="center">
      <a href="https://github.com/Jlowkei9">
        <img src="https://github.com/Jlowkee19.png" width="100px;" alt="Jlowkee19"/>
        <br />
        <sub><b>Jlowkee19</b></sub>
      </a>
      <br />
      <sub>Lorenz Soriano</sub>
    </td>
  </tr>
</table>

## 🐛 Known Issues

- None currently reported

## 📝 Changelog

### [Unreleased]
- Save/load system implementation
- Flashlight mechanics refinement
- Battery collection system
- UI improvements

## 🚀 Roadmap

- [ ] Additional levels and environments
- [ ] More interactive objects
- [ ] Story elements and narrative
- [ ] Additional survival mechanics
- [ ] Sound design improvements
- [ ] Performance optimizations

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request. For major changes, please open an issue first to discuss what you would like to change.

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📧 Contact

Project Link: [https://github.com/BAusten2/Ashes_Beneath](https://github.com/BAusten2/Ashes_Beneath)

## 🙏 Acknowledgments

- Unity Technologies for the game engine
- All contributors who have helped shape this project
- The horror game community for inspiration

---

<div align="center">
  
**🎃 Ooooo spooky 🎃**

*Made with 💀 by the Ashes Beneath team*

</div>
