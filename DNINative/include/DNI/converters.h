#pragma once

#include "DNI.h"
#include "NativeToDotnetType.h"
#include <map>
#include <string>
#include <vector>


namespace DNI
{

	template<typename Ret, typename Input>
	Ret convertTo(DNI* pDni, const Input& in);

	// covert to managed objects
	static bool convert(DNI* pDni, const char* input, Types::DNIString& out)
	{
		size_t len = strlen(input);
		out = pDni->CreateManagedStringFromChar(input, (int)len);
		return true;
	}

	static bool convert(DNI* pDni, const std::string& input, Types::DNIString& out)
	{
		out = pDni->CreateManagedStringFromChar(input.c_str(), (int)input.length());
		return true;
	}

	static bool convert(DNI* pDni, const int input, Types::DNIObject& out)
	{
		out = pDni->IntToObject(input);
		return true;
	}

	static bool convert(DNI* pDni, const bool input, Types::DNIObject& out)
	{
		out = pDni->BoolToObject(input);
		return true;
	}

	static bool convert(DNI* pDni, const double input, Types::DNIObject& out)
	{
		out = pDni->DoubleToObject(input);
		return true;
	}

	static bool convert(DNI* pDni, const std::string& input, Types::DNIObject& out)
	{
		out = pDni->CreateManagedStringFromChar(input.c_str(), (int)input.length());
		return true;
	}

	template< typename T1, typename T2>
	static bool convert(DNI* pDni, const std::map<T1, T2>& in, Types::DNIObject& out)
	{
		const std::string& strMapType = GetTypeName(&in);
		const std::string& strArg1Type = GetTypeName((T1*)(nullptr));
		const std::string& strArg2Type = GetTypeName((T1*)(nullptr));
		const std::string& args = strArg1Type + "," + strArg2Type;
		const Types::DNIString strTypeName = convertTo<Types::DNIString>(pDni, strMapType);
		Types::DNIString strGenericParameters = convertTo<Types::DNIString>(pDni, args);
		Types::DNIObject dictType = pDni->GetGenericType(strTypeName, strGenericParameters);
		Types::DNIObject dicObject = pDni->CreateInstance(dictType, nullptr);
		
		Types::DNIString addMethodName = convertTo<Types::DNIString>(pDni, "Add");
		Types::DNIObject addMethod = pDni->GetMethod(dicObject, addMethodName, strGenericParameters);
		
		Types::DNIString objectNamestr = convertTo<Types::DNIString>(pDni, "System.Object");
		Types::DNIObjectArray objArray = pDni->NewObjectArray(objectNamestr, 2);
		for (const auto& item : in)
		{
			Types::DNIObject key = convertTo<Types::DNIObject>(pDni, item.first);
			Types::DNIObject val = convertTo<Types::DNIObject>(pDni, item.second);
			Types::DNIObject params[]{ key, val };
			pDni->SetArrayElementsPtr(objArray, params,0,2);
			pDni->InvokeMethod(dicObject, addMethod, objArray);
		}
		out = dicObject;
		return true;
	}

	static bool convert(DNI* pDni, const std::vector<int>& input, Types::DNIIntArray& out)
	{
		int length = (int)input.size();
		out = pDni->NewIntArray(length);
		pDni->SetIntArrayElements(out, &input[0], 0, length);
		return true;
	}

	// covert to native types
	static bool convert(DNI* pDni, const char* input, std::string& out)
	{
		size_t len = strlen(input);
		out = std::move(std::string(input,len));
		return true;
	}

	static bool convert(DNI* pDni, const Types::DNIObject in, bool& out)
	{
		out = pDni->GetBoolFromObject(in);
		return true;
	}

	static bool convert(DNI* pDni, const Types::DNIObject in, std::string& out)
	{
		char* pStr = pDni->StringToANSIString(in);
		out = std::move(std::string(pStr));
		pDni->FreeCoTaskMem(pStr);
		return true;
	}

	static bool convert(DNI* pDni, const Types::DNIString in, std::string& out)
	{
		return convert(pDni, (Types::DNIObject)in, out);
	}

	static bool convert(DNI* pDni, const  Types::DNIObject in, int& out)
	{
		out = pDni->GetIntFromObject(in);
		return true;
	}

	static bool convert(DNI* pDni, const  Types::DNIObject in, double& out)
	{
		out = pDni->GetDoubleFromObject(in);
		return true;
	}

	template< typename T1, typename T2>
	static bool convert(DNI* pDni, Types::DNIObject input, std::map<T1, T2>& out)
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

	static bool convert(DNI* pDni, Types::DNIIntArray input, std::vector<int>& out)
	{
		const int length = pDni->GetArraySize(input);
		if (length <= 0)
			return false;
		out.resize(length);
		const int copied = pDni->GetIntArrayElements(input, &out[0], 0, length);
		return copied == length;
	}

	//enum converter
	template< typename T>
	static bool convert(DNI* pDni, Types::DNIEnum input, T& out)
	{
		out = (T)input;
		return true;
	}

	template<typename Ret, typename Input>
	static Ret convertTo(DNI* pDni, const Input& in)
	{
		Ret temp;
		convert(pDni, in, temp);
		return temp;
	}

}