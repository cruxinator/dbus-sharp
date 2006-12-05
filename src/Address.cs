// Copyright 2006 Alp Toker <alp@atoker.com>
// This software is made available under the MIT License
// See COPYING for details

using System;
using System.Text;
using System.Collections.Generic;

namespace NDesk.DBus
{
	public class BadAddressException : Exception
	{
		public BadAddressException (string reason) : base (reason) {}
	}

	public class AddressEntry
	{
		public string Method;
		public IDictionary<string,string> Properties = new SortedDictionary<string,string> ();

		public override string ToString ()
		{
			//TODO: hex escaping

			StringBuilder sb = new StringBuilder ();
			sb.Append (Escape (Method));
			sb.Append (':');

			bool first = true;
			foreach (KeyValuePair<string,string> prop in Properties) {
				if (first)
					first = false;
				else
					sb.Append (',');

				sb.Append (Escape (prop.Key));
				sb.Append ('=');
				sb.Append (Escape (prop.Value));
			}

			return sb.ToString ();
		}

		static string Escape (string str)
		{
			if (str == null)
				return String.Empty;

			StringBuilder sb = new StringBuilder ();
			int len = str.Length;

			for (int i = 0 ; i != len ; i++) {
				char c = str[i];

				//everything other than the optionally escaped chars _must_ be escaped
				if (!Char.IsLetterOrDigit (c) && c != '-' && c != '_' && c != '/' && c != '\\' && c != '.')
					sb.Append (Uri.HexEscape (c));
				else
					sb.Append (c);
			}

			return sb.ToString ();
		}
	}

	public class Address
	{
		//(unix:(path|abstract)=.*,guid=.*|tcp:host=.*(,port=.*)?);? ...
		public static AddressEntry[] Parse (string addresses)
		{
			if (addresses == null)
				throw new ArgumentNullException (addresses);

			List<AddressEntry> entries = new List<AddressEntry> ();

			foreach (string entryStr in addresses.Split (';')) {
				AddressEntry entry = new AddressEntry ();

				string[] parts = entryStr.Split (':');

				if (parts.Length < 2)
					throw new BadAddressException ("No colon found");
				if (parts.Length > 2)
					throw new BadAddressException ("Too many colons found");

				entry.Method = Unescape (parts[0]);

				foreach (string propStr in parts[1].Split (',')) {
					parts = propStr.Split ('=');

					if (parts.Length < 2)
						throw new BadAddressException ("No equals sign found");
					if (parts.Length > 2)
						throw new BadAddressException ("Too many equals signs found");

					entry.Properties[Unescape (parts[0])] = Unescape (parts[1]);
				}

				entries.Add (entry);
			}

			return entries.ToArray ();
		}

		static string Unescape (string str)
		{
			if (str == null)
				return String.Empty;

			StringBuilder sb = new StringBuilder ();
			int len = str.Length;
			int i = 0;
			while (i != len) {
				if (Uri.IsHexEncoding (str, i))
					sb.Append (Uri.HexUnescape (str, ref i));
				else
					sb.Append (str[i++]);
			}

			return sb.ToString ();
		}

		const string SYSTEM_BUS_ADDRESS = "unix:path=/var/run/dbus/system_bus_socket";
		public static string System
		{
			get {
				string addr = Environment.GetEnvironmentVariable ("DBUS_SYSTEM_BUS_ADDRESS");

				if (String.IsNullOrEmpty (addr))
					addr = SYSTEM_BUS_ADDRESS;

				return addr;
			}
		}

		public static string Session
		{
			get {
				return Environment.GetEnvironmentVariable ("DBUS_SESSION_BUS_ADDRESS");
			}
		}

		public static string Starter
		{
			get {
				return Environment.GetEnvironmentVariable ("DBUS_STARTER_ADDRESS");
			}
		}

		public static string StarterBusType
		{
			get {
				return Environment.GetEnvironmentVariable ("DBUS_STARTER_BUS_TYPE");
			}
		}
	}
}
