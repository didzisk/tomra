using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pant
{
	public record ContainerDef
	{
		public string Code { get; init; }
		public string Name { get; init; }
		public int Price { get; init; }
	}
}
