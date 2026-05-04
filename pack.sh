mkdir ./DiscordRPContinued
mkdir ./DiscordRPContinued/Plugins
cp ./DiscordRP/bin/Release/DiscordRP.dll ./DiscordRPContinued/Plugins/DiscordRP.dll
cp -r ./DiscordRP/Libraries/linux ./DiscordRPContinued/Plugins/
cp -r ./DiscordRP/Libraries/osx ./DiscordRPContinued/Plugins/
cp -r ./DiscordRP/Libraries/win32 ./DiscordRPContinued/Plugins/
cp -r ./DiscordRP/Libraries/win64 ./DiscordRPContinued/Plugins/
cp ./LICENSE ./DiscordRPContinued/LICENSE

if [ $# -eq 1 ]; then
    if [ $1 == "help" ]; then
        echo "help, zip"
    fi

    if [ $1 == "zip" ]; then
        zip -r './DiscordRPContinued.zip' './DiscordRPContinued'
    fi
fi