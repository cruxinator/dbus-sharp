// Copyright 2006 Alp Toker <alp@atoker.com>
// Copyright 2010 Alan McGovern <alan.mcgovern@gmail.com>
// This software is made available under the MIT License
// See COPYING for details

using System;

namespace DBus
{
	public class BusException : Exception
	{
		public BusException (string errorName, string errorMessage)
		{
			if (!IsValidErrorName (errorName))
				throw new ArgumentException (string.Format ("'{0}' is not a valid error name", errorName), "errorName");

			this.ErrorName = errorName;
			this.ErrorMessage = errorMessage;
		}

		public BusException (string errorName, string format, params object[] args)
		{
			this.ErrorName = errorName;
			this.ErrorMessage = String.Format (format, args);
		}

		public override string Message
		{
			get
			{
				return ErrorName + ": " + ErrorMessage;
			}
		}

		public readonly string ErrorName;

		public readonly string ErrorMessage;

		internal static bool IsValidErrorName (string errorName)
		{
			if (errorName == null)
				throw new ArgumentNullException ("errorName");

			// https://dbus.freedesktop.org/doc/dbus-specification.html#message-protocol-names
			// "Error names have the same restrictions as interface names."
			return Mapper.IsValidInterfaceName (errorName);
		}
	}
}
