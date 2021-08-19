using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pant
{
	public class MachineState
	{
		public Dictionary<string, int> TotalState { get; init; }
		public Dictionary<string, int> CurrentState { get; init; }
		public int PaidOut { get; set; }

	}
}
