cd /D E:\Work\BinFCProject
del /q *.*

rmdir /s /q E:\Work\BinFCProject\Modules

cd /D E:\Work\BinFC\Server\BinFC\FatCamel.Host
dotnet publish -o E:\Work\BinFCProject
cd /D ..
cd /D TelegramFatCamel.Module
dotnet publish -o E:\Work\BinFCProject\Modules\TelegramFatCamel.Module
cd /D ..
cd /D Storage.Module
dotnet publish -o E:\Work\BinFCProject\Modules\Storage.Module

cd /D E:\Work\BinFCProject
start .

pause