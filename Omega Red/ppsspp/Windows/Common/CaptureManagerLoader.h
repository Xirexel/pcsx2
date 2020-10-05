#pragma once
#include <Unknwnbase.h>

class CaptureManagerLoader
{

	typedef HRESULT(STDAPICALLTYPE *PDllGetClassObject) (REFCLSID, REFIID, void**);

	HMODULE mHCaptureManagerLibrary = nullptr;

	PDllGetClassObject mPtrFuncDllGetClassObject = nullptr;

public:

	static CaptureManagerLoader& getInstance()
	{
		static CaptureManagerLoader lInstance;

		return lInstance;
	}

	HRESULT createCalssFactory(const IID& lIDD, IClassFactory** aPtrPtrIClassFactory)
	{
		HRESULT lhresult(E_FAIL);

		do
		{
			mHCaptureManagerLibrary = LoadLibraryEx(L"CaptureManager.dll", NULL, 0);

			if (mHCaptureManagerLibrary == nullptr)
			{
				_tprintf(TEXT("Library CaptureManager.dll is not found\n"));

				return -1;
			}

			if (mHCaptureManagerLibrary == nullptr)
			{
				lhresult = CoGetClassObject(lIDD, CLSCTX_INPROC_SERVER, nullptr, IID_PPV_ARGS(aPtrPtrIClassFactory));

				if (SUCCEEDED(lhresult))
					break;
			}



			if (mPtrFuncDllGetClassObject == nullptr)
			{
				mPtrFuncDllGetClassObject = (PDllGetClassObject)GetProcAddress(mHCaptureManagerLibrary, "DllGetClassObject");

				if (mPtrFuncDllGetClassObject == nullptr)
				{
					_tprintf(TEXT("Function DllGetClassObject is not found\n"));

					return -1;
				}

			}

			lhresult = mPtrFuncDllGetClassObject(lIDD, IID_PPV_ARGS(aPtrPtrIClassFactory));
			
		} while (false);

		return lhresult;
	}

private:

	CaptureManagerLoader()
	{

	}

	~CaptureManagerLoader()
	{

	}

	CaptureManagerLoader(const CaptureManagerLoader&) = delete;
	CaptureManagerLoader(CaptureManagerLoader&) = delete;

};
