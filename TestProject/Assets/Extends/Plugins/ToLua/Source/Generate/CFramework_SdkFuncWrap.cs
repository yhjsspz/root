﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class CFramework_SdkFuncWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(CFramework.SdkFunc), typeof(CFramework.Singleton<CFramework.SdkFunc>));
		L.RegFunction("CallSdk", CallSdk);
		L.RegFunction("New", _CreateCFramework_SdkFunc);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateCFramework_SdkFunc(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 0)
			{
				CFramework.SdkFunc obj = new CFramework.SdkFunc();
				ToLua.PushObject(L, obj);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to ctor method: CFramework.SdkFunc.New");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CallSdk(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 3);
			CFramework.SdkFunc obj = (CFramework.SdkFunc)ToLua.CheckObject<CFramework.SdkFunc>(L, 1);
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			LuaFunction arg1 = ToLua.CheckLuaFunction(L, 3);
			obj.CallSdk(arg0, arg1);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}
}

