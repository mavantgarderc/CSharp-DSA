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
BENCHMARK_RESULTS_DIR=${2:-./benchmark-results}
BENCHMARK_FILTER=${BENCHMARK_FILTER:-"*"}
BENCHMARK_WARMUP=${BENCHMARK_WARMUP:-3}
BENCHMARK_ITERATIONS=${BENCHMARK_ITERATIONS:-5}

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

# Main benchmark process
main() {
    log_info "Starting benchmark execution..."
    log_info "Configuration: $CONFIGURATION"
    log_info "Results Directory: $BENCHMARK_RESULTS_DIR"
    log_info "Filter: $BENCHMARK_FILTER"

    # Check if .NET is installed
    if ! command -v dotnet &> /dev/null; then
        log_error ".NET CLI not found. Please install .NET 9 SDK."
        exit 1
    fi

    # Find benchmark projects
    BENCHMARK_PROJECTS=$(find . -name "*.csproj" -type f | xargs grep -l "BenchmarkDotNet" | head -10)

    if [ -z "$BENCHMARK_PROJECTS" ]; then
        log_warning "No BenchmarkDotNet projects found."
        log_info "To add benchmarking to your project, install: dotnet add package BenchmarkDotNet"

        # Offer to run performance tests instead
        run_performance_tests
        exit 0
    fi

    log_info "Found benchmark projects:"
    echo "$BENCHMARK_PROJECTS" | while read -r project; do
        log_info "  - $project"
    done

    # Create results directory
    mkdir -p "$BENCHMARK_RESULTS_DIR"

    # Restore and build benchmark projects
    log_info "Preparing benchmark projects..."
    dotnet restore --verbosity minimal
    dotnet build --configuration $CONFIGURATION --no-restore --verbosity minimal

    # Run benchmarks for each project
    echo "$BENCHMARK_PROJECTS" | while read -r project; do
        run_benchmarks_for_project "$project"
    done

    # Generate summary report
    generate_summary_report

    log_success "Benchmark execution completed!"
    log_info "Results available at: $BENCHMARK_RESULTS_DIR"
}

run_benchmarks_for_project() {
    local project="$1"
    local project_name=$(basename "$project" .csproj)

    log_info "Running benchmarks for: $project_name"

    # Create project-specific results directory
    local project_results_dir="$BENCHMARK_RESULTS_DIR/$project_name"
    mkdir -p "$project_results_dir"

    # Run BenchmarkDotNet
    dotnet run \
        --project "$project" \
        --configuration $CONFIGURATION \
        --no-build \
        -- \
        --filter "$BENCHMARK_FILTER" \
        --artifacts "$project_results_dir" \
        --exporters json,html,csv \
        --warmupCount $BENCHMARK_WARMUP \
        --iterationCount $BENCHMARK_ITERATIONS \
        --statisticalTest 3ms \
        --memory \
        --disassembly \
        || log_warning "Benchmarks failed for $project_name"

    # Copy results to main results directory
    if [ -d "$project_results_dir/results" ]; then
        cp -r "$project_results_dir/results/"* "$project_results_dir/" 2>/dev/null || true
    fi
}

run_performance_tests() {
    log_info "Looking for performance/load test projects..."

    # Look for test projects that might contain performance tests
    PERF_TEST_PROJECTS=$(find . -name "*.csproj" -type f | xargs grep -l -E "(NBomber|k6|Performance|Load)" | head -10)

    if [ -n "$PERF_TEST_PROJECTS" ]; then
        log_info "Found performance test projects:"
        echo "$PERF_TEST_PROJECTS" | while read -r project; do
            log_info "  - $project"

            # Run performance tests
            dotnet test "$project" \
                --configuration $CONFIGURATION \
                --logger "console;verbosity=normal" \
                --filter "Category=Performance|Category=Load" \
                --results-directory "$BENCHMARK_RESULTS_DIR" \
                || log_warning "Performance tests failed for $project"
        done
    else
        log_info "No performance test projects found."

        # Create a simple HTTP benchmark if this is a web API
        create_simple_api_benchmark
    fi
}

create_simple_api_benchmark() {
    local api_project=$(find . -name "*.csproj" -type f | xargs grep -l -E "(Microsoft.AspNetCore|Web)" | head -n 1)

    if [ -n "$api_project" ]; then
        log_info "Creating simple API benchmark for: $(basename "$api_project" .csproj)"

        # Check if curl is available
        if command -v curl &> /dev/null; then
            run_curl_benchmark
        elif command -v wget &> /dev/null; then
            run_wget_benchmark
        else
            log_warning "No benchmarking tools available (curl, wget not found)"
        fi
    fi
}

run_curl_benchmark() {
    log_info "Running simple HTTP benchmark with curl..."

    # This assumes the API is running or can be started
    local base_url="http://localhost:5000"
    local endpoints=("/health" "/api/health" "/" "/weatherforecast")

    echo "# Simple HTTP Benchmark Results" > "$BENCHMARK_RESULTS_DIR/simple-benchmark.md"
    echo "Generated on: $(date)" >> "$BENCHMARK_RESULTS_DIR/simple-benchmark.md"
    echo "" >> "$BENCHMARK_RESULTS_DIR/simple-benchmark.md"

    for endpoint in "${endpoints[@]}"; do
        log_info "Testing endpoint: $endpoint"

        # Run simple benchmark (if service is running)
        curl -o /dev/null -s -w "Endpoint: $endpoint\nTime: %{time_total}s\nStatus: %{http_code}\n\n" \
            "$base_url$endpoint" >> "$BENCHMARK_RESULTS_DIR/simple-benchmark.md" 2>/dev/null || \
            echo "Endpoint: $endpoint - Not available" >> "$BENCHMARK_RESULTS_DIR/simple-benchmark.md"
    done

    log_info "Simple benchmark results saved to: $BENCHMARK_RESULTS_DIR/simple-benchmark.md"
}

run_wget_benchmark() {
    log_info "Running simple HTTP benchmark with wget..."

    local base_url="http://localhost:5000"
    local endpoints=("/health" "/api/health" "/" "/weatherforecast")

    echo "# Simple HTTP Benchmark Results" > "$BENCHMARK_RESULTS_DIR/simple-benchmark.md"
    echo "Generated on: $(date)" >> "$BENCHMARK_RESULTS_DIR/simple-benchmark.md"
    echo "" >> "$BENCHMARK_RESULTS_DIR/simple-benchmark.md"

    for endpoint in "${endpoints[@]}"; do
        log_info "Testing endpoint: $endpoint"

        wget --quiet --output-document=/dev/null --server-response \
            "$base_url$endpoint" 2>&1 | grep -E "(HTTP|time)" >> "$BENCHMARK_RESULTS_DIR/simple-benchmark.md" || \
            echo "Endpoint: $endpoint - Not available" >> "$BENCHMARK_RESULTS_DIR/simple-benchmark.md"
    done
}

generate_summary_report() {
    log_info "Generating benchmark summary report..."

    local summary_file="$BENCHMARK_RESULTS_DIR/summary.md"

    echo "# Benchmark Summary Report" > "$summary_file"
    echo "Generated on: $(date)" >> "$summary_file"
    echo "" >> "$summary_file"

    # List all JSON result files
    JSON_FILES=$(find "$BENCHMARK_RESULTS_DIR" -name "*.json" -type f)

    if [ -n "$JSON_FILES" ]; then
        echo "## BenchmarkDotNet Results" >> "$summary_file"
        echo "" >> "$summary_file"

        echo "$JSON_FILES" | while read -r json_file; do
            echo "- [$(basename "$json_file")]($json_file)" >> "$summary_file"
        done

        echo "" >> "$summary_file"
    fi

    # List HTML reports
    HTML_FILES=$(find "$BENCHMARK_RESULTS_DIR" -name "*.html" -type f)

    if [ -n "$HTML_FILES" ]; then
        echo "## HTML Reports" >> "$summary_file"
        echo "" >> "$summary_file"

        echo "$HTML_FILES" | while read -r html_file; do
            echo "- [$(basename "$html_file")]($html_file)" >> "$summary_file"
        done
    fi

    log_info "Summary report generated: $summary_file"
}

# Help function
show_help() {
    echo "Usage: $0 [CONFIGURATION] [BENCHMARK_RESULTS_DIR]"
    echo ""
    echo "Arguments:"
    echo "  CONFIGURATION          Build configuration (Debug|Release) [default: Release]"
    echo "  BENCHMARK_RESULTS_DIR  Benchmark results directory [default: ./benchmark-results]"
    echo ""
    echo "Environment Variables:"
    echo "  BENCHMARK_FILTER       Filter benchmarks to run [default: *]"
    echo "  BENCHMARK_WARMUP       Number of warmup iterations [default: 3]"
    echo "  BENCHMARK_ITERATIONS   Number of benchmark iterations [default: 5]"
    echo ""
    echo "Examples:"
    echo "  $0                              # Run all benchmarks in Release mode"
    echo "  $0 Debug                        # Run benchmarks in Debug mode"
    echo "  BENCHMARK_FILTER=*Json* $0      # Run only JSON-related benchmarks"
    echo "  BENCHMARK_WARMUP=5 $0           # Use 5 warmup iterations"
}

# Check for help flag
if [[ "$1" == "-h" || "$1" == "--help" ]]; then
    show_help
    exit 0
fi

# Execute main function
main "$@"
