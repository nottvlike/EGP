/*
** $Id: lua.c,v 1.230.1.1 2017/04/19 17:29:57 roberto Exp $
** Lua stand-alone interpreter
** See Copyright Notice in lua.h
*/

#define lua_extension_c

#include "lua.h"
#include "lauxlib.h"

#include <stdio.h>
#include <string.h>

LUALIB_API int lua_extension_rawlen(lua_State *L, int idx)
{
	size_t ret = lua_rawlen(L, idx);
	return (int)ret;
}

LUA_API void lua_extension_newuserdata(lua_State *L, int val)
{
	int* pointer = (int*)lua_newuserdata(L, sizeof(int));
	*pointer = val;
}