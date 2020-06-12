using UnityEngine;
using Lua;

public class TestCase : MonoBehaviour 
{
    void Awake() 
    {
        LuaExtension.DoString("return 20 + 20");
        var result = (int)LuaExtension.ToNumber(1);
        LuaExtension.Pop(1);
        Debug.Log("result = " + result);   
    }
}