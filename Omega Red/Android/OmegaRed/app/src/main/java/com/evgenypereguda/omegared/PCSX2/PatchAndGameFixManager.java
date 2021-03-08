package com.evgenypereguda.omegared.PCSX2;

public class PatchAndGameFixManager {

    private static PatchAndGameFixManager m_Instance = null;

    private PatchAndGameFixManager(){}

    public static PatchAndGameFixManager getInstance()
    {
        if (m_Instance == null)
            m_Instance = new PatchAndGameFixManager();

        return m_Instance;
    }

    public void LoadAllPatchesAndStuff()
    {

    }
}
