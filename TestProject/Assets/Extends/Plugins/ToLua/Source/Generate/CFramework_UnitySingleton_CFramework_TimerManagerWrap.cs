﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class CFramework_UnitySingleton_CFramework_TimerManagerWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(CFramework.UnitySingleton<CFramework.TimerManager>), typeof(UnityEngine.MonoBehaviour), "UnitySingleton_CFramework_TimerManager");
		L.RegFunction("__eq", op_Equality);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("_Instance", get__Instance, set__Instance);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int op_Equality(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			UnityEngine.Object arg0 = (UnityEngine.Object)ToLua.ToObject(L, 1);
			UnityEngine.Object arg1 = (UnityEngine.Object)ToLua.ToObject(L, 2);
			bool o = arg0 == arg1;
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get__Instance(IntPtr L)
	{
		try
		{
			ToLua.Push(L, CFramework.UnitySingleton<CFramework.TimerManager>._Instance);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set__Instance(IntPtr L)
	{
		try
		{
			CFramework.UnitySingleton<CFramework.TimerManager> arg0 = (CFramework.UnitySingleton<CFramework.TimerManager>)ToLua.CheckObject<CFramework.UnitySingleton<CFramework.TimerManager>>(L, 2);
			CFramework.UnitySingleton<CFramework.TimerManager>._Instance = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}
}

