//
// Created by Evgeny Pereguda on 7/7/2019.
//

#include <jni.h>
#include <string>
#include <vector>

#include "Interface.h"
#include "../SIMDEmitter/SIMDEmitter.h"


extern std::wstring Java_To_WStr(JNIEnv *env, jstring string);

extern std::string Java_To_Str(JNIEnv *env, jstring string);

extern std::vector<std::string> splitMethod(JNIEnv *env, jstring string);


static JavaVM* s_JavaVM = nullptr;

static jobject g_PCSX2LibNative = nullptr;


extern "C"
JNIEXPORT void JNICALL
Java_com_evgenypereguda_omegared_PCSX2_PCSX2LibNative_registerPCSX2Lib(JNIEnv *env,
																	   jobject instance) {

	env->GetJavaVM(&s_JavaVM);

	g_PCSX2LibNative = env->NewGlobalRef(instance);
}


extern "C"
JNIEXPORT void JNICALL
Java_com_evgenypereguda_omegared_PCSX2_PCSX2LibNative_NativeDetectCpuAndUserMode(JNIEnv *env,
																				 jobject instance) {
#if (defined(__ANDROID__) && !defined(ANDROID_ABI_X86))
	SIMDEmitter::init();
#endif

	DetectCpuAndUserModeFunc();
}


extern "C"
JNIEXPORT void JNICALL
Java_com_evgenypereguda_omegared_PCSX2_PCSX2LibNative_NativeAllocateCoreStuffs(JNIEnv *env,
																			   jobject instance,
																			   jstring a_config_) {
	AllocateCoreStuffsFunc(Java_To_WStr(env, a_config_).data());
}


extern "C"
JNIEXPORT void JNICALL
Java_com_evgenypereguda_omegared_PCSX2_PCSX2LibNative_NativeSetElfPathFunc(JNIEnv *env,
																		   jobject instance,
																		   jstring a_config_) {
	const char *a_config = env->GetStringUTFChars(a_config_, 0);

	PCSX2_Hle_SetElfPathFunc(a_config);

	env->ReleaseStringUTFChars(a_config_, a_config);
}


extern "C"
JNIEXPORT void JNICALL
Java_com_evgenypereguda_omegared_PCSX2_PCSX2LibNative_SetSPU2(JNIEnv *env, jobject instance,
															  jlong a_ptr) {
	setSPU2((PCSX2Lib::API::SPU2_API*)a_ptr);
}

extern "C"
JNIEXPORT void JNICALL
Java_com_evgenypereguda_omegared_PCSX2_PCSX2LibNative_SetCDVD(JNIEnv *env, jobject instance,
															  jlong a_ptr) {
	setCDVD((PCSX2Lib::API::CDVD_API*)a_ptr);
}

extern "C"
JNIEXPORT void JNICALL
Java_com_evgenypereguda_omegared_PCSX2_PCSX2LibNative_SetGS(JNIEnv *env, jobject instance,
															  jlong a_ptr) {
	setGS((PCSX2Lib::API::GS_API*)a_ptr);
}

extern "C"
JNIEXPORT void JNICALL
Java_com_evgenypereguda_omegared_PCSX2_PCSX2LibNative_SetDEV9(JNIEnv *env, jobject instance,
															  jlong a_ptr) {
	setDEV9((PCSX2Lib::API::DEV9_API*)a_ptr);
}

extern "C"
JNIEXPORT void JNICALL
Java_com_evgenypereguda_omegared_PCSX2_PCSX2LibNative_SetMcd(JNIEnv *env, jobject instance,
															  jlong a_ptr) {
	setMcd((PCSX2Lib::API::MCD_API*)a_ptr);
}

extern "C"
JNIEXPORT void JNICALL
Java_com_evgenypereguda_omegared_PCSX2_PCSX2LibNative_SetPAD(JNIEnv *env, jobject instance,
															 jlong a_ptr) {
	setPAD((PCSX2Lib::API::PAD_API*)a_ptr);
}

extern "C"
JNIEXPORT void JNICALL
Java_com_evgenypereguda_omegared_PCSX2_PCSX2LibNative_SetFW(JNIEnv *env, jobject instance,
															jlong a_ptr) {
	setFW((PCSX2Lib::API::FW_API*)a_ptr);
}

extern "C"
JNIEXPORT void JNICALL
Java_com_evgenypereguda_omegared_PCSX2_PCSX2LibNative_SetUSB(JNIEnv *env, jobject instance,
															jlong a_ptr) {
	setUSB((PCSX2Lib::API::USB_API*)a_ptr);
}

static std::string s_PluginsInitCallback;

static std::string s_PluginsCloseCallback;

static std::string s_PluginsShutdownCallback;

static std::string s_PluginsOpenCallback;

static std::string s_PluginsAreLoadedCallback;

static std::string s_UIEnableSysActionsCallback;

static std::string s_LoadAllPatchesAndStuffCallback;

static std::string s_LoadBIOSCallbackCallback;

static std::string s_CDVDNVMCallback;

static std::string s_CDVDGetMechaVerCallback;

static std::string s_DoFreezeCallback;

static void callNoneArgMethod(const std::string& aMethodCallback)
{

	if(s_JavaVM == nullptr)
		return;

	JNIEnv* l_Env = nullptr;


	const jint get_res = s_JavaVM->GetEnv(reinterpret_cast<void**>(&l_Env), JNI_VERSION_1_6);

	if (get_res == JNI_EDETACHED) {
		if (s_JavaVM->AttachCurrentThread(&l_Env, NULL) != 0) {
			return;
		}
	} else if (get_res != JNI_OK) {
		return;
	}

	jclass l_PCSX2LibNative = l_Env->GetObjectClass(g_PCSX2LibNative);

	jmethodID methodId=l_Env->GetMethodID(l_PCSX2LibNative, aMethodCallback.data(), "()V");

	l_Env->CallVoidMethod(g_PCSX2LibNative, methodId);
}

static bool callBooleanNoneArgMethod(const std::string& aMethodCallback)
{
	bool l_result = false;

	if(s_JavaVM == nullptr)
		return l_result;

	JNIEnv* l_Env = nullptr;


	const jint get_res = s_JavaVM->GetEnv(reinterpret_cast<void**>(&l_Env), JNI_VERSION_1_6);

	if (get_res == JNI_EDETACHED) {
		if (s_JavaVM->AttachCurrentThread(&l_Env, NULL) != 0) {
			return l_result;
		}
	} else if (get_res != JNI_OK) {
		return l_result;
	}

	jclass l_PCSX2LibNative = l_Env->GetObjectClass(g_PCSX2LibNative);

	jmethodID methodId=l_Env->GetMethodID(l_PCSX2LibNative, aMethodCallback.data(), "()Z");

	l_result = l_Env->CallBooleanMethod(g_PCSX2LibNative, methodId);

	return l_result;
}

static void callOneUInteArgMethod(const std::string& aMethodCallback, unsigned int arg)
{

	if(s_JavaVM == nullptr)
		return;

	JNIEnv* l_Env = nullptr;


	const jint get_res = s_JavaVM->GetEnv(reinterpret_cast<void**>(&l_Env), JNI_VERSION_1_6);

	if (get_res == JNI_EDETACHED) {
		if (s_JavaVM->AttachCurrentThread(&l_Env, NULL) != 0) {
			return;
		}
	} else if (get_res != JNI_OK) {
		return;
	}

	jclass l_PCSX2LibNative = l_Env->GetObjectClass(g_PCSX2LibNative);

	jmethodID methodId=l_Env->GetMethodID(l_PCSX2LibNative, aMethodCallback.data(), "(I)V");

	l_Env->CallVoidMethod(g_PCSX2LibNative, methodId, (int) arg);
}

static void callOnePtrOneUInteArgMethod(const std::string& aMethodCallback, unsigned char* arg1, int arg2)
{
	if(s_JavaVM == nullptr)
		return;

	JNIEnv* l_Env = nullptr;


	const jint get_res = s_JavaVM->GetEnv(reinterpret_cast<void**>(&l_Env), JNI_VERSION_1_6);

	if (get_res == JNI_EDETACHED) {
		if (s_JavaVM->AttachCurrentThread(&l_Env, NULL) != 0) {
			return;
		}
	} else if (get_res != JNI_OK) {
		return;
	}

	jclass l_PCSX2LibNative = l_Env->GetObjectClass(g_PCSX2LibNative);

	jmethodID methodId=l_Env->GetMethodID(l_PCSX2LibNative, aMethodCallback.data(), "(JI)V");

	l_Env->CallVoidMethod(g_PCSX2LibNative, methodId, (s64)arg1, arg2);

}

static void callOnePtrTwoIntBoolean(const std::string& aMethodCallback, unsigned char* arg1, int arg2)
{
	if(s_JavaVM == nullptr)
		return;

	JNIEnv* l_Env = nullptr;


	const jint get_res = s_JavaVM->GetEnv(reinterpret_cast<void**>(&l_Env), JNI_VERSION_1_6);

	if (get_res == JNI_EDETACHED) {
		if (s_JavaVM->AttachCurrentThread(&l_Env, NULL) != 0) {
			return;
		}
	} else if (get_res != JNI_OK) {
		return;
	}

	jclass l_PCSX2LibNative = l_Env->GetObjectClass(g_PCSX2LibNative);

	jmethodID methodId=l_Env->GetMethodID(l_PCSX2LibNative, aMethodCallback.data(), "(Ljava/nio/ByteBuffer;)V");

	jobject l_buffer = l_Env->NewDirectByteBuffer(arg1, arg2);

	l_Env->CallVoidMethod(g_PCSX2LibNative, methodId, l_buffer);
}

static void callOnePtrTwoIntBoolean(const std::string& aMethodCallback, unsigned char* arg1, int arg2, int arg3, bool arg4)
{
	if(s_JavaVM == nullptr)
		return;

	JNIEnv* l_Env = nullptr;


	const jint get_res = s_JavaVM->GetEnv(reinterpret_cast<void**>(&l_Env), JNI_VERSION_1_6);

	if (get_res == JNI_EDETACHED) {
		if (s_JavaVM->AttachCurrentThread(&l_Env, NULL) != 0) {
			return;
		}
	} else if (get_res != JNI_OK) {
		return;
	}

	jclass l_PCSX2LibNative = l_Env->GetObjectClass(g_PCSX2LibNative);

	jmethodID methodId=l_Env->GetMethodID(l_PCSX2LibNative, aMethodCallback.data(), "(Ljava/nio/ByteBuffer;IZ)V");

	jobject l_buffer = l_Env->NewDirectByteBuffer(arg1, arg3);

	l_Env->CallVoidMethod(g_PCSX2LibNative, methodId, l_buffer, arg2, arg4);
}

static int callStaticThreeInteArgMethod(const std::string& aMethodCallback, int arg1, int arg2, int arg3)
{
	int l_result = -1;

	if(s_JavaVM == nullptr)
		return l_result;

	JNIEnv* l_Env = nullptr;


	const jint get_res = s_JavaVM->GetEnv(reinterpret_cast<void**>(&l_Env), JNI_VERSION_1_6);

	if (get_res == JNI_EDETACHED) {
		if (s_JavaVM->AttachCurrentThread(&l_Env, NULL) != 0) {
			return l_result;
		}
	} else if (get_res != JNI_OK) {
		return l_result;
	}

	jclass l_PCSX2LibNative = l_Env->GetObjectClass(g_PCSX2LibNative);

	jmethodID methodId=l_Env->GetStaticMethodID(l_PCSX2LibNative, aMethodCallback.data(), "(III)I");

	l_result = l_Env->CallIntMethod(g_PCSX2LibNative, methodId, arg1, arg2, arg3);

	return l_result;
}

CALLBACK void SetPluginsInitCallback()
{
	callNoneArgMethod(s_PluginsInitCallback);
}


extern "C"
JNIEXPORT void JNICALL
Java_com_evgenypereguda_omegared_PCSX2_PCSX2LibNative_SetPluginsInitCallback(JNIEnv *env,
																			 jobject instance,
																			 jstring aMethod) {

	s_PluginsInitCallback = Java_To_Str(env, aMethod);

	::setPluginsInitCallback(
		SetPluginsInitCallback
		);
}

CALLBACK void SetPluginsCloseCallback()
{
	callNoneArgMethod(s_PluginsCloseCallback);
}

extern "C"
JNIEXPORT void JNICALL
Java_com_evgenypereguda_omegared_PCSX2_PCSX2LibNative_SetPluginsCloseCallback(JNIEnv *env,
																			  jobject instance,
																			  jstring aMethod) {

	s_PluginsCloseCallback = Java_To_Str(env, aMethod);

	::setPluginsCloseCallback(
		SetPluginsCloseCallback
	);
}


CALLBACK void SetPluginsShutdownCallback()
{
	callNoneArgMethod(s_PluginsShutdownCallback);
}

extern "C"
JNIEXPORT void JNICALL
Java_com_evgenypereguda_omegared_PCSX2_PCSX2LibNative_SetPluginsShutdownCallback(JNIEnv *env,
																				 jobject instance,
																				 jstring aMethod) {

	s_PluginsShutdownCallback = Java_To_Str(env, aMethod);

	::setPluginsShutdownCallback(
		SetPluginsShutdownCallback
	);
}

CALLBACK void SetPluginsOpenCallback()
{
	callNoneArgMethod(s_PluginsOpenCallback);
}

extern "C"
JNIEXPORT void JNICALL
Java_com_evgenypereguda_omegared_PCSX2_PCSX2LibNative_SetPluginsOpenCallback(JNIEnv *env,
																			 jobject instance,
																			 jstring aMethod) {

	s_PluginsOpenCallback = Java_To_Str(env, aMethod);

	::setPluginsOpenCallback(
		SetPluginsOpenCallback
	);
}


CALLBACK bool SetPluginsAreLoadedCallback()
{
	return callBooleanNoneArgMethod(s_PluginsAreLoadedCallback);
}

extern "C"
JNIEXPORT void JNICALL
Java_com_evgenypereguda_omegared_PCSX2_PCSX2LibNative_SetPluginsAreLoadedCallback(JNIEnv *env,
																				  jobject instance,
																				  jstring aMethod) {

	s_PluginsAreLoadedCallback = Java_To_Str(env, aMethod);

	::setPluginsAreLoadedCallback(
		SetPluginsAreLoadedCallback
	);
}



CALLBACK void SetUI_EnableSysActionsCallback()
{
	callNoneArgMethod(s_UIEnableSysActionsCallback);
}

extern "C"
JNIEXPORT void JNICALL
Java_com_evgenypereguda_omegared_PCSX2_PCSX2LibNative_SetUIEnableSysActionsCallback(JNIEnv *env,
																					jobject instance,
																					jstring aMethod) {

	s_UIEnableSysActionsCallback = Java_To_Str(env, aMethod);

	::setUI_EnableSysActionsCallback(
		SetUI_EnableSysActionsCallback
	);
}


CALLBACK void SetLoadAllPatchesAndStuffCallback(unsigned int arg1)
{
	callOneUInteArgMethod(s_LoadAllPatchesAndStuffCallback, arg1);
}

extern "C"
JNIEXPORT void JNICALL
Java_com_evgenypereguda_omegared_PCSX2_PCSX2LibNative_SetLoadAllPatchesAndStuffCallback(JNIEnv *env,
																						jobject instance,
																						jstring aMethod) {

	s_LoadAllPatchesAndStuffCallback = Java_To_Str(env, aMethod);

	::setLoadAllPatchesAndStuffCallback(
		SetLoadAllPatchesAndStuffCallback
	);
}


CALLBACK void SetLoadBIOSCallbackCallback(unsigned char* arg1, int arg2)
{
	callOnePtrOneUInteArgMethod(s_LoadBIOSCallbackCallback, arg1, arg2);
}

extern "C"
JNIEXPORT void JNICALL
Java_com_evgenypereguda_omegared_PCSX2_PCSX2LibNative_SetLoadBIOSCallbackCallback(JNIEnv *env,
																				  jobject instance,
																				  jstring aMethod) {

	s_LoadBIOSCallbackCallback = Java_To_Str(env, aMethod);

	::setLoadBIOSCallbackCallback(
		SetLoadBIOSCallbackCallback
	);
}




CALLBACK void SetCDVDNVMCallback(unsigned char* arg1, int arg2, int arg3, bool arg4)
{
	callOnePtrTwoIntBoolean(s_CDVDNVMCallback, arg1, arg2, arg3, arg4);
}

extern "C"
JNIEXPORT void JNICALL
Java_com_evgenypereguda_omegared_PCSX2_PCSX2LibNative_SetCDVDNVMCallback(JNIEnv *env,
																		 jobject instance,
																		 jstring aMethod) {

	s_CDVDNVMCallback = Java_To_Str(env, aMethod);

	::setCDVDNVMCallback(
		SetCDVDNVMCallback
	);
}


CALLBACK void SetCDVDGetMechaVerCallback(unsigned char* arg1)
{
	callOnePtrTwoIntBoolean(s_CDVDGetMechaVerCallback, arg1, 4);
}

extern "C"
JNIEXPORT void JNICALL
Java_com_evgenypereguda_omegared_PCSX2_PCSX2LibNative_SetCDVDGetMechaVerCallback(JNIEnv *env,
																				 jobject instance,
																				 jstring aMethod) {

	s_CDVDGetMechaVerCallback = Java_To_Str(env, aMethod);

	::setCDVDGetMechaVerCallback(
		SetCDVDGetMechaVerCallback
	);
}


CALLBACK int SetDoFreezeCallback(void* arg1, int arg2, int arg3)
{
	return callStaticThreeInteArgMethod(s_DoFreezeCallback, (long)arg1, arg2, arg3);
}


extern "C"
JNIEXPORT void JNICALL
Java_com_evgenypereguda_omegared_PCSX2_PCSX2LibNative_SetDoFreezeCallback(JNIEnv *env,
																		  jobject instance,
																		  jstring aMethod) {

	s_DoFreezeCallback = Java_To_Str(env, aMethod);

	::setDoFreezeCallback(
		SetDoFreezeCallback
	);
}


extern "C"
JNIEXPORT void JNICALL
Java_com_evgenypereguda_omegared_PCSX2_PCSX2LibNative_SysThreadBaseResumeFunc(JNIEnv *env,
																			  jobject instance) {

	SysThreadBase_ResumeFunc();
}



extern "C"
JNIEXPORT void JNICALL
Java_com_evgenypereguda_omegared_PCSX2_PCSX2LibNative_NativeMTGSCancelFunc(JNIEnv *env,
																		   jobject instance) {

	MTGS_CancelFunc();
}



extern "C"
JNIEXPORT void JNICALL
Java_com_evgenypereguda_omegared_PCSX2_PCSX2LibNative_NativeMTGSResumeFunc(JNIEnv *env,
																		   jobject instance) {
	MTGS_ResumeFunc();
}



extern "C"
JNIEXPORT void JNICALL
Java_com_evgenypereguda_omegared_PCSX2_PCSX2LibNative_NativeOpenPluginSPU2Func(JNIEnv *env,
																			   jobject instance) {

	openPlugin_SPU2Func();

}



extern "C"
JNIEXPORT void JNICALL
Java_com_evgenypereguda_omegared_PCSX2_PCSX2LibNative_NativeOpenPluginDEV9Func(JNIEnv *env,
																			   jobject instance) {


	openPlugin_DEV9Func();

}


extern "C"
JNIEXPORT void JNICALL
Java_com_evgenypereguda_omegared_PCSX2_PCSX2LibNative_NativeOpenPluginUSBFunc(JNIEnv *env,
																			  jobject instance) {

	openPlugin_USBFunc();
}


extern "C"
JNIEXPORT void JNICALL
Java_com_evgenypereguda_omegared_PCSX2_PCSX2LibNative_NativeOpenPluginFWFunc(JNIEnv *env,
																			 jobject instance) {
	openPlugin_FWFunc();
}



extern "C"
JNIEXPORT void JNICALL
Java_com_evgenypereguda_omegared_PCSX2_PCSX2LibNative_NativeMTGSWaitForOpenFunc(JNIEnv *env,
																				jobject instance) {

	MTGS_WaitForOpenFunc();

}



extern "C"
JNIEXPORT void JNICALL
Java_com_evgenypereguda_omegared_PCSX2_PCSX2LibNative_NativeApplySettingsFunc(JNIEnv *env,
																			  jobject instance,
																			  jstring aXmlPcsx2Config) {
	auto l_XmlPcsx2Config = Java_To_WStr(env, aXmlPcsx2Config);

	ApplySettingsFunc(l_XmlPcsx2Config.data());
}




extern "C"
JNIEXPORT void JNICALL
Java_com_evgenypereguda_omegared_PCSX2_PCSX2LibNative_NativeVTLBAllocPpmapFinc(JNIEnv *env,
																			   jobject instance) {

	VTLB_Alloc_PpmapFinc();
}

extern void Test();

extern "C"
JNIEXPORT void JNICALL
Java_com_evgenypereguda_omegared_PCSX2_PCSX2LibNative_CPUTestFinc(JNIEnv *env,
jobject instance) {
	Test();
}


