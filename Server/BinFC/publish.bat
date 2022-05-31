cd /D E:\Work\BinFCProject
del /q *.*

rmdir /s /q E:\Work\BinFCProject\Modules

cd /D E:\Work\BinFC\Server\BinFC\FatCamel.Host
dotnet publish -c release -o E:\Work\BinFCProject
cd /D ..
cd /D TelegramFatCamel.Module
dotnet publish -c release -o E:\Work\BinFCProject\Modules\TelegramFatCamel.Module
cd /D ..
cd /D Storage.Module
dotnet publish -c release -o E:\Work\BinFCProject\Modules\Storage.Module
cd /D ..
cd /D BinanceApi.Module
dotnet publish -c release -o E:\Work\BinFCProject\Modules\BinanceApi.Module
cd /D ..
cd /D WorkerService.Module
dotnet publish -c release -o E:\Work\BinFCProject\Modules\WorkerService.Module

cd /D E:\Work\BinFCProject
start .

pause