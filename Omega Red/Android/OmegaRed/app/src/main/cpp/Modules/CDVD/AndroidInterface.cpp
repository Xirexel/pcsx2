//
// Created by Xirexel on 7/10/2019.
//


#include <jni.h>
#include <string>

#include "ModuleInterface.h"

extern std::wstring Java_To_WStr(JNIEnv *env, jstring string);

extern "C"
JNIEXPORT jlong JNICALL
Java_com_xirexel_omegared_PCSX2_Modules_CDVD_GetPCSX2LibAPI(JNIEnv *env,
																			  jobject instance) {

	return (long)(getAPI());

}

extern "C"
JNIEXPORT jstring JNICALL
Java_com_xirexel_omegared_PCSX2_Modules_CDVD_Execute(JNIEnv *env, jobject instance,
																	 jstring aCommand) {

	jstring l_result = nullptr;

	wchar_t* l_nativeresult = nullptr;

	execute(Java_To_WStr(env, aCommand).data(), &l_nativeresult);

	if(l_nativeresult != nullptr)
	{
		auto l_length = wcslen(l_nativeresult);

		jchar* str2 = (jchar*)malloc((l_length+1)*sizeof(jchar));
		int i;
		for (i = 0; i < l_length; i++)
			// This discards two bytes in str[i], but these should be 0 in UTF-16
			str2[i] = l_nativeresult[i];

		str2[l_length] = 0;
		// Now we have a compatible string!
		l_result = env->NewString(str2, l_length);
		// And we can free the buffer, to prevent memory leaks
		free(str2);


		releaseString(l_nativeresult);
	}

	return l_result;
}



