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
				{"P", new ContainerDef{Code = "P", Name = "Plastic bottle", Price = 2, MsToProcess = 0}},
				{"B", new ContainerDef{Code = "B", Name = "Big plastic bottle", Price = 3, MsToProcess = 0}},
				{"C", new ContainerDef{Code = "C", Name = "Can", Price = 2, MsToProcess = 0/*speed up the tests*/}}
			},
			InverseLotteryChance = (int) Math.Round(1/0.001),
			LotteryPayout = 10000,
			LotteryTicketPrice = 0.5m
			
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
			var machine = new Machine(s);
			Assert.That(s.AcceptableContainers.ContainsKey("P"));
			Assert.That(s.AcceptableContainers.ContainsKey("G"));
			Assert.That(machine.GetAcceptableContainers(), Does.Contain(new ContainerDef { Code = "P", Name = "Plastic bottle", Price = 2 }));
		}

		[Test]
		public void CanDelayAcceptance()
		{
			const int msToProcess = 100;
			var s = new MachineSettings
			{
				AcceptableContainers = new Dictionary<string, ContainerDef>
				{
					{"G", new ContainerDef{Code = "G", Name = "Gallon Can", Price = 12, MsToProcess = msToProcess}}
				}
			};
			var machine = new Machine(s);
			var ticks = DateTime.Now.Ticks;
			machine.AcceptContainer("G");
			ticks = DateTime.Now.Ticks - ticks;
			Assert.That(ticks, Is.GreaterThan(TimeSpan.TicksPerMillisecond * msToProcess));
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

			string serialized = machine.SerializeStatus();
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
			//med default vinnersjanse er resultatet mellom 5 og 30 gevinster... dårlig business
			//kravene sier derimot ingenting om forbud mot negativ balanse
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

		[Test]
		public void CanSerializeReadableStatus()
		{
			var machine = new Machine(_testMachineSettings);
			machine.AcceptContainer("P");
			machine.AcceptContainer("B");
			Console.WriteLine(machine.GetCurrentState());
		}

		[Test]
		public void CanGetNumberOfTickets()
		{
			var machine = new Machine(_testMachineSettings);
			machine.AcceptContainer("P");
			machine.AcceptContainer("B");
			machine.AcceptContainer("C");
			var numTickets = machine.GetNumTickets();
			Assert.That(numTickets, Is.EqualTo(14));
		}


	}
}
