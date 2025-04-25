#!/bin/bash
cd "$(dirname "$0")"

echo "📦 Building SOLTEC.WikiDocGen..."
dotnet build SOLTEC.WikiDocGen.csproj -c Release
if [ $? -ne 0 ]; then
  echo "❌ Build failed. Aborting."
  exit 1
fi

echo "🚀 Running Wiki Generator..."
dotnet run --project SOLTEC.WikiDocGen.csproj --configuration Release