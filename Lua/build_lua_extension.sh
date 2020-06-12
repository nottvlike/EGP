#!/bin/bash

export UNITY_PATH="/Volumes/ExtremeSSD/Applications/UnityApp/2018.4.17f1/Unity.app"
export LUA_PATH="Extension"

export CURENT_DIR=`pwd`
export LUA_SRC=

export LUA_REFERENCES=-r:$UNITY_PATH/Contents/Managed/UnityEngine.dll,\
$UNITY_PATH/Contents/UnityExtensions/Unity/GUISystem/UnityEngine.UI.dll

export LUA_DEFINES="-d:UNITY_5_3_OR_NEWER;UNITY_5_4_OR_NEWER;UNITY_5_5_OR_NEWER;UNITY_5_6_OR_NEWER;\
UNITY_2017_1_OR_NEWER;UNITY_2017_2_OR_NEWER;UNITY_2017_3_OR_NEWER;UNITY_2017_4_OR_NEWER;UNITY_2018_1_OR_NEWER;\
UNITY_2018_2_OR_NEWER;UNITY_2018_3_OR_NEWER;UNITY_2018_4_OR_NEWER;UNITY_2018_4_0;UNITY_2018_4;UNITY_2018;\
NET_STANDARD_2_0;CSHARP_7_OR_LATER;CSHARP_7_3_OR_NEWER"

#生成需要编译的c#文件列表
function search_path() 
{
    for file in `ls $1`
    do
        if [ -d $1"/"$file ]
        then
            search_path $1"/"$file
        else
            #echo $1"/"$file
            FilePath=$1"/"$file
            LUA_SRC="$LUA_SRC $FilePath"
        fi
    done
}

search_path $CURENT_DIR/$LUA_PATH

cd $UNITY_PATH/Contents/Mono/bin

mcs $LUA_REFERENCES $LUA_DEFINES -sdk:4.7.1 -target:library $LUA_SRC -out:$CURENT_DIR/libLua.dll

cd $CURENT_DIR

mv libLua.dll ../Assets/EGP/Plugins/libLua.dll