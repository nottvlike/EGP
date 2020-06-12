using System;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Lua 
{
#pragma warning disable 414
    public class MonoPInvokeCallbackAttribute : System.Attribute
    {
        private Type type;
        public MonoPInvokeCallbackAttribute(Type t)
        {
            type = t;
        }
    }
#pragma warning restore 414

    public enum LuaTypes : int
    {
        LUA_TNONE = -1,
        LUA_TNIL = 0,
        LUA_TBOOLEAN = 1,
        LUA_TLIGHTUSERDATA = 2,
        LUA_TNUMBER = 3,
        LUA_TSTRING = 4,
        LUA_TTABLE = 5,
        LUA_TFUNCTION = 6,
        LUA_TUSERDATA = 7,
        LUA_TTHREAD = 8,
    }

    public enum LuaGCOptions
    {
        LUA_GCSTOP = 0,
        LUA_GCRESTART = 1,
        LUA_GCCOLLECT = 2,
        LUA_GCCOUNT = 3,
        LUA_GCCOUNTB = 4,
        LUA_GCSTEP = 5,
        LUA_GCSETPAUSE = 6,
        LUA_GCSETSTEPMUL = 7,
    }

    public enum LuaThreadStatus : int
    {
        LUA_YIELD = 1,
        LUA_ERRRUN = 2,
        LUA_ERRSYNTAX = 3,
        LUA_ERRMEM = 4,
        LUA_ERRERR = 5,
    }

    public sealed class LuaIndexes
    {
        public static int LUA_REGISTRYINDEX = -1000000 - 1000;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ReaderInfo
    {
        public String chunkData;
        public bool finished;
    }

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int LuaCSFunction(IntPtr luaState);
#else
    public delegate int LuaCSFunction(IntPtr luaState);
#endif

    public delegate string LuaChunkReader(IntPtr luaState, ref ReaderInfo data, ref uint size);

    public delegate int LuaFunctionCallback(IntPtr luaState);

    public sealed class LuaExtension
    {
#region Native Method
    #if UNITY_IPHONE && !UNITY_EDITOR
        const string LUA_EXTENSION = "__Internal";
    #else
        const string LUA_EXTENSION = "lua";
    #endif

    #region state manipulation
        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr luaL_newstate();

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern void lua_close(IntPtr luaState);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr lua_newthread(IntPtr L);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr lua_atpanic(IntPtr luaState, LuaCSFunction panicf);
    #endregion

    #region basic stack manipulation
        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern int lua_gettop(IntPtr luaState);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern void lua_settop(IntPtr luaState, int newTop);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern void lua_pushvalue(IntPtr luaState, int index);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern int lua_rotate(IntPtr luaState, int index, int n);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern void lua_copy(IntPtr luaState,int from,int toidx);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern int lua_checkstack(IntPtr luaState, int extra);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern void lua_xmove(IntPtr from, IntPtr to, int n);
    #endregion

    #region access functions (stack -> C)
        [DllImport(LUA_EXTENSION,CallingConvention=CallingConvention.Cdecl)]
        static extern int lua_isnumber(IntPtr luaState, int index);

        [DllImport(LUA_EXTENSION,CallingConvention=CallingConvention.Cdecl)]
        static extern int lua_isstring(IntPtr luaState, int index);

        [DllImport(LUA_EXTENSION,CallingConvention=CallingConvention.Cdecl)]
        static extern int lua_iscfunction(IntPtr luaState, int index);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern int lua_isinteger(IntPtr luaState, int p);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern int lua_isuserdata(IntPtr luaState, int stackPos);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern LuaTypes lua_type(IntPtr luaState, int index);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr lua_typename(IntPtr luaState, int type);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern double lua_tonumberx(IntPtr luaState, int index, IntPtr x);
     
        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern Int64 lua_tointegerx(IntPtr luaState, int index,IntPtr x);

        [DllImport(LUA_EXTENSION,CallingConvention=CallingConvention.Cdecl)]
        static extern int lua_toboolean(IntPtr luaState, int index);

        [DllImport(LUA_EXTENSION,CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr lua_tolstring(IntPtr L, int index, out IntPtr strLen);//[-0, +0, m]

        [DllImport(LUA_EXTENSION,CallingConvention=CallingConvention.Cdecl)]
        static extern string lua_tostring(IntPtr luaState, int index);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern int lua_extension_rawlen(IntPtr luaState, int index);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr lua_tocfunction(IntPtr luaState, int index);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr lua_touserdata(IntPtr luaState, int index);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern int lua_tothread(IntPtr L, int index);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr lua_topointer(IntPtr L, int index);
    #endregion

    #region Comparison and arithmetic functions
        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern void lua_arith(IntPtr luaState, int op);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern int lua_rawequal(IntPtr luaState, int stackPos1, int stackPos2);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern int lua_compare(IntPtr luaState, int index1, int index2, int op);
    #endregion

    #region push functions (C -> stack)
        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern void lua_pushnil(IntPtr luaState);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern void lua_pushnumber(IntPtr luaState, double number);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern void lua_pushinteger(IntPtr luaState, Int64 i);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern void luaS_pushlstring(IntPtr luaState, byte[] str, int size);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern void lua_pushstring(IntPtr luaState, string str);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern void lua_pushcclosure(IntPtr l, IntPtr f, int nup);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern void lua_pushboolean(IntPtr luaState, int value);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern void lua_pushlightuserdata(IntPtr luaState, IntPtr udata);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern int lua_pushthread(IntPtr L);
    #endregion

    #region get functions (Lua -> stack)
        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern int lua_getglobal(IntPtr luaState, string name);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern int lua_gettable(IntPtr luaState, int index);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern int lua_getfield(IntPtr luaState, int stackPos, string meta);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern int lua_geti(IntPtr luaState, int index, Int64 n);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern int lua_rawget(IntPtr luaState, int index);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern int lua_rawgeti(IntPtr luaState, int tableIndex, Int64 index);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern int lua_rawgetp(IntPtr luaState, int tableIndex, IntPtr p);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern void lua_createtable(IntPtr luaState, int narr, int nrec);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern void lua_extension_newuserdata(IntPtr luaState, int val);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern int lua_getmetatable(IntPtr luaState, int objIndex);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern int lua_getuservalue(IntPtr luaState, int objIndex);
    #endregion

    #region set functions (stack -> Lua)
        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern void lua_setglobal(IntPtr luaState, string name);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern void lua_settable(IntPtr luaState, int index);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern void lua_setfield(IntPtr luaState, int stackPos, string name);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern void lua_seti(IntPtr luaState, int tableIndex, Int64 index);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern void lua_rawset(IntPtr luaState, int index);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern void lua_rawseti(IntPtr luaState, int tableIndex, Int64 index);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern void lua_rawsetp(IntPtr luaState, int tableIndex, IntPtr p);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern int lua_setmetatable(IntPtr luaState, int objIndex);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern void lua_setuservalue(IntPtr luaState, int index);
    #endregion

    #region 'load' and 'call' functions (load and run Lua code)
        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern int lua_callk(IntPtr luaState, int nArgs, int nResults,int ctx,IntPtr k);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern int lua_pcallk(IntPtr luaState, int nArgs, int nResults, int errfunc,int ctx,IntPtr k);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern int lua_load(IntPtr luaState, LuaChunkReader chunkReader, ref ReaderInfo data, string chunkName);
    #endregion

    #region coroutine functions
        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern int lua_resume(IntPtr L, IntPtr from, int narg);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern int lua_status(IntPtr L);
    #endregion

    #region garbage-collection function and options
        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern int lua_gc(IntPtr luaState, LuaGCOptions what, int data);
    #endregion

    #region miscellaneous functions
        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern int lua_error(IntPtr luaState);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern int lua_next(IntPtr luaState, int index);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern void lua_concat(IntPtr luaState, int n);
    #endregion

    #region lualib functions
        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern string luaL_gsub(IntPtr luaState, string str, string pattern, string replacement);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern int luaL_callmeta(IntPtr luaState, int stackPos, string name);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern void luaL_openlibs(IntPtr luaState);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern int luaL_loadstring(IntPtr luaState, string chunk);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern int luaL_loadbufferx(IntPtr luaState, byte[] buff, int size, string name, IntPtr x);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern int luaL_ref(IntPtr luaState, int registryIndex);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern void luaL_unref(IntPtr luaState, int registryIndex, int reference);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern int luaL_getmetafield(IntPtr luaState, int stackPos, string field);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern int luaL_newmetatable(IntPtr luaState, string meta);
 
        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr luaL_checkudata(IntPtr luaState, int stackPos, string meta);

        [DllImport(LUA_EXTENSION, CallingConvention = CallingConvention.Cdecl)]
        static extern void luaL_where(IntPtr luaState, int level);
    #endregion
#endregion

#region Extension
        static IntPtr _luaState;
        public static IntPtr LuaState
        {
            get
            {
                if (_luaState == IntPtr.Zero)
                {
                    _luaState = luaL_newstate();
                }

                return _luaState;
            }
        }

        static string TypeNameStr(LuaTypes type)
        {
            IntPtr p = lua_typename(LuaState, (int)type);
            return Marshal.PtrToStringAnsi(p);
        }

        public static string TypeName(int stackPos)
        {
            return TypeNameStr(lua_type(LuaState, stackPos));
        }

        public static bool IsFunction(int stackPos)
        {
            return lua_type(LuaState, stackPos) == LuaTypes.LUA_TFUNCTION;
        }

        public static bool IsLightUserdata(int stackPos)
        {
            return lua_type(LuaState, stackPos) == LuaTypes.LUA_TLIGHTUSERDATA;
        }

        public static bool IsTable(int stackPos)
        {
            return lua_type(LuaState, stackPos) == LuaTypes.LUA_TTABLE;
        }

        public static bool IsThread(int stackPos)
        {
            return lua_type(LuaState, stackPos) == LuaTypes.LUA_TTHREAD;
        }

        public static int DoString(string chunk)
        {
            int result = luaL_loadstring(LuaState, chunk);
            if (result != 0)
                return result;

            return Pcall(0, -1, 0);
        }

        public static void Insert(int newTop)
        {
            lua_rotate(LuaState, newTop, 1);
        }

        public static void PushGlobalTable()
        {
            lua_rawgeti(LuaState, LuaIndexes.LUA_REGISTRYINDEX, 2); 
        }

        public static void CreateTable()
        {
            lua_createtable(LuaState, 0, 0);
        }

        public static int RawLen(int stackPos)
        {
            return lua_extension_rawlen(LuaState, stackPos);
        }

        public static int Call(int nArgs, int nResults)
        {
            return lua_callk(LuaState, nArgs, nResults, 0, IntPtr.Zero);
        }

        public static int Pcall(int nArgs, int nResults, int errfunc)
        {
            return lua_pcallk(LuaState, nArgs, nResults, errfunc, 0, IntPtr.Zero);
        }

        public static double ToNumber(int index)
        {
            return lua_tonumberx(LuaState, index, IntPtr.Zero);
        }   

        public static int ToInteger(int index)
        {
            return (int)lua_tointegerx(LuaState, index, IntPtr.Zero);
        }


        public static int LoadBuffer(byte[] buff, int size, string name)
        {
            return luaL_loadbufferx(LuaState, buff, size, name, IntPtr.Zero);
        }

        public static void Remove(int idx)
        {
            lua_rotate(LuaState, (idx), -1);
            Pop(1);
        }

        public static Int64 CheckInteger(int stackPos) 
        {
            CheckType(stackPos, LuaTypes.LUA_TNUMBER);
            return lua_tointegerx(LuaState, stackPos, IntPtr.Zero);
        }
        
        public static int Resume(int narg)
        {
            return lua_resume(LuaState, IntPtr.Zero, narg);
        }

        public static void Replace(int index) 
        {
            lua_copy(LuaState, -1, (index));
            Pop(1);
        }

        public static int Equal(int index1, int index2)
        {
            return lua_compare(LuaState, index1, index2, 0);
        }

        public static void Pop(int amount)
        {
            lua_settop(LuaState, -(amount) - 1);
        }

        public static bool IsNil(int index)
        {
            return (lua_type(LuaState, index) == LuaTypes.LUA_TNIL);
        }

        public static bool IsNumber(int index)
        {
            return lua_isnumber(LuaState, index) > 0;
        }
        
        public static bool IsBoolean(int index)
        {
            return lua_type(LuaState, index) == LuaTypes.LUA_TBOOLEAN;
        }

        public static void GetRef(int reference)
        {
            lua_rawgeti(LuaState, LuaIndexes.LUA_REGISTRYINDEX, reference);
        }

        public static void UnRef(int reference)
        {
            luaL_unref(LuaState, LuaIndexes.LUA_REGISTRYINDEX, reference);
        }

        public static bool IsString(int index)
        {
            return lua_isstring(LuaState, index) > 0;
        }

        public static bool IsCFunction(int index)
        {
            return lua_iscfunction(LuaState, index) > 0;
        }

        public static void CheckType(int p, LuaTypes t)
        {
            LuaTypes ct = lua_type(LuaState, p);
            if (ct != t)
            {
                throw new Exception(string.Format("arg {0} expect {1}, got {2}", p, TypeNameStr(t), TypeNameStr(ct)));
            }
        }

        public static void PushCFunction(LuaCSFunction function)
        {
#if SLUA_STANDALONE
            // Add all LuaCSFunction�� or they will be GC collected!  (problem at windows, .net framework 4.5, `CallbackOnCollectedDelegated` exception)
            GCHandle.Alloc(function);
#endif
            IntPtr fn = Marshal.GetFunctionPointerForDelegate(function);
            lua_pushcclosure(LuaState, fn, 0);
        }

        public static bool ToBoolean(int index)
        {
            return lua_toboolean(LuaState, index) > 0;
        }

        public static string ToString(int index)
        {
            IntPtr strlen;
            IntPtr str = lua_tolstring(LuaState, index, out strlen);
            if (str != IntPtr.Zero)
            {
                int len = strlen.ToInt32();
                byte[] buffer = new byte[len];
                Marshal.Copy(str, buffer, 0, len);
                return Encoding.UTF8.GetString(buffer);
            }

            return string.Empty;
        }

        public static byte[] ToBytes(int index)
        {
            IntPtr strlen;
            IntPtr str = lua_tolstring(LuaState, index, out strlen);
            if (str != IntPtr.Zero)
            {
                int buff_len = strlen.ToInt32();
                byte[] buffer = new byte[buff_len];
                Marshal.Copy(str, buffer, 0, buff_len);
                return buffer;
            }

            return null;
        }

        public static bool CheckStack(int extra)
        {
            return lua_checkstack(LuaState, extra) > 0;
        }

        public static void PushBoolean(bool value)
        {
            lua_pushboolean(LuaState, value ? 1 : 0);
        }

        public static void PushLString(byte[] str, int size)
        {
            luaS_pushlstring(LuaState, str, size);
        }

        public static void GetField(string meta)
        {
            lua_getfield(LuaState, LuaIndexes.LUA_REGISTRYINDEX, meta);
        }

        public static bool GetMetaField(int stackPos, string field)
        {
            return luaL_getmetafield(LuaState, stackPos, field) > 0;
        }

        public static void PushCClosure(LuaCSFunction f, int nup)
        {
#if SLUA_STANDALONE
            // Add all LuaCSFunction�� or they will be GC collected!  (problem at windows, .net framework 4.5, `CallbackOnCollectedDelegated` exception)
            GCHandle.Alloc(f);
#endif
            IntPtr fn = Marshal.GetFunctionPointerForDelegate(f);
            lua_pushcclosure(LuaState, fn, nup);
        }

        public static double CheckNumber(IntPtr luaState, int stackPos)
        {
            CheckType(stackPos, LuaTypes.LUA_TNUMBER);
            return ToNumber(stackPos);
        }

        public static int AbsIndex(int index)
        {
            return index > 0 ? index : lua_gettop(LuaState) + index + 1;
        }

        public static int UpValueIndex(int i)
        {
            return LuaIndexes.LUA_REGISTRYINDEX - i;
        }
#endregion
    }

}