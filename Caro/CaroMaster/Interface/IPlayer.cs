using System.Threading.Tasks;

namespace CaroMaster
{
	public interface IPlayer
	{
		int LimitThinkingTime { get; set; }

		char Character { get; set; }

		Task Action();
	}
}
