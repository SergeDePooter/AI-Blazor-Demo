# CityTrip Planner - Docker Setup

## Prerequisites

- Docker and Docker Compose installed
- Google Maps API Key

## Getting Started

1. **Set up your environment file:**
   
   Copy the example environment file and add your Google API key:
   ```bash
   cp .env.example .env
   ```
   
   Edit `.env` and replace `YOUR_GOOGLE_API_KEY_HERE` with your actual Google Maps API key.

2. **Build and run the application:**
   ```bash
   docker-compose up --build
   ```

3. **Access the application:**
   
   Open your browser and navigate to:
   ```
   http://localhost:8080
   ```

## Configuration

The application accepts the following environment variables:

- `GOOGLE_API_KEY` (required): Your Google Maps API key for map functionality

## Stopping the Application

To stop the application, press `Ctrl+C` in the terminal, or run:
```bash
docker-compose down
```

## Notes

- The application uses in-memory storage, so data will be lost when the container is stopped.
- For production use, consider configuring persistent storage and proper secrets management.
