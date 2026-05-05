# Game-Developer-Spin-Wheel-Case-Study
This project is a Case Study for Vertigo Games (Game Developer - New Grad)

# Spin Wheel Game Case Study

## Project Overview
This project is a technical case study developed for the Game Developer (New Grad) position at Vertigo Games. It implements a data-driven, modular spin-wheel system with a focus on scalable architecture and smooth user experience.

---

## Technical Implementation Details
This document outlines the engineering standards and architectural decisions applied to the project, focusing on maintainability and performance.

### Data-Driven Architecture
The project utilizes ScriptableObjects (SO) to decouple game data from logic, facilitating rapid iteration and balancing without code modifications.

*   **ZoneConfigSO**: Centralizes global rules including Total Zones, Safe Zone Intervals, and Super Zone Intervals.
*   **WheelConfigSO**: Defines the content for different wheel tiers (Bronze, Silver, Gold), mapping specific rewards and probabilities to wheel slices.
*   **Custom Data Types**: Structs and Classes like `WheelSliceData` bundle reward types and amounts, ensuring type safety throughout the reward pipeline.

### OOP Principles (Inheritance & Abstraction)
Core Object-Oriented Programming principles are used to reduce redundancy and enforce a rigid structure.

*   **Inheritance**: Base classes are utilized for UI elements and wheel logic to share common behaviors (e.g., `BaseWheel`) while allowing specific implementations in derived classes.
*   **Abstraction**: Complex internal operations, such as probability calculations and rotation physics, are abstracted to keep higher-level services focused on the game flow.
*   **Encapsulation**: Critical game data, such as the gold balance in `EconomyService`, is kept private. 

### Design Patterns
The architecture implements several industry-standard patterns to handle systematic complexity.

*   **Factory Pattern**: `WheelFactory` handles the instantiation of different wheel configurations based on the current zone.
*   **Strategy Pattern**: Different reward types (Currency, Items, Bomb) execute unique logic through distinct strategies, keeping the core spin result handler modular.
*   **Builder Pattern**: `WheelUIBuilder` acts as an orchestrator that dynamically constructs the visual wheel by binding configuration data to modular prefabs.
*   **Observer Pattern**: A static `GameSignals` system allows decoupled communication, where views subscribe to events without needing direct references to core services.
*   **Flyweight Pattern**: The `WheelThemeManager` re-skins a single wheel instance's properties to represent different tiers, significantly reducing memory overhead.

---

## Presentation & UI Layer
The UI is built using a View-Controller approach to ensure the visual layer remains independent of the underlying game state.

*   **WheelAnimationView**: Dedicated exclusively to the physical rotation and easing curves of the wheel using DOTween.
*   **WheelProgressView**: Manages the persistent display of the player's current stage and upcoming zones.
*   **State-Aware UI**: Components like `BombPopupView` react instantly to the `GameStateManager`, ensuring popups only appear during relevant game phases.

---

## Project Organization
The scene and folder structures are organized using a Context-Based architecture to ensure clear separation of concerns.

*   **GameContext_SpinWheel**: Acts as the root container, housing dedicated sub-hierarchies for Systems, Managers, and the UI Root.
*   **Modular Folder Structure**: Scripts are subdivided into Services, Managers, and Factories. Configurations are isolated in `SO_Configs` to keep data separate from visual assets.
*   **Asset Management**: Centralized folders for `UI_Assets` (Sprite Atlases, fonts) and modular prefabs ensure the project remains navigable as it scales.

---

## SOLID Principles Applied
*   **Single Responsibility (SRP)**: Each script has a narrow purpose; `EconomyService` handles balance, while `WheelAnimationView` handles movement.
*   **Open/Closed (OCP)**: New wheel tiers or rewards can be added via SOs and new Strategy classes without modifying existing code.
*   **Liskov Substitution (LSP)**: Specialized wheel variants can replace the base tier without breaking the core `SpinService` logic.
*   **Interface Segregation (ISP)**: Systems interact through specific signals rather than monolithic interfaces.
*   **Dependency Inversion (DIP)**: High-level logic depends on abstract signals and configurations rather than specific UI implementations.

---

## Game Juice & User Experience
*   **Dynamic Easing**: Using DOTween, the wheel utilizes custom deceleration curves to mimic physical momentum and build tension.
*   **Visual Transitions**: `WheelThemeManager` provides immediate feedback by swapping colors and sprites during tier transitions.
*   **Responsive UI**: Smooth transitions for popups and real-time multiplier updates ensure a reactive interface.

---

## Developer Information
*   **Name**: Arda Gülnaz
*   **Role**: Game Developer
*   **Education**: Ege University - Computer Engineering
*   **Critical Strike User ID**: 6269DE722482D71E
*   **Polygun Arena User ID**: 1A444F9D843F5E26