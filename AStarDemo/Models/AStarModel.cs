using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AStarDemo.Models
{
	public class AStarModel
	{
		public string Message
		{
			get;
			set;
		}

		public Grid Graph = new Grid();
	}
}