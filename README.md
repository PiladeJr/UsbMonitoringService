# UsbMonitoringService

![Build Status](https://img.shields.io/badge/build-passing-brightgreen)
![License](https://img.shields.io/badge/license-MIT-blue)
![Version](https://img.shields.io/badge/version-1.0.0-informational)
![Coverage](https://img.shields.io/badge/coverage-90%25-yellowgreen)

Servizio .NET Worker per monitorare dispositivi USB, registrare eventi di connessione/disconnessione e salvare informazioni persistenti.

## table of contents

- [Demo](#demo)
- [Features](#features)
- [Tech Stack](#tech-stack)
- [Installation](#installation)
- [Usage](#usage)
- [Contributing](#contributing)
- [License](#license)

## Demo

Aggiungi qui screenshot, GIF o una breve registrazione del servizio in esecuzione.

## Features

- Rilevamento eventi USB in background
- Persistenza sessioni e eventi su database
- Repository separati per dispositivi, sessioni ed eventi
- Architettura modulare (`Monitor`, `Infrastructure`, `Persistence`)

## Tech Stack

- .NET 8 Worker Service
- Entity Framework Core
- WMI per rilevamento USB (Windows)

## Installation

```bash
git clone https://github.com/PiladeJr/UsbMonitoringService.git
cd UsbMonitoringService
dotnet restore
```

## Usage

```bash
dotnet run --project UsbMonitoringService.csproj
```

## Contributing

Pull requests welcome.

## License

Questo progetto è distribuito sotto licenza [MIT](LICENSE).
