using System;
using System.Collections.Generic;

namespace Pant
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");
			MachineSettings _testMachineSettings = new MachineSettings
			{
				AcceptableContainers = new Dictionary<string, ContainerDef>
				{
					{"P", new ContainerDef{Code = "P", Name = "Plastic bottle", Price = 2, MsToProcess = 1000}},
					{"B", new ContainerDef{Code = "B", Name = "Big plastic bottle", Price = 3, MsToProcess = 1000}},
					{"C", new ContainerDef{Code = "C", Name = "Can", Price = 2, MsToProcess = 2000}}
				},
				InverseLotteryChance = (int)Math.Round(1 / 0.001),
				LotteryPayout = 10000,
				LotteryTicketPrice = 0.5m
			};

			MachineFrontend m = new MachineFrontend(new Machine(_testMachineSettings));

			m.Start();
		}
	}
}
