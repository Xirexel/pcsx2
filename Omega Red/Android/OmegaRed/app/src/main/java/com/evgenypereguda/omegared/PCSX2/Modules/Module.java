package com.evgenypereguda.omegared.PCSX2.Modules;

public abstract class Module {

    protected abstract long GetPCSX2LibAPI();

    protected abstract String Execute(String aCommand);

    public long getPCSX2LibAPI()
    {
        return GetPCSX2LibAPI();
    }

    public String execute(String aCommand)
    {
        return Execute(aCommand);
    }

    public ModuleManager.ModuleType ModuleType;
}
