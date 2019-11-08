#!/bin/bash

#生成需要编译的c#文件列表
function build_dir() 
{
    cd ./$1

    ./build.sh
    ./build_editor.sh

    cd ..
}

export inputPath=$1

if [ $inputPath == "All" ]; then
	build_dir Core
elif [[ $inputPath == "Core" ]]; then
    build_dir $inputPath
fi