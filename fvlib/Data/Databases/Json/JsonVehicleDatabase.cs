using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft;
using fvlib.Models;
using System.Linq;

namespace fvlib.Databases.Json
{
	public class JsonVehicleDatabase : IVehicleDatabase
	{
		private string path;

		public JsonVehicleDatabase(string path){
			this.path = path;
			if(!File.Exists(path)){
				InitEmptyDatabase(path);
			}
		}

		public static void InitEmptyDatabase(string path)
		{
			File.WriteAllText(path, JsonConvert.SerializeObject(new Fahrzeug[] { }));
		}

		public List<Fahrzeug> GetAll()
		{
			return Load();
		}

		public Fahrzeug GetById(string id)
		{
			List<Fahrzeug> all = GetAll();
			return all.Find(f => f.InterneKennung == id);
		}

		public void Remove(string id)
		{
			List<Fahrzeug> all = GetAll();
			int index = all.FindIndex(f => f.InterneKennung == id);
			if (index == -1)
				return;

			all.RemoveAt(index);
			Save(all);
		}

		public void Upsert(Fahrzeug o)
		{
			List<Fahrzeug> all = GetAll();
			int index = all.FindIndex(f => f.InterneKennung == o.InterneKennung);
			if(index == -1)
				all.Add(o);
			else
				all[index] = o;
			
			Save(all);
		}

		public void Update(Fahrzeug o)
		{
			List<Fahrzeug> all = GetAll();
			int index = all.FindIndex(f => f.InterneKennung == o.InterneKennung);
			if (index == -1)
				throw new Exception("Key not found!");
			
			all[index] = o;
			Save(all);
		}

		public void Insert(Fahrzeug o)
		{
			List<Fahrzeug> all = GetAll();
			int index = all.FindIndex(f => f.InterneKennung == o.InterneKennung);
			if (index != -1)
				throw new Exception("Key already present in Database!");

			all.Add(o);
			Save(all);
		}

		public List<Fahrzeug> GetAllByType(Type t)
		{
			return GetAll().FindAll(f => f.GetType() == t).ToList();
		}

		private List<Fahrzeug> Load()
		{
			string json = File.ReadAllText(path);
			List<JObject> all = JsonConvert.DeserializeObject<List<JObject>>(json);
			List<Fahrzeug> result = new List<Fahrzeug>(); 
			foreach(JObject o in all){
				result.Add(GetVehicleFromJObject(o));
			}

			return result;
		}

		private void Save(IEnumerable<Fahrzeug> o)
		{
			List<JObject> all = new List<JObject>();
			foreach (Fahrzeug vehicle in o){
				all.Add(GetJObjectFromVehicle(vehicle));
			}
			File.WriteAllText(path, JsonConvert.SerializeObject(all, Formatting.Indented));
		}


		private static Fahrzeug GetVehicleFromJObject(JObject j){
			Type type = Type.GetType(j["Type"].ToString());
			j.Remove("Type");
			return (Fahrzeug)j.ToObject(type);
		}

		private static JObject GetJObjectFromVehicle(Fahrzeug vehicle){
			JObject j = JObject.FromObject(vehicle);
			j.Add("Type", vehicle.GetType().FullName);
			return j;
		}
	}
}