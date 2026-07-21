@echo off
title RestaurantPOS - Stopping
echo Stopping RestaurantPOS services...
taskkill /FI "WindowTitle eq RestaurantPOS API" /T /F >nul 2>&1
taskkill /FI "WindowTitle eq RestaurantPOS Web" /T /F >nul 2>&1
taskkill /IM RestaurantPOS.Api.exe /F >nul 2>&1
taskkill /IM RestaurantPOS.Web.exe /F >nul 2>&1
echo All services stopped.
timeout /t 2 /nobreak >nul
