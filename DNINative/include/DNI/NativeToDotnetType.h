#pragma once

#include "DNI.h"
#include <string>
#include <map>

namespace DNI
{
	static const std::string GetStringTypeName()
	{
		return "System.String,System.String";
	}

	template<typename T1, typename T2>
	static const std::string GetTypeName(const std::map<T1,T2>*)
	{
		return "System.Collections.Generic.Dictionary`2";
	}

	static const std::string GetTypeName(const std::string*)
	{
		return "System.String";
	}
}