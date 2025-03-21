# Battle Game Documentation

## Overview
This is a Unity-based space battle game where players control a spaceship and can use voice commands to control various aspects of the game. The game features different types of enemies, power-ups, and a unique voice command system.

## Core Components

### Player Controls
- **SpaceshipController**: Manages the player's spaceship, including:
  - Basic movement
  - Bullet firing system
  - Shield activation
  - Power bomb ability
  - Enemy targeting system

### Enemy System
- **Enemy**: Base class for all enemy types:
  - Circular (fast, low health)
  - Triangular (medium speed, medium health)
  - MiniBoss (slow, high health)
  - Boss (slowest, highest health)
  - Each type has unique movement patterns and health values

### Combat System
- **BulletBehavior**: Handles projectile mechanics:
  - Regular bullets
  - Powered shots (increased damage)
  - Collision detection with enemies

### Voice Command System
- **Speech_To_Text**: Implements voice recognition using HuggingFace API:
  - Records audio from microphone
  - Converts speech to text
  - Processes commands for game actions

- **MoveObjectsByCommand**: Processes voice commands to:
  - Move the spaceship
  - Target enemies
  - Activate special abilities
  - Handle game interactions

### Game Management
- **GameManager**: Controls game state:
  - Handles game over conditions
  - Manages UI panels
  - Controls game restart and quit functionality

### Utility Classes
- **DirectionFunctions**: Provides standardized movement directions
- **ColorFunctions**: Manages color assignments for game objects
- **EnemyFunctions**: Contains helper methods for enemy behavior
- **PlayerCommands**: Defines available player commands
- **ISpeechToText**: Interface for speech recognition system

## Setup Instructions

1. **Required Components**:
   - Unity 2020.3 or later
   - HuggingFace API key (configure in Speech_To_Text.cs)
   - Microphone access permissions

2. **Scene Setup**:
   - Create a Canvas with UI elements
   - Add the GameManager to an empty GameObject
   - Set up the player spaceship with SpaceshipController
   - Configure enemy prefabs with the Enemy component
   - Add MoveObjectsByCommand to handle voice inputs

3. **Voice Command System**:
   - Create a SpeechRecognition GameObject
   - Add the Speech_To_Text component
   - Configure the API key in the inspector
   - Set up the record button in the UI

## Voice Commands

### Movement Commands
- "Move left"
- "Move right"
- "Move up"
- "Move down"

### Combat Commands
- "Activate shield"
- "Fire power bomb"
- "Target [enemy type]"

### Enemy Types
- "Circular enemy"
- "Triangular enemy"
- "MiniBoss"
- "Boss"

## Game Features

### Player Abilities
1. **Shield System**:
   - Duration: 5 seconds
   - Cooldown: 10 seconds
   - Protects from enemy collisions

2. **Power Bomb**:
   - Increased damage
   - Larger projectile size
   - Special visual effects

3. **Targeting System**:
   - Lock-on to specific enemy types
   - Automatic rotation towards target
   - Enhanced accuracy

### Enemy Behavior
1. **Movement Patterns**:
   - Different speeds for each type
   - Chase player behavior
   - Collision detection

2. **Health System**:
   - Circular: 50 HP
   - Triangular: 75 HP
   - MiniBoss: 150 HP
   - Boss: 300 HP

## Technical Details

### API Integration
- Uses HuggingFace's Whisper model for speech recognition
- Endpoint: `https://api-inference.huggingface.co/models/openai/whisper-large-v3`
- Requires API key configuration

### Audio Processing
- Records at 44.1kHz
- Converts to WAV format for API
- Maximum recording time: 30 seconds

### UI Components
- Game Over panel
- Recording status indicator
- Command feedback display
- Enemy health indicators

## Troubleshooting

### Common Issues
1. **Voice Recognition Not Working**:
   - Check microphone permissions
   - Verify API key is valid
   - Ensure proper audio format

2. **Enemy Movement Issues**:
   - Verify player reference is set
   - Check collider setup
   - Ensure proper canvas setup

3. **Game Over Not Triggering**:
   - Verify GameManager is in scene
   - Check collision layers
   - Ensure proper tag setup

## Future Improvements
1. Add more voice commands
2. Implement different difficulty levels
3. Add scoring system
4. Include power-up system
5. Add visual feedback for voice commands
6. Implement enemy spawning system
7. Add sound effects and music 