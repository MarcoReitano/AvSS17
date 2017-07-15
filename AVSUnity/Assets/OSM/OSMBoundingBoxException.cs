using System;

namespace OSM
{
	public class OSMBoundingBoxException : ApplicationException
	{
		public OSMBoundingBoxException () : base("Wrong Bounding Box Data. Use lat down, long left,  lat up, long right!")
		{
		}
	}
}

