using System;
using System.Collections.Generic;
using System.Text;
using fvlib.Models;

namespace fvlib.Databases
{
	public interface IVehicleDatabase
	{
		public void Update(Fahrzeug o);
		public void Insert(Fahrzeug o);
		public void Remove(string id);
		public List<Fahrzeug> GetAll();
		public List<Fahrzeug> GetAllByType(Type t);
		public Fahrzeug GetById(string id);
	}
}