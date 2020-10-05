package com.xirexel.omegared;

import android.app.Application;
import android.content.Context;

import com.xirexel.omegared.Adapters.*;
import com.xirexel.omegared.PCSX2.Modules.ModuleManager;

public class GlobalApplication extends Application {

    private static Context appContext;

    @Override
    public void onCreate() {
        super.onCreate();
        appContext = getApplicationContext();

        BIOSAdapter.getInstance();

        ISOAdapter.getInstance();

        ModuleManager.getInstance();
    }

    public static Context getAppContext() {
        return appContext;
    }
}
