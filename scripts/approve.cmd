@echo off
rem Human-only: approve the plan of a run. Usage: scripts\approve.cmd <run-id>
pushd "%~dp0.."
dotnet "tools\EventTools\bin\Release\net10.0\EventTools.dll" approve %*
set RC=%ERRORLEVEL%
popd
exit /b %RC%
