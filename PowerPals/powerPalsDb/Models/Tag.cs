using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace powerPalsDb.Models
{
	public class Tag
	{
		public int Id { get; set; }
		public string Name { get; set; } = null!;

		public int PostId { get; set; }
		public Post Post { get; set; } = null!;
	}
}
