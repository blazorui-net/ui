#!/bin/bash

# BlazorBlueprint Test Runner
# Usage: ./run-tests.sh [--accept]
#
# Options:
#   --accept    Auto-accept new Verify snapshots (use after reviewing .received.txt files)
#
# First run:
#   Tests will FAIL because no .verified.txt baselines exist yet.
#   Review the generated .received.txt files, then run with --accept to promote them.

clear

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
TEST_PROJECT="$PROJECT_ROOT/tests/BlazorBlueprint.Tests/BlazorBlueprint.Tests.csproj"

if [ ! -f "$TEST_PROJECT" ]; then
    echo "Error: Test project not found at $TEST_PROJECT"
    exit 1
fi

echo ""
echo "BlazorBlueprint Test Runner"
echo "==========================="
echo ""

if [ "$1" = "--accept" ]; then
    echo "Mode: Accept new snapshots"
    echo ""

    SNAPSHOT_DIR="$PROJECT_ROOT/tests/BlazorBlueprint.Tests/ApiSurface"

    # Find all .received.txt files and promote them to .verified.txt
    found=0
    for received in "$SNAPSHOT_DIR"/*.received.txt; do
        [ -f "$received" ] || continue
        verified="${received%.received.txt}.verified.txt"
        echo "  Accepting: $(basename "$received")"
        mv "$received" "$verified"
        found=1
    done

    if [ "$found" -eq 0 ]; then
        echo "  No .received.txt files found. Run tests first without --accept."
        echo ""
    else
        echo ""
        echo "Snapshots accepted. Running tests to confirm..."
        echo ""
    fi
fi

echo "Running tests..."
echo "Project: $TEST_PROJECT"
echo "----------------------------------------"
echo ""

dotnet test "$TEST_PROJECT" --verbosity normal

exit_code=$?

echo ""
echo "----------------------------------------"

if [ $exit_code -eq 0 ]; then
    echo "All tests passed."
else
    echo "Tests failed (exit code: $exit_code)"
    echo ""
    echo "If this is the first run, .received.txt files have been generated."
    echo "Review them, then run: ./scripts/run-tests.sh --accept"
fi

exit $exit_code
