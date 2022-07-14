#include "pch.h"
#include "Zoo.h"

namespace ClassTest
{

	int Zoo::GetAnimalCount()
	{
		return (int)m_Animals.size();
	}

	bool Zoo::AddAnimal(Animal* pAnimals)
	{
		std::shared_ptr<Animal> ptrAnimal(pAnimals);
		m_Animals.push_back(ptrAnimal);
		return true;
	}
	
	bool Zoo::RemoveAnimal(const std::string& name)
	{
		for(auto it = m_Animals.begin();  it != m_Animals.end(); ++it)
		{
			if ((*it)->getName() == name)
			{
				m_Animals.erase(it);
				return true;
			}
		}
		return false;
	}

	Animal* Zoo::CreateAnimal(const AnimalTypes type, const std::string& name)
	{
		switch (type)
		{
		case AnimalTypes::Dog:
		{
			auto dog = new Dog(name);
			return dog;
		}
		case AnimalTypes::Cat:
		{
			auto cat = new Cat(name);
			return cat;
		}
		}
		return nullptr;
	}
}