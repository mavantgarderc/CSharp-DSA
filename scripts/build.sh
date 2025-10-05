#!/bin/bash
set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Configuration
CONFIGURATION=${1:-Release}
OUTPUT_DIR=${2:-./artifacts}
SOLUTION_FILE=$(find . -name "*.sln" -type f | head -n 1)
PROJECT_FILE=$(find . -name "*.csproj" -type f | grep -E "(Web|Api)" | head -n 1)

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

# Main build process
main() {
    log_info "Starting build process..."
    log_info "Configuration: $CONFIGURATION"
    log_info "Output Directory: $OUTPUT_DIR"

    # Check if .NET 9 is installed
    if ! command -v dotnet &> /dev/null; then
        log_error ".NET CLI not found. Please install .NET 9 SDK."
        exit 1
    fi

    # Verify .NET version
    DOTNET_VERSION=$(dotnet --version)
    log_info "Using .NET version: $DOTNET_VERSION"

    # Clean previous builds
    log_info "Cleaning previous builds..."
    dotnet clean --configuration $CONFIGURATION --verbosity minimal

    # Restore packages
    log_info "Restoring NuGet packages..."
    dotnet restore --verbosity minimal

    # Build solution or project
    if [ -n "$SOLUTION_FILE" ]; then
        log_info "Building solution: $SOLUTION_FILE"
        dotnet build "$SOLUTION_FILE" \
            --configuration $CONFIGURATION \
            --no-restore \
            --verbosity minimal \
            --output "$OUTPUT_DIR"
    elif [ -n "$PROJECT_FILE" ]; then
        log_info "Building project: $PROJECT_FILE"
        dotnet build "$PROJECT_FILE" \
            --configuration $CONFIGURATION \
            --no-restore \
            --verbosity minimal \
            --output "$OUTPUT_DIR"
    else
        log_error "No solution or project file found."
        exit 1
    fi

    # Publish for deployment
    if [ "$CONFIGURATION" = "Release" ]; then
        log_info "Publishing application..."
        mkdir -p "$OUTPUT_DIR/publish"

        if [ -n "$PROJECT_FILE" ]; then
            dotnet publish "$PROJECT_FILE" \
                --configuration $CONFIGURATION \
                --no-build \
                --output "$OUTPUT_DIR/publish" \
                --verbosity minimal \
                --self-contained false
        fi
    fi

    # Build summary
    log_success "Build completed successfully!"
    log_info "Artifacts location: $OUTPUT_DIR"

    if [ -d "$OUTPUT_DIR/publish" ]; then
        log_info "Published application available at: $OUTPUT_DIR/publish"
    fi
}

# Help function
show_help() {
    echo "Usage: $0 [CONFIGURATION] [OUTPUT_DIR]"
    echo ""
    echo "Arguments:"
    echo "  CONFIGURATION  Build configuration (Debug|Release) [default: Release]"
    echo "  OUTPUT_DIR     Output directory [default: ./artifacts]"
    echo ""
    echo "Examples:"
    echo "  $0                    # Build in Release mode"
    echo "  $0 Debug              # Build in Debug mode"
    echo "  $0 Release ./dist     # Build in Release mode to ./dist"
}

# Check for help flag
if [[ "$1" == "-h" || "$1" == "--help" ]]; then
    show_help
    exit 0
fi

# Execute main function
main "$@"
