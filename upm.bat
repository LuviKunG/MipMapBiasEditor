@echo off
set /p version=Enter Version: 
echo %version%
git subtree split --prefix="Assets/Mip Map Bias Editor" --branch upm
git tag %version% upm
pause