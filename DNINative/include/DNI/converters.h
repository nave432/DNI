#pragma once

#include "DNI.h"
#include <map>
#include <string>
//#include <WTypes.h>

namespace DNI
{

	template<typename Ret, typename Input>
	Ret convertTo(DNI* pDni, const Input& in);

	// covert to managed objects
	bool convert(DNI* pDni, const char* input, Types::DNIString& out)
	{
		size_t len = strlen(input);
		out = pDni->CreateManagedStringFromChar(input, (int)len);
		return true;
	}

	bool convert(DNI* pDni, const int input, Types::DNIObject& out)
	{
		out = pDni->IntToObject(input);
		return true;
	}

	bool convert(DNI* pDni, const bool input, Types::DNIObject& out)
	{
		out = pDni->BoolToObject(input);
		return true;
	}

	bool convert(DNI* pDni, const double input, Types::DNIObject& out)
	{
		out = pDni->DoubleToObject(input);
		return true;
	}

	bool convert(DNI* pDni, const std::string& input, Types::DNIObject& out)
	{
		out = pDni->CreateManagedStringFromChar(input.c_str(), (int)input.length());
		return true;
	}

	template< typename T1, typename T2>
	bool convert(DNI* pDni, const std::map<T1, T2>& in, Types::DNIObject& out)
	{
		Types::DNIString strTypeName = convertTo<Types::DNIString>(pDni, "System.Collections.Generic.Dictionary`2");
		Types::DNIString strGenericParameters = convertTo<Types::DNIString>(pDni, "System.String,System.String");
		Types::DNIObject dictType = pDni->GetGenericType(strTypeName, strGenericParameters);
		Types::DNIObject dicObject = pDni->CreateInstance(dictType, nullptr);
		
		Types::DNIString addMethodName = convertTo<Types::DNIString>(pDni, "Add");
		Types::DNIObject addMethod = pDni->GetMethod(dicObject, addMethodName, strGenericParameters);
		
		
		for (const auto& item : in)
		{
			Types::DNIObject key = convertTo<Types::DNIObject>(pDni, item.first);
			Types::DNIObject val = convertTo<Types::DNIObject>(pDni, item.second);
			//pDni->InvokeMethod(dicObject, addMethod)
		}
		out = dicObject;
		return true;
	}

	// covert to native types
	bool convert(DNI* pDni, const Types::DNIObject in, bool& out)
	{
		out = pDni->GetBoolFromObject(in);
		return true;
	}

	bool convert(DNI* pDni, const Types::DNIObject in, std::string& out)
	{
		char* pStr = pDni->StringToANSIString(in);
		out = std::move(std::string(pStr));
		pDni->FreeCoTaskMem(pStr);
		return true;
	}

	bool convert(DNI* pDni, const  Types::DNIObject in, int& out)
	{
		out = pDni->GetIntFromObject(in);
		return true;
	}

	bool convert(DNI* pDni, const  Types::DNIObject in, double& out)
	{
		out = pDni->GetDoubleFromObject(in);
		return true;
	}

	template< typename T1, typename T2>
	bool convert(DNI* pDni, Types::DNIObject input, std::map<T1, T2>& out)
	{
		Types::DNIString strEmptyParams = convertTo<Types::DNIString>(pDni, "");
		Types::DNIObject emptyParams = nullptr;
		// 1. GetEnumerator
		Types::DNIObject itr = nullptr;
		{
			Types::DNIString strGetEnumeratorMethod = convertTo<Types::DNIString>(pDni, "GetEnumerator");
			Types::DNIObject pMethod = pDni->GetMethod(input, strGetEnumeratorMethod, strEmptyParams);
			itr = pDni->InvokeMethod(input, pMethod, emptyParams);
		}

		// 2. Get MoveNext method
		Types::DNIObject moveNextMethod = nullptr;
		{
			Types::DNIString strGetEnumeratorMethod = convertTo<Types::DNIString>(pDni, "MoveNext");
			moveNextMethod = pDni->GetMethod(itr, strGetEnumeratorMethod, strEmptyParams);
		}

		Types::DNIString strCurrentProperty = convertTo<Types::DNIString>(pDni, "Current");
		Types::DNIString strKeyProperty	 = convertTo<Types::DNIString>(pDni, "Key");
		Types::DNIString strValueProperty	 = convertTo<Types::DNIString>(pDni, "Value");
		// 3. call MoveNext
		while(convertTo<bool>(pDni, pDni->InvokeMethod(itr, moveNextMethod, emptyParams)))
		{
			Types::DNIObject currentItem = pDni->GetProperty(itr, strCurrentProperty);
			Types::DNIObject keyPtr = pDni->GetProperty(currentItem, strKeyProperty);
			Types::DNIObject valuePtr = pDni->GetProperty(currentItem, strValueProperty);
			T1 key	 = convertTo<T1>(pDni, keyPtr);
			T2 value = convertTo<T2>(pDni, valuePtr);
			out.insert(std::make_pair(key, value));
		}
		return true;
	}

	template<typename Ret, typename Input>
	Ret convertTo(DNI* pDni, const Input& in)
	{
		Ret temp;
		convert(pDni, in, temp);
		return temp;
	}

}