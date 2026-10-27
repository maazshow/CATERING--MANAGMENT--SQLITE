@echo off
REM Apne computer par (jahan .NET 9 SDK hai) chalayein. Output: publish\ folder client ko dein.
dotnet publish CateringMvc\CateringMvc.csproj -c Release -r win-x64 --self-contained true -o publish\app
copy run.bat publish\run.bat
echo.
echo Tayyar! "publish" folder client ke computer par copy karein aur run.bat chalayein.
pause
