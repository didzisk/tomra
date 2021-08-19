using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Pant
{
	public class MachineFrontend:IMachineFrontend
	{
		private readonly IMachine _machine;

		public MachineFrontend(IMachine machine)
		{
			_machine = machine;
		}

		public void Start()
		{
			while (true)
			{
				DisplayStatus();
				var command = Console.ReadLine()?.ToUpper(CultureInfo.InvariantCulture);
				switch (command)
				{
					case "EXIT":
						return;
					case "M":
						break;
					case "L":
						DoLottery(cheat: false);
						break;
					case "MOTHERLODE": //from The Sims
						DoLottery(cheat: true);
						break;
					default:
						_machine.AcceptContainer(command);
						break;
				}
			}

        }

		private void DoLottery(bool cheat)
		{
			Console.WriteLine($"You have {_machine.GetNumTickets()} tickets to use. Answer Y to participate in the lottery.");
			if (Console.ReadLine()?.ToUpper(CultureInfo.InvariantCulture) == "Y")
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("My bosses have told me to ask for a double confirmation.");
				Console.WriteLine("Do you really want to participate in the lottery (Y/N)?");
				Console.ResetColor();
				if (Console.ReadLine()?.ToUpper(CultureInfo.InvariantCulture) == "Y")
				{
					var numTickets = _machine.GetNumTickets();
					var money = 0;
					money = cheat ? _machine.WinLottery() : _machine.DoLottery(numTickets);
					if (money > 0)
					{
						Console.ForegroundColor = ConsoleColor.Green;
						Console.WriteLine("----------------------------------------------------------");
						Console.WriteLine("Receipt");
						Console.WriteLine($"Congratulations, using your {numTickets} tickets, you won {money} kr!");
						Console.WriteLine("----------------------------------------------------------");
						Console.ResetColor();
					}
					else
					{
						Console.WriteLine("Sorry, better luck next time!");
					}
				}
			}

		}

		private void DisplayStatus()
		{
			Console.WriteLine();
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("==========================================================");
			Console.ResetColor();
			Console.WriteLine("\"exit\" to stop the machine");
			Console.WriteLine();
			Console.WriteLine("Write the code and press <enter> to deposit a container.");
			Console.WriteLine("Following containers are accepted:");
			Console.WriteLine(string.Join("\r\n", 
				_machine.GetAcceptableContainers()
					.Select(x => $"\"{x.Code}\" - {x.Price} kr for \"{x.Name}\""
					)));
			Console.WriteLine();
			Console.WriteLine("Complete the process by entering a command:");
			Console.WriteLine("\"L\" - enter a lottery for a chance to win 10000");
			Console.WriteLine("\"M\" - pay money");
			Console.WriteLine(string.Join("\r\n",
				_machine.GetCurrentState()
				.Select(x => $"{x.Count} x {x.Price} kr  = {x.Price * x.Count} kr {x.Name}"
				)));
		}
	}
}
