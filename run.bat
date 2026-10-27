@echo off
cd /d "%~dp0app"
start "" http://localhost:5080
CateringMvc.exe
