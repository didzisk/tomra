using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Pant
{
	public class Machine:IMachine
	{
		private readonly MachineSettings _settings;
		private readonly MachineState _state;

		public Machine(MachineSettings settings)
		{
			_settings = settings;
			_state = new MachineState { CurrentState = new Dictionary<string, int>(), TotalState = new Dictionary<string, int>() };
			StatusFileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
				"MachineStatus.txt");
		}



		public MachineState State => _state;

		public string StatusFileName { get; }

		public void AcceptContainer(string code)
		{
			if (_settings.AcceptableContainers.ContainsKey(code))
			{
				if (_state.CurrentState.ContainsKey(code))
				{
					_state.CurrentState[code] += 1;
				}
				else
				{
					_state.CurrentState[code] = 1;
				}

				if (_state.TotalState.ContainsKey(code))
				{
					_state.TotalState[code] += 1;
				}
				else
				{
					_state.TotalState[code] = 1;
				}

				File.WriteAllText(StatusFileName, SerializeStatus());
				Thread.Sleep(_settings.AcceptableContainers[code].MsToProcess); //could "await" here, but I have no other thread waiting for me
			}
		}

		private void CloseSession(int paidAmount)
		{
			var valueOfContainers =
				_state.CurrentState.Sum(kvp => _settings.AcceptableContainers[kvp.Key].Price * kvp.Value);
			foreach (var key in _state.CurrentState.Keys)
			{
				_state.CurrentState[key] = 0;
			}
			_state.PaidOut += paidAmount;
			_state.Balance += valueOfContainers - paidAmount;
			File.WriteAllText(StatusFileName, SerializeStatus());
		}

		public string SerializeStatus()
		{
			JsonSerializerOptions serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
			{
				IncludeFields = true,
				WriteIndented = true
			};
			return JsonSerializer.Serialize(_state, serializerOptions);
		}

		private int GetCurrentPayout()
		{
			return _state.CurrentState.Sum(kvp => _settings.AcceptableContainers[kvp.Key].Price * kvp.Value);
		}

		public int PayForContainers()
		{
			var currentPayout = GetCurrentPayout();
			CloseSession(currentPayout);
			return currentPayout;
		}

		public int GetNumTickets()
		{
			var currentPayout = GetCurrentPayout();
			return (int)(currentPayout / _settings.LotteryTicketPrice); //ignore div by zero
		}

		public int DoLottery(int numTickets)
		{
			Random rnd = new Random();
			for (int i = 0; i < numTickets; i++)
			{
				//Ignoring the possibly multiple winning tickets
				var x = rnd.Next(1, _settings.InverseLotteryChance);
				if (x == 1)
				{
					//ignoring impurity of the function
					return WinLottery();
				}
			}
			return LoseLottery();
		}

		public int WinLottery()
		{
			CloseSession(_settings.LotteryPayout);
			return _settings.LotteryPayout;
		}
		public int LoseLottery()
		{
			CloseSession(0);
			return 0;
		}

		public IEnumerable<ContainerDef> GetAcceptableContainers()
		{
			return _settings.AcceptableContainers.Values;
		}

		public IEnumerable<StatusLineDto> GetCurrentState()
		{
			return from acc in _settings.AcceptableContainers
				join cur in _state.CurrentState on acc.Key equals cur.Key into gj
				from subCur in gj.DefaultIfEmpty()
				select new StatusLineDto(Code: acc.Value.Code, Price: acc.Value.Price, Name: acc.Value.Name, Count: subCur.Value);
		}

	}
}
