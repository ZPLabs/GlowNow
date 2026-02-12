#!/bin/bash
set -e

# --- Configuration ---
# All paths are relative to the repository root
STARTUP_PROJECT="apps/api/src/Api/GlowNow.Api/GlowNow.Api.csproj"
MODULES=("Identity" "Business" "Catalog" "Team")

# --- Colors ---
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

echo -e "${BLUE}🚀 GlowNow Database Migration Tool${NC}"

# 1. Check for .NET SDK
if ! command -v dotnet &> /dev/null
then
    echo -e "${RED}❌ Error: .NET SDK is not installed.${NC}"
    exit 1
fi

# 2. Check for dotnet-ef tool
if ! dotnet ef --version &> /dev/null
then
    echo -e "${YELLOW}⚠️  dotnet-ef tool not found. Attempting to install globally...${NC}"
    dotnet tool install --global dotnet-ef || true
    
    # Check again if it's now available in path
    if ! dotnet ef --version &> /dev/null
    then
        echo -e "${RED}❌ Error: dotnet-ef is not in your PATH. Try running: export PATH=\"\$PATH:\$HOME/.dotnet/tools\"${NC}"
        exit 1
    fi
fi

# 3. Run Migrations
for MODULE in "${MODULES[@]}"; do
    echo -e "\n${BLUE}--------------------------------------------------${NC}"
    echo -e "📦 ${GREEN}Migrating Module: $MODULE${NC}"
    echo -e "${BLUE}--------------------------------------------------${NC}"
    
    INFRA_PROJECT="apps/api/src/Modules/$MODULE/GlowNow.$MODULE.Infrastructure/GlowNow.$MODULE.Infrastructure.csproj"
    CONTEXT="${MODULE}DbContext"
    
    # Use -v for verbose output if a migration fails
    dotnet ef database update \
        --project "$INFRA_PROJECT" \
        --startup-project "$STARTUP_PROJECT" \
        --context "$CONTEXT" \
        --no-build # Assuming the project is already built, or omit if unsure
done

echo -e "\n${GREEN}✅ All migrations applied successfully!${NC}"
