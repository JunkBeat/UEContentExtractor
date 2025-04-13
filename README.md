# UEContentExtractor
A small program for extracting content from UE 4/5 games using CUE4Parse. 

Extracted audio is automatically converted to wav using binka and vgmstream.

<img src="https://github.com/user-attachments/assets/5201fffd-b429-4e8b-8b21-be4926852eb0" style="width: 40%;" />

## How to use
- Make sure you have dotnet runtime 8+ installed (https://dotnet.microsoft.com/en-us/download)
- Click on **Start** and wait a bit.
- Extracted files will appear in **exports** folder.

# Building the Project in Visual Studio

## Requirements

- Visual Studio 2022 or later
- .NET 8.0 SDK or higher

## Setup Instructions

1. **Clone the repository** or download the project files.

2. **Open the solution (.sln)** file in Visual Studio.

3. **Restore NuGet packages**:
    - Open the **Package Manager Console** (`Tools > NuGet Package Manager > Package Manager Console`)
    - Run the following commands:

      ```powershell
      Install-Package CUE4Parse
      Install-Package CUE4Parse-Conversion
      ```

    Alternatively, you can right-click the project in **Solution Explorer** → **Manage NuGet Packages** → search and install:
    - `CUE4Parse`
    - `CUE4Parse-Conversion`

4. **Build the project**:
    - Select `Build > Build Solution` or press `Ctrl + Shift + B`.
