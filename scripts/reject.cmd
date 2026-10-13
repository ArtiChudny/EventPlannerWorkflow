@echo off
rem Human-only: reject the plan with feedback. Usage: scripts\reject.cmd <run-id> your feedback text
pushd "%~dp0.."
dotnet "tools\EventTools\bin\Release\net10.0\EventTools.dll" reject %*
set RC=%ERRORLEVEL%
popd
exit /b %RC%
