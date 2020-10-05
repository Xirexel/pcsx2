package com.xirexel.omegared;

import com.xirexel.omegared.PCSX2.*;

import java.util.*;

public final class GameController {

    public enum StatusEnum
    {
        NoneInitilized,
        Initilized,
        Stopped,
        Started,
        Paused
    }

    private static GameController m_Instance = null;

    private GameController(){}

    public static GameController getInstance()
    {
        if (m_Instance == null)
            m_Instance = new GameController();

        return m_Instance;
    }


    private List<GameControllerStatus> changeStatusEvent = new ArrayList<>();

    private StatusEnum m_Status = StatusEnum.NoneInitilized;



    public void addListener(GameControllerStatus a_listener){changeStatusEvent.add(a_listener);}

    public void PlayPause()
    {
        switch (m_Status)
        {
            case Stopped:
            case Initilized:
            case Paused:
//                LockScreenManager.Instance.showStarting();
                StartInner();
                setStatus(StatusEnum.Started);
                break;
            case Started:
                PauseInner();
                setStatus(StatusEnum.Paused);
                break;
            default:
                break;
        }
    }

    public void Stop(){}

    public void setStatus(StatusEnum a_Status)
    {
        m_Status = a_Status;

        for (GameControllerStatus Event:
                changeStatusEvent) {
            Event.run(a_Status);
        }
    }

    private void PauseInner() {
    }

    private void StartInner() {

        PCSX2Controller.getInstance().Start();
    }

}
