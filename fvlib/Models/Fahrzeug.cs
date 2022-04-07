using System;

namespace fvlib.Models
{
	public abstract class Fahrzeug
	{
		public string InterneKennung { get; set; }

		public float Verbrauch { get; set; }

		public float Länge { get; set; }

		public float Breite { get; set; }

		public string Kennzeichnung { get; set; }
	}
}