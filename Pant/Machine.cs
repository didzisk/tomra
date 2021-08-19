using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
		}

		public MachineState State => _state;

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
		}

		public int PayForContainers()
		{
			var currentPayout =
				_state.CurrentState.Sum(kvp => _settings.AcceptableContainers[kvp.Key].Price * kvp.Value);
			CloseSession(currentPayout);
			return currentPayout;
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

		public void PersistState()
		{
			throw new NotImplementedException();
		}

	}
}
