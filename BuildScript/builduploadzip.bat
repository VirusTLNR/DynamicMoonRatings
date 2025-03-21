@echo off
rmdir "../bin/BepInEx" /s /q
mkdir "../bin/BepInEx/plugins/VirusTLNR-DynamicMoonRatings"
powershell Copy-Item -Path "../bin/Debug/netstandard2.1/DynamicMoonRatings.dll" -Destination "../bin/BepInEx/plugins/VirusTLNR-DynamicMoonRatings/DynamicMoonRatings.dll"
powershell Compress-Archive^
    -Force^
    -Path "../bin/BepInEx",^
          "./manifest.json",^
          "./icon.png",^
          "../README.md",^
          "../CHANGELOG.md"^
    -DestinationPath "../bin/DynamicMoonRatings.zip"