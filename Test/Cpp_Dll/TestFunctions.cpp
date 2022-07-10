#include "pch.h"
#include <iostream>
#include "../../DNINative/include/DNI/DNI.h"
#include "../../DNINative/include/DNI/converters.h"

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

	_declspec(dllexport) DNI::Types::DNIArray intArrayFunction(DNI::DNI* pDni, DNI::Types::DNIIntArray pArray)
	{
		unsigned int length = pDni->GetArraySize(pArray);
		int* nativeIntArray = new int[length];
		int copied = pDni->GetIntArrayElements(pArray, nativeIntArray, 0, length);
		DNI::Types::DNIIntArray pArrayOut = pDni->NewIntArray(length);
		pDni->SetIntArrayElements(pArrayOut, nativeIntArray, 0, length);
		return pArrayOut;
	}


	_declspec(dllexport) DNI::Types::DNIString stringFunction(DNI::DNI* pDni, char* string)
	{
		std::cout << string << std::endl;		
		DNI::Types::DNIString strRet = DNI::convertTo<DNI::Types::DNIString>(pDni, string);
		return strRet;
	}

	_declspec(dllexport) DNI::Types::DNIObject functionTakingDictionary(DNI::DNI* pDni, DNI::Types::DNIObject input)
	{
		std::map<std::string, int> mapRet = DNI::convertTo<std::map<std::string, int> >(pDni, input);
		std::map<std::string, std::string> output;
		for (auto item : mapRet)
		{
			output[item.first] = std::to_string(item.second);
		}
		return DNI::convertTo<DNI::Types::DNIObject>(pDni, output);
	}
}