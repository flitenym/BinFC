cd /D E:\Work\BinFCProject
del /q *.*

rmdir /s /q E:\Work\BinFCProject\Modules

cd /D E:\Work\BinFC\Server\BinFC\Host
dotnet publish -o E:\Work\BinFCProject
cd /D ..
cd /D Telegram.Module
dotnet publish -o E:\Work\BinFCProject\Modules\Telegram.Module
cd /D ..
cd /D Storage.Module
dotnet publish -o E:\Work\BinFCProject\Modules\Storage.Module
cd /D ..
cd /D BinanceApi.Module
dotnet publish -o E:\Work\BinFCProject\Modules\BinanceApi.Module
cd /D ..
cd /D WorkerService.Module
dotnet publish -o E:\Work\BinFCProject\Modules\WorkerService.Module

cd /D E:\Work\BinFCProject
start .

pause