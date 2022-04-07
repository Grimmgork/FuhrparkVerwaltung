using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using fvlib;
using fvlib.Databases.Json;
using fvlib.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Reflection;

namespace fvcli
{
	class Program
	{
		static FuhrparkVerwaltung fuhrparkVerwaltung;

		static void Main(string[] args)
		{
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
			fuhrparkVerwaltung.Delete(id);
			Console.WriteLine($"Deleted {id}!");
		}

		static void AllCommand(string[] args)
		{
			Type type = null;
			if (args.Length > 0) {
				type = FindType(args[0]);
			}

			List<Fahrzeug> result;
			if (type == null)
				result = fuhrparkVerwaltung.All();
			else
				result = fuhrparkVerwaltung.AllByType(type);
			
			foreach(Fahrzeug f in result){
				Console.WriteLine("-");
				Console.WriteLine(RenderVehicle(f));
			}
		}

		static void CreateCommand(string[] args)
		{
			string typeRef = args[0].ToLower();
			Fahrzeug fahrzeug = (Fahrzeug) Activator.CreateInstance(FindType(typeRef));
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

			string fieldName = args[1];
			string value = args[2];

			PropertyInfo p = fahrzeug.GetType().GetProperty(fieldName);
			p.SetValue(fahrzeug, Convert.ChangeType(value, p.PropertyType));
			fuhrparkVerwaltung.Update(fahrzeug);

			Console.WriteLine(RenderVehicle(fahrzeug));
		}


		static Type FindType(string typeRef)
		{
			string typeRefLowerCase = typeRef.ToLower();
			switch(typeRefLowerCase)
			{
				case "auto":
					return typeof(Auto);
				case "gabelstapler":
					return typeof(Gabelstapler);
				case "lastkraftwagen":
					return typeof(Lastkraftwagen);
				case "hubwagen":
					return typeof(Hubwagen);
			}
			return null;
		}

		static string RenderVehicle(Fahrzeug f)
		{
			string result = $"Typ: {f.GetType().Name}\n";
			result += $"Interne Kennung: {f.InterneKennung}\n";
			result += $"Kennzeichnung: {f.Kennzeichnung}\n";
			result += $"Länge: {f.Länge}\n";
			result += $"Breite: {f.Breite}\n";
			result += $"Verbrauch: {f.Verbrauch}\n";

			switch(f){
				case Auto:
					result += $"Plätze: {(f as Auto).Plätze}\n";
					break;
				case Lastkraftwagen:
					result += $"Ladekapazität: {(f as Lastkraftwagen).Ladekapazität}\n";
					break;
				case Gabelstapler:
					result += $"Hublast: {(f as Gabelstapler).Hublast}\n";
					break;
				case Hubwagen:
					result += $"Elektrisch?: {(f as Hubwagen).IstElektrisch}\n";
					break;
			}

			return result;
		}
	}
}