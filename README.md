# MarbleScroll

A Windows utility that enables smooth scrolling using mouse button 4 (back/X1) for trackball and mouse users.

## Features

- **Smooth Scrolling**: Use the X1 (back) mouse button for natural scrolling
- **System Tray Integration**: Runs quietly in the background with tray icon
- **Lightweight**: Only 746KB executable size
- **Clean Architecture**: Modular, maintainable codebase with separation of concerns

## Installation

### Option 1: Download Pre-built Executable
1. Download `MarbleScroll.exe` from the releases
2. Run the executable - it will start minimized to system tray

### Option 2: Build from Source
1. Clone this repository
2. Ensure you have .NET 6.0 SDK installed
3. Build and run:
   ```bash
   dotnet build
   dotnet run
   ```

### Option 3: Create Standalone Executable
To create your own optimized standalone executable:
```bash
dotnet publish -c Release
```
The executable will be created at: `bin\Release\net6.0-windows\win-x64\publish\MarbleScroll.exe`

## Usage

1. **Start the Application**: Run `MarbleScroll.exe`
2. **System Tray**: The application minimizes to the system tray with "Marble Scroll" tooltip
3. **Scrolling**: Hold the X1 (back/fourth) mouse button and move your mouse to scroll
4. **Exit**: Right-click the system tray icon and select "Exit"

## System Requirements

- **Operating System**: Windows 10/11
- **.NET Runtime**: .NET 6.0 Desktop Runtime (required for the optimized executable)
- **Mouse**: Any mouse or trackball with X1/X2 buttons

## Architecture

The application follows a clean, modular architecture:

```
MarbleScroll/
├── Core/                    # Core business logic
│   ├── MarbleScrollService.cs   # Main service orchestration
│   └── ScrollProcessor.cs       # Mouse hook and scroll processing
├── UI/                      # User interface components
│   ├── MarbleForm.cs           # Main form and system tray
│   └── MarbleForm.Designer.cs  # Form designer code
├── Models/                  # Data models
│   ├── ScrollPosition.cs      # Position tracking
│   └── ScrollDelta.cs         # Scroll delta calculations
├── Interop/                 # Windows API interop
│   ├── WindowsApi.cs          # Windows API declarations
│   ├── MouseMessages.cs       # Mouse message constants
│   └── NativeStructures.cs    # Native Windows structures
├── Configuration/           # Configuration management
│   └── ScrollConfiguration.cs  # Scroll behavior settings
└── Program.cs              # Application entry point
```

## Configuration

The application uses sensible defaults but can be customized by modifying [`ScrollConfiguration.cs`](Configuration/ScrollConfiguration.cs):

- **Scroll sensitivity**: Adjust mouse movement to scroll ratio
- **Smooth scrolling**: Enable/disable smooth scrolling behavior
- **Button mapping**: Configure which mouse button triggers scrolling

## Development

### Prerequisites
- Visual Studio 2022 or VS Code
- .NET 6.0 SDK
- Windows 10/11

### Building
```bash
# Debug build
dotnet build

# Release build
dotnet build -c Release

# Create optimized executable
dotnet publish -c Release
```

### Key Components

- **Mouse Hook**: Low-level Windows mouse hook for X1 button detection
- **Scroll Processing**: Converts mouse movement to scroll wheel events
- **System Tray**: Minimal UI with context menu for exit
- **Clean Architecture**: Separation of concerns with dependency injection ready structure

## Troubleshooting

### Application Won't Start
- Ensure .NET 6.0 Desktop Runtime is installed
- Check Windows Event Viewer for error details
- Try running as administrator

### Scrolling Not Working
- Verify your mouse has X1/X2 buttons
- Check if other applications are interfering with mouse hooks
- Restart the application

### High CPU Usage
- This typically indicates a mouse hook issue
- Restart the application
- Check for Windows updates

## Contributing

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/your-feature`
3. Commit your changes: `git commit -am 'Add some feature'`
4. Push to the branch: `git push origin feature/your-feature`
5. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- Original concept inspired by trackball users' need for smooth scrolling
- Built with modern .NET 6.0 for performance and maintainability
- Clean architecture principles for extensibility

## Version History

- **v1.0.0**: Initial release with refactored architecture
  - Fixed mouse stuttering and crash issues
  - Optimized executable size (746KB)
  - Clean modular codebase
  - "Marble Scroll" branding