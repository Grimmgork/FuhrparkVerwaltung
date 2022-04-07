﻿using System;
using System.Collections.Generic;
using System.Text;
using fvlib.Databases;
using fvlib.Models;

namespace fvlib
{
	public class FuhrparkVerwaltung
	{
		private IVehicleDatabase database;
		
		public FuhrparkVerwaltung(IVehicleDatabase db){
			database = db;
		}

		public List<Fahrzeug> All(){
			return database.GetAll();
		}

		public List<Fahrzeug> AllByType(Type t){
			return database.GetAllByType(t);
		}

		public void Update(Fahrzeug o){
			database.Update(o);
		}

		public string Insert(Fahrzeug o){
			o.InterneKennung = GenerateNewId();
			database.Insert(o);
			return o.InterneKennung;
		}

		public void Delete(string id)
		{
			database.Delete(id);
		}

		public Fahrzeug GetById(string id){
			return database.GetById(id);
		}

		private static string GenerateNewId()
		{
			return Guid.NewGuid().ToString();
		}
	}
}
