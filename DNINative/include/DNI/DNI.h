#pragma once

#include <map>

namespace DNI
{

	namespace Types
	{
		struct _DNIObject {}; using DNIObject = _DNIObject*;
		struct _DNIObjectArray : public _DNIObject {}; using DNIObjectArray = _DNIObjectArray*;
		struct _DNIString : public _DNIObject {};  using DNIString	 = _DNIString*;
		struct _DNIArray : public _DNIObject {};   using DNIArray	 = _DNIArray*;
		struct _DNIIntArray : public _DNIArray {}; using DNIIntArray = _DNIIntArray*;
	}

	using PtrGetBoolFromObjectPtr			= bool(*)(const Types::DNIObject ptr);
	using PtrGetIntFromObjectPtr			= int(*)(const Types::DNIObject ptr);
	using PtrGetDoubleFromObjectPtr			= double(*)(const Types::DNIObject ptr);

	using PtrBoolToObject					= Types::DNIObject (*) (bool boolVal);
	using PtrIntToObject					= Types::DNIObject (*) (int intVal);
	using PtrDoubleToObject					= Types::DNIObject (*) (double dbl);

	// array functions
	using PtrNewObjectArray					= Types::DNIObjectArray(*)(Types::DNIString strObjectTypeName, int size);
	using PtrGetArraySizePtr				= int(*) (const Types::DNIArray ptr);
	using PtrSetArrayElementsPtr			= int(*)(Types::DNIObjectArray managedArrayObject, Types::DNIObject* srcNativeArrayPtr, const int startIndex, const int length);
	using PtrGetIntArrayElements			= int(*)(const Types::DNIIntArray managedArrayObject, int* destNativeArrayPtr, const int startIndex, const int length);
	using PtrSetIntArrayElementsPtr			= int(*)(Types::DNIIntArray managedArrayObject, const int* srcNativeArrayPtr, const int startIndex, const int length);
	using PtrNewIntArrayPtr					= Types::DNIIntArray(*)(const int size);
	
	
	// reflection
	using PtrNewObjectArray					= Types::DNIObjectArray (*)(const Types::DNIString strObjectTypeName, const int size);
	using PtrGetMethodPtr					= Types::DNIObject (*)(const Types::DNIObject managedObject, const Types::DNIString methodName, const Types::DNIString signature);
	using PtrInvokeMethodPtr				= Types::DNIObject (*)(const Types::DNIObject managedObject, const Types::DNIObject methodPtr,  const Types::DNIObject parameters);
	using PtrGetPropertyPtr					= Types::DNIObject (*)(const Types::DNIObject managedObject, const Types::DNIString propertName);
	using PtrGetGenericType					= Types::DNIObject (*)(const Types::DNIString typeName,		 const Types::DNIString parameters);
	using PtrCreateInstance					= Types::DNIObject (*)(const Types::DNIObject type, Types::DNIObjectArray parameters);
	
	// string function
	using PtrCreateManagedStringFromCharPtr = Types::DNIString (*)(const void* ptr, const int len);
	using PtrGetStringLength				= int(*)(Types::DNIObject stringPtr);
	using PtrStringToANSIStringPtr			= char*(*)(Types::DNIObject stringPtr);
	using PtrFreeCoTaskMemPtr				= void(*)(char* nativeCharPtr);

	struct DNI
	{
		
		PtrGetBoolFromObjectPtr			  GetBoolFromObject;
		PtrGetIntFromObjectPtr			  GetIntFromObject;
		PtrGetDoubleFromObjectPtr		  GetDoubleFromObject;

		PtrBoolToObject					  BoolToObject;
		PtrIntToObject					  IntToObject;
		PtrDoubleToObject				  DoubleToObject;

		PtrNewObjectArray				  NewObjectArray;
		PtrGetArraySizePtr                GetArraySize;
		PtrSetArrayElementsPtr			  SetArrayElementsPtr;
		PtrGetIntArrayElements			  GetIntArrayElements;
		PtrSetIntArrayElementsPtr		  SetIntArrayElements;
		PtrNewIntArrayPtr				  NewIntArray;
		
		// reflection function
		PtrGetMethodPtr					  GetMethod;
		PtrInvokeMethodPtr				  InvokeMethod;
		PtrGetPropertyPtr				  GetProperty;
		PtrGetGenericType				  GetGenericType;
		PtrCreateInstance				  CreateInstance;

		// string function
		PtrCreateManagedStringFromCharPtr CreateManagedStringFromChar;
		PtrGetStringLength				  GetStringLength;
		PtrStringToANSIStringPtr		  StringToANSIString;
		PtrFreeCoTaskMemPtr				  FreeCoTaskMem;
	};

}