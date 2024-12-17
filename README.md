
# SystemCleaner

SystemCleaner is a simple CLI application written in C# made to help me perform various system maintenance tasks from a command line.

## Features

- **DNS Cache Cleaning**: Flush DNS resolver cache
- **Browser Cache Clearing**: Clears the cache for multiple browsers (Chrome, Firefox, Edge, Brave)
- **Windows Update Cache Management**: Stop, clear, and restart Windows Update service
- **Recycle Bin Emptying**: Quickly and silently empty the recycle bin
- **Disk Cleanup**: Launch Windows built-in Disk Cleanup utility
- **Thumbnail Cache Clearing**: Remove thumbnail cache files

## Requirements

- Windows Operating System
- .NET Framework
- Administrator privileges

## Installation

1. Clone the repository:
   ```
   git clone https://github.com/Huskeyyy/SystemCleaner.git
   ```

2. Open the solution in Visual Studio or compile using .NET CLI.

## Usage

**Important: Run the application as an Administrator**

1. Launch the application
2. Choose from the following options:
   - `1`: Clean DNS Cache
   - `2`: Clear Browser Cache
   - `3`: Clear Windows Update Cache
   - `4`: Empty Recycle Bin
   - `5`: Run Disk Cleanup
   - `6`: Clear Thumbnail Cache
   - `0`: Exit the application

## Security Note

This application requires administrator privileges to perform system-level cleaning operations. Always ensure you understand the implications of clearing system caches.

## Contributing

If you would like to contribute to this project, please fork the repository and submit a pull request with your changes.

## License

This project is licensed under the MIT License - please see the LICENSE file for details.
