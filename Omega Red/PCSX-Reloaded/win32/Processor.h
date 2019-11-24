#pragma once

class Processor
{
    enum State {

		INACTIVE,
		STARTED,
		PAUSED
    };

	State m_State;

	bool m_is_paused;

    bool m_is_shutdowned;

	int resumeInner();

public:
    Processor();
    virtual ~Processor();

	int Start();

	int Pause();
	
	int Shutdown();

};
