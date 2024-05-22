using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicRequestBot.Data
{
	public static class Storage
	{
		static List<Request> requestList=new List<Request>();
		public static void Add(Request request)
		{
			if(request.PhoneNumber!=null)
			{
				requestList.Add(request);
			}
		}
	}
}
