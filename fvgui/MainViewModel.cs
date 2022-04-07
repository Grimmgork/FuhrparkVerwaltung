using fvlib.Models;
using fvlib;
using fvlib.Databases.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fvgui
{
	public class MainViewModel : PropertyChangedBase
	{
		List<Fahrzeug> _result;
		public List<Fahrzeug> Result {
			get{
				return _result;
			}
			set{
				_result = value;
				NotifyPropertyChanged("Result");
			}
		}

		public MainViewModel(){

			FuhrparkVerwaltung fv = new FuhrparkVerwaltung(new JsonVehicleDatabase(@"c:\users\eric\desktop\database.json"));
			Result = fv.All();
		}
    }
}
