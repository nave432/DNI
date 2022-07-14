#include "pch.h"
#include "../../DNINative/include/DNI/DNI.h"
#include "../../DNINative/include/DNI/converters.h"
#include "Zoo.h"
#include <string>

using namespace ClassTest;
using namespace DNI::Types;

extern "C" _declspec(dllexport) DNINativeObject ClassTest_Zoo_Constructor(DNI::DNI * pDni)
{
	return new Zoo();
}

extern "C" _declspec(dllexport) void ClassTest_Zoo_Destructor(DNI::DNI* pDni, DNINativeObject pClassPtr)
{
	delete ((Zoo*)pClassPtr);
}

extern "C" _declspec(dllexport) int ClassTest_Zoo_GetAnimalCount(DNI::DNI * pDni, DNINativeObject pClassPtr)
{
	return ((Zoo*)pClassPtr)->GetAnimalCount();
}

extern "C" _declspec(dllexport) bool ClassTest_Zoo_AddAnimal(DNI::DNI * pDni, DNINativeObject pClassPtr, Animal * pAnimals)
{
	return ((Zoo*)pClassPtr)->AddAnimal(pAnimals);
}

extern "C" _declspec(dllexport) bool ClassTest_Zoo_RemoveAnimal(DNI::DNI * pDni, DNINativeObject pClassPtr,const char* name)
{
	return ((Zoo*)pClassPtr)->RemoveAnimal(DNI::convertTo<std::string>(pDni, name));
}

extern "C" _declspec(dllexport) DNINativeObject ClassTest_Zoo_CreateAnimal(DNI::DNI * pDni, DNINativeObject pClassPtr, const DNIEnum type, const char* name)
{
	return ((Zoo*)pClassPtr)->CreateAnimal(
		DNI::convertTo<AnimalTypes>(pDni, type),
		DNI::convertTo<std::string>(pDni, name));
}