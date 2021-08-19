using System;
using System.Collections.Generic;
using NUnit;
using NUnit.Framework;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Pant.Tests
{
	[TestFixture]
	public class PantTests
	{
		private MachineSettings _testMachineSettings = new MachineSettings
		{
			AcceptableContainers = new Dictionary<string, ContainerDef>
			{
				{"P", new ContainerDef{Code = "P", Name = "Plastic bottle", Price = 2}},
				{"B", new ContainerDef{Code = "B", Name = "Big plastic bottle", Price = 3}},
				{"C", new ContainerDef{Code = "C", Name = "Can", Price = 2}}
			},
			InverseLotteryChance = (int) Math.Round(1/0.001),
			LotteryPayout = 10000
		};

		[Test]
		public void CanInitAcceptableContainers()
		{
			var s = new MachineSettings
			{
				AcceptableContainers = new Dictionary<string, ContainerDef>
				{
					{"P", new ContainerDef{Code = "P", Name = "Plastic bottle", Price = 2}},
					{"G", new ContainerDef{Code = "G", Name = "Gallon Can", Price = 12}}
				}
			};
			Assert.That(s.AcceptableContainers.ContainsKey("P"));
			Assert.That(s.AcceptableContainers.ContainsKey("G"));
		}

		[Test]
		public void CanDepositIntoEmptyMachine()
		{
			var machine = new Machine(_testMachineSettings);
			machine.AcceptContainer("P");
			Assert.That(machine.State.CurrentState["P"], Is.EqualTo(1));
			Assert.That(machine.State.CurrentState, Does.Not.ContainKey("B"));
		}

		[Test]
		public void CanSerializeState()
		{
			var machine = new Machine(_testMachineSettings);
			machine.AcceptContainer("P");
			machine.AcceptContainer("B");
			machine.AcceptContainer("B");
			machine.PayForContainers();
			machine.AcceptContainer("C");
			machine.AcceptContainer("C");

			JsonSerializerOptions serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
			{
				IncludeFields = true, 
				WriteIndented = true
			};
			string serialized = JsonSerializer.Serialize(machine.State, serializerOptions);
				Console.WriteLine(serialized);
		}

		[Test]
		public void CanCalculatePayout()
		{
			var machine = new Machine(_testMachineSettings);
			machine.AcceptContainer("P");
			machine.AcceptContainer("B");
			machine.AcceptContainer("B");
			machine.AcceptContainer("C");
			machine.AcceptContainer("C");
			machine.AcceptContainer("C");
			var amount=machine.PayForContainers();
			Assert.That(amount, Is.EqualTo(14));
		}

		[Test] [Ignore("Integration test for developer")]
		public void Run10KLotteries()
		{
			int x = 0;
			for (int i = 0; i < 10000; i++)
			{
				var machine = new Machine(_testMachineSettings);
				if (machine.DoLottery(1)>0)
					x++;
			}
			Console.WriteLine($"10000 lotteries gave {x} wins");
		}

		[Test]
		public void PayingForContainersKeepsBalanceUnchanged()
		{
			var machine = new Machine(_testMachineSettings);
			machine.AcceptContainer("P");
			machine.AcceptContainer("B");
			var amount = machine.PayForContainers();
			Assert.That(amount, Is.EqualTo(5));
			Assert.That(machine.State.Balance, Is.EqualTo(0));
		}

		[Test]
		public void LosingLotteryIncreasesBalance()
		{
			var machine = new Machine(_testMachineSettings);
			machine.AcceptContainer("P");
			machine.AcceptContainer("B");
			var amount = machine.LoseLottery();
			Assert.That(amount, Is.EqualTo(0));
			Assert.That(machine.State.Balance, Is.EqualTo(5));
		}

		[Test]
		public void WinningLotteryDecreasesBalance()
		{
			var machine = new Machine(_testMachineSettings);
			machine.AcceptContainer("P");
			machine.AcceptContainer("B");
			var amount = machine.WinLottery();
			Assert.That(amount, Is.EqualTo(_testMachineSettings.LotteryPayout));
			Assert.That(machine.State.Balance, Is.EqualTo(5-_testMachineSettings.LotteryPayout));
		}


	}
}
