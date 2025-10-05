#!/bin/bash
set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Configuration
CONFIGURATION=${1:-Debug}
TEST_RESULTS_DIR=${2:-./test-results}
COVERAGE_THRESHOLD=${COVERAGE_THRESHOLD:-80}
GENERATE_COVERAGE=${GENERATE_COVERAGE:-true}

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

# Main test process
main() {
    log_info "Starting test execution..."
    log_info "Configuration: $CONFIGURATION"
    log_info "Test Results Directory: $TEST_RESULTS_DIR"

    # Check if .NET is installed
    if ! command -v dotnet &> /dev/null; then
        log_error ".NET CLI not found. Please install .NET 9 SDK."
        exit 1
    fi

    # Find test projects
    TEST_PROJECTS=$(find . -name "*.csproj" -type f | xargs grep -l "Microsoft.NET.Test.Sdk\|xunit\|NUnit\|MSTest" | head -10)

    if [ -z "$TEST_PROJECTS" ]; then
        log_warning "No test projects found."
        exit 0
    fi

    log_info "Found test projects:"
    echo "$TEST_PROJECTS" | while read -r project; do
        log_info "  - $project"
    done

    # Create results directory
    mkdir -p "$TEST_RESULTS_DIR"

    # Restore packages
    log_info "Restoring test project dependencies..."
    dotnet restore --verbosity minimal

    # Build test projects
    log_info "Building test projects..."
    dotnet build --configuration $CONFIGURATION --no-restore --verbosity minimal

    # Run tests with coverage
    if [ "$GENERATE_COVERAGE" = "true" ]; then
        log_info "Running tests with code coverage..."

        # Check if coverlet is available
        if ! dotnet tool list -g | grep -q "coverlet.console"; then
            log_info "Installing coverlet global tool..."
            dotnet tool install --global coverlet.console --version 6.0.0 || true
        fi

        # Run tests with coverage collection
        dotnet test \
            --configuration $CONFIGURATION \
            --no-build \
            --verbosity normal \
            --logger "trx;LogFileName=test-results.trx" \
            --logger "console;verbosity=normal" \
            --results-directory "$TEST_RESULTS_DIR" \
            --collect:"XPlat Code Coverage" \
            --settings coverlet.runsettings 2>/dev/null || \
        dotnet test \
            --configuration $CONFIGURATION \
            --no-build \
            --verbosity normal \
            --logger "trx;LogFileName=test-results.trx" \
            --logger "console;verbosity=normal" \
            --results-directory "$TEST_RESULTS_DIR" \
            --collect:"XPlat Code Coverage"

        # Generate coverage reports
        generate_coverage_report
    else
        log_info "Running tests without coverage..."
        dotnet test \
            --configuration $CONFIGURATION \
            --no-build \
            --verbosity normal \
            --logger "trx;LogFileName=test-results.trx" \
            --logger "console;verbosity=normal" \
            --results-directory "$TEST_RESULTS_DIR"
    fi

    # Process test results
    process_test_results

    log_success "Test execution completed!"
}

generate_coverage_report() {
    log_info "Generating coverage reports..."

    # Find coverage files
    COVERAGE_FILES=$(find "$TEST_RESULTS_DIR" -name "coverage.cobertura.xml" -type f)

    if [ -n "$COVERAGE_FILES" ]; then
        # Install reportgenerator if not available
        if ! dotnet tool list -g | grep -q "dotnet-reportgenerator-globaltool"; then
            log_info "Installing ReportGenerator global tool..."
            dotnet tool install --global dotnet-reportgenerator-globaltool --version 5.2.4 || true
        fi

        # Generate HTML report
        mkdir -p "$TEST_RESULTS_DIR/coverage-report"

        reportgenerator \
            "-reports:$TEST_RESULTS_DIR/**/coverage.cobertura.xml" \
            "-targetdir:$TEST_RESULTS_DIR/coverage-report" \
            "-reporttypes:Html;TextSummary" \
            "-verbosity:Warning" 2>/dev/null || log_warning "Could not generate coverage report"

        # Display coverage summary
        if [ -f "$TEST_RESULTS_DIR/coverage-report/Summary.txt" ]; then
            log_info "Coverage Summary:"
            cat "$TEST_RESULTS_DIR/coverage-report/Summary.txt"

            # Check coverage threshold
            COVERAGE_PERCENTAGE=$(grep -o "Line coverage: [0-9.]*%" "$TEST_RESULTS_DIR/coverage-report/Summary.txt" | grep -o "[0-9.]*")
            if [ -n "$COVERAGE_PERCENTAGE" ]; then
                if (( $(echo "$COVERAGE_PERCENTAGE >= $COVERAGE_THRESHOLD" | bc -l) )); then
                    log_success "Coverage threshold met: $COVERAGE_PERCENTAGE% >= $COVERAGE_THRESHOLD%"
                else
                    log_warning "Coverage below threshold: $COVERAGE_PERCENTAGE% < $COVERAGE_THRESHOLD%"
                fi
            fi
        fi
    else
        log_warning "No coverage files found."
    fi
}

process_test_results() {
    # Find and display test result summary
    TRX_FILES=$(find "$TEST_RESULTS_DIR" -name "*.trx" -type f)

    if [ -n "$TRX_FILES" ]; then
        log_info "Test results saved to: $TEST_RESULTS_DIR"

        # Basic result parsing (works with most test frameworks)
        echo "$TRX_FILES" | while read -r trx_file; do
            if grep -q "failed" "$trx_file"; then
                FAILED_COUNT=$(grep -o 'failed="[0-9]*"' "$trx_file" | grep -o '[0-9]*' || echo "0")
                if [ "$FAILED_COUNT" -gt 0 ]; then
                    log_error "Found $FAILED_COUNT failed test(s)"
                fi
            fi
        done
    fi
}

# Help function
show_help() {
    echo "Usage: $0 [CONFIGURATION] [TEST_RESULTS_DIR]"
    echo ""
    echo "Arguments:"
    echo "  CONFIGURATION     Build configuration (Debug|Release) [default: Debug]"
    echo "  TEST_RESULTS_DIR  Test results directory [default: ./test-results]"
    echo ""
    echo "Environment Variables:"
    echo "  COVERAGE_THRESHOLD  Minimum coverage percentage [default: 80]"
    echo "  GENERATE_COVERAGE   Generate coverage report (true|false) [default: true]"
    echo ""
    echo "Examples:"
    echo "  $0                           # Run tests in Debug mode with coverage"
    echo "  $0 Release                   # Run tests in Release mode"
    echo "  GENERATE_COVERAGE=false $0   # Run tests without coverage"
    echo "  COVERAGE_THRESHOLD=90 $0     # Set coverage threshold to 90%"
}

# Check for help flag
if [[ "$1" == "-h" || "$1" == "--help" ]]; then
    show_help
    exit 0
fi

# Execute main function
main "$@"
