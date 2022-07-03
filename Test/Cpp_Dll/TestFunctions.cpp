#include "pch.h"
#include <iostream>
#include "../../DNINative/include/DNI/DNI.h"

extern "C"
{
	_declspec(dllexport) void voidFunction()
	{
		std::cout << "from a void function" << std::endl;
	}

	_declspec(dllexport) int intFunction(int i)
	{
		return i;
	}

	_declspec(dllexport) double doubleFunction(double d)
	{
		return d;
	}

	_declspec(dllexport) char charFunction(char c)
	{
		return c;
	}

	_declspec(dllexport) DNI::Types::DNIArray* intArrayFunction(DNI::DNI* pDni, DNI::Types::DNIIntArray* pArray)
	{
		unsigned int length = pDni->GetArraySizePtr(pArray);
		int* nativeIntArray = new int[length + 1];
		nativeIntArray[length] = 0;
		int copied = pDni->GetIntArrayElementsPtr(pArray, nativeIntArray, 0, length);
		DNI::Types::DNIIntArray* pArrayOut = pDni->NewIntArrayPtr(length);
		pDni->SetIntArrayElementsPtr(pArrayOut, nativeIntArray, 0, length);
		return pArrayOut;
	}


	_declspec(dllexport) DNI::Types::DNIString stringFunction(DNI::DNI* pDni, char* string)
	{
		std::cout << string << std::endl;		
		return convert(pDni, string);
	}
}