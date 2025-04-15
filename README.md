# EF Core Traces Proof of Concept

This project demonstrates tracing and metrics collection for Entity Framework Core database operations. It shows how to capture and visualize database interactions through System.Diagnostics.Activity and OpenTelemetry.

## Project Overview

The solution consists of multiple components:

- **DataLayer**: Contains EF Core context, entities, repositories, and telemetry observers
- **DataGenerator**: Utility to populate the database with sample data
- **Web**: ASP.NET Core web application that uses the data layer
- **Docker Compose**: Configuration for monitoring infrastructure (Grafana, Prometheus, OpenTelemetry Collector, Jaeger)

Telemetry is automatically collected from EF Core operations and exposed through OpenTelemetry, allowing visualization in Grafana dashboards.

## Setup and Running Instructions

### 1. Start Docker Compose Services

The monitoring infrastructure is configured in Docker Compose. Start it with:

```bash
cd compose
docker compose up -d
```

This will start:
- PostgreSQL database on port 35432
- Prometheus for metrics storage
- Jaeger for distributed tracing
- OpenTelemetry Collector to receive telemetry data
- Grafana for visualization on http://localhost:8080/grafana

### 2. Generate Sample Data

The DataGenerator project populates the database with sample customers, products, and orders:

```bash
cd DataGenerator
dotnet run
```

This will:
- Create the database schema if it doesn't exist
- Generate 1,000 customers and 100 products
- Create initial orders for each customer
- Generate 250,000 orders for the first customer to demonstrate performance patterns

### 3. Run the Web Application

The web application provides UI to browse the generated data:

```bash
cd Web
dotnet run
```

Access the application at http://localhost:5171

### 4. Explore Telemetry Data

After running the application and generating some traffic:

1. Access Grafana at http://localhost:8080/grafana
2. Navigate to dashboards:
   - Web Monitoring: Shows HTTP request metrics
   - R.E.D. Metrics: Shows Rate, Errors, Duration metrics
   - Traces Search: Allows exploring individual traces