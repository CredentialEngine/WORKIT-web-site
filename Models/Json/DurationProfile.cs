﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Models.Json
{
	[DataContract]
	public class DurationProfile : JsonLDObject
	{
		public DurationProfile()
		{
			type = "ceterms:DurationProfile";
		}
	}

}
