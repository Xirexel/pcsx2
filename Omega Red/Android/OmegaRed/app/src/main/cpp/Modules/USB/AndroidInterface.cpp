//
// Created by Evgeny Pereguda on 7/10/2019.
//


#include <jni.h>
#include <string>

#include "ModuleInterface.h"

extern std::wstring Java_To_WStr(JNIEnv *env, jstring string);

extern "C"
JNIEXPORT jlong JNICALL
Java_com_evgenypereguda_omegared_PCSX2_Modules_USB_GetPCSX2LibAPI(JNIEnv *env,
																			  jobject instance) {

	return (long)(getAPI());

}

extern "C"
JNIEXPORT jstring JNICALL
Java_com_evgenypereguda_omegared_PCSX2_Modules_USB_Execute(JNIEnv *env, jobject instance,
																	 jstring aCommand) {

	jstring l_result = nullptr;

	wchar_t* l_nativeresult = nullptr;

	execute(Java_To_WStr(env, aCommand).data(), &l_nativeresult);

	if(l_nativeresult != nullptr)
	{
		l_result = env->NewString((jchar*)l_result, wcslen(l_nativeresult));

		releaseString(l_nativeresult);
	}

	return l_result;
}



