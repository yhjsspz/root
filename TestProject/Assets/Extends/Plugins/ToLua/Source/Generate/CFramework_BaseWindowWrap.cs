﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class CFramework_BaseWindowWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(CFramework.BaseWindow), typeof(CFramework.BaseView));
		L.RegFunction("Dispose", Dispose);
		L.RegFunction("New", _CreateCFramework_BaseWindow);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateCFramework_BaseWindow(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 4)
			{
				string arg0 = ToLua.CheckString(L, 1);
				string[] arg1 = ToLua.CheckStringArray(L, 2);
				bool arg2 = LuaDLL.luaL_checkboolean(L, 3);
				UnityEngine.Transform arg3 = (UnityEngine.Transform)ToLua.CheckObject<UnityEngine.Transform>(L, 4);
				CFramework.BaseWindow obj = new CFramework.BaseWindow(arg0, arg1, arg2, arg3);
				ToLua.PushObject(L, obj);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to ctor method: CFramework.BaseWindow.New");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Dispose(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			CFramework.BaseWindow obj = (CFramework.BaseWindow)ToLua.CheckObject<CFramework.BaseWindow>(L, 1);
			obj.Dispose();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}
}

