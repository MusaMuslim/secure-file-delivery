# Secure File Delivery System

A production-grade secure file statement delivery system built for Capitec Bank, featuring time-limited download links, AES-256 encryption, and comprehensive audit logging.

## 🏗️ Architecture

Clean Architecture with clear separation of concerns:

```
SecureFileDelivery/
├── Domain/           # Business entities, enums, exceptions
├── Application/      # Business logic, DTOs, service interfaces
├── Infrastructure/   # Data access, external services, security
└── API/              # REST endpoints, controllers, middleware
```

**Design Patterns:**
- Clean Architecture (Onion/Hexagonal)
- Repository Pattern
- Dependency Injection
- CQRS-lite (separate read/write operations)
- Domain-Driven Design principles

## ✨ Features

### Core Functionality
- ✅ **Secure File Upload** - AES-256 encryption at rest
- ✅ **Time-Limited Download Links** - Configurable expiration (default: 60 minutes)
- ✅ **Access Control** - Cryptographically secure tokens, max access count limits
- ✅ **Audit Logging** - Complete access trail for compliance
- ✅ **Multiple File Types** - PDF, CSV, Excel support

### Security
- 🔒 **File Encryption** - AES-256 encryption for files at rest
- 🔑 **JWT Authentication** - Stateless token-based auth
- 🛡️ **API Key Support** - System-to-system authentication
- 📝 **Audit Trail** - Track all file access with IP, timestamp, user
- ⏱️ **Auto-Expiring Links** - Links expire after configured time
- 🚫 **Revocable Links** - Manually revoke links if needed

### Technical Excellence
- 🐳 **Docker Support** - Fully containerized application
- 📊 **Swagger/OpenAPI** - Interactive API documentation
- ✅ **Integration Tests** - Comprehensive test coverage
- 🔍 **Health Checks** - Production-ready monitoring endpoints
- 📦 **SQLite Database** - EF Core with migrations

## 🚀 Tech Stack

- **.NET 8** - Latest LTS framework
- **ASP.NET Core** - Web API
- **Entity Framework Core** - ORM with SQLite
- **JWT** - Authentication
- **AES-256** - Encryption
- **Docker** - Containerization
- **xUnit** - Testing
- **Swagger/OpenAPI** - API documentation
- **Serilog** - Structured logging

## 📋 Prerequisites

- .NET 8 SDK
- Docker Desktop (for containerized deployment)
- Visual Studio 2022 or VS Code

## 🏃 Quick Start

### Option 1: Using Docker (Recommended)

```bash
# Clone the repository
git clone https://github.com/MusaMuslim/secure-file-delivery.git
cd secure-file-delivery

# Start with Docker Compose
docker-compose up

# Access Swagger UI
http://localhost:8080/swagger
```

### Option 2: Using .NET CLI

```bash
# Navigate to API project
cd SecureFileDelivery.API

# Restore dependencies
dotnet restore

# Run the application
dotnet run

# Access Swagger UI
http://localhost:5000/swagger
```

### Option 3: Using Visual Studio

1. Open `SecureFileDelivery.sln`
2. Set `SecureFileDelivery.API` as startup project
3. Press **F5**
4. Swagger opens automatically

## 🔧 Configuration

Key settings in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=securefiledelivery.db"
  },
  "FileStorage": {
    "RootPath": "FileStorage",
    "MaxFileSizeBytes": 52428800
  },
  "Security": {
    "JwtSecret": "your-secret-key",
    "ApiKeys": ["demo-api-key-12345"]
  },
  "DownloadLinks": {
    "DefaultExpirationMinutes": 60,
    "MaxExpirationMinutes": 1440
  }
}
```

## 📡 API Endpoints

### Authentication
- `POST /api/auth/token` - Generate JWT token

### Statements
- `POST /api/statements` - Upload statement file
- `GET /api/statements/{id}` - Get statement details
- `GET /api/statements/account/{accountNumber}` - List statements by account
- `DELETE /api/statements/{id}` - Delete statement

### Download Links
- `POST /api/statements/{id}/links` - Generate download link
- `GET /api/statements/{id}/links` - List links for statement
- `GET /api/statements/links/{token}` - Get link details

### Download
- `GET /api/download/{token}` - Download file (public endpoint)

### Health
- `GET /health` - Health check endpoint

## 🔐 Authentication

### JWT Authentication

1. Generate token:
```bash
POST /api/auth/token
Content-Type: application/json

{
  "username": "testuser",
  "password": "testpass"
}
```

2. Use token in subsequent requests:
```
Authorization: Bearer <your-jwt-token>
```

### API Key Authentication

For system-to-system communication:
```
X-API-Key: demo-api-key-12345
```

## 📖 Usage Example

### Complete Workflow

```bash
# 1. Generate authentication token
POST /api/auth/token
{
  "username": "bankuser",
  "password": "secure123"
}

# 2. Upload a statement (with Authorization header)
POST /api/statements
Content-Type: multipart/form-data
- file: statement.pdf
- accountNumber: "12345678"
- periodStart: "2024-01-01"
- periodEnd: "2024-01-31"
- uploadedBy: "BankSystem"

Response: { "id": "abc-123-def", ... }

# 3. Generate download link
POST /api/statements/abc-123-def/links
{
  "expirationMinutes": 60,
  "createdBy": "BankUser"
}

Response: { 
  "token": "xyz789...",
  "downloadUrl": "http://localhost:8080/api/download/xyz789..."
}

# 4. Share link with customer (no auth needed)
GET /api/download/xyz789...

# File downloads automatically!
```

## 🧪 Testing

### Run All Tests
```bash
dotnet test
```

### Run Specific Test
```bash
dotnet test --filter FullyQualifiedName~HealthCheckTests
```

### Test Coverage
- ✅ Health checks
- ✅ Authentication (JWT generation)
- ✅ File upload with encryption
- ✅ Download link generation
- ✅ File download with decryption
- ✅ Error scenarios (invalid tokens, missing files)

## 🐳 Docker

### Build Image
```bash
docker build -t securefiledelivery:latest .
```

### Run Container
```bash
docker run -p 8080:8080 securefiledelivery:latest
```

### Using Docker Compose (Recommended)
```bash
# Start services
docker-compose up -d

# View logs
docker-compose logs -f

# Stop services
docker-compose down

# Rebuild and restart
docker-compose up --build
```

## 📁 Project Structure

```
SecureFileDelivery/
├── SecureFileDelivery.Domain/
│   ├── Entities/              # Statement, DownloadLink, AuditLog
│   ├── Enums/                 # FileType, LinkStatus
│   └── Exceptions/            # Domain-specific exceptions
├── SecureFileDelivery.Application/
│   ├── DTOs/                  # Request/Response objects
│   ├── Interfaces/            # Service contracts
│   ├── Services/              # Business logic implementations
│   └── Validators/            # Input validation (FluentValidation)
├── SecureFileDelivery.Infrastructure/
│   ├── Data/                  # DbContext, EF configurations
│   ├── Repositories/          # Data access implementations
│   ├── Services/              # File storage, encryption
│   └── Security/              # JWT service
├── SecureFileDelivery.API/
│   ├── Controllers/           # REST API endpoints
│   ├── Attributes/            # API key authentication
│   └── Program.cs             # Application startup
├── SecureFileDelivery.Tests/
│   └── Integration tests      # API tests
├── Dockerfile                 # Multi-stage Docker build
├── docker-compose.yml         # Container orchestration
└── README.md                  # This file
```

## 🎯 Design Decisions

### Why Clean Architecture?
- **Separation of Concerns**: Business logic independent of frameworks
- **Testability**: Domain and application layers easily tested
- **Maintainability**: Clear boundaries between layers
- **Flexibility**: Easy to swap infrastructure (SQLite → PostgreSQL)

### Why Time-Limited Links?
- **Security**: Reduces window for unauthorized access
- **Compliance**: Aligns with data protection regulations (POPIA, GDPR)
- **User Experience**: Simple sharing without requiring customer accounts

### Why AES-256 Encryption?
- **Industry Standard**: Banking-grade encryption
- **Regulatory Compliance**: Required for sensitive financial data
- **At-Rest Protection**: Files encrypted even if storage is compromised

### Why Repository Pattern?
- **Abstraction**: Decouples business logic from data access
- **Testing**: Easy to mock repositories for unit tests
- **Flexibility**: Can switch between SQL, NoSQL, or cloud storage

### Why SQLite for Demo?
- **Zero Configuration**: No database server setup required
- **Portable**: Single file database
- **Easy to Test**: Perfect for development and demos
- **Production Path**: Simple migration to PostgreSQL/SQL Server

### Why JWT + API Keys?
- **Dual Authentication**: JWT for users, API keys for systems
- **Stateless**: No server-side session storage
- **Scalable**: Works in distributed/load-balanced environments
- **Layered Security**: Defense in depth approach

## 🔄 Future Enhancements

- [ ] **Cloud Storage**: AWS S3 or Azure Blob Storage integration
- [ ] **Database**: PostgreSQL for production scalability
- [ ] **Caching**: Redis for performance optimization
- [ ] **Notifications**: Email/SMS alerts for file downloads
- [ ] **Analytics**: Dashboard for usage metrics
- [ ] **MFA**: Multi-factor authentication
- [ ] **Virus Scanning**: ClamAV integration for uploaded files
- [ ] **Rate Limiting**: Advanced throttling per user/system
- [ ] **Monitoring**: Application Insights or ELK stack
- [ ] **CI/CD**: GitHub Actions or Azure DevOps pipelines

## 🔒 Security Considerations

### Implemented
- ✅ File encryption at rest (AES-256)
- ✅ Secure token generation (cryptographically random)
- ✅ Time-limited access (configurable expiration)
- ✅ Audit logging (complete access trail)
- ✅ Input validation (FluentValidation)
- ✅ HTTPS support (production)
- ✅ CORS configuration
- ✅ SQL injection protection (EF Core parameterized queries)

### Production Recommendations
- Use HTTPS only (enforce SSL/TLS)
- Store secrets in Azure Key Vault or AWS Secrets Manager
- Implement rate limiting (e.g., 100 requests/minute per user)
- Add Web Application Firewall (WAF)
- Enable application monitoring and alerting
- Regular security audits and penetration testing
- Implement virus scanning for uploaded files
- Use separate encryption keys per environment

## 📝 Notes

- **Database**: `securefiledelivery.db` (auto-created on first run)
- **File Storage**: `FileStorage/` directory (auto-created)
- **Logs**: Structured console output (JSON in production)
- **Migrations**: Auto-applied on application startup

## 🤝 Contributing

This is an assessment project, but feedback is welcome:
1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Open a Pull Request

## 👤 Author

**Musaddiq Muslim**

Built for Capitec Bank Software Engineer Level 2 Assessment  
January 2026

## 📧 Contact

For questions about this project, please reach out via GitHub issues.

## 📄 License

This project is part of a job application assessment and is for demonstration purposes.

---

**Thank you for reviewing this project!** 🚀