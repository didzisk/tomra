using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pant
{
	public interface IMachine
	{
		void AcceptContainer(string code);
		int PayMoney();
		bool DoLottery();
		void PersistState();
	}
}
