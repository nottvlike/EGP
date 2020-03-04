#!/bin/bash

export UNITY_PATH="/Volumes/ExtremeSSD/Applications/UnityApp/2018.4.0f1/Unity.app"

export productType=$2

#生成需要编译的c#文件列表
function build_dir() 
{
    cd ./$1

    ./build.sh $productType
    ./build_editor.sh $productType

    cd ..
}

export inputPath=$1

if [ $inputPath == "All" ]; then
	build_dir Core
	build_dir UI
	build_dir Object
elif [[ $inputPath == "Core" || $inputPath == "UI" || $inputPath == "Object" ]]; then
    build_dir $inputPath
fi