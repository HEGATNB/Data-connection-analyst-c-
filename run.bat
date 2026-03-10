@echo off
chcp 65001 >nul
setlocal enabledelayedexpansion

echo ========================================
echo    Анализатор сетевых подключений
echo ========================================
echo.

:: Переходим в папку с проектом
cd /d "%~dp0NetworkAnalyzer"

:: Проверяем, существует ли папка проекта
if not exist "NetworkAnalyzer.csproj" (
    echo [ОШИБКА] Файл проекта не найден!
    echo Текущая папка: %CD%
    echo Убедитесь, что папка NetworkAnalyzer существует в PKS_PROJ2
    pause
    exit /b 1
)

echo [1/3] Восстановление пакетов...
call dotnet restore >nul 2>&1
if errorlevel 1 (
    echo [ОШИБКА] Не удалось восстановить пакеты
    pause
    exit /b 1
)
echo [ГОТОВО]

echo [2/3] Сборка проекта...
call dotnet build --no-restore -c Debug >nul 2>&1
if errorlevel 1 (
    echo [ОШИБКА] Не удалось собрать проект
    pause
    exit /b 1
)
echo [ГОТОВО]

echo [3/3] Запуск приложения...
echo.
echo Запуск NetworkAnalyzer...
echo.

:: Запускаем приложение
start /B dotnet run --no-build

echo Приложение запущено!
echo.
echo Нажмите любую клавишу для закрытия этого окна...
pause >nul
exit /b 0