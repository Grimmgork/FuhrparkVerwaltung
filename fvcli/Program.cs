using System;
using System.Collections.Generic;
using System.Linq;

using fvlib;
using fvlib.Databases.Json;
using fvlib.Models;

namespace fvcli
{
	class Program
	{
		static FuhrparkVerwaltung fuhrparkVerwaltung;
		static PropertyMap<string> vehicleView;

		static void Main(string[] args)
		{
			vehicleView = new PropertyMap<string>();
			vehicleView.RegisterProperty(typeof(Fahrzeug), "InterneKennung", "Interne Kennung");
			vehicleView.RegisterProperty(typeof(Fahrzeug), "Verbrauch", "Verbrauch");
			vehicleView.RegisterProperty(typeof(Fahrzeug), "Länge", "Länge");
			vehicleView.RegisterProperty(typeof(Fahrzeug), "Breite", "Breite");
			vehicleView.RegisterProperty(typeof(Fahrzeug), "Kennzeichnung", "Kennzeichnung");
			vehicleView.RegisterProperty(typeof(Auto), "Plätze", "Plätze");
			vehicleView.RegisterProperty(typeof(Lastkraftwagen), "Ladekapazität", "Ladekapazität");
			vehicleView.RegisterProperty(typeof(Gabelstapler), "Hublast", "Hublast");
			vehicleView.RegisterProperty(typeof(Hubwagen), "IstElektrisch", "Elektrisch?");

			fuhrparkVerwaltung = new FuhrparkVerwaltung(new JsonVehicleDatabase(@"c:\users\eric\desktop\database.json"));

			try{
				RouteCommand(args);
			}
			catch(Exception e){
				Console.WriteLine($"ERROR: {e.Message}");
			}
		}

		static void RouteCommand(string[] args)
		{
			if (args.Length == 0){
				InfoCommand();
				return;
			}

			string command = args[0].ToLower();
			args = args.Skip(1).ToArray();

			switch(command){
				case "all":
					AllCommand(args);
					break;
				case "edit":
					EditCommand(args);
					break;
				case "create":
					CreateCommand(args);
					break;
				case "remove":
					RemoveCommand(args);
					break;
				default:
					throw new Exception($"Command '{command}' not found!");
			}
		}

		static void InfoCommand()
		{
			string msg = "";
			msg += @"  ___              __ __ " + "\n";
			msg += @".'  _|.--.--.----.|  |__|" + "\n";
			msg += @"|   _||  |  |  __||  |  |" + "\n";
			msg += @"|__|  \____/|____||__|__|" + "\n";
			msg += "Fuhrparkverwaltung ...\n";
			msg += "\n";
			msg += "commands:\n";
			msg += "	all\n";
			msg += "	create\n";
			msg += "	edit\n";
			msg += "	remove\n";

			Console.WriteLine(msg);
		}

		static void RemoveCommand(string[] args)
		{
			string id = args[0];
			fuhrparkVerwaltung.Remove(id);
			Console.WriteLine($"Deleted {id}!");
		}

		static void AllCommand(string[] args)
		{
			List<Fahrzeug> result;
			if (args.Length > 0){
				string typeName = args[0];
				Type type = GetVehicleType(typeName);
				result = fuhrparkVerwaltung.GetAllByType(type);
			}
			else{
				result = fuhrparkVerwaltung.GetAll();
			}
			
			foreach(Fahrzeug f in result){
				Console.WriteLine("-");
				Console.WriteLine(RenderVehicle(f));
			}
		}

		static void CreateCommand(string[] args)
		{
			string typeName = args[0];
			Fahrzeug fahrzeug = (Fahrzeug) Activator.CreateInstance(GetVehicleType(typeName));
			Console.WriteLine(fuhrparkVerwaltung.Insert(fahrzeug));
		}

		static void EditCommand(string[] args)
		{
			string id = args[0];
			Fahrzeug fahrzeug = fuhrparkVerwaltung.GetById(id);

			if (args.Length == 1){
				Console.WriteLine(RenderVehicle(fahrzeug));
				return;
			}

			int fieldNumber = int.Parse(args[1]);
			string value = args[2];

			Property<string> property = vehicleView.GetProperties(fahrzeug.GetType())[fieldNumber-1];
			property.SetValue(fahrzeug, Convert.ChangeType(value, property.PropertyType));
			fuhrparkVerwaltung.Update(fahrzeug);

			Console.WriteLine(RenderVehicle(fahrzeug));
		}

		static Type GetVehicleType(string typeName)
		{
			VehicleType type = (VehicleType) Enum.Parse(typeof(VehicleType), typeName, true);
			switch(type)
			{
				case VehicleType.Auto:
					return typeof(Auto);
				case VehicleType.Gabelstapler:
					return typeof(Gabelstapler);
				case VehicleType.Lastkraftwagen:
					return typeof(Lastkraftwagen);
				case VehicleType.Hubwagen:
					return typeof(Hubwagen);
			}
			return null;
		}

		static string RenderVehicle(Fahrzeug f)
		{
			string result = $"Typ: {f.GetType().Name}\n";
			Property<string>[] properties = vehicleView.GetProperties(f.GetType());
			for(int i = 0; i < properties.Length; i++){
				Property<string> property = properties[i];
				result += $"{i + 1} {property.MetaData}: {property.GetValue(f)}\n";
			}
			return result;
		}
	}

	public enum VehicleType
	{
		Gabelstapler,
		Hubwagen,
		Auto,
		Lastkraftwagen
	}
}