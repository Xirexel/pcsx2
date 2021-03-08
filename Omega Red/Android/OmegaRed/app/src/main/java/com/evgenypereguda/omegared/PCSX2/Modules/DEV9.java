package com.evgenypereguda.omegared.PCSX2.Modules;

public final class DEV9 extends Module {

    static {
        System.loadLibrary("DEV9");
    }

    public DEV9(){ModuleType = ModuleManager.ModuleType.DEV9;}

    protected native long GetPCSX2LibAPI();

    protected native String Execute(String aCommand);
}
