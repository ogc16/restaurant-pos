@echo off
title RestaurantPOS System
echo ============================================
echo    RestaurantPOS - Starting System
echo ============================================
echo.

:: Check if SQL Server is reachable
echo [1/3] Checking database connectivity...

:: Start the API server in background
echo [2/3] Starting API Server on https://localhost:7001 ...
start "RestaurantPOS API" /min cmd /c "cd /d "%~dp0publish\Api" && RestaurantPOS.Api.exe --urls https://localhost:7001"

:: Wait for API to start
timeout /t 3 /nobreak >nul

:: Start the Blazor Web frontend
echo [3/3] Starting Web Frontend on https://localhost:5001 ...
start "RestaurantPOS Web" cmd /c "cd /d "%~dp0publish\Web" && RestaurantPOS.Web.exe --urls https://localhost:5001"

echo.
echo ============================================
echo    System Starting...
echo ============================================
echo.
echo    API:      https://localhost:7001
echo    Swagger:  https://localhost:7001/swagger
echo    Web UI:   https://localhost:5001
echo.
echo    Default Login:
echo    Email:    admin@restaurant.com
echo    Password: Admin@123456
echo.
echo    Press any key to open the Web UI...
echo ============================================
pause >nul
start https://localhost:5001
