if [ $1 == "help" ]; then
	echo "build_lua.sh [Android | iOS | Win32 | MacOSX | Linux]"
	exit 0
fi

export PLATFORM=$1

if [[ $PLATFORM == "Android" || $PLATFORM == "iOS" || $PLATFORM == "Win32" || $PLATFORM == "MacOSX" || $PLATFORM == "Linux" ]]; then
	echo "begin build" $PLATFORM 
else
	echo "Wrong argument!"
	exit 1
fi

rm -rf ./build
mkdir ./build
cd build

export PLATFORM=$1
export LUA_LIB_PATH="lua-5.3.5"

if [ $PLATFORM == "MacOSX" ]; then
	cmake .. -G Xcode -DPLATFORM=$PLATFORM -DLUA_LIB_PATH=$LUA_LIB_PATH
	xcodebuild -target lua -configuration Release
	rm -rf ../../Assets/Plugins/lua.bundle	
	cp -rf ./Release/lua.bundle ../../Assets/EGP/Plugins/lua.bundle	
else
	cmake .. -DPLATFORM=$PLATFORM -DLUA_LIB_PATH=$LUA_LIB_PATH
	make >> /dev/null
fi

cd ..

echo "finished build"