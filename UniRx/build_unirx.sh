#!/bin/bash

export UNITY_PATH="/Volumes/ExtremeSSD/Applications/Unity/Unity.app"
export UNIRX_PATH="7.0.0"

export CURENT_DIR=`pwd`
export UNIRX_SRC=

export UNIRX_REFERENCES=-r:$UNITY_PATH/Contents/Managed/UnityEngine.dll,\
$UNITY_PATH/Contents/UnityExtensions/Unity/GUISystem/UnityEngine.UI.dll

export UNIRX_DEFINES="-d:UNITY_5_3_OR_NEWER;UNITY_5_4_OR_NEWER;UNITY_5_5_OR_NEWER;UNITY_5_6_OR_NEWER;\
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
            UNIRX_SRC="$UNIRX_SRC $FilePath"
        fi
    done
}

search_path $CURENT_DIR/$UNIRX_PATH

cd $UNITY_PATH/Contents/Mono/bin

mcs $UNIRX_REFERENCES $UNIRX_DEFINES -sdk:4.7.1 -target:library $UNIRX_SRC -out:$CURENT_DIR/libUniRx.dll

cd $CURENT_DIR

mv libUniRx.dll ../Assets/EGP/Plugins/libUniRx.dll