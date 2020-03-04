#!/bin/bash

export CURENT_DIR=`pwd`
export ECS_SRC=

export ECS_REFERENCES=-r:$UNITY_PATH/Contents/Managed/UnityEngine.dll,\
$UNITY_PATH/Contents/UnityExtensions/Unity/GUISystem/UnityEngine.UI.dll,\
$UNITY_PATH/Contents/Managed/UnityEditor.dll,\
$CURENT_DIR/../../Assets/EGP/Plugins/libUniRx.dll,\
$CURENT_DIR/../../Assets/EGP/Plugins/libAssetManager.dll,\
$CURENT_DIR/../../Assets/EGP/Plugins/libECSCore.dll,\
$CURENT_DIR/../../Assets/EGP/Plugins/Demigiant/DOTween/DOTween.dll,\
$CURENT_DIR/../../Assets/EGP/Plugins/Demigiant/DOTween/DOTweenModule.dll

export ECS_DEFINES="-d:UNITY_5_3_OR_NEWER;UNITY_5_4_OR_NEWER;UNITY_5_5_OR_NEWER;UNITY_5_6_OR_NEWER;\
UNITY_2017_1_OR_NEWER;UNITY_2017_2_OR_NEWER;UNITY_2017_3_OR_NEWER;UNITY_2017_4_OR_NEWER;UNITY_2018_1_OR_NEWER;\
UNITY_2018_2_OR_NEWER;UNITY_2018_3_OR_NEWER;UNITY_2018_4_OR_NEWER;UNITY_2018_4_0;UNITY_2018_4;UNITY_2018;\
NET_STANDARD_2_0;CSHARP_7_OR_LATER;CSHARP_7_3_OR_NEWER"

export ProductType=$1

if [ "$ProductType" != "release" ]; then
    echo 'debug mode'
    ECS_DEFINES=${ECS_DEFINES}";DEBUG"
else
    echo 'release mode'
fi

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

search_path $CURENT_DIR/Script

cd $UNITY_PATH/Contents/Mono/bin

mcs $ECS_REFERENCES $ECS_DEFINES -sdk:4.7.1 -target:library $ECS_SRC -out:$CURENT_DIR/libECSObject.dll

cd $CURENT_DIR

mv libECSObject.dll ../../Assets/EGP/Plugins/libECSObject.dll