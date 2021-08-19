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
			}
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
			machine.PayMoney();
			machine.AcceptContainer("C");
			machine.AcceptContainer("C");
			string serialized = JsonSerializer.Serialize(machine.State);
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
			var amount=machine.PayMoney();
			Assert.That(amount, Is.EqualTo(14));

		}
	}
}
