@echo off
pushd "%~dp0"
tools\NAnt\NAnt %*
popd
