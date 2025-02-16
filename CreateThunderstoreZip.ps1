
$SolutionDir = "C:\Projects\ValheimDev\ModDev\AugaLite"
$ProjectDir = "$SolutionDir\AugaLite"
$TargetDLL = "$ProjectDir\bin\Release\AugaLite.dll"

Copy-Item -Path "$TargetDLL" -Destination "$SolutionDir\ThunderstorePackage\plugins" -Force
Copy-Item -Path "$ProjectDir\translations.json" -Destination "$SolutionDir\ThunderstorePackage\plugins" -Force
Copy-Item -Path "$ProjectDir\README.md" -Destination "$SolutionDir\ThunderstorePackage" -Force

Compress-Archive -Path "$SolutionDir\ThunderstorePackage\*" -DestinationPath "$TargetDLL.zip" -Force

