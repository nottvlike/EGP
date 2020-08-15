#!/bin/bash

export UNITY_PATH="/Volumes/ExtremeSSD/Applications/UnityApp/2018.4.17f1/Unity.app"

export productType=$2

#生成需要编译的c#文件列表
function build_dir() 
{
    cd ./$1

    ./build.sh $productType

    cd ..
}

export inputPath=$1

if [ $inputPath == "All" ]; then
	build_dir UI
	build_dir Object
	build_dir Core
	build_dir Asset
	build_dir Editor
elif [[ $inputPath == "Core" || $inputPath == "UI" || $inputPath == "Object" || $inputPath == "Asset" ]]; then
    build_dir $inputPath
fi