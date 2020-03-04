#!/bin/bash

export CURENT_DIR=`pwd`
export ECS_SRC=

export ECS_REFERENCES=-r:$UNITY_PATH/Contents/Managed/UnityEngine.dll,\
$UNITY_PATH/Contents/UnityExtensions/Unity/GUISystem/UnityEngine.UI.dll,\
$UNITY_PATH/Contents/Managed/UnityEditor.dll,\
$CURENT_DIR/../../Assets/EGP/Plugins/libUniRx.dll,\
$CURENT_DIR/../../Assets/EGP/Plugins/libECSCore.dll

export ECS_DEFINES="-d:DEBUG;UNITY_EDITOR;UNITY_EDITOR_64"

#生成需要编译的c#文件列表
function search_path() 
{
    for file in `ls $1`
    do
        if [ -d $1"/"$file ]
        then
            search_path $1"/"$file
        else
            if [ ${file##*.} == "cs" ];
            then
                #echo $1"/"$file
                FilePath=$1"/"$file
                ECS_SRC="$ECS_SRC $FilePath"
            fi
        fi
    done
}

search_path $CURENT_DIR/Editor

#cd $UNITY_PATH/Contents/Mono/bin

#mcs $ECS_REFERENCES $ECS_DEFINES -sdk:4.7.1 -target:library $ECS_SRC -out:$CURENT_DIR/libECSCoreEditor.dll

#cd $CURENT_DIR

#mv libECSCoreEditor.dll ../../Assets/EGP/Editor/libECSCoreEditor.dll