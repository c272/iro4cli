# Builds iro4cli in Release mode, configured as a standalone application without need for the .NET runtime.
# Generates binaries for Linux, Windows and Mac.

sudo rm -rf bin/
CYAN='\033[0;36m'
GREEN='\033[0;32m'
NC='\033[0m'

mkdir -p bin/ReleaseZipped

function buildVariant {
    echo -e "${CYAN}Building variant for $1...${NC}"
    dotnet publish -r $1 -c Release --self-contained true --p:DebugSymbols=false --p:DebugType=None
    zip -r bin/ReleaseZipped/$1.zip bin/Release/net6.0/$1/publish
    echo -e "${GREEN}Build complete for $1.${NC}"
}

buildVariant win-x64
buildVariant linux-x64
buildVariant linux-musl-x64
buildVariant linux-arm64
buildVariant osx-x64
buildVariant osx-arm64

echo -e "${GREEN}All builds completed. Exiting...${NC}"