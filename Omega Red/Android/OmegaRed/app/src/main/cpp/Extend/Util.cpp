//
// Created by Xirexel on 7/10/2019.
//

#include <jni.h>
#include <android/log.h>
#include <string>
#include <vector>

std::wstring Java_To_WStr(JNIEnv *env, jstring string)
{
	std::wstring value;

	const jchar *raw = env->GetStringChars(string, 0);
	jsize len = env->GetStringLength(string);

	value.assign(raw, raw + len);

	env->ReleaseStringChars(string, raw);

	return value;
}

std::string Java_To_Str(JNIEnv *env, jstring string)
{
	std::string value;

	const char *raw = env->GetStringUTFChars(string, 0);

	value = std::string(raw);

	env->ReleaseStringUTFChars(string, raw);

	return value;
}

#define APPNAME "MyApp"

std::vector<std::string> splitMethod(JNIEnv *env, jstring string)
{
	std::vector<std::string> l_split;

	auto l_signature = Java_To_Str(env, string);

	auto l_find = l_signature.find('|');

	while (l_find != std::string::npos)
	{
		l_split.push_back(l_signature.substr(0, l_find));

		l_signature = l_signature.substr(l_find + 1);

		l_find = l_signature.find('|');
	}

	l_split.push_back(l_signature);

	return l_split;
}
