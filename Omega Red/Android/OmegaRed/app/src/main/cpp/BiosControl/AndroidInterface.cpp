//
// Created by Evgeny Pereguda on 7/7/2019.
//

#include <jni.h>
#include <string>
#include <vector>

#include "BiosControl.h"


extern std::string Java_To_Str(JNIEnv *env, jstring string);


extern "C"
JNIEXPORT jstring JNICALL
Java_com_evgenypereguda_omegared_PCSX2_Tools_BiosControl_IsBIOS(JNIEnv *env, jobject instance,
																jstring aFilename) {

	auto l_filename = Java_To_Str(env, aFilename);

	std::string l_zone;
	std::string l_version;
	int l_versionInt;
	std::string l_data;
	std::string l_build;

	std::string l_result("");

	if(BiosControl::IsBIOS(
		l_filename,
		l_zone,
		l_version,
		l_versionInt,
		l_data,
		l_build
	))
	{
		l_result = l_zone;
		l_result += ":" + l_version;
		l_result += ":" + l_data;
		l_result += ":" + l_build;
	}

	return env->NewStringUTF(l_result.data());
}


extern "C"
JNIEXPORT jlong JNICALL
Java_com_evgenypereguda_omegared_PCSX2_Tools_BiosControl_getBIOSChecksum(JNIEnv *env,
																		 jobject instance,
																		 jstring aFilename) {

	auto l_filename = Java_To_Str(env, aFilename);

	auto l_value = BiosControl::getBIOSChecksum(l_filename);

	jlong l_result = 0;

	l_result |= l_value;

	return l_result;
}


extern "C"
JNIEXPORT void JNICALL
Java_com_evgenypereguda_omegared_PCSX2_Tools_BiosControl_LoadBIOS__Ljava_lang_String_2JI(
	JNIEnv *env, jobject instance, jstring aFilename, jlong arg1, jint arg2) {

	auto l_filename = Java_To_Str(env, aFilename);

	BiosControl::loadBIOS(l_filename, (void*)arg1, arg2);
}






