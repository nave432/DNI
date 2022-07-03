#pragma once



namespace DNI
{

	namespace Types
	{
		using DNIString = void*;

		struct DNIArray{};
		struct DNIIntArray : public DNIArray{};
	}

	using PtrCreateManagedStringFromCharPtr = void* (*)(void* ptr, int len);
	using PtrGetArraySizePtr				= int(*) (Types::DNIArray* ptr);
	using PtrGetIntArrayElements			= int(*)(Types::DNIIntArray* managedArrayObject, int* destNativeArrayPtr, int startIndex, int length);
	using PtrSetIntArrayElementsPtr			= int(*)(Types::DNIIntArray* managedArrayObject, int* srcNativeArrayPtr, int startIndex, int length);
	using PtrNewIntArrayPtr					= Types::DNIIntArray*(*)(int size);
	struct DNI
	{
		PtrCreateManagedStringFromCharPtr CreateManagedStringFromCharPtr;
		PtrGetArraySizePtr                GetArraySizePtr;
		PtrGetIntArrayElements			  GetIntArrayElementsPtr;
		PtrSetIntArrayElementsPtr		  SetIntArrayElementsPtr;
		PtrNewIntArrayPtr				  NewIntArrayPtr;
	};

	template<class Dest, class Src>
	Dest convert(DNI* pDni, Src* src)
	{
		
	}

	Types::DNIString convert(DNI* pDni, char* src)
	{
		size_t len = strlen(src);
		return pDni->CreateManagedStringFromCharPtr(src, (int)len);
	}
}