using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealDistanceAPI.Models
{
	public class User
	{
		[Key]
        public Guid ID { get; set; }
		public Guid ApiKey { get; set; }
	}
}

