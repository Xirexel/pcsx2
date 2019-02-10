

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 8.00.0603 */
/* at Mon Dec 10 15:26:27 2018
 */
/* Compiler settings for CaptureManagerTypeInfo.idl:
    Oicf, W1, Zp8, env=Win32 (32b run), target_arch=X86 8.00.0603 
    protocol : dce , ms_ext, c_ext, robust
    error checks: allocation ref bounds_check enum stub_data 
    VC __declspec() decoration level: 
         __declspec(uuid()), __declspec(selectany), __declspec(novtable)
         DECLSPEC_UUID(), MIDL_INTERFACE()
*/
/* @@MIDL_FILE_HEADING(  ) */

#pragma warning( disable: 4049 )  /* more than 64k source lines */


/* verify that the <rpcndr.h> version is high enough to compile this file*/
#ifndef __REQUIRED_RPCNDR_H_VERSION__
#define __REQUIRED_RPCNDR_H_VERSION__ 475
#endif

#include "rpc.h"
#include "rpcndr.h"

#ifndef __RPCNDR_H_VERSION__
#error this stub requires an updated version of <rpcndr.h>
#endif // __RPCNDR_H_VERSION__

#ifndef COM_NO_WINDOWS_H
#include "windows.h"
#include "ole2.h"
#endif /*COM_NO_WINDOWS_H*/

#ifndef __CaptureManagerTypeInfo_h__
#define __CaptureManagerTypeInfo_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __ILogPrintOutControl_FWD_DEFINED__
#define __ILogPrintOutControl_FWD_DEFINED__
typedef interface ILogPrintOutControl ILogPrintOutControl;

#endif 	/* __ILogPrintOutControl_FWD_DEFINED__ */


#ifndef __ISourceControl_FWD_DEFINED__
#define __ISourceControl_FWD_DEFINED__
typedef interface ISourceControl ISourceControl;

#endif 	/* __ISourceControl_FWD_DEFINED__ */


#ifndef __ISinkControl_FWD_DEFINED__
#define __ISinkControl_FWD_DEFINED__
typedef interface ISinkControl ISinkControl;

#endif 	/* __ISinkControl_FWD_DEFINED__ */


#ifndef __IFileSinkFactory_FWD_DEFINED__
#define __IFileSinkFactory_FWD_DEFINED__
typedef interface IFileSinkFactory IFileSinkFactory;

#endif 	/* __IFileSinkFactory_FWD_DEFINED__ */


#ifndef __ISampleGrabberCallSinkFactory_FWD_DEFINED__
#define __ISampleGrabberCallSinkFactory_FWD_DEFINED__
typedef interface ISampleGrabberCallSinkFactory ISampleGrabberCallSinkFactory;

#endif 	/* __ISampleGrabberCallSinkFactory_FWD_DEFINED__ */


#ifndef __ISampleGrabberCall_FWD_DEFINED__
#define __ISampleGrabberCall_FWD_DEFINED__
typedef interface ISampleGrabberCall ISampleGrabberCall;

#endif 	/* __ISampleGrabberCall_FWD_DEFINED__ */


#ifndef __ISampleGrabberCallbackSinkFactory_FWD_DEFINED__
#define __ISampleGrabberCallbackSinkFactory_FWD_DEFINED__
typedef interface ISampleGrabberCallbackSinkFactory ISampleGrabberCallbackSinkFactory;

#endif 	/* __ISampleGrabberCallbackSinkFactory_FWD_DEFINED__ */


#ifndef __ISampleGrabberCallback_FWD_DEFINED__
#define __ISampleGrabberCallback_FWD_DEFINED__
typedef interface ISampleGrabberCallback ISampleGrabberCallback;

#endif 	/* __ISampleGrabberCallback_FWD_DEFINED__ */


#ifndef __IEVRSinkFactory_FWD_DEFINED__
#define __IEVRSinkFactory_FWD_DEFINED__
typedef interface IEVRSinkFactory IEVRSinkFactory;

#endif 	/* __IEVRSinkFactory_FWD_DEFINED__ */


#ifndef __IEVRMultiSinkFactory_FWD_DEFINED__
#define __IEVRMultiSinkFactory_FWD_DEFINED__
typedef interface IEVRMultiSinkFactory IEVRMultiSinkFactory;

#endif 	/* __IEVRMultiSinkFactory_FWD_DEFINED__ */


#ifndef __IMediaTypeParser_FWD_DEFINED__
#define __IMediaTypeParser_FWD_DEFINED__
typedef interface IMediaTypeParser IMediaTypeParser;

#endif 	/* __IMediaTypeParser_FWD_DEFINED__ */


#ifndef __IStrideForBitmap_FWD_DEFINED__
#define __IStrideForBitmap_FWD_DEFINED__
typedef interface IStrideForBitmap IStrideForBitmap;

#endif 	/* __IStrideForBitmap_FWD_DEFINED__ */


#ifndef __IStreamControl_FWD_DEFINED__
#define __IStreamControl_FWD_DEFINED__
typedef interface IStreamControl IStreamControl;

#endif 	/* __IStreamControl_FWD_DEFINED__ */


#ifndef __ISpreaderNodeFactory_FWD_DEFINED__
#define __ISpreaderNodeFactory_FWD_DEFINED__
typedef interface ISpreaderNodeFactory ISpreaderNodeFactory;

#endif 	/* __ISpreaderNodeFactory_FWD_DEFINED__ */


#ifndef __ISwitcherNodeFactory_FWD_DEFINED__
#define __ISwitcherNodeFactory_FWD_DEFINED__
typedef interface ISwitcherNodeFactory ISwitcherNodeFactory;

#endif 	/* __ISwitcherNodeFactory_FWD_DEFINED__ */


#ifndef __IEncoderControl_FWD_DEFINED__
#define __IEncoderControl_FWD_DEFINED__
typedef interface IEncoderControl IEncoderControl;

#endif 	/* __IEncoderControl_FWD_DEFINED__ */


#ifndef __IEncoderNodeFactory_FWD_DEFINED__
#define __IEncoderNodeFactory_FWD_DEFINED__
typedef interface IEncoderNodeFactory IEncoderNodeFactory;

#endif 	/* __IEncoderNodeFactory_FWD_DEFINED__ */


#ifndef __IWebCamControl_FWD_DEFINED__
#define __IWebCamControl_FWD_DEFINED__
typedef interface IWebCamControl IWebCamControl;

#endif 	/* __IWebCamControl_FWD_DEFINED__ */


#ifndef __IByteStreamSinkFactory_FWD_DEFINED__
#define __IByteStreamSinkFactory_FWD_DEFINED__
typedef interface IByteStreamSinkFactory IByteStreamSinkFactory;

#endif 	/* __IByteStreamSinkFactory_FWD_DEFINED__ */


#ifndef __ISessionControl_FWD_DEFINED__
#define __ISessionControl_FWD_DEFINED__
typedef interface ISessionControl ISessionControl;

#endif 	/* __ISessionControl_FWD_DEFINED__ */


#ifndef __ISession_FWD_DEFINED__
#define __ISession_FWD_DEFINED__
typedef interface ISession ISession;

#endif 	/* __ISession_FWD_DEFINED__ */


#ifndef __ISessionCallback_FWD_DEFINED__
#define __ISessionCallback_FWD_DEFINED__
typedef interface ISessionCallback ISessionCallback;

#endif 	/* __ISessionCallback_FWD_DEFINED__ */


#ifndef __ICaptureManagerControl_FWD_DEFINED__
#define __ICaptureManagerControl_FWD_DEFINED__
typedef interface ICaptureManagerControl ICaptureManagerControl;

#endif 	/* __ICaptureManagerControl_FWD_DEFINED__ */


#ifndef __IVersionControl_FWD_DEFINED__
#define __IVersionControl_FWD_DEFINED__
typedef interface IVersionControl IVersionControl;

#endif 	/* __IVersionControl_FWD_DEFINED__ */


#ifndef __IEVRStreamControl_FWD_DEFINED__
#define __IEVRStreamControl_FWD_DEFINED__
typedef interface IEVRStreamControl IEVRStreamControl;

#endif 	/* __IEVRStreamControl_FWD_DEFINED__ */


#ifndef __IInitilaizeCaptureSource_FWD_DEFINED__
#define __IInitilaizeCaptureSource_FWD_DEFINED__
typedef interface IInitilaizeCaptureSource IInitilaizeCaptureSource;

#endif 	/* __IInitilaizeCaptureSource_FWD_DEFINED__ */


#ifndef __ICurrentMediaType_FWD_DEFINED__
#define __ICurrentMediaType_FWD_DEFINED__
typedef interface ICurrentMediaType ICurrentMediaType;

#endif 	/* __ICurrentMediaType_FWD_DEFINED__ */


#ifndef __ISourceRequestResult_FWD_DEFINED__
#define __ISourceRequestResult_FWD_DEFINED__
typedef interface ISourceRequestResult ISourceRequestResult;

#endif 	/* __ISourceRequestResult_FWD_DEFINED__ */


#ifndef __ICaptureProcessor_FWD_DEFINED__
#define __ICaptureProcessor_FWD_DEFINED__
typedef interface ICaptureProcessor ICaptureProcessor;

#endif 	/* __ICaptureProcessor_FWD_DEFINED__ */


#ifndef __IRenderingControl_FWD_DEFINED__
#define __IRenderingControl_FWD_DEFINED__
typedef interface IRenderingControl IRenderingControl;

#endif 	/* __IRenderingControl_FWD_DEFINED__ */


#ifndef __ISwitcherControl_FWD_DEFINED__
#define __ISwitcherControl_FWD_DEFINED__
typedef interface ISwitcherControl ISwitcherControl;

#endif 	/* __ISwitcherControl_FWD_DEFINED__ */


#ifndef __CoLogPrintOut_FWD_DEFINED__
#define __CoLogPrintOut_FWD_DEFINED__

#ifdef __cplusplus
typedef class CoLogPrintOut CoLogPrintOut;
#else
typedef struct CoLogPrintOut CoLogPrintOut;
#endif /* __cplusplus */

#endif 	/* __CoLogPrintOut_FWD_DEFINED__ */


#ifndef __CoCaptureManager_FWD_DEFINED__
#define __CoCaptureManager_FWD_DEFINED__

#ifdef __cplusplus
typedef class CoCaptureManager CoCaptureManager;
#else
typedef struct CoCaptureManager CoCaptureManager;
#endif /* __cplusplus */

#endif 	/* __CoCaptureManager_FWD_DEFINED__ */


#ifndef __ISourceControl_FWD_DEFINED__
#define __ISourceControl_FWD_DEFINED__
typedef interface ISourceControl ISourceControl;

#endif 	/* __ISourceControl_FWD_DEFINED__ */


#ifndef __ISinkControl_FWD_DEFINED__
#define __ISinkControl_FWD_DEFINED__
typedef interface ISinkControl ISinkControl;

#endif 	/* __ISinkControl_FWD_DEFINED__ */


#ifndef __IFileSinkFactory_FWD_DEFINED__
#define __IFileSinkFactory_FWD_DEFINED__
typedef interface IFileSinkFactory IFileSinkFactory;

#endif 	/* __IFileSinkFactory_FWD_DEFINED__ */


#ifndef __ISessionControl_FWD_DEFINED__
#define __ISessionControl_FWD_DEFINED__
typedef interface ISessionControl ISessionControl;

#endif 	/* __ISessionControl_FWD_DEFINED__ */


#ifndef __ISession_FWD_DEFINED__
#define __ISession_FWD_DEFINED__
typedef interface ISession ISession;

#endif 	/* __ISession_FWD_DEFINED__ */


#ifndef __ISessionCallback_FWD_DEFINED__
#define __ISessionCallback_FWD_DEFINED__
typedef interface ISessionCallback ISessionCallback;

#endif 	/* __ISessionCallback_FWD_DEFINED__ */


#ifndef __ISampleGrabberCallSinkFactory_FWD_DEFINED__
#define __ISampleGrabberCallSinkFactory_FWD_DEFINED__
typedef interface ISampleGrabberCallSinkFactory ISampleGrabberCallSinkFactory;

#endif 	/* __ISampleGrabberCallSinkFactory_FWD_DEFINED__ */


#ifndef __ISampleGrabberCall_FWD_DEFINED__
#define __ISampleGrabberCall_FWD_DEFINED__
typedef interface ISampleGrabberCall ISampleGrabberCall;

#endif 	/* __ISampleGrabberCall_FWD_DEFINED__ */


#ifndef __IMediaTypeParser_FWD_DEFINED__
#define __IMediaTypeParser_FWD_DEFINED__
typedef interface IMediaTypeParser IMediaTypeParser;

#endif 	/* __IMediaTypeParser_FWD_DEFINED__ */


#ifndef __IStrideForBitmap_FWD_DEFINED__
#define __IStrideForBitmap_FWD_DEFINED__
typedef interface IStrideForBitmap IStrideForBitmap;

#endif 	/* __IStrideForBitmap_FWD_DEFINED__ */


#ifndef __ISampleGrabberCallbackSinkFactory_FWD_DEFINED__
#define __ISampleGrabberCallbackSinkFactory_FWD_DEFINED__
typedef interface ISampleGrabberCallbackSinkFactory ISampleGrabberCallbackSinkFactory;

#endif 	/* __ISampleGrabberCallbackSinkFactory_FWD_DEFINED__ */


#ifndef __ISampleGrabberCallback_FWD_DEFINED__
#define __ISampleGrabberCallback_FWD_DEFINED__
typedef interface ISampleGrabberCallback ISampleGrabberCallback;

#endif 	/* __ISampleGrabberCallback_FWD_DEFINED__ */


#ifndef __IEVRSinkFactory_FWD_DEFINED__
#define __IEVRSinkFactory_FWD_DEFINED__
typedef interface IEVRSinkFactory IEVRSinkFactory;

#endif 	/* __IEVRSinkFactory_FWD_DEFINED__ */


#ifndef __IStreamControl_FWD_DEFINED__
#define __IStreamControl_FWD_DEFINED__
typedef interface IStreamControl IStreamControl;

#endif 	/* __IStreamControl_FWD_DEFINED__ */


#ifndef __ISpreaderNodeFactory_FWD_DEFINED__
#define __ISpreaderNodeFactory_FWD_DEFINED__
typedef interface ISpreaderNodeFactory ISpreaderNodeFactory;

#endif 	/* __ISpreaderNodeFactory_FWD_DEFINED__ */


#ifndef __ISwitcherNodeFactory_FWD_DEFINED__
#define __ISwitcherNodeFactory_FWD_DEFINED__
typedef interface ISwitcherNodeFactory ISwitcherNodeFactory;

#endif 	/* __ISwitcherNodeFactory_FWD_DEFINED__ */


#ifndef __IEncoderControl_FWD_DEFINED__
#define __IEncoderControl_FWD_DEFINED__
typedef interface IEncoderControl IEncoderControl;

#endif 	/* __IEncoderControl_FWD_DEFINED__ */


#ifndef __IEncoderNodeFactory_FWD_DEFINED__
#define __IEncoderNodeFactory_FWD_DEFINED__
typedef interface IEncoderNodeFactory IEncoderNodeFactory;

#endif 	/* __IEncoderNodeFactory_FWD_DEFINED__ */


#ifndef __IWebCamControl_FWD_DEFINED__
#define __IWebCamControl_FWD_DEFINED__
typedef interface IWebCamControl IWebCamControl;

#endif 	/* __IWebCamControl_FWD_DEFINED__ */


#ifndef __IByteStreamSinkFactory_FWD_DEFINED__
#define __IByteStreamSinkFactory_FWD_DEFINED__
typedef interface IByteStreamSinkFactory IByteStreamSinkFactory;

#endif 	/* __IByteStreamSinkFactory_FWD_DEFINED__ */


#ifndef __IVersionControl_FWD_DEFINED__
#define __IVersionControl_FWD_DEFINED__
typedef interface IVersionControl IVersionControl;

#endif 	/* __IVersionControl_FWD_DEFINED__ */


#ifndef __IEVRStreamControl_FWD_DEFINED__
#define __IEVRStreamControl_FWD_DEFINED__
typedef interface IEVRStreamControl IEVRStreamControl;

#endif 	/* __IEVRStreamControl_FWD_DEFINED__ */


#ifndef __IEVRMultiSinkFactory_FWD_DEFINED__
#define __IEVRMultiSinkFactory_FWD_DEFINED__
typedef interface IEVRMultiSinkFactory IEVRMultiSinkFactory;

#endif 	/* __IEVRMultiSinkFactory_FWD_DEFINED__ */


#ifndef __ICaptureProcessor_FWD_DEFINED__
#define __ICaptureProcessor_FWD_DEFINED__
typedef interface ICaptureProcessor ICaptureProcessor;

#endif 	/* __ICaptureProcessor_FWD_DEFINED__ */


#ifndef __IInitilaizeCaptureSource_FWD_DEFINED__
#define __IInitilaizeCaptureSource_FWD_DEFINED__
typedef interface IInitilaizeCaptureSource IInitilaizeCaptureSource;

#endif 	/* __IInitilaizeCaptureSource_FWD_DEFINED__ */


#ifndef __ISourceRequestResult_FWD_DEFINED__
#define __ISourceRequestResult_FWD_DEFINED__
typedef interface ISourceRequestResult ISourceRequestResult;

#endif 	/* __ISourceRequestResult_FWD_DEFINED__ */


#ifndef __ICurrentMediaType_FWD_DEFINED__
#define __ICurrentMediaType_FWD_DEFINED__
typedef interface ICurrentMediaType ICurrentMediaType;

#endif 	/* __ICurrentMediaType_FWD_DEFINED__ */


#ifndef __IRenderingControl_FWD_DEFINED__
#define __IRenderingControl_FWD_DEFINED__
typedef interface IRenderingControl IRenderingControl;

#endif 	/* __IRenderingControl_FWD_DEFINED__ */


#ifndef __ISwitcherControl_FWD_DEFINED__
#define __ISwitcherControl_FWD_DEFINED__
typedef interface ISwitcherControl ISwitcherControl;

#endif 	/* __ISwitcherControl_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_CaptureManagerTypeInfo_0000_0000 */
/* [local] */ 

typedef /* [helpstring][v1_enum][uuid] */  DECLSPEC_UUID("5CA95537-3733-441D-B9F4-1F38CCFC56D3") 
enum LogLevel
    {
        INFO_LEVEL	= 0,
        ERROR_LEVEL	= ( INFO_LEVEL + 1 ) 
    } 	LogLevel;

typedef /* [helpstring][v1_enum][uuid] */  DECLSPEC_UUID("A492B132-CF20-4562-BAC4-290EB4D0ADA4") 
enum WebCamParametrFlag
    {
        Auto	= 1,
        Manual	= 2
    } 	WebCamParametrFlag;

typedef /* [helpstring][v1_enum][uuid] */  DECLSPEC_UUID("437C2465-72D9-436E-9B51-327EF76DB4A5") 
enum WebCamParametr
    {
        BRIGHTNESS	= 0,
        CONTRAST	= ( BRIGHTNESS + 1 ) ,
        HUE	= ( CONTRAST + 1 ) ,
        SATURATION	= ( HUE + 1 ) ,
        SHARPNESS	= ( SATURATION + 1 ) ,
        GAMMA	= ( SHARPNESS + 1 ) ,
        COLORENABLE	= ( GAMMA + 1 ) ,
        WHITRBALANCE	= ( COLORENABLE + 1 ) ,
        BACKLIGHTCOMPENSATION	= ( WHITRBALANCE + 1 ) ,
        GAIN	= ( BACKLIGHTCOMPENSATION + 1 ) ,
        PAN	= ( GAIN + 1 ) ,
        TILT	= ( PAN + 1 ) ,
        ROLL	= ( TILT + 1 ) ,
        ZOOM	= ( ROLL + 1 ) ,
        EXPOSURE	= ( ZOOM + 1 ) ,
        IRIS	= ( EXPOSURE + 1 ) ,
        FOCUS	= ( IRIS + 1 ) 
    } 	WebCamParametr;

typedef /* [helpstring][v1_enum][uuid] */  DECLSPEC_UUID("DFAE427C-9493-4022-8FD2-D0280B9A422C") 
enum SessionCallbackEventCode
    {
        UnknownEvent	= 0,
        Error	= ( UnknownEvent + 1 ) ,
        Status_Error	= ( Error + 1 ) ,
        Execution_Error	= ( Status_Error + 1 ) ,
        ItIsReadyToStart	= ( Execution_Error + 1 ) ,
        ItIsStarted	= ( ItIsReadyToStart + 1 ) ,
        ItIsPaused	= ( ItIsStarted + 1 ) ,
        ItIsStopped	= ( ItIsPaused + 1 ) ,
        ItIsEnded	= ( ItIsStopped + 1 ) ,
        ItIsClosed	= ( ItIsEnded + 1 ) ,
        VideoCaptureDeviceRemoved	= ( ItIsClosed + 1 ) 
    } 	SessionCallbackEventCode;



extern RPC_IF_HANDLE __MIDL_itf_CaptureManagerTypeInfo_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_CaptureManagerTypeInfo_0000_0000_v0_0_s_ifspec;

#ifndef __ILogPrintOutControl_INTERFACE_DEFINED__
#define __ILogPrintOutControl_INTERFACE_DEFINED__

/* interface ILogPrintOutControl */
/* [dual][helpstring][oleautomation][uuid][object] */ 


EXTERN_C const IID IID_ILogPrintOutControl;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("73B67834-E7BD-40B7-9730-8C13BF098B9F")
    ILogPrintOutControl : public IDispatch
    {
    public:
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE setVerbose( 
            /* [in] */ DWORD aLevelType,
            /* [in] */ BSTR aFilePath,
            /* [in] */ boolean aState) = 0;
        
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE addPrintOutDestination( 
            /* [in] */ DWORD aLevelType,
            /* [in] */ BSTR aFilePath) = 0;
        
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE removePrintOutDestination( 
            /* [in] */ DWORD aLevelType,
            /* [in] */ BSTR aFilePath) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct ILogPrintOutControlVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ILogPrintOutControl * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ILogPrintOutControl * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ILogPrintOutControl * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            ILogPrintOutControl * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            ILogPrintOutControl * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            ILogPrintOutControl * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            ILogPrintOutControl * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *setVerbose )( 
            ILogPrintOutControl * This,
            /* [in] */ DWORD aLevelType,
            /* [in] */ BSTR aFilePath,
            /* [in] */ boolean aState);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *addPrintOutDestination )( 
            ILogPrintOutControl * This,
            /* [in] */ DWORD aLevelType,
            /* [in] */ BSTR aFilePath);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *removePrintOutDestination )( 
            ILogPrintOutControl * This,
            /* [in] */ DWORD aLevelType,
            /* [in] */ BSTR aFilePath);
        
        END_INTERFACE
    } ILogPrintOutControlVtbl;

    interface ILogPrintOutControl
    {
        CONST_VTBL struct ILogPrintOutControlVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ILogPrintOutControl_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ILogPrintOutControl_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ILogPrintOutControl_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ILogPrintOutControl_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define ILogPrintOutControl_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define ILogPrintOutControl_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define ILogPrintOutControl_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define ILogPrintOutControl_setVerbose(This,aLevelType,aFilePath,aState)	\
    ( (This)->lpVtbl -> setVerbose(This,aLevelType,aFilePath,aState) ) 

#define ILogPrintOutControl_addPrintOutDestination(This,aLevelType,aFilePath)	\
    ( (This)->lpVtbl -> addPrintOutDestination(This,aLevelType,aFilePath) ) 

#define ILogPrintOutControl_removePrintOutDestination(This,aLevelType,aFilePath)	\
    ( (This)->lpVtbl -> removePrintOutDestination(This,aLevelType,aFilePath) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ILogPrintOutControl_INTERFACE_DEFINED__ */


#ifndef __ISourceControl_INTERFACE_DEFINED__
#define __ISourceControl_INTERFACE_DEFINED__

/* interface ISourceControl */
/* [dual][helpstring][oleautomation][uuid][object] */ 


EXTERN_C const IID IID_ISourceControl;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("1276CC17-BCA8-4200-87BB-7180EF562447")
    ISourceControl : public IDispatch
    {
    public:
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE getCollectionOfSources( 
            /* [out][in] */ BSTR *aPtrPtrXMLstring) = 0;
        
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE getSourceOutputMediaType( 
            /* [in] */ BSTR aSymbolicLink,
            /* [in] */ DWORD aIndexStream,
            /* [in] */ DWORD aIndexMediaType,
            /* [out] */ IUnknown **aPtrPtrOutputMediaType) = 0;
        
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE createSourceNode( 
            /* [in] */ BSTR aSymbolicLink,
            /* [in] */ DWORD aIndexStream,
            /* [in] */ DWORD aIndexMediaType,
            /* [out] */ IUnknown **aPtrPtrTopologyNode) = 0;
        
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE createSourceNodeWithDownStreamConnection( 
            /* [in] */ BSTR aSymbolicLink,
            /* [in] */ DWORD aIndexStream,
            /* [in] */ DWORD aIndexMediaType,
            /* [in] */ IUnknown *aPtrDownStreamTopologyNode,
            /* [out] */ IUnknown **aPtrPtrTopologyNode) = 0;
        
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE createSource( 
            /* [in] */ BSTR aSymbolicLink,
            /* [out] */ IUnknown **aPtrPtrMediaSource) = 0;
        
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE createSourceFromCaptureProcessor( 
            /* [in] */ IUnknown *aPtrCaptureProcessor,
            /* [out] */ IUnknown **aPtrPtrMediaSource) = 0;
        
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE getSourceOutputMediaTypeFromMediaSource( 
            /* [in] */ IUnknown *aPtrMediaSource,
            /* [in] */ DWORD aIndexStream,
            /* [in] */ DWORD aIndexMediaType,
            /* [out] */ IUnknown **aPtrPtrOutputMediaType) = 0;
        
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE createSourceNodeFromExternalSource( 
            /* [in] */ IUnknown *aPtrMediaSource,
            /* [in] */ DWORD aIndexStream,
            /* [in] */ DWORD aIndexMediaType,
            /* [out] */ IUnknown **aPtrPtrTopologyNode) = 0;
        
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE createSourceNodeFromExternalSourceWithDownStreamConnection( 
            /* [in] */ IUnknown *aPtrMediaSource,
            /* [in] */ DWORD aIndexStream,
            /* [in] */ DWORD aIndexMediaType,
            /* [in] */ IUnknown *aPtrDownStreamTopologyNode,
            /* [out] */ IUnknown **aPtrPtrTopologyNode) = 0;
        
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE createSourceControl( 
            /* [in] */ BSTR aSymbolicLink,
            /* [in] */ REFIID aREFIID,
            /* [out] */ IUnknown **aPtrPtrControl) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct ISourceControlVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ISourceControl * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ISourceControl * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ISourceControl * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            ISourceControl * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            ISourceControl * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            ISourceControl * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            ISourceControl * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *getCollectionOfSources )( 
            ISourceControl * This,
            /* [out][in] */ BSTR *aPtrPtrXMLstring);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *getSourceOutputMediaType )( 
            ISourceControl * This,
            /* [in] */ BSTR aSymbolicLink,
            /* [in] */ DWORD aIndexStream,
            /* [in] */ DWORD aIndexMediaType,
            /* [out] */ IUnknown **aPtrPtrOutputMediaType);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *createSourceNode )( 
            ISourceControl * This,
            /* [in] */ BSTR aSymbolicLink,
            /* [in] */ DWORD aIndexStream,
            /* [in] */ DWORD aIndexMediaType,
            /* [out] */ IUnknown **aPtrPtrTopologyNode);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *createSourceNodeWithDownStreamConnection )( 
            ISourceControl * This,
            /* [in] */ BSTR aSymbolicLink,
            /* [in] */ DWORD aIndexStream,
            /* [in] */ DWORD aIndexMediaType,
            /* [in] */ IUnknown *aPtrDownStreamTopologyNode,
            /* [out] */ IUnknown **aPtrPtrTopologyNode);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *createSource )( 
            ISourceControl * This,
            /* [in] */ BSTR aSymbolicLink,
            /* [out] */ IUnknown **aPtrPtrMediaSource);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *createSourceFromCaptureProcessor )( 
            ISourceControl * This,
            /* [in] */ IUnknown *aPtrCaptureProcessor,
            /* [out] */ IUnknown **aPtrPtrMediaSource);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *getSourceOutputMediaTypeFromMediaSource )( 
            ISourceControl * This,
            /* [in] */ IUnknown *aPtrMediaSource,
            /* [in] */ DWORD aIndexStream,
            /* [in] */ DWORD aIndexMediaType,
            /* [out] */ IUnknown **aPtrPtrOutputMediaType);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *createSourceNodeFromExternalSource )( 
            ISourceControl * This,
            /* [in] */ IUnknown *aPtrMediaSource,
            /* [in] */ DWORD aIndexStream,
            /* [in] */ DWORD aIndexMediaType,
            /* [out] */ IUnknown **aPtrPtrTopologyNode);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *createSourceNodeFromExternalSourceWithDownStreamConnection )( 
            ISourceControl * This,
            /* [in] */ IUnknown *aPtrMediaSource,
            /* [in] */ DWORD aIndexStream,
            /* [in] */ DWORD aIndexMediaType,
            /* [in] */ IUnknown *aPtrDownStreamTopologyNode,
            /* [out] */ IUnknown **aPtrPtrTopologyNode);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *createSourceControl )( 
            ISourceControl * This,
            /* [in] */ BSTR aSymbolicLink,
            /* [in] */ REFIID aREFIID,
            /* [out] */ IUnknown **aPtrPtrControl);
        
        END_INTERFACE
    } ISourceControlVtbl;

    interface ISourceControl
    {
        CONST_VTBL struct ISourceControlVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ISourceControl_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ISourceControl_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ISourceControl_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ISourceControl_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define ISourceControl_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define ISourceControl_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define ISourceControl_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define ISourceControl_getCollectionOfSources(This,aPtrPtrXMLstring)	\
    ( (This)->lpVtbl -> getCollectionOfSources(This,aPtrPtrXMLstring) ) 

#define ISourceControl_getSourceOutputMediaType(This,aSymbolicLink,aIndexStream,aIndexMediaType,aPtrPtrOutputMediaType)	\
    ( (This)->lpVtbl -> getSourceOutputMediaType(This,aSymbolicLink,aIndexStream,aIndexMediaType,aPtrPtrOutputMediaType) ) 

#define ISourceControl_createSourceNode(This,aSymbolicLink,aIndexStream,aIndexMediaType,aPtrPtrTopologyNode)	\
    ( (This)->lpVtbl -> createSourceNode(This,aSymbolicLink,aIndexStream,aIndexMediaType,aPtrPtrTopologyNode) ) 

#define ISourceControl_createSourceNodeWithDownStreamConnection(This,aSymbolicLink,aIndexStream,aIndexMediaType,aPtrDownStreamTopologyNode,aPtrPtrTopologyNode)	\
    ( (This)->lpVtbl -> createSourceNodeWithDownStreamConnection(This,aSymbolicLink,aIndexStream,aIndexMediaType,aPtrDownStreamTopologyNode,aPtrPtrTopologyNode) ) 

#define ISourceControl_createSource(This,aSymbolicLink,aPtrPtrMediaSource)	\
    ( (This)->lpVtbl -> createSource(This,aSymbolicLink,aPtrPtrMediaSource) ) 

#define ISourceControl_createSourceFromCaptureProcessor(This,aPtrCaptureProcessor,aPtrPtrMediaSource)	\
    ( (This)->lpVtbl -> createSourceFromCaptureProcessor(This,aPtrCaptureProcessor,aPtrPtrMediaSource) ) 

#define ISourceControl_getSourceOutputMediaTypeFromMediaSource(This,aPtrMediaSource,aIndexStream,aIndexMediaType,aPtrPtrOutputMediaType)	\
    ( (This)->lpVtbl -> getSourceOutputMediaTypeFromMediaSource(This,aPtrMediaSource,aIndexStream,aIndexMediaType,aPtrPtrOutputMediaType) ) 

#define ISourceControl_createSourceNodeFromExternalSource(This,aPtrMediaSource,aIndexStream,aIndexMediaType,aPtrPtrTopologyNode)	\
    ( (This)->lpVtbl -> createSourceNodeFromExternalSource(This,aPtrMediaSource,aIndexStream,aIndexMediaType,aPtrPtrTopologyNode) ) 

#define ISourceControl_createSourceNodeFromExternalSourceWithDownStreamConnection(This,aPtrMediaSource,aIndexStream,aIndexMediaType,aPtrDownStreamTopologyNode,aPtrPtrTopologyNode)	\
    ( (This)->lpVtbl -> createSourceNodeFromExternalSourceWithDownStreamConnection(This,aPtrMediaSource,aIndexStream,aIndexMediaType,aPtrDownStreamTopologyNode,aPtrPtrTopologyNode) ) 

#define ISourceControl_createSourceControl(This,aSymbolicLink,aREFIID,aPtrPtrControl)	\
    ( (This)->lpVtbl -> createSourceControl(This,aSymbolicLink,aREFIID,aPtrPtrControl) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ISourceControl_INTERFACE_DEFINED__ */


#ifndef __ISinkControl_INTERFACE_DEFINED__
#define __ISinkControl_INTERFACE_DEFINED__

/* interface ISinkControl */
/* [dual][helpstring][oleautomation][uuid][object] */ 


EXTERN_C const IID IID_ISinkControl;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("C6BA3732-197E-438B-8E73-277759A7B58F")
    ISinkControl : public IDispatch
    {
    public:
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE getCollectionOfSinks( 
            /* [out] */ BSTR *aPtrPtrXMLstring) = 0;
        
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE createSinkFactory( 
            /* [in] */ REFGUID aRefContainerTypeGUID,
            /* [in] */ REFIID aREFIID,
            /* [out] */ IUnknown **aPtrPtrSink) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct ISinkControlVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ISinkControl * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ISinkControl * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ISinkControl * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            ISinkControl * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            ISinkControl * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            ISinkControl * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            ISinkControl * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *getCollectionOfSinks )( 
            ISinkControl * This,
            /* [out] */ BSTR *aPtrPtrXMLstring);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *createSinkFactory )( 
            ISinkControl * This,
            /* [in] */ REFGUID aRefContainerTypeGUID,
            /* [in] */ REFIID aREFIID,
            /* [out] */ IUnknown **aPtrPtrSink);
        
        END_INTERFACE
    } ISinkControlVtbl;

    interface ISinkControl
    {
        CONST_VTBL struct ISinkControlVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ISinkControl_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ISinkControl_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ISinkControl_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ISinkControl_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define ISinkControl_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define ISinkControl_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define ISinkControl_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define ISinkControl_getCollectionOfSinks(This,aPtrPtrXMLstring)	\
    ( (This)->lpVtbl -> getCollectionOfSinks(This,aPtrPtrXMLstring) ) 

#define ISinkControl_createSinkFactory(This,aRefContainerTypeGUID,aREFIID,aPtrPtrSink)	\
    ( (This)->lpVtbl -> createSinkFactory(This,aRefContainerTypeGUID,aREFIID,aPtrPtrSink) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ISinkControl_INTERFACE_DEFINED__ */


#ifndef __IFileSinkFactory_INTERFACE_DEFINED__
#define __IFileSinkFactory_INTERFACE_DEFINED__

/* interface IFileSinkFactory */
/* [dual][helpstring][oleautomation][uuid][object] */ 


EXTERN_C const IID IID_IFileSinkFactory;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("D6E342E3-7DDD-4858-AB91-4253643864C2")
    IFileSinkFactory : public IDispatch
    {
    public:
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE createOutputNodes( 
            /* [in] */ VARIANT aArrayPtrCompressedMediaTypes,
            /* [in] */ BSTR aPtrFileName,
            /* [out] */ VARIANT *aPtrArrayPtrTopologyOutputNodes) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IFileSinkFactoryVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IFileSinkFactory * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IFileSinkFactory * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IFileSinkFactory * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IFileSinkFactory * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IFileSinkFactory * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IFileSinkFactory * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IFileSinkFactory * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *createOutputNodes )( 
            IFileSinkFactory * This,
            /* [in] */ VARIANT aArrayPtrCompressedMediaTypes,
            /* [in] */ BSTR aPtrFileName,
            /* [out] */ VARIANT *aPtrArrayPtrTopologyOutputNodes);
        
        END_INTERFACE
    } IFileSinkFactoryVtbl;

    interface IFileSinkFactory
    {
        CONST_VTBL struct IFileSinkFactoryVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IFileSinkFactory_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IFileSinkFactory_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IFileSinkFactory_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IFileSinkFactory_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IFileSinkFactory_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IFileSinkFactory_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IFileSinkFactory_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define IFileSinkFactory_createOutputNodes(This,aArrayPtrCompressedMediaTypes,aPtrFileName,aPtrArrayPtrTopologyOutputNodes)	\
    ( (This)->lpVtbl -> createOutputNodes(This,aArrayPtrCompressedMediaTypes,aPtrFileName,aPtrArrayPtrTopologyOutputNodes) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IFileSinkFactory_INTERFACE_DEFINED__ */


#ifndef __ISampleGrabberCallSinkFactory_INTERFACE_DEFINED__
#define __ISampleGrabberCallSinkFactory_INTERFACE_DEFINED__

/* interface ISampleGrabberCallSinkFactory */
/* [dual][helpstring][oleautomation][uuid][object] */ 


EXTERN_C const IID IID_ISampleGrabberCallSinkFactory;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("759D24FF-C5D6-4B65-8DDF-8A2B2BECDE39")
    ISampleGrabberCallSinkFactory : public IDispatch
    {
    public:
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE createOutputNode( 
            /* [in] */ REFGUID aRefMajorType,
            /* [in] */ REFGUID aRefSubType,
            /* [in] */ DWORD aSampleByteSize,
            /* [in] */ REFIID aREFIID,
            /* [out] */ IUnknown **aPtrPtrISampleGrabberCall) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct ISampleGrabberCallSinkFactoryVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ISampleGrabberCallSinkFactory * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ISampleGrabberCallSinkFactory * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ISampleGrabberCallSinkFactory * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            ISampleGrabberCallSinkFactory * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            ISampleGrabberCallSinkFactory * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            ISampleGrabberCallSinkFactory * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            ISampleGrabberCallSinkFactory * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *createOutputNode )( 
            ISampleGrabberCallSinkFactory * This,
            /* [in] */ REFGUID aRefMajorType,
            /* [in] */ REFGUID aRefSubType,
            /* [in] */ DWORD aSampleByteSize,
            /* [in] */ REFIID aREFIID,
            /* [out] */ IUnknown **aPtrPtrISampleGrabberCall);
        
        END_INTERFACE
    } ISampleGrabberCallSinkFactoryVtbl;

    interface ISampleGrabberCallSinkFactory
    {
        CONST_VTBL struct ISampleGrabberCallSinkFactoryVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ISampleGrabberCallSinkFactory_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ISampleGrabberCallSinkFactory_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ISampleGrabberCallSinkFactory_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ISampleGrabberCallSinkFactory_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define ISampleGrabberCallSinkFactory_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define ISampleGrabberCallSinkFactory_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define ISampleGrabberCallSinkFactory_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define ISampleGrabberCallSinkFactory_createOutputNode(This,aRefMajorType,aRefSubType,aSampleByteSize,aREFIID,aPtrPtrISampleGrabberCall)	\
    ( (This)->lpVtbl -> createOutputNode(This,aRefMajorType,aRefSubType,aSampleByteSize,aREFIID,aPtrPtrISampleGrabberCall) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ISampleGrabberCallSinkFactory_INTERFACE_DEFINED__ */


#ifndef __ISampleGrabberCall_INTERFACE_DEFINED__
#define __ISampleGrabberCall_INTERFACE_DEFINED__

/* interface ISampleGrabberCall */
/* [dual][helpstring][local][oleautomation][uuid][object] */ 


EXTERN_C const IID IID_ISampleGrabberCall;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("118AD3F7-D9A3-4146-AB35-F16421DC995E")
    ISampleGrabberCall : public IDispatch
    {
    public:
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE readData( 
            /* [in] */ LPVOID aPtrData,
            /* [out] */ DWORD *aByteSize) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct ISampleGrabberCallVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ISampleGrabberCall * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ISampleGrabberCall * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ISampleGrabberCall * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            ISampleGrabberCall * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            ISampleGrabberCall * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            ISampleGrabberCall * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            ISampleGrabberCall * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *readData )( 
            ISampleGrabberCall * This,
            /* [in] */ LPVOID aPtrData,
            /* [out] */ DWORD *aByteSize);
        
        END_INTERFACE
    } ISampleGrabberCallVtbl;

    interface ISampleGrabberCall
    {
        CONST_VTBL struct ISampleGrabberCallVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ISampleGrabberCall_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ISampleGrabberCall_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ISampleGrabberCall_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ISampleGrabberCall_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define ISampleGrabberCall_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define ISampleGrabberCall_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define ISampleGrabberCall_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define ISampleGrabberCall_readData(This,aPtrData,aByteSize)	\
    ( (This)->lpVtbl -> readData(This,aPtrData,aByteSize) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ISampleGrabberCall_INTERFACE_DEFINED__ */


#ifndef __ISampleGrabberCallbackSinkFactory_INTERFACE_DEFINED__
#define __ISampleGrabberCallbackSinkFactory_INTERFACE_DEFINED__

/* interface ISampleGrabberCallbackSinkFactory */
/* [helpstring][oleautomation][uuid][object] */ 


EXTERN_C const IID IID_ISampleGrabberCallbackSinkFactory;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("3D64C48E-EDA4-4EE1-8436-58B64DD7CF13")
    ISampleGrabberCallbackSinkFactory : public IUnknown
    {
    public:
        virtual /* [helpstring] */ HRESULT STDMETHODCALLTYPE createOutputNode( 
            /* [in] */ REFGUID aRefMajorType,
            /* [in] */ REFGUID aRefSubType,
            /* [in] */ IUnknown *aPtrISampleGrabberCallback,
            /* [out] */ IUnknown **aPtrPtrTopologyNode) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct ISampleGrabberCallbackSinkFactoryVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ISampleGrabberCallbackSinkFactory * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ISampleGrabberCallbackSinkFactory * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ISampleGrabberCallbackSinkFactory * This);
        
        /* [helpstring] */ HRESULT ( STDMETHODCALLTYPE *createOutputNode )( 
            ISampleGrabberCallbackSinkFactory * This,
            /* [in] */ REFGUID aRefMajorType,
            /* [in] */ REFGUID aRefSubType,
            /* [in] */ IUnknown *aPtrISampleGrabberCallback,
            /* [out] */ IUnknown **aPtrPtrTopologyNode);
        
        END_INTERFACE
    } ISampleGrabberCallbackSinkFactoryVtbl;

    interface ISampleGrabberCallbackSinkFactory
    {
        CONST_VTBL struct ISampleGrabberCallbackSinkFactoryVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ISampleGrabberCallbackSinkFactory_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ISampleGrabberCallbackSinkFactory_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ISampleGrabberCallbackSinkFactory_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ISampleGrabberCallbackSinkFactory_createOutputNode(This,aRefMajorType,aRefSubType,aPtrISampleGrabberCallback,aPtrPtrTopologyNode)	\
    ( (This)->lpVtbl -> createOutputNode(This,aRefMajorType,aRefSubType,aPtrISampleGrabberCallback,aPtrPtrTopologyNode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ISampleGrabberCallbackSinkFactory_INTERFACE_DEFINED__ */


#ifndef __ISampleGrabberCallback_INTERFACE_DEFINED__
#define __ISampleGrabberCallback_INTERFACE_DEFINED__

/* interface ISampleGrabberCallback */
/* [helpstring][local][oleautomation][uuid][object] */ 


EXTERN_C const IID IID_ISampleGrabberCallback;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("C52F4DC9-BD3F-45FC-9839-1340AEFD25AF")
    ISampleGrabberCallback : public IUnknown
    {
    public:
        virtual /* [local][helpstring] */ HRESULT STDMETHODCALLTYPE invoke( 
            /* [in] */ REFGUID aGUIDMajorMediaType,
            /* [in] */ DWORD aSampleFlags,
            /* [in] */ LONGLONG aSampleTime,
            /* [in] */ LONGLONG aSampleDuration,
            /* [in] */ LPVOID aPtrSampleBuffer,
            /* [in] */ DWORD aSampleSize) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct ISampleGrabberCallbackVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ISampleGrabberCallback * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ISampleGrabberCallback * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ISampleGrabberCallback * This);
        
        /* [local][helpstring] */ HRESULT ( STDMETHODCALLTYPE *invoke )( 
            ISampleGrabberCallback * This,
            /* [in] */ REFGUID aGUIDMajorMediaType,
            /* [in] */ DWORD aSampleFlags,
            /* [in] */ LONGLONG aSampleTime,
            /* [in] */ LONGLONG aSampleDuration,
            /* [in] */ LPVOID aPtrSampleBuffer,
            /* [in] */ DWORD aSampleSize);
        
        END_INTERFACE
    } ISampleGrabberCallbackVtbl;

    interface ISampleGrabberCallback
    {
        CONST_VTBL struct ISampleGrabberCallbackVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ISampleGrabberCallback_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ISampleGrabberCallback_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ISampleGrabberCallback_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ISampleGrabberCallback_invoke(This,aGUIDMajorMediaType,aSampleFlags,aSampleTime,aSampleDuration,aPtrSampleBuffer,aSampleSize)	\
    ( (This)->lpVtbl -> invoke(This,aGUIDMajorMediaType,aSampleFlags,aSampleTime,aSampleDuration,aPtrSampleBuffer,aSampleSize) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ISampleGrabberCallback_INTERFACE_DEFINED__ */


#ifndef __IEVRSinkFactory_INTERFACE_DEFINED__
#define __IEVRSinkFactory_INTERFACE_DEFINED__

/* interface IEVRSinkFactory */
/* [dual][helpstring][local][oleautomation][uuid][object] */ 


EXTERN_C const IID IID_IEVRSinkFactory;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("2F34AF87-D349-45AA-A5F1-E4104D5C458E")
    IEVRSinkFactory : public IDispatch
    {
    public:
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE createOutputNode( 
            /* [in] */ LPVOID aHWND,
            /* [out] */ IUnknown **aPtrPtrTopologyNode) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IEVRSinkFactoryVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEVRSinkFactory * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEVRSinkFactory * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEVRSinkFactory * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IEVRSinkFactory * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IEVRSinkFactory * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IEVRSinkFactory * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IEVRSinkFactory * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *createOutputNode )( 
            IEVRSinkFactory * This,
            /* [in] */ LPVOID aHWND,
            /* [out] */ IUnknown **aPtrPtrTopologyNode);
        
        END_INTERFACE
    } IEVRSinkFactoryVtbl;

    interface IEVRSinkFactory
    {
        CONST_VTBL struct IEVRSinkFactoryVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEVRSinkFactory_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEVRSinkFactory_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEVRSinkFactory_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEVRSinkFactory_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IEVRSinkFactory_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IEVRSinkFactory_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IEVRSinkFactory_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define IEVRSinkFactory_createOutputNode(This,aHWND,aPtrPtrTopologyNode)	\
    ( (This)->lpVtbl -> createOutputNode(This,aHWND,aPtrPtrTopologyNode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEVRSinkFactory_INTERFACE_DEFINED__ */


#ifndef __IEVRMultiSinkFactory_INTERFACE_DEFINED__
#define __IEVRMultiSinkFactory_INTERFACE_DEFINED__

/* interface IEVRMultiSinkFactory */
/* [dual][helpstring][local][oleautomation][uuid][object] */ 


EXTERN_C const IID IID_IEVRMultiSinkFactory;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("10E52132-A73F-4A9E-A91B-FE18C91D6837")
    IEVRMultiSinkFactory : public IDispatch
    {
    public:
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE createOutputNodes( 
            /* [in] */ LPVOID aHandle,
            /* [in] */ IUnknown *aPtrUnkTarget,
            /* [in] */ DWORD aOutputNodeAmount,
            /* [out] */ VARIANT *aPtrArrayPtrTopologyOutputNodes) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IEVRMultiSinkFactoryVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEVRMultiSinkFactory * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEVRMultiSinkFactory * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEVRMultiSinkFactory * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IEVRMultiSinkFactory * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IEVRMultiSinkFactory * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IEVRMultiSinkFactory * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IEVRMultiSinkFactory * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *createOutputNodes )( 
            IEVRMultiSinkFactory * This,
            /* [in] */ LPVOID aHandle,
            /* [in] */ IUnknown *aPtrUnkTarget,
            /* [in] */ DWORD aOutputNodeAmount,
            /* [out] */ VARIANT *aPtrArrayPtrTopologyOutputNodes);
        
        END_INTERFACE
    } IEVRMultiSinkFactoryVtbl;

    interface IEVRMultiSinkFactory
    {
        CONST_VTBL struct IEVRMultiSinkFactoryVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEVRMultiSinkFactory_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEVRMultiSinkFactory_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEVRMultiSinkFactory_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEVRMultiSinkFactory_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IEVRMultiSinkFactory_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IEVRMultiSinkFactory_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IEVRMultiSinkFactory_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define IEVRMultiSinkFactory_createOutputNodes(This,aHandle,aPtrUnkTarget,aOutputNodeAmount,aPtrArrayPtrTopologyOutputNodes)	\
    ( (This)->lpVtbl -> createOutputNodes(This,aHandle,aPtrUnkTarget,aOutputNodeAmount,aPtrArrayPtrTopologyOutputNodes) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEVRMultiSinkFactory_INTERFACE_DEFINED__ */


#ifndef __IMediaTypeParser_INTERFACE_DEFINED__
#define __IMediaTypeParser_INTERFACE_DEFINED__

/* interface IMediaTypeParser */
/* [dual][helpstring][oleautomation][uuid][object] */ 


EXTERN_C const IID IID_IMediaTypeParser;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("74F0DC2B-E470-4359-A1E7-467B521BDFE1")
    IMediaTypeParser : public IDispatch
    {
    public:
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE parse( 
            /* [in] */ IUnknown *aPtrMediaType,
            /* [out] */ BSTR *aPtrPtrXMLstring) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IMediaTypeParserVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IMediaTypeParser * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IMediaTypeParser * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IMediaTypeParser * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IMediaTypeParser * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IMediaTypeParser * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IMediaTypeParser * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IMediaTypeParser * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *parse )( 
            IMediaTypeParser * This,
            /* [in] */ IUnknown *aPtrMediaType,
            /* [out] */ BSTR *aPtrPtrXMLstring);
        
        END_INTERFACE
    } IMediaTypeParserVtbl;

    interface IMediaTypeParser
    {
        CONST_VTBL struct IMediaTypeParserVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IMediaTypeParser_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IMediaTypeParser_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IMediaTypeParser_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IMediaTypeParser_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IMediaTypeParser_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IMediaTypeParser_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IMediaTypeParser_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define IMediaTypeParser_parse(This,aPtrMediaType,aPtrPtrXMLstring)	\
    ( (This)->lpVtbl -> parse(This,aPtrMediaType,aPtrPtrXMLstring) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IMediaTypeParser_INTERFACE_DEFINED__ */


#ifndef __IStrideForBitmap_INTERFACE_DEFINED__
#define __IStrideForBitmap_INTERFACE_DEFINED__

/* interface IStrideForBitmap */
/* [dual][helpstring][oleautomation][uuid][object] */ 


EXTERN_C const IID IID_IStrideForBitmap;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("74D903C9-69E6-4FC7-BF7A-9F47605C52BE")
    IStrideForBitmap : public IDispatch
    {
    public:
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE getStrideForBitmap( 
            /* [in] */ REFGUID aMFVideoFormat,
            /* [in] */ DWORD aWidthInPixels,
            /* [out] */ LONG *aPtrStride) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IStrideForBitmapVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IStrideForBitmap * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IStrideForBitmap * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IStrideForBitmap * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IStrideForBitmap * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IStrideForBitmap * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IStrideForBitmap * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IStrideForBitmap * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *getStrideForBitmap )( 
            IStrideForBitmap * This,
            /* [in] */ REFGUID aMFVideoFormat,
            /* [in] */ DWORD aWidthInPixels,
            /* [out] */ LONG *aPtrStride);
        
        END_INTERFACE
    } IStrideForBitmapVtbl;

    interface IStrideForBitmap
    {
        CONST_VTBL struct IStrideForBitmapVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IStrideForBitmap_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IStrideForBitmap_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IStrideForBitmap_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IStrideForBitmap_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IStrideForBitmap_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IStrideForBitmap_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IStrideForBitmap_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define IStrideForBitmap_getStrideForBitmap(This,aMFVideoFormat,aWidthInPixels,aPtrStride)	\
    ( (This)->lpVtbl -> getStrideForBitmap(This,aMFVideoFormat,aWidthInPixels,aPtrStride) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IStrideForBitmap_INTERFACE_DEFINED__ */


#ifndef __IStreamControl_INTERFACE_DEFINED__
#define __IStreamControl_INTERFACE_DEFINED__

/* interface IStreamControl */
/* [dual][helpstring][oleautomation][uuid][object] */ 


EXTERN_C const IID IID_IStreamControl;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("E8F25B4A-8C71-4C9E-BD8C-82260DC4C21B")
    IStreamControl : public IDispatch
    {
    public:
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE getCollectionOfStreamControlNodeFactories( 
            /* [out] */ BSTR *aPtrPtrXMLstring) = 0;
        
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE createStreamControlNodeFactory( 
            /* [in] */ REFIID aREFIID,
            /* [out] */ IUnknown **aPtrPtrStreamControlNodeFactory) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IStreamControlVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IStreamControl * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IStreamControl * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IStreamControl * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IStreamControl * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IStreamControl * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IStreamControl * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IStreamControl * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *getCollectionOfStreamControlNodeFactories )( 
            IStreamControl * This,
            /* [out] */ BSTR *aPtrPtrXMLstring);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *createStreamControlNodeFactory )( 
            IStreamControl * This,
            /* [in] */ REFIID aREFIID,
            /* [out] */ IUnknown **aPtrPtrStreamControlNodeFactory);
        
        END_INTERFACE
    } IStreamControlVtbl;

    interface IStreamControl
    {
        CONST_VTBL struct IStreamControlVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IStreamControl_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IStreamControl_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IStreamControl_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IStreamControl_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IStreamControl_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IStreamControl_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IStreamControl_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define IStreamControl_getCollectionOfStreamControlNodeFactories(This,aPtrPtrXMLstring)	\
    ( (This)->lpVtbl -> getCollectionOfStreamControlNodeFactories(This,aPtrPtrXMLstring) ) 

#define IStreamControl_createStreamControlNodeFactory(This,aREFIID,aPtrPtrStreamControlNodeFactory)	\
    ( (This)->lpVtbl -> createStreamControlNodeFactory(This,aREFIID,aPtrPtrStreamControlNodeFactory) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IStreamControl_INTERFACE_DEFINED__ */


#ifndef __ISpreaderNodeFactory_INTERFACE_DEFINED__
#define __ISpreaderNodeFactory_INTERFACE_DEFINED__

/* interface ISpreaderNodeFactory */
/* [dual][helpstring][oleautomation][uuid][object] */ 


EXTERN_C const IID IID_ISpreaderNodeFactory;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("85DFAAA1-4CC0-4A88-AE28-8F492E552CCA")
    ISpreaderNodeFactory : public IDispatch
    {
    public:
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE createSpreaderNode( 
            /* [in] */ VARIANT aArrayPtrDownStreamTopologyNodes,
            /* [out] */ IUnknown **aPtrPtrTopologySpreaderNode) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct ISpreaderNodeFactoryVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ISpreaderNodeFactory * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ISpreaderNodeFactory * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ISpreaderNodeFactory * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            ISpreaderNodeFactory * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            ISpreaderNodeFactory * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            ISpreaderNodeFactory * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            ISpreaderNodeFactory * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *createSpreaderNode )( 
            ISpreaderNodeFactory * This,
            /* [in] */ VARIANT aArrayPtrDownStreamTopologyNodes,
            /* [out] */ IUnknown **aPtrPtrTopologySpreaderNode);
        
        END_INTERFACE
    } ISpreaderNodeFactoryVtbl;

    interface ISpreaderNodeFactory
    {
        CONST_VTBL struct ISpreaderNodeFactoryVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ISpreaderNodeFactory_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ISpreaderNodeFactory_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ISpreaderNodeFactory_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ISpreaderNodeFactory_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define ISpreaderNodeFactory_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define ISpreaderNodeFactory_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define ISpreaderNodeFactory_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define ISpreaderNodeFactory_createSpreaderNode(This,aArrayPtrDownStreamTopologyNodes,aPtrPtrTopologySpreaderNode)	\
    ( (This)->lpVtbl -> createSpreaderNode(This,aArrayPtrDownStreamTopologyNodes,aPtrPtrTopologySpreaderNode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ISpreaderNodeFactory_INTERFACE_DEFINED__ */


#ifndef __ISwitcherNodeFactory_INTERFACE_DEFINED__
#define __ISwitcherNodeFactory_INTERFACE_DEFINED__

/* interface ISwitcherNodeFactory */
/* [dual][helpstring][oleautomation][uuid][object] */ 


EXTERN_C const IID IID_ISwitcherNodeFactory;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("B8187D19-EBFD-4F8D-80D9-A15EBA3B369C")
    ISwitcherNodeFactory : public IDispatch
    {
    public:
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE createSwitcherNode( 
            /* [in] */ IUnknown *aPtrDownStreamTopologyNode,
            /* [out] */ IUnknown **aPtrPtrTopologySwitcherNode) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct ISwitcherNodeFactoryVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ISwitcherNodeFactory * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ISwitcherNodeFactory * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ISwitcherNodeFactory * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            ISwitcherNodeFactory * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            ISwitcherNodeFactory * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            ISwitcherNodeFactory * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            ISwitcherNodeFactory * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *createSwitcherNode )( 
            ISwitcherNodeFactory * This,
            /* [in] */ IUnknown *aPtrDownStreamTopologyNode,
            /* [out] */ IUnknown **aPtrPtrTopologySwitcherNode);
        
        END_INTERFACE
    } ISwitcherNodeFactoryVtbl;

    interface ISwitcherNodeFactory
    {
        CONST_VTBL struct ISwitcherNodeFactoryVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ISwitcherNodeFactory_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ISwitcherNodeFactory_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ISwitcherNodeFactory_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ISwitcherNodeFactory_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define ISwitcherNodeFactory_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define ISwitcherNodeFactory_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define ISwitcherNodeFactory_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define ISwitcherNodeFactory_createSwitcherNode(This,aPtrDownStreamTopologyNode,aPtrPtrTopologySwitcherNode)	\
    ( (This)->lpVtbl -> createSwitcherNode(This,aPtrDownStreamTopologyNode,aPtrPtrTopologySwitcherNode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ISwitcherNodeFactory_INTERFACE_DEFINED__ */


#ifndef __IEncoderControl_INTERFACE_DEFINED__
#define __IEncoderControl_INTERFACE_DEFINED__

/* interface IEncoderControl */
/* [dual][helpstring][oleautomation][uuid][object] */ 


EXTERN_C const IID IID_IEncoderControl;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("96223507-D8FF-4EC1-B125-71AA7F9726A4")
    IEncoderControl : public IDispatch
    {
    public:
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE getCollectionOfEncoders( 
            /* [out] */ BSTR *aPtrPtrXMLstring) = 0;
        
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE getMediaTypeCollectionOfEncoder( 
            /* [in] */ IUnknown *aPtrUncompressedMediaType,
            /* [in] */ REFCLSID aRefEncoderCLSID,
            /* [out] */ BSTR *aPtrPtrXMLstring) = 0;
        
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE createEncoderNodeFactory( 
            /* [in] */ REFCLSID aRefEncoderCLSID,
            /* [in] */ REFIID aREFIID,
            /* [out] */ IUnknown **aPtrPtrEncoderNodeFactory) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IEncoderControlVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEncoderControl * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEncoderControl * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEncoderControl * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IEncoderControl * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IEncoderControl * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IEncoderControl * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IEncoderControl * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *getCollectionOfEncoders )( 
            IEncoderControl * This,
            /* [out] */ BSTR *aPtrPtrXMLstring);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *getMediaTypeCollectionOfEncoder )( 
            IEncoderControl * This,
            /* [in] */ IUnknown *aPtrUncompressedMediaType,
            /* [in] */ REFCLSID aRefEncoderCLSID,
            /* [out] */ BSTR *aPtrPtrXMLstring);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *createEncoderNodeFactory )( 
            IEncoderControl * This,
            /* [in] */ REFCLSID aRefEncoderCLSID,
            /* [in] */ REFIID aREFIID,
            /* [out] */ IUnknown **aPtrPtrEncoderNodeFactory);
        
        END_INTERFACE
    } IEncoderControlVtbl;

    interface IEncoderControl
    {
        CONST_VTBL struct IEncoderControlVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEncoderControl_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEncoderControl_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEncoderControl_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEncoderControl_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IEncoderControl_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IEncoderControl_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IEncoderControl_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define IEncoderControl_getCollectionOfEncoders(This,aPtrPtrXMLstring)	\
    ( (This)->lpVtbl -> getCollectionOfEncoders(This,aPtrPtrXMLstring) ) 

#define IEncoderControl_getMediaTypeCollectionOfEncoder(This,aPtrUncompressedMediaType,aRefEncoderCLSID,aPtrPtrXMLstring)	\
    ( (This)->lpVtbl -> getMediaTypeCollectionOfEncoder(This,aPtrUncompressedMediaType,aRefEncoderCLSID,aPtrPtrXMLstring) ) 

#define IEncoderControl_createEncoderNodeFactory(This,aRefEncoderCLSID,aREFIID,aPtrPtrEncoderNodeFactory)	\
    ( (This)->lpVtbl -> createEncoderNodeFactory(This,aRefEncoderCLSID,aREFIID,aPtrPtrEncoderNodeFactory) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEncoderControl_INTERFACE_DEFINED__ */


#ifndef __IEncoderNodeFactory_INTERFACE_DEFINED__
#define __IEncoderNodeFactory_INTERFACE_DEFINED__

/* interface IEncoderNodeFactory */
/* [dual][helpstring][oleautomation][uuid][object] */ 


EXTERN_C const IID IID_IEncoderNodeFactory;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("A56E11D8-D602-4792-8570-38C283FC0AA3")
    IEncoderNodeFactory : public IDispatch
    {
    public:
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE createCompressedMediaType( 
            /* [in] */ IUnknown *aPtrUncompressedMediaType,
            /* [in] */ REFGUID aRefEncodingModeGUID,
            /* [in] */ DWORD aEncodingModeValue,
            /* [in] */ DWORD aIndexCompressedMediaType,
            /* [out] */ IUnknown **aPtrPtrCompressedMediaType) = 0;
        
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE createEncoderNode( 
            /* [in] */ IUnknown *aPtrUncompressedMediaType,
            /* [in] */ REFGUID aRefEncodingModeGUID,
            /* [in] */ DWORD aEncodingModeValue,
            /* [in] */ DWORD aIndexCompressedMediaType,
            /* [in] */ IUnknown *aPtrDownStreamNode,
            /* [out] */ IUnknown **aPtrPtrEncoderNode) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IEncoderNodeFactoryVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEncoderNodeFactory * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEncoderNodeFactory * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEncoderNodeFactory * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IEncoderNodeFactory * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IEncoderNodeFactory * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IEncoderNodeFactory * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IEncoderNodeFactory * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *createCompressedMediaType )( 
            IEncoderNodeFactory * This,
            /* [in] */ IUnknown *aPtrUncompressedMediaType,
            /* [in] */ REFGUID aRefEncodingModeGUID,
            /* [in] */ DWORD aEncodingModeValue,
            /* [in] */ DWORD aIndexCompressedMediaType,
            /* [out] */ IUnknown **aPtrPtrCompressedMediaType);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *createEncoderNode )( 
            IEncoderNodeFactory * This,
            /* [in] */ IUnknown *aPtrUncompressedMediaType,
            /* [in] */ REFGUID aRefEncodingModeGUID,
            /* [in] */ DWORD aEncodingModeValue,
            /* [in] */ DWORD aIndexCompressedMediaType,
            /* [in] */ IUnknown *aPtrDownStreamNode,
            /* [out] */ IUnknown **aPtrPtrEncoderNode);
        
        END_INTERFACE
    } IEncoderNodeFactoryVtbl;

    interface IEncoderNodeFactory
    {
        CONST_VTBL struct IEncoderNodeFactoryVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEncoderNodeFactory_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEncoderNodeFactory_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEncoderNodeFactory_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEncoderNodeFactory_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IEncoderNodeFactory_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IEncoderNodeFactory_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IEncoderNodeFactory_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define IEncoderNodeFactory_createCompressedMediaType(This,aPtrUncompressedMediaType,aRefEncodingModeGUID,aEncodingModeValue,aIndexCompressedMediaType,aPtrPtrCompressedMediaType)	\
    ( (This)->lpVtbl -> createCompressedMediaType(This,aPtrUncompressedMediaType,aRefEncodingModeGUID,aEncodingModeValue,aIndexCompressedMediaType,aPtrPtrCompressedMediaType) ) 

#define IEncoderNodeFactory_createEncoderNode(This,aPtrUncompressedMediaType,aRefEncodingModeGUID,aEncodingModeValue,aIndexCompressedMediaType,aPtrDownStreamNode,aPtrPtrEncoderNode)	\
    ( (This)->lpVtbl -> createEncoderNode(This,aPtrUncompressedMediaType,aRefEncodingModeGUID,aEncodingModeValue,aIndexCompressedMediaType,aPtrDownStreamNode,aPtrPtrEncoderNode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEncoderNodeFactory_INTERFACE_DEFINED__ */


#ifndef __IWebCamControl_INTERFACE_DEFINED__
#define __IWebCamControl_INTERFACE_DEFINED__

/* interface IWebCamControl */
/* [helpstring][uuid][object] */ 


EXTERN_C const IID IID_IWebCamControl;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("3BD92C4C-5E06-4901-AE0B-D97E3902EAFC")
    IWebCamControl : public IDispatch
    {
    public:
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE getCamParametrs( 
            /* [out] */ BSTR *aXMLstring) = 0;
        
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE getCamParametr( 
            /* [in] */ DWORD aParametrIndex,
            /* [out] */ LONG *aCurrentValue,
            /* [out] */ LONG *aMin,
            /* [out] */ LONG *aMax,
            /* [out] */ LONG *aStep,
            /* [out] */ LONG *aDefault,
            /* [out] */ LONG *aFlag) = 0;
        
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE setCamParametr( 
            /* [in] */ DWORD aParametrIndex,
            /* [in] */ LONG aNewValue,
            /* [in] */ LONG aFlag) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IWebCamControlVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IWebCamControl * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IWebCamControl * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IWebCamControl * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IWebCamControl * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IWebCamControl * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IWebCamControl * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IWebCamControl * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *getCamParametrs )( 
            IWebCamControl * This,
            /* [out] */ BSTR *aXMLstring);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *getCamParametr )( 
            IWebCamControl * This,
            /* [in] */ DWORD aParametrIndex,
            /* [out] */ LONG *aCurrentValue,
            /* [out] */ LONG *aMin,
            /* [out] */ LONG *aMax,
            /* [out] */ LONG *aStep,
            /* [out] */ LONG *aDefault,
            /* [out] */ LONG *aFlag);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *setCamParametr )( 
            IWebCamControl * This,
            /* [in] */ DWORD aParametrIndex,
            /* [in] */ LONG aNewValue,
            /* [in] */ LONG aFlag);
        
        END_INTERFACE
    } IWebCamControlVtbl;

    interface IWebCamControl
    {
        CONST_VTBL struct IWebCamControlVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IWebCamControl_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IWebCamControl_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IWebCamControl_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IWebCamControl_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IWebCamControl_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IWebCamControl_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IWebCamControl_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define IWebCamControl_getCamParametrs(This,aXMLstring)	\
    ( (This)->lpVtbl -> getCamParametrs(This,aXMLstring) ) 

#define IWebCamControl_getCamParametr(This,aParametrIndex,aCurrentValue,aMin,aMax,aStep,aDefault,aFlag)	\
    ( (This)->lpVtbl -> getCamParametr(This,aParametrIndex,aCurrentValue,aMin,aMax,aStep,aDefault,aFlag) ) 

#define IWebCamControl_setCamParametr(This,aParametrIndex,aNewValue,aFlag)	\
    ( (This)->lpVtbl -> setCamParametr(This,aParametrIndex,aNewValue,aFlag) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IWebCamControl_INTERFACE_DEFINED__ */


#ifndef __IByteStreamSinkFactory_INTERFACE_DEFINED__
#define __IByteStreamSinkFactory_INTERFACE_DEFINED__

/* interface IByteStreamSinkFactory */
/* [helpstring][oleautomation][uuid][object] */ 


EXTERN_C const IID IID_IByteStreamSinkFactory;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("2E891049-964A-4D08-8F36-95CE8CB0DE9B")
    IByteStreamSinkFactory : public IUnknown
    {
    public:
        virtual /* [helpstring] */ HRESULT STDMETHODCALLTYPE createOutputNodes( 
            /* [in] */ VARIANT aArrayPtrCompressedMediaTypes,
            /* [in] */ IUnknown *aPtrByteStream,
            /* [out] */ VARIANT *aPtrArrayPtrTopologyOutputNodes) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IByteStreamSinkFactoryVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IByteStreamSinkFactory * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IByteStreamSinkFactory * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IByteStreamSinkFactory * This);
        
        /* [helpstring] */ HRESULT ( STDMETHODCALLTYPE *createOutputNodes )( 
            IByteStreamSinkFactory * This,
            /* [in] */ VARIANT aArrayPtrCompressedMediaTypes,
            /* [in] */ IUnknown *aPtrByteStream,
            /* [out] */ VARIANT *aPtrArrayPtrTopologyOutputNodes);
        
        END_INTERFACE
    } IByteStreamSinkFactoryVtbl;

    interface IByteStreamSinkFactory
    {
        CONST_VTBL struct IByteStreamSinkFactoryVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IByteStreamSinkFactory_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IByteStreamSinkFactory_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IByteStreamSinkFactory_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IByteStreamSinkFactory_createOutputNodes(This,aArrayPtrCompressedMediaTypes,aPtrByteStream,aPtrArrayPtrTopologyOutputNodes)	\
    ( (This)->lpVtbl -> createOutputNodes(This,aArrayPtrCompressedMediaTypes,aPtrByteStream,aPtrArrayPtrTopologyOutputNodes) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IByteStreamSinkFactory_INTERFACE_DEFINED__ */


#ifndef __ISessionControl_INTERFACE_DEFINED__
#define __ISessionControl_INTERFACE_DEFINED__

/* interface ISessionControl */
/* [dual][helpstring][oleautomation][uuid][object] */ 


EXTERN_C const IID IID_ISessionControl;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("D0C58520-A941-4C0F-81B0-3ED8A4DE11ED")
    ISessionControl : public IDispatch
    {
    public:
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE createSession( 
            /* [in] */ VARIANT aArrayPtrSourceNodesOfTopology,
            /* [in] */ REFIID aREFIID,
            /* [out] */ IUnknown **aPtrPtrSession) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct ISessionControlVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ISessionControl * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ISessionControl * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ISessionControl * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            ISessionControl * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            ISessionControl * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            ISessionControl * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            ISessionControl * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *createSession )( 
            ISessionControl * This,
            /* [in] */ VARIANT aArrayPtrSourceNodesOfTopology,
            /* [in] */ REFIID aREFIID,
            /* [out] */ IUnknown **aPtrPtrSession);
        
        END_INTERFACE
    } ISessionControlVtbl;

    interface ISessionControl
    {
        CONST_VTBL struct ISessionControlVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ISessionControl_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ISessionControl_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ISessionControl_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ISessionControl_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define ISessionControl_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define ISessionControl_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define ISessionControl_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define ISessionControl_createSession(This,aArrayPtrSourceNodesOfTopology,aREFIID,aPtrPtrSession)	\
    ( (This)->lpVtbl -> createSession(This,aArrayPtrSourceNodesOfTopology,aREFIID,aPtrPtrSession) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ISessionControl_INTERFACE_DEFINED__ */


#ifndef __ISession_INTERFACE_DEFINED__
#define __ISession_INTERFACE_DEFINED__

/* interface ISession */
/* [dual][helpstring][oleautomation][uuid][object] */ 


EXTERN_C const IID IID_ISession;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("742AC001-D1E0-40A8-8EFE-BA1A550F8805")
    ISession : public IDispatch
    {
    public:
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE startSession( 
            /* [in] */ LONGLONG aStartPositionInHundredNanosecondUnits,
            /* [in] */ REFGUID aGUIDTimeFormat) = 0;
        
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE stopSession( void) = 0;
        
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE pauseSession( void) = 0;
        
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE closeSession( void) = 0;
        
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE getSessionDescriptor( 
            /* [out] */ DWORD *aPtrSessionDescriptor) = 0;
        
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE getIConnectionPointContainer( 
            /* [in] */ REFIID aREFIID,
            /* [out] */ IUnknown **aPtrPtrControl) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct ISessionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ISession * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ISession * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ISession * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            ISession * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            ISession * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            ISession * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            ISession * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *startSession )( 
            ISession * This,
            /* [in] */ LONGLONG aStartPositionInHundredNanosecondUnits,
            /* [in] */ REFGUID aGUIDTimeFormat);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *stopSession )( 
            ISession * This);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *pauseSession )( 
            ISession * This);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *closeSession )( 
            ISession * This);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *getSessionDescriptor )( 
            ISession * This,
            /* [out] */ DWORD *aPtrSessionDescriptor);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *getIConnectionPointContainer )( 
            ISession * This,
            /* [in] */ REFIID aREFIID,
            /* [out] */ IUnknown **aPtrPtrControl);
        
        END_INTERFACE
    } ISessionVtbl;

    interface ISession
    {
        CONST_VTBL struct ISessionVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ISession_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ISession_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ISession_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ISession_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define ISession_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define ISession_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define ISession_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define ISession_startSession(This,aStartPositionInHundredNanosecondUnits,aGUIDTimeFormat)	\
    ( (This)->lpVtbl -> startSession(This,aStartPositionInHundredNanosecondUnits,aGUIDTimeFormat) ) 

#define ISession_stopSession(This)	\
    ( (This)->lpVtbl -> stopSession(This) ) 

#define ISession_pauseSession(This)	\
    ( (This)->lpVtbl -> pauseSession(This) ) 

#define ISession_closeSession(This)	\
    ( (This)->lpVtbl -> closeSession(This) ) 

#define ISession_getSessionDescriptor(This,aPtrSessionDescriptor)	\
    ( (This)->lpVtbl -> getSessionDescriptor(This,aPtrSessionDescriptor) ) 

#define ISession_getIConnectionPointContainer(This,aREFIID,aPtrPtrControl)	\
    ( (This)->lpVtbl -> getIConnectionPointContainer(This,aREFIID,aPtrPtrControl) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ISession_INTERFACE_DEFINED__ */


#ifndef __ISessionCallback_INTERFACE_DEFINED__
#define __ISessionCallback_INTERFACE_DEFINED__

/* interface ISessionCallback */
/* [helpstring][oleautomation][uuid][object] */ 


EXTERN_C const IID IID_ISessionCallback;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("522E9849-E3A8-4FD0-853F-97D9B02B0E72")
    ISessionCallback : public IUnknown
    {
    public:
        virtual /* [helpstring] */ HRESULT STDMETHODCALLTYPE invoke( 
            /* [in] */ DWORD aCallbackEventCode,
            /* [in] */ DWORD aSessionDescriptor) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct ISessionCallbackVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ISessionCallback * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ISessionCallback * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ISessionCallback * This);
        
        /* [helpstring] */ HRESULT ( STDMETHODCALLTYPE *invoke )( 
            ISessionCallback * This,
            /* [in] */ DWORD aCallbackEventCode,
            /* [in] */ DWORD aSessionDescriptor);
        
        END_INTERFACE
    } ISessionCallbackVtbl;

    interface ISessionCallback
    {
        CONST_VTBL struct ISessionCallbackVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ISessionCallback_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ISessionCallback_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ISessionCallback_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ISessionCallback_invoke(This,aCallbackEventCode,aSessionDescriptor)	\
    ( (This)->lpVtbl -> invoke(This,aCallbackEventCode,aSessionDescriptor) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ISessionCallback_INTERFACE_DEFINED__ */


#ifndef __ICaptureManagerControl_INTERFACE_DEFINED__
#define __ICaptureManagerControl_INTERFACE_DEFINED__

/* interface ICaptureManagerControl */
/* [dual][helpstring][oleautomation][uuid][object] */ 


EXTERN_C const IID IID_ICaptureManagerControl;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("D4F5F10A-8F70-43CF-8CF1-EC331DA2F829")
    ICaptureManagerControl : public IDispatch
    {
    public:
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE createControl( 
            /* [in] */ REFIID aREFIID,
            /* [out] */ IUnknown **aPtrPtrControl) = 0;
        
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE createMisc( 
            /* [in] */ REFIID aREFIID,
            /* [out] */ IUnknown **aPtrPtrMisc) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct ICaptureManagerControlVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ICaptureManagerControl * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ICaptureManagerControl * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ICaptureManagerControl * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            ICaptureManagerControl * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            ICaptureManagerControl * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            ICaptureManagerControl * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            ICaptureManagerControl * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *createControl )( 
            ICaptureManagerControl * This,
            /* [in] */ REFIID aREFIID,
            /* [out] */ IUnknown **aPtrPtrControl);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *createMisc )( 
            ICaptureManagerControl * This,
            /* [in] */ REFIID aREFIID,
            /* [out] */ IUnknown **aPtrPtrMisc);
        
        END_INTERFACE
    } ICaptureManagerControlVtbl;

    interface ICaptureManagerControl
    {
        CONST_VTBL struct ICaptureManagerControlVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ICaptureManagerControl_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ICaptureManagerControl_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ICaptureManagerControl_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ICaptureManagerControl_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define ICaptureManagerControl_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define ICaptureManagerControl_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define ICaptureManagerControl_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define ICaptureManagerControl_createControl(This,aREFIID,aPtrPtrControl)	\
    ( (This)->lpVtbl -> createControl(This,aREFIID,aPtrPtrControl) ) 

#define ICaptureManagerControl_createMisc(This,aREFIID,aPtrPtrMisc)	\
    ( (This)->lpVtbl -> createMisc(This,aREFIID,aPtrPtrMisc) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ICaptureManagerControl_INTERFACE_DEFINED__ */


#ifndef __IVersionControl_INTERFACE_DEFINED__
#define __IVersionControl_INTERFACE_DEFINED__

/* interface IVersionControl */
/* [dual][helpstring][oleautomation][uuid][object] */ 


EXTERN_C const IID IID_IVersionControl;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("39DC3AEF-3B59-4C0D-A1B2-54BF2653C056")
    IVersionControl : public IDispatch
    {
    public:
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE getVersion( 
            /* [out] */ DWORD *aPtrMAJOR,
            /* [out] */ DWORD *aPtrMINOR,
            /* [out] */ DWORD *aPtrPATCH,
            /* [out] */ BSTR *aPtrPtrAdditionalLabel) = 0;
        
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE getXMLStringVersion( 
            /* [out] */ BSTR *aPtrPtrXMLstring) = 0;
        
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE checkVersion( 
            /* [in] */ DWORD aMAJOR,
            /* [in] */ DWORD aMINOR,
            /* [in] */ DWORD aPATCH,
            /* [out] */ boolean *aPtrResult) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IVersionControlVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IVersionControl * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IVersionControl * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IVersionControl * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IVersionControl * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IVersionControl * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IVersionControl * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IVersionControl * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *getVersion )( 
            IVersionControl * This,
            /* [out] */ DWORD *aPtrMAJOR,
            /* [out] */ DWORD *aPtrMINOR,
            /* [out] */ DWORD *aPtrPATCH,
            /* [out] */ BSTR *aPtrPtrAdditionalLabel);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *getXMLStringVersion )( 
            IVersionControl * This,
            /* [out] */ BSTR *aPtrPtrXMLstring);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *checkVersion )( 
            IVersionControl * This,
            /* [in] */ DWORD aMAJOR,
            /* [in] */ DWORD aMINOR,
            /* [in] */ DWORD aPATCH,
            /* [out] */ boolean *aPtrResult);
        
        END_INTERFACE
    } IVersionControlVtbl;

    interface IVersionControl
    {
        CONST_VTBL struct IVersionControlVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IVersionControl_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IVersionControl_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IVersionControl_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IVersionControl_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IVersionControl_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IVersionControl_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IVersionControl_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define IVersionControl_getVersion(This,aPtrMAJOR,aPtrMINOR,aPtrPATCH,aPtrPtrAdditionalLabel)	\
    ( (This)->lpVtbl -> getVersion(This,aPtrMAJOR,aPtrMINOR,aPtrPATCH,aPtrPtrAdditionalLabel) ) 

#define IVersionControl_getXMLStringVersion(This,aPtrPtrXMLstring)	\
    ( (This)->lpVtbl -> getXMLStringVersion(This,aPtrPtrXMLstring) ) 

#define IVersionControl_checkVersion(This,aMAJOR,aMINOR,aPATCH,aPtrResult)	\
    ( (This)->lpVtbl -> checkVersion(This,aMAJOR,aMINOR,aPATCH,aPtrResult) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IVersionControl_INTERFACE_DEFINED__ */


#ifndef __IEVRStreamControl_INTERFACE_DEFINED__
#define __IEVRStreamControl_INTERFACE_DEFINED__

/* interface IEVRStreamControl */
/* [dual][helpstring][oleautomation][uuid][object] */ 


EXTERN_C const IID IID_IEVRStreamControl;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("47F9883C-77B1-4A0B-9233-B3EAFA8F387E")
    IEVRStreamControl : public IDispatch
    {
    public:
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE setPosition( 
            /* [in] */ IUnknown *aPtrEVROutputNode,
            /* [in] */ FLOAT aLeft,
            /* [in] */ FLOAT aRight,
            /* [in] */ FLOAT aTop,
            /* [in] */ FLOAT aBottom) = 0;
        
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE setZOrder( 
            /* [in] */ IUnknown *aPtrEVROutputNode,
            /* [in] */ DWORD aZOrder) = 0;
        
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE getPosition( 
            /* [in] */ IUnknown *aPtrEVROutputNode,
            /* [out] */ FLOAT *aPtrLeft,
            /* [out] */ FLOAT *aPtrRight,
            /* [out] */ FLOAT *aPtrTop,
            /* [out] */ FLOAT *aPtrBottom) = 0;
        
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE getZOrder( 
            /* [in] */ IUnknown *aPtrEVROutputNode,
            /* [out] */ DWORD *aPtrZOrder) = 0;
        
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE flush( 
            /* [in] */ IUnknown *aPtrEVROutputNode) = 0;
        
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE setSrcPosition( 
            /* [in] */ IUnknown *aPtrEVROutputNode,
            /* [in] */ FLOAT aLeft,
            /* [in] */ FLOAT aRight,
            /* [in] */ FLOAT aTop,
            /* [in] */ FLOAT aBottom) = 0;
        
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE getSrcPosition( 
            /* [in] */ IUnknown *aPtrEVROutputNode,
            /* [out] */ FLOAT *aPtrLeft,
            /* [out] */ FLOAT *aPtrRight,
            /* [out] */ FLOAT *aPtrTop,
            /* [out] */ FLOAT *aPtrBottom) = 0;
        
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE getCollectionOfFilters( 
            /* [in] */ IUnknown *aPtrEVROutputNode,
            /* [out] */ BSTR *aPtrPtrXMLstring) = 0;
        
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE setFilterParametr( 
            /* [in] */ IUnknown *aPtrEVROutputNode,
            /* [in] */ DWORD aParametrIndex,
            /* [in] */ LONG aNewValue,
            /* [in] */ BOOL aIsEnabled) = 0;
        
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE getCollectionOfOutputFeatures( 
            /* [in] */ IUnknown *aPtrEVROutputNode,
            /* [out] */ BSTR *aPtrPtrXMLstring) = 0;
        
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE setOutputFeatureParametr( 
            /* [in] */ IUnknown *aPtrEVROutputNode,
            /* [in] */ DWORD aParametrIndex,
            /* [in] */ LONG aNewValue) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IEVRStreamControlVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IEVRStreamControl * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IEVRStreamControl * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IEVRStreamControl * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IEVRStreamControl * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IEVRStreamControl * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IEVRStreamControl * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IEVRStreamControl * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *setPosition )( 
            IEVRStreamControl * This,
            /* [in] */ IUnknown *aPtrEVROutputNode,
            /* [in] */ FLOAT aLeft,
            /* [in] */ FLOAT aRight,
            /* [in] */ FLOAT aTop,
            /* [in] */ FLOAT aBottom);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *setZOrder )( 
            IEVRStreamControl * This,
            /* [in] */ IUnknown *aPtrEVROutputNode,
            /* [in] */ DWORD aZOrder);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *getPosition )( 
            IEVRStreamControl * This,
            /* [in] */ IUnknown *aPtrEVROutputNode,
            /* [out] */ FLOAT *aPtrLeft,
            /* [out] */ FLOAT *aPtrRight,
            /* [out] */ FLOAT *aPtrTop,
            /* [out] */ FLOAT *aPtrBottom);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *getZOrder )( 
            IEVRStreamControl * This,
            /* [in] */ IUnknown *aPtrEVROutputNode,
            /* [out] */ DWORD *aPtrZOrder);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *flush )( 
            IEVRStreamControl * This,
            /* [in] */ IUnknown *aPtrEVROutputNode);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *setSrcPosition )( 
            IEVRStreamControl * This,
            /* [in] */ IUnknown *aPtrEVROutputNode,
            /* [in] */ FLOAT aLeft,
            /* [in] */ FLOAT aRight,
            /* [in] */ FLOAT aTop,
            /* [in] */ FLOAT aBottom);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *getSrcPosition )( 
            IEVRStreamControl * This,
            /* [in] */ IUnknown *aPtrEVROutputNode,
            /* [out] */ FLOAT *aPtrLeft,
            /* [out] */ FLOAT *aPtrRight,
            /* [out] */ FLOAT *aPtrTop,
            /* [out] */ FLOAT *aPtrBottom);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *getCollectionOfFilters )( 
            IEVRStreamControl * This,
            /* [in] */ IUnknown *aPtrEVROutputNode,
            /* [out] */ BSTR *aPtrPtrXMLstring);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *setFilterParametr )( 
            IEVRStreamControl * This,
            /* [in] */ IUnknown *aPtrEVROutputNode,
            /* [in] */ DWORD aParametrIndex,
            /* [in] */ LONG aNewValue,
            /* [in] */ BOOL aIsEnabled);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *getCollectionOfOutputFeatures )( 
            IEVRStreamControl * This,
            /* [in] */ IUnknown *aPtrEVROutputNode,
            /* [out] */ BSTR *aPtrPtrXMLstring);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *setOutputFeatureParametr )( 
            IEVRStreamControl * This,
            /* [in] */ IUnknown *aPtrEVROutputNode,
            /* [in] */ DWORD aParametrIndex,
            /* [in] */ LONG aNewValue);
        
        END_INTERFACE
    } IEVRStreamControlVtbl;

    interface IEVRStreamControl
    {
        CONST_VTBL struct IEVRStreamControlVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IEVRStreamControl_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IEVRStreamControl_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IEVRStreamControl_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IEVRStreamControl_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IEVRStreamControl_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IEVRStreamControl_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IEVRStreamControl_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define IEVRStreamControl_setPosition(This,aPtrEVROutputNode,aLeft,aRight,aTop,aBottom)	\
    ( (This)->lpVtbl -> setPosition(This,aPtrEVROutputNode,aLeft,aRight,aTop,aBottom) ) 

#define IEVRStreamControl_setZOrder(This,aPtrEVROutputNode,aZOrder)	\
    ( (This)->lpVtbl -> setZOrder(This,aPtrEVROutputNode,aZOrder) ) 

#define IEVRStreamControl_getPosition(This,aPtrEVROutputNode,aPtrLeft,aPtrRight,aPtrTop,aPtrBottom)	\
    ( (This)->lpVtbl -> getPosition(This,aPtrEVROutputNode,aPtrLeft,aPtrRight,aPtrTop,aPtrBottom) ) 

#define IEVRStreamControl_getZOrder(This,aPtrEVROutputNode,aPtrZOrder)	\
    ( (This)->lpVtbl -> getZOrder(This,aPtrEVROutputNode,aPtrZOrder) ) 

#define IEVRStreamControl_flush(This,aPtrEVROutputNode)	\
    ( (This)->lpVtbl -> flush(This,aPtrEVROutputNode) ) 

#define IEVRStreamControl_setSrcPosition(This,aPtrEVROutputNode,aLeft,aRight,aTop,aBottom)	\
    ( (This)->lpVtbl -> setSrcPosition(This,aPtrEVROutputNode,aLeft,aRight,aTop,aBottom) ) 

#define IEVRStreamControl_getSrcPosition(This,aPtrEVROutputNode,aPtrLeft,aPtrRight,aPtrTop,aPtrBottom)	\
    ( (This)->lpVtbl -> getSrcPosition(This,aPtrEVROutputNode,aPtrLeft,aPtrRight,aPtrTop,aPtrBottom) ) 

#define IEVRStreamControl_getCollectionOfFilters(This,aPtrEVROutputNode,aPtrPtrXMLstring)	\
    ( (This)->lpVtbl -> getCollectionOfFilters(This,aPtrEVROutputNode,aPtrPtrXMLstring) ) 

#define IEVRStreamControl_setFilterParametr(This,aPtrEVROutputNode,aParametrIndex,aNewValue,aIsEnabled)	\
    ( (This)->lpVtbl -> setFilterParametr(This,aPtrEVROutputNode,aParametrIndex,aNewValue,aIsEnabled) ) 

#define IEVRStreamControl_getCollectionOfOutputFeatures(This,aPtrEVROutputNode,aPtrPtrXMLstring)	\
    ( (This)->lpVtbl -> getCollectionOfOutputFeatures(This,aPtrEVROutputNode,aPtrPtrXMLstring) ) 

#define IEVRStreamControl_setOutputFeatureParametr(This,aPtrEVROutputNode,aParametrIndex,aNewValue)	\
    ( (This)->lpVtbl -> setOutputFeatureParametr(This,aPtrEVROutputNode,aParametrIndex,aNewValue) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IEVRStreamControl_INTERFACE_DEFINED__ */


#ifndef __IInitilaizeCaptureSource_INTERFACE_DEFINED__
#define __IInitilaizeCaptureSource_INTERFACE_DEFINED__

/* interface IInitilaizeCaptureSource */
/* [helpstring][oleautomation][uuid][object] */ 


EXTERN_C const IID IID_IInitilaizeCaptureSource;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("23B3BCE7-3003-48E6-9FA3-9F5F8439F3DC")
    IInitilaizeCaptureSource : public IUnknown
    {
    public:
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE setPresentationDescriptor( 
            /* [in] */ BSTR aXMLPresentationDescriptor) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IInitilaizeCaptureSourceVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IInitilaizeCaptureSource * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IInitilaizeCaptureSource * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IInitilaizeCaptureSource * This);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *setPresentationDescriptor )( 
            IInitilaizeCaptureSource * This,
            /* [in] */ BSTR aXMLPresentationDescriptor);
        
        END_INTERFACE
    } IInitilaizeCaptureSourceVtbl;

    interface IInitilaizeCaptureSource
    {
        CONST_VTBL struct IInitilaizeCaptureSourceVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IInitilaizeCaptureSource_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IInitilaizeCaptureSource_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IInitilaizeCaptureSource_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IInitilaizeCaptureSource_setPresentationDescriptor(This,aXMLPresentationDescriptor)	\
    ( (This)->lpVtbl -> setPresentationDescriptor(This,aXMLPresentationDescriptor) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IInitilaizeCaptureSource_INTERFACE_DEFINED__ */


#ifndef __ICurrentMediaType_INTERFACE_DEFINED__
#define __ICurrentMediaType_INTERFACE_DEFINED__

/* interface ICurrentMediaType */
/* [helpstring][oleautomation][uuid][object] */ 


EXTERN_C const IID IID_ICurrentMediaType;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("2835286D-C2AF-4A66-AEDA-3ABB8A244E86")
    ICurrentMediaType : public IUnknown
    {
    public:
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE getMediaTypeIndex( 
            /* [out] */ DWORD *aPtrMediaTypeIndex) = 0;
        
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE getStreamIndex( 
            /* [out] */ DWORD *aPtrStreamIndex) = 0;
        
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE getMediaType( 
            /* [out] */ IUnknown **aPtrMediaType) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct ICurrentMediaTypeVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ICurrentMediaType * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ICurrentMediaType * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ICurrentMediaType * This);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *getMediaTypeIndex )( 
            ICurrentMediaType * This,
            /* [out] */ DWORD *aPtrMediaTypeIndex);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *getStreamIndex )( 
            ICurrentMediaType * This,
            /* [out] */ DWORD *aPtrStreamIndex);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *getMediaType )( 
            ICurrentMediaType * This,
            /* [out] */ IUnknown **aPtrMediaType);
        
        END_INTERFACE
    } ICurrentMediaTypeVtbl;

    interface ICurrentMediaType
    {
        CONST_VTBL struct ICurrentMediaTypeVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ICurrentMediaType_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ICurrentMediaType_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ICurrentMediaType_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ICurrentMediaType_getMediaTypeIndex(This,aPtrMediaTypeIndex)	\
    ( (This)->lpVtbl -> getMediaTypeIndex(This,aPtrMediaTypeIndex) ) 

#define ICurrentMediaType_getStreamIndex(This,aPtrStreamIndex)	\
    ( (This)->lpVtbl -> getStreamIndex(This,aPtrStreamIndex) ) 

#define ICurrentMediaType_getMediaType(This,aPtrMediaType)	\
    ( (This)->lpVtbl -> getMediaType(This,aPtrMediaType) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ICurrentMediaType_INTERFACE_DEFINED__ */


#ifndef __ISourceRequestResult_INTERFACE_DEFINED__
#define __ISourceRequestResult_INTERFACE_DEFINED__

/* interface ISourceRequestResult */
/* [helpstring][local][oleautomation][uuid][object] */ 


EXTERN_C const IID IID_ISourceRequestResult;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("80561D39-612C-42AF-B866-5E2B2E6F39C7")
    ISourceRequestResult : public IUnknown
    {
    public:
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE setData( 
            /* [in] */ LPVOID aPtrData,
            /* [in] */ DWORD aByteSize,
            /* [in] */ BOOL aIsKeyFrame) = 0;
        
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE getStreamIndex( 
            /* [out] */ DWORD *aPtrStreamIndex) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct ISourceRequestResultVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ISourceRequestResult * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ISourceRequestResult * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ISourceRequestResult * This);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *setData )( 
            ISourceRequestResult * This,
            /* [in] */ LPVOID aPtrData,
            /* [in] */ DWORD aByteSize,
            /* [in] */ BOOL aIsKeyFrame);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *getStreamIndex )( 
            ISourceRequestResult * This,
            /* [out] */ DWORD *aPtrStreamIndex);
        
        END_INTERFACE
    } ISourceRequestResultVtbl;

    interface ISourceRequestResult
    {
        CONST_VTBL struct ISourceRequestResultVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ISourceRequestResult_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ISourceRequestResult_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ISourceRequestResult_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ISourceRequestResult_setData(This,aPtrData,aByteSize,aIsKeyFrame)	\
    ( (This)->lpVtbl -> setData(This,aPtrData,aByteSize,aIsKeyFrame) ) 

#define ISourceRequestResult_getStreamIndex(This,aPtrStreamIndex)	\
    ( (This)->lpVtbl -> getStreamIndex(This,aPtrStreamIndex) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ISourceRequestResult_INTERFACE_DEFINED__ */


#ifndef __ICaptureProcessor_INTERFACE_DEFINED__
#define __ICaptureProcessor_INTERFACE_DEFINED__

/* interface ICaptureProcessor */
/* [helpstring][oleautomation][uuid][object] */ 


EXTERN_C const IID IID_ICaptureProcessor;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("F6591EF6-535E-4A83-BE0B-87135B173C78")
    ICaptureProcessor : public IUnknown
    {
    public:
        virtual /* [helpstring] */ HRESULT STDMETHODCALLTYPE start( 
            /* [in] */ LONGLONG aStartPositionInHundredNanosecondUnits,
            /* [in] */ REFGUID aGUIDTimeFormat) = 0;
        
        virtual /* [helpstring] */ HRESULT STDMETHODCALLTYPE stop( void) = 0;
        
        virtual /* [helpstring] */ HRESULT STDMETHODCALLTYPE pause( void) = 0;
        
        virtual /* [helpstring] */ HRESULT STDMETHODCALLTYPE shutdown( void) = 0;
        
        virtual /* [helpstring] */ HRESULT STDMETHODCALLTYPE initilaize( 
            /* [in] */ IUnknown *aPtrIInitilaizeCaptureSource) = 0;
        
        virtual /* [helpstring] */ HRESULT STDMETHODCALLTYPE setCurrentMediaType( 
            /* [in] */ IUnknown *aPtrICurrentMediaType) = 0;
        
        virtual /* [helpstring] */ HRESULT STDMETHODCALLTYPE sourceRequest( 
            /* [in] */ IUnknown *aPtrISourceRequestResult) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct ICaptureProcessorVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ICaptureProcessor * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ICaptureProcessor * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ICaptureProcessor * This);
        
        /* [helpstring] */ HRESULT ( STDMETHODCALLTYPE *start )( 
            ICaptureProcessor * This,
            /* [in] */ LONGLONG aStartPositionInHundredNanosecondUnits,
            /* [in] */ REFGUID aGUIDTimeFormat);
        
        /* [helpstring] */ HRESULT ( STDMETHODCALLTYPE *stop )( 
            ICaptureProcessor * This);
        
        /* [helpstring] */ HRESULT ( STDMETHODCALLTYPE *pause )( 
            ICaptureProcessor * This);
        
        /* [helpstring] */ HRESULT ( STDMETHODCALLTYPE *shutdown )( 
            ICaptureProcessor * This);
        
        /* [helpstring] */ HRESULT ( STDMETHODCALLTYPE *initilaize )( 
            ICaptureProcessor * This,
            /* [in] */ IUnknown *aPtrIInitilaizeCaptureSource);
        
        /* [helpstring] */ HRESULT ( STDMETHODCALLTYPE *setCurrentMediaType )( 
            ICaptureProcessor * This,
            /* [in] */ IUnknown *aPtrICurrentMediaType);
        
        /* [helpstring] */ HRESULT ( STDMETHODCALLTYPE *sourceRequest )( 
            ICaptureProcessor * This,
            /* [in] */ IUnknown *aPtrISourceRequestResult);
        
        END_INTERFACE
    } ICaptureProcessorVtbl;

    interface ICaptureProcessor
    {
        CONST_VTBL struct ICaptureProcessorVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ICaptureProcessor_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ICaptureProcessor_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ICaptureProcessor_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ICaptureProcessor_start(This,aStartPositionInHundredNanosecondUnits,aGUIDTimeFormat)	\
    ( (This)->lpVtbl -> start(This,aStartPositionInHundredNanosecondUnits,aGUIDTimeFormat) ) 

#define ICaptureProcessor_stop(This)	\
    ( (This)->lpVtbl -> stop(This) ) 

#define ICaptureProcessor_pause(This)	\
    ( (This)->lpVtbl -> pause(This) ) 

#define ICaptureProcessor_shutdown(This)	\
    ( (This)->lpVtbl -> shutdown(This) ) 

#define ICaptureProcessor_initilaize(This,aPtrIInitilaizeCaptureSource)	\
    ( (This)->lpVtbl -> initilaize(This,aPtrIInitilaizeCaptureSource) ) 

#define ICaptureProcessor_setCurrentMediaType(This,aPtrICurrentMediaType)	\
    ( (This)->lpVtbl -> setCurrentMediaType(This,aPtrICurrentMediaType) ) 

#define ICaptureProcessor_sourceRequest(This,aPtrISourceRequestResult)	\
    ( (This)->lpVtbl -> sourceRequest(This,aPtrISourceRequestResult) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ICaptureProcessor_INTERFACE_DEFINED__ */


#ifndef __IRenderingControl_INTERFACE_DEFINED__
#define __IRenderingControl_INTERFACE_DEFINED__

/* interface IRenderingControl */
/* [helpstring][local][oleautomation][uuid][object] */ 


EXTERN_C const IID IID_IRenderingControl;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("1CBCAF1C-1809-41DE-A728-23DBF86A6170")
    IRenderingControl : public IDispatch
    {
    public:
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE enableInnerRendering( 
            /* [in] */ IUnknown *aPtrEVROutputNode,
            /* [in] */ BOOL aIsInnerRendering) = 0;
        
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE renderToTarget( 
            /* [in] */ IUnknown *aPtrEVROutputNode,
            /* [in] */ IUnknown *aPtrRenderTarget,
            /* [in] */ BOOL aCopyMode) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IRenderingControlVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IRenderingControl * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IRenderingControl * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IRenderingControl * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IRenderingControl * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IRenderingControl * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IRenderingControl * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IRenderingControl * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *enableInnerRendering )( 
            IRenderingControl * This,
            /* [in] */ IUnknown *aPtrEVROutputNode,
            /* [in] */ BOOL aIsInnerRendering);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *renderToTarget )( 
            IRenderingControl * This,
            /* [in] */ IUnknown *aPtrEVROutputNode,
            /* [in] */ IUnknown *aPtrRenderTarget,
            /* [in] */ BOOL aCopyMode);
        
        END_INTERFACE
    } IRenderingControlVtbl;

    interface IRenderingControl
    {
        CONST_VTBL struct IRenderingControlVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IRenderingControl_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IRenderingControl_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IRenderingControl_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IRenderingControl_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IRenderingControl_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IRenderingControl_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IRenderingControl_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define IRenderingControl_enableInnerRendering(This,aPtrEVROutputNode,aIsInnerRendering)	\
    ( (This)->lpVtbl -> enableInnerRendering(This,aPtrEVROutputNode,aIsInnerRendering) ) 

#define IRenderingControl_renderToTarget(This,aPtrEVROutputNode,aPtrRenderTarget,aCopyMode)	\
    ( (This)->lpVtbl -> renderToTarget(This,aPtrEVROutputNode,aPtrRenderTarget,aCopyMode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IRenderingControl_INTERFACE_DEFINED__ */


#ifndef __ISwitcherControl_INTERFACE_DEFINED__
#define __ISwitcherControl_INTERFACE_DEFINED__

/* interface ISwitcherControl */
/* [helpstring][local][oleautomation][uuid][object] */ 


EXTERN_C const IID IID_ISwitcherControl;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("9D9FEEFB-0905-4829-866E-22AD28320183")
    ISwitcherControl : public IDispatch
    {
    public:
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE pauseSwitchers( 
            /* [in] */ DWORD aSessionDescriptor) = 0;
        
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE resumeSwitchers( 
            /* [in] */ DWORD aSessionDescriptor) = 0;
        
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE pauseSwitcher( 
            /* [in] */ IUnknown *aPtrSwitcherNode) = 0;
        
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE resumeSwitcher( 
            /* [in] */ IUnknown *aPtrSwitcherNode) = 0;
        
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE detachSwitchers( 
            /* [in] */ DWORD aSessionDescriptor) = 0;
        
        virtual /* [id][helpstring] */ HRESULT STDMETHODCALLTYPE attachSwitcher( 
            /* [in] */ IUnknown *aPtrSwitcherNode,
            /* [in] */ IUnknown *aPtrDownStreamNode) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct ISwitcherControlVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ISwitcherControl * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ISwitcherControl * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ISwitcherControl * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            ISwitcherControl * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            ISwitcherControl * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            ISwitcherControl * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            ISwitcherControl * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *pauseSwitchers )( 
            ISwitcherControl * This,
            /* [in] */ DWORD aSessionDescriptor);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *resumeSwitchers )( 
            ISwitcherControl * This,
            /* [in] */ DWORD aSessionDescriptor);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *pauseSwitcher )( 
            ISwitcherControl * This,
            /* [in] */ IUnknown *aPtrSwitcherNode);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *resumeSwitcher )( 
            ISwitcherControl * This,
            /* [in] */ IUnknown *aPtrSwitcherNode);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *detachSwitchers )( 
            ISwitcherControl * This,
            /* [in] */ DWORD aSessionDescriptor);
        
        /* [id][helpstring] */ HRESULT ( STDMETHODCALLTYPE *attachSwitcher )( 
            ISwitcherControl * This,
            /* [in] */ IUnknown *aPtrSwitcherNode,
            /* [in] */ IUnknown *aPtrDownStreamNode);
        
        END_INTERFACE
    } ISwitcherControlVtbl;

    interface ISwitcherControl
    {
        CONST_VTBL struct ISwitcherControlVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ISwitcherControl_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define ISwitcherControl_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define ISwitcherControl_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define ISwitcherControl_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define ISwitcherControl_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define ISwitcherControl_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define ISwitcherControl_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define ISwitcherControl_pauseSwitchers(This,aSessionDescriptor)	\
    ( (This)->lpVtbl -> pauseSwitchers(This,aSessionDescriptor) ) 

#define ISwitcherControl_resumeSwitchers(This,aSessionDescriptor)	\
    ( (This)->lpVtbl -> resumeSwitchers(This,aSessionDescriptor) ) 

#define ISwitcherControl_pauseSwitcher(This,aPtrSwitcherNode)	\
    ( (This)->lpVtbl -> pauseSwitcher(This,aPtrSwitcherNode) ) 

#define ISwitcherControl_resumeSwitcher(This,aPtrSwitcherNode)	\
    ( (This)->lpVtbl -> resumeSwitcher(This,aPtrSwitcherNode) ) 

#define ISwitcherControl_detachSwitchers(This,aSessionDescriptor)	\
    ( (This)->lpVtbl -> detachSwitchers(This,aSessionDescriptor) ) 

#define ISwitcherControl_attachSwitcher(This,aPtrSwitcherNode,aPtrDownStreamNode)	\
    ( (This)->lpVtbl -> attachSwitcher(This,aPtrSwitcherNode,aPtrDownStreamNode) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __ISwitcherControl_INTERFACE_DEFINED__ */



#ifndef __CaptureManagerLibrary_LIBRARY_DEFINED__
#define __CaptureManagerLibrary_LIBRARY_DEFINED__

/* library CaptureManagerLibrary */
/* [helpstring][version][uuid] */ 



































EXTERN_C const IID LIBID_CaptureManagerLibrary;

EXTERN_C const CLSID CLSID_CoLogPrintOut;

#ifdef __cplusplus

class DECLSPEC_UUID("4563EE3E-DA1E-4911-9F40-88A284E2DD69")
CoLogPrintOut;
#endif

EXTERN_C const CLSID CLSID_CoCaptureManager;

#ifdef __cplusplus

class DECLSPEC_UUID("D5F07FB8-CE60-4017-B215-95C8A0DDF42A")
CoCaptureManager;
#endif
#endif /* __CaptureManagerLibrary_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

unsigned long             __RPC_USER  BSTR_UserSize(     unsigned long *, unsigned long            , BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserMarshal(  unsigned long *, unsigned char *, BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserUnmarshal(unsigned long *, unsigned char *, BSTR * ); 
void                      __RPC_USER  BSTR_UserFree(     unsigned long *, BSTR * ); 

unsigned long             __RPC_USER  VARIANT_UserSize(     unsigned long *, unsigned long            , VARIANT * ); 
unsigned char * __RPC_USER  VARIANT_UserMarshal(  unsigned long *, unsigned char *, VARIANT * ); 
unsigned char * __RPC_USER  VARIANT_UserUnmarshal(unsigned long *, unsigned char *, VARIANT * ); 
void                      __RPC_USER  VARIANT_UserFree(     unsigned long *, VARIANT * ); 

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


