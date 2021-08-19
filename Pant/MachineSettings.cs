using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pant
{
	public class MachineSettings
	{
		public Dictionary<string, ContainerDef> AcceptableContainers { get; init; }
		public int InverseLotteryChance { get; init; }
		public decimal LotteryTicketPrice { get; init; }
		public int LotteryPayout { get; init; }
	}
}
