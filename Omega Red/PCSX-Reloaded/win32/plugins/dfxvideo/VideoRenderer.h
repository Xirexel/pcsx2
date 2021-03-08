#pragma once

class VideoRenderer
{
public:
    VideoRenderer();
    virtual ~VideoRenderer();
	
    void execute(const wchar_t *a_command, wchar_t **a_result);

private:

    int init(void *sharedhandle, void *capturehandle, void *directXDeviceNative);
    void shutdown();
};