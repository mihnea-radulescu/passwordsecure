#!/bin/sh

project_file=PasswordSecure/PasswordSecure.csproj

platform_linux_arm64=linux-arm64
platform_linux_x64=linux-x64

platform_windows_arm64=win-arm64
platform_windows_x64=win-x64

dotnet publish $project_file --output publish/$platform_linux_arm64 -p:DebugType=none -p:PublishSingleFile=true -p:PublishReadyToRun=true --self-contained true --configuration Release --framework net10.0 --runtime $platform_linux_arm64

dotnet publish $project_file --output publish/$platform_linux_x64 -p:DebugType=none -p:PublishSingleFile=true -p:PublishReadyToRun=true --self-contained true --configuration Release --framework net10.0 --runtime $platform_linux_x64

dotnet publish $project_file --output publish/$platform_windows_arm64 -p:DebugType=none -p:PublishSingleFile=true -p:PublishReadyToRun=true --self-contained true --configuration Release --framework net10.0 --runtime $platform_windows_arm64

dotnet publish $project_file --output publish/$platform_windows_x64 -p:DebugType=none -p:PublishSingleFile=true -p:PublishReadyToRun=true --self-contained true --configuration Release --framework net10.0 --runtime $platform_windows_x64
