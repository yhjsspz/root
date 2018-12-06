package com.test;

import android.app.Activity;
import android.widget.Toast;

import com.unity3d.player.*;

public class SdkManager {

    private Activity _activity;
    public SdkManager(Activity activity){
        _activity = activity;
    }
    public String androidTest(String _objName ,final String _num,String _funcStr)
    {
        UnityPlayer.UnitySendMessage(_objName, _funcStr, "Come from:" + (Integer.parseInt(_num) + 1));
        _activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                Toast.makeText(_activity, _num, Toast.LENGTH_LONG).show();
            }
        });
        return  "res 2";
    }

}
