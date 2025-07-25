#!/bin/bash
set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

# Configuration
ENVIRONMENT=${1:-staging}
DEPLOYMENT_TARGET=${2:-docker}
BUILD_VERSION=${BUILD_VERSION:-$(date +%Y%m%d-%H%M%S)}
DOCKER_REGISTRY=${DOCKER_REGISTRY:-""}
DOCKER_TAG=${DOCKER_TAG:-"latest"}

# Functions
log_info() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

log_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

log_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

log_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

confirm_deployment() {
    if [ "$ENVIRONMENT" = "production" ]; then
        log_warning "You are about to deploy to PRODUCTION environment!"
        read -p "Are you sure you want to continue? (yes/no): " -r
        if [[ ! $REPLY =~ ^[Yy][Ee][Ss]$ ]]; then
            log_info "Deployment cancelled."
            exit 0
        fi
    fi
}

# Main deployment process
main() {
    log_info "Starting deployment process..."
    log_info "Environment: $ENVIRONMENT"
    log_info "Target: $DEPLOYMENT_TARGET"
    log_info "Version: $BUILD_VERSION"

    # Confirm deployment for production
    confirm_deployment

    # Check prerequisites
    check_prerequisites

    # Build application
    build_application

    # Deploy based on target
    case $DEPLOYMENT_TARGET in
        "docker")
            deploy_docker
            ;;
        "iis")
            deploy_iis
            ;;
        "systemd")
            deploy_systemd
            ;;
        *)
            log_error "Unknown deployment target: $DEPLOYMENT_TARGET"
            show_help
            exit 1
            ;;
    esac

    # Post-deployment verification
    verify_deployment

    log_success "Deployment completed successfully!"
}

check_prerequisites() {
    log_info "Checking prerequisites..."

    # Check .NET CLI
    if ! command -v dotnet &> /dev/null; then
        log_error ".NET CLI not found. Please install .NET 9 SDK."
        exit 1
    fi

    # Check deployment target specific tools
    case $DEPLOYMENT_TARGET in
        "docker")
            if ! command -v docker &> /dev/null; then
                log_error "Docker not found. Please install Docker."
                exit 1
            fi
            ;;
    esac
}

build_application() {
    log_info "Building application for deployment..."

    # Find the main project file
    PROJECT_FILE=$(find . -name "*.csproj" -type f | grep -E "(Web|Api)" | head -n 1)

    if [ -z "$PROJECT_FILE" ]; then
        log_error "No web/API project file found."
        exit 1
    fi

    log_info "Building project: $PROJECT_FILE"

    # Clean and restore
    dotnet clean --configuration Release --verbosity minimal
    dotnet restore --verbosity minimal

    # Publish application
    mkdir -p "./publish"
    dotnet publish "$PROJECT_FILE" \
        --configuration Release \
        --output "./publish" \
        --verbosity minimal \
        --self-contained false \
        --no-restore
}

deploy_docker() {
    log_info "Deploying with Docker..."

    # Check if Dockerfile exists
    if [ ! -f "Dockerfile" ]; then
        log_info "Creating Dockerfile..."
        create_dockerfile
    fi

    # Build Docker image
    local image_name="$(basename $(pwd) | tr '[:upper:]' '[:lower:]')"
    local full_image_name="$image_name:$DOCKER_TAG"

    if [ -n "$DOCKER_REGISTRY" ]; then
        full_image_name="$DOCKER_REGISTRY/$full_image_name"
    fi

    log_info "Building Docker image: $full_image_name"
    docker build -t "$full_image_name" .

    # Push to registry if configured
    if [ -n "$DOCKER_REGISTRY" ]; then
        log_info "Pushing to registry: $DOCKER_REGISTRY"
        docker push "$full_image_name"
    fi

    # Deploy based on environment
    deploy_docker_environment "$full_image_name"
}

create_dockerfile() {
    cat > Dockerfile << 'EOF'
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["*.csproj", "./"]
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "$(find . -name '*.dll' -type f | head -n 1)"]
EOF

    log_info "Dockerfile created."
}

deploy_docker_environment() {
    local image_name="$1"

    case $ENVIRONMENT in
        "local"|"development")
            log_info "Starting container locally..."
            docker run -d \
                --name "$(basename $(pwd))-$ENVIRONMENT" \
                -p 8080:8080 \
                -p 8081:8081 \
                "$image_name"
            ;;
        "staging"|"production")
            if [ -f "docker-compose.$ENVIRONMENT.yml" ]; then
                log_info "Deploying with Docker Compose..."
                docker-compose -f "docker-compose.$ENVIRONMENT.yml" up -d
            else
                log_warning "No docker-compose.$ENVIRONMENT.yml found. Creating basic deployment..."
                create_docker_compose "$image_name"
                docker-compose -f "docker-compose.$ENVIRONMENT.yml" up -d
            fi
            ;;
    esac
}

create_docker_compose() {
    local image_name="$1"

    cat > "docker-compose.$ENVIRONMENT.yml" << EOF
version: '3.8'
services:
  api:
    image: $image_name
    ports:
      - "8080:8080"
      - "8081:8081"
    environment:
      - ASPNETCORE_ENVIRONMENT=$ENVIRONMENT
      - ASPNETCORE_URLS=http://+:8080;https://+:8081
    restart: unless-stopped
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:8080/health"]
      interval: 30s
      timeout: 10s
      retries: 3
      start_period: 40s
EOF

    log_info "Docker Compose file created: docker-compose.$ENVIRONMENT.yml"
}

create_deployment_zip() {
    local zip_file="deployment-$BUILD_VERSION.zip"

    if command -v zip &> /dev/null; then
        cd publish && zip -r "../$zip_file" . && cd ..
    else
        log_error "zip command not found. Please install zip utility."
        exit 1
    fi

    echo "$zip_file"
}

deploy_iis() {
    log_info "Deploying to IIS..."

    local iis_path="${IIS_DEPLOYMENT_PATH:-C:\\inetpub\\wwwroot\\$(basename $(pwd))}"
    local site_name="${IIS_SITE_NAME:-$(basename $(pwd))-$ENVIRONMENT}"

    log_info "IIS Path: $iis_path"
    log_info "Site Name: $site_name"

    # Check if running on Windows or if we can access the IIS path
    if [[ "$OSTYPE" == "msys" ]] || [[ "$OSTYPE" == "cygwin" ]] || [[ -d "$iis_path" ]]; then
        # Stop application pool
        if command -v powershell &> /dev/null; then
            powershell -Command "Import-Module WebAdministration; Stop-WebAppPool -Name '$site_name' -ErrorAction SilentlyContinue"
        fi

        # Copy files
        log_info "Copying files to IIS directory..."
        cp -r publish/* "$iis_path/"

        # Start application pool
        if command -v powershell &> /dev/null; then
            powershell -Command "Import-Module WebAdministration; Start-WebAppPool -Name '$site_name' -ErrorAction SilentlyContinue"
        fi
    else
        log_error "Cannot deploy to IIS from this environment. Please run on Windows or configure remote deployment."
        exit 1
    fi
}

deploy_systemd() {
    log_info "Deploying as systemd service..."

    local service_name="$(basename $(pwd) | tr '[:upper:]' '[:lower:]')-$ENVIRONMENT"
    local app_path="${SYSTEMD_APP_PATH:-/opt/$(basename $(pwd))}"
    local service_user="${SYSTEMD_USER:-www-data}"

    log_info "Service Name: $service_name"
    log_info "App Path: $app_path"

    # Create application directory
    sudo mkdir -p "$app_path"

    # Copy published files
    sudo cp -r publish/* "$app_path/"
    sudo chown -R $service_user:$service_user "$app_path"
    sudo chmod +x "$app_path"/*.dll

    # Create systemd service file
    create_systemd_service "$service_name" "$app_path" "$service_user"

    # Reload systemd and start service
    sudo systemctl daemon-reload
    sudo systemctl enable "$service_name"
    sudo systemctl restart "$service_name"
}

create_systemd_service() {
    local service_name="$1"
    local app_path="$2"
    local service_user="$3"
    local dll_file=$(find "$app_path" -name "*.dll" -type f | head -n 1)

    sudo tee "/etc/systemd/system/$service_name.service" > /dev/null << EOF
[Unit]
Description=$(basename $(pwd)) Web API - $ENVIRONMENT
After=network.target

[Service]
Type=notify
ExecStart=/usr/bin/dotnet $dll_file
Restart=always
RestartSec=5
KillSignal=SIGINT
SyslogIdentifier=$service_name
User=$service_user
Environment=ASPNETCORE_ENVIRONMENT=$ENVIRONMENT
Environment=ASPNETCORE_URLS=http://localhost:5000
WorkingDirectory=$app_path

[Install]
WantedBy=multi-user.target
EOF

    log_info "Systemd service created: /etc/systemd/system/$service_name.service"
}

verify_deployment() {
    log_info "Verifying deployment..."

    local health_endpoint=""
    local max_attempts=30
    local attempt=0

    # Determine health endpoint based on deployment target
    case $DEPLOYMENT_TARGET in
        "docker")
            health_endpoint="http://localhost:8080/health"
            ;;
        "systemd")
            health_endpoint="http://localhost:5000/health"
            ;;
        *)
            log_warning "Health check not configured for $DEPLOYMENT_TARGET"
            return 0
            ;;
    esac

    if [ -n "$health_endpoint" ]; then
        log_info "Checking health endpoint: $health_endpoint"

        while [ $attempt -lt $max_attempts ]; do
            if curl -f -s "$health_endpoint" &> /dev/null; then
                log_success "Health check passed!"
                return 0
            fi

            attempt=$((attempt + 1))
            log_info "Health check attempt $attempt/$max_attempts failed, retrying in 10s..."
            sleep 10
        done

        log_warning "Health check failed after $max_attempts attempts"
    fi
}

# Help function
show_help() {
    echo "Usage: $0 [ENVIRONMENT] [DEPLOYMENT_TARGET]"
    echo ""
    echo "Arguments:"
    echo "  ENVIRONMENT        Target environment (local|development|staging|production) [default: staging]"
    echo "  DEPLOYMENT_TARGET  Deployment target (docker|iis|systemd) [default: docker]"
    echo ""
    echo "Environment Variables:"
    echo "  BUILD_VERSION           Version tag for the build [default: timestamp]"
    echo "  DOCKER_REGISTRY         Docker registry URL"
    echo "  DOCKER_TAG              Docker image tag [default: latest]"
    echo "  AZURE_RESOURCE_GROUP    Azure resource group name"
    echo "  AZURE_APP_NAME          Azure web app name"
    echo "  AZURE_APP_SERVICE_PLAN  Azure app service plan name"
    echo "  AZURE_LOCATION          Azure deployment location [default: eastus]"
    echo "  AZURE_SKU               Azure app service plan SKU [default: B1]"
    echo "  IIS_DEPLOYMENT_PATH     IIS deployment directory"
    echo "  IIS_SITE_NAME           IIS site name"
    echo "  SYSTEMD_APP_PATH        Systemd service app directory [default: /opt/appname]"
    echo "  SYSTEMD_USER            Systemd service user [default: www-data]"
    echo ""
    echo "Examples:"
    echo "  $0                          # Deploy to staging with Docker"
    echo "  $0 production docker        # Deploy to production with Docker"
    echo "  BUILD_VERSION=v1.2.3 $0     # Deploy with specific version"
}

# Check for help flag
if [[ "$1" == "-h" || "$1" == "--help" ]]; then
    show_help
    exit 0
fi

# Execute main function
main "$@"
