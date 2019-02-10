#pragma once

#include <Unknwnbase.h>
#include <atomic>


template <typename... Interfaces>
struct BaseUnknown:
	public Interfaces...
{

public:

	BaseUnknown() :
		mRefCount(1){}

	// IUnknown implementation

	STDMETHODIMP QueryInterface(
		REFIID aRefIID,
		void** aPtrPtrVoidObject)
	{
		HRESULT lresult = S_OK;

		do
		{
			if (aPtrPtrVoidObject == NULL)
			{
				lresult = E_POINTER;

				break;
			}

			bool lboolResult = findInterface(aRefIID, aPtrPtrVoidObject);

			if (lboolResult)
			{
				AddRef();
			}
			else
			{
				lboolResult = findIncapsulatedInterface(aRefIID, aPtrPtrVoidObject);
			}

			if (!lboolResult)
			{
				*aPtrPtrVoidObject = NULL;

				lresult = E_NOINTERFACE;
			}

		} while (false);

		return lresult;
	}
			
	STDMETHODIMP_(ULONG) AddRef()
	{
		return ++mRefCount;
	}

	STDMETHODIMP_(ULONG) Release()
	{
		auto lCount = --mRefCount;

		if (lCount == 0)
		{
			delete this;
		}

		return lCount;
	}
		
protected:

	virtual ~BaseUnknown(){}


	virtual bool findIncapsulatedInterface(
		REFIID aRefIID,
		void** aPtrPtrVoidObject)
	{
		return false;
	}

	virtual bool findInterface(
		REFIID aRefIID,
		void** aPtrPtrVoidObject)
	{
		if (aRefIID == IID_IUnknown)
		{
			return castToIUnknow(
				aPtrPtrVoidObject,
				static_cast<Interfaces*>(this)...);
		}
		else
		{
			return castInterfaces(
				aRefIID,
				aPtrPtrVoidObject,
				static_cast<Interfaces*>(this)...);
		}
	}



	template<typename Interface>
	bool castInterfaces(
		REFIID aRefIID,
		void** aPtrPtrVoidObject,
		Interface* aThis)
	{
		bool lresult = aRefIID == __uuidof(Interface);

		if (lresult)
		{
			*aPtrPtrVoidObject = aThis;

			if (*aPtrPtrVoidObject == nullptr)
				lresult = false;
		}

		return lresult;
	}

private:
	std::atomic<ULONG> mRefCount;


				
	template<typename Interface, typename... Args>
	bool castToIUnknow(
		void** aPtrPtrVoidObject,
		Interface* aThis,
		Args... aRest)
	{
		return castToIUnknow(aPtrPtrVoidObject, aThis);
	}

	template<typename Interface>
	bool castToIUnknow(
		void** aPtrPtrVoidObject,
		Interface* aThis)
	{
		bool lresult = false;

		*aPtrPtrVoidObject = static_cast<IUnknown*>(aThis);

		if (*aPtrPtrVoidObject != nullptr)
			lresult = true;

		return lresult;
	}

	template<typename Interface, typename... Args>
	bool castInterfaces(
		REFIID aRefIID,
		void** aPtrPtrVoidObject,
		Interface* aThis,
		Args... aRest)
	{
		bool lresult = castInterfaces(aRefIID, aPtrPtrVoidObject, aThis);

		if (!lresult)
		{
			lresult = castInterfaces(
				aRefIID,
				aPtrPtrVoidObject,
				aRest...);
		}

		return lresult;
	}

};
