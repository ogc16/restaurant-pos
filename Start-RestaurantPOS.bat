@echo off
title RestaurantPOS System
echo ============================================
echo    RestaurantPOS - Starting System
echo ============================================
echo.

:: Start the API server in background
echo [1/2] Starting API Server on http://localhost:5000 ...
start "RestaurantPOS API" /min cmd /c "cd /d "%~dp0publish\Api" && dotnet RestaurantPOS.Api.dll"

:: Wait for API to start
timeout /t 3 /nobreak >nul

:: Start the Blazor Web frontend
echo [2/2] Starting Web Frontend on http://localhost:5001 ...
start "RestaurantPOS Web" cmd /c "cd /d "%~dp0publish\Web" && dotnet RestaurantPOS.Web.dll"

echo.
echo ============================================
echo    System Starting...
echo ============================================
echo.
echo    API:      http://localhost:5000
echo    Swagger:  http://localhost:5000/swagger
echo    Web UI:   http://localhost:5001
echo.
echo    Default Login:
echo    Email:    admin@restaurant.com
echo    Password: Admin@123456
echo.
echo    Press any key to open the Web UI...
echo ============================================
pause >nul
start http://localhost:5001
