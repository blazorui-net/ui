#!/bin/bash

# BlazorBlueprint Demo Runner
# Usage: ./run-demo.sh [server|wasm|auto|ssr]
# If no argument provided, shows interactive selection menu

clear

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"

# Demo project paths
DEMO_SERVER="$PROJECT_ROOT/demos/BlazorBlueprint.Demo.Server/BlazorBlueprint.Demo.Server.csproj"
DEMO_WASM="$PROJECT_ROOT/demos/BlazorBlueprint.Demo.Wasm/BlazorBlueprint.Demo.Wasm.csproj"
DEMO_AUTO="$PROJECT_ROOT/demos/BlazorBlueprint.Demo.Auto/BlazorBlueprint.Demo.Auto.csproj"
DEMO_SSR="$PROJECT_ROOT/demos/BlazorBlueprint.Demo.SSR/BlazorBlueprint.Demo.SSR.csproj"

run_demo() {
    local demo_type="$1"
    local project_path=""
    local demo_name=""

    case "$demo_type" in
        server|1)
            project_path="$DEMO_SERVER"
            demo_name="Blazor Server (InteractiveServer)"
            ;;
        wasm|2)
            project_path="$DEMO_WASM"
            demo_name="Blazor WebAssembly (Standalone)"
            ;;
        auto|3)
            project_path="$DEMO_AUTO"
            demo_name="Blazor Auto (Server + WASM hybrid)"
            ;;
        ssr|4)
            project_path="$DEMO_SSR"
            demo_name="Blazor Static SSR (No interactivity by default)"
            ;;
        *)
            echo "Unknown demo type: $demo_type"
            echo "Valid options: server, wasm, auto, ssr"
            exit 1
            ;;
    esac

    if [ ! -f "$project_path" ]; then
        echo "Error: Project not found at $project_path"
        exit 1
    fi

    echo ""
    echo "Starting $demo_name..."
    echo "Project: $project_path"
    echo ""
    echo "Press Ctrl+C to stop the server"
    echo "----------------------------------------"
    echo ""

    dotnet run --project "$project_path"
}

show_menu() {
    echo ""
    echo "BlazorBlueprint Demo Runner"
    echo "==========================="
    echo ""
    echo "Select a demo to run:"
    echo ""
    echo "  1) Server  - Blazor Server (InteractiveServer mode)"
    echo "  2) WASM    - Blazor WebAssembly (Standalone, runs in browser)"
    echo "  3) Auto    - Blazor Auto (Server first, then WASM)"
    echo "  4) SSR     - Static SSR (No interactivity, tests SSR compatibility)"
    echo ""
    echo "  q) Quit"
    echo ""
    read -p "Enter your choice [1-4]: " choice

    case "$choice" in
        1|server)
            run_demo "server"
            ;;
        2|wasm)
            run_demo "wasm"
            ;;
        3|auto)
            run_demo "auto"
            ;;
        4|ssr)
            run_demo "ssr"
            ;;
        q|Q)
            echo "Goodbye!"
            exit 0
            ;;
        *)
            echo "Invalid choice. Please enter 1, 2, 3, 4, or q."
            exit 1
            ;;
    esac
}

# Main script logic
if [ -n "$1" ]; then
    # Argument provided, run that demo directly
    run_demo "$1"
else
    # No argument, show interactive menu
    show_menu
fi
