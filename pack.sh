mkdir ./DiscordRPContinued
mkdir ./DiscordRPContinued/Plugins
cp ./DiscordRP/bin/Release/DiscordRP.dll ./DiscordRPContinued/Plugins/DiscordRP.dll
cp -r ./DiscordRP/Libraries/linux ./DiscordRPContinued/Plugins/linux
cp -r ./DiscordRP/Libraries/osx ./DiscordRPContinued/Plugins/osx
cp -r ./DiscordRP/Libraries/win32 ./DiscordRPContinued/Plugins/win32
cp -r ./DiscordRP/Libraries/win64 ./DiscordRPContinued/Plugins/win64
cp ./LICENSE ./DiscordRPContinued/LICENSE