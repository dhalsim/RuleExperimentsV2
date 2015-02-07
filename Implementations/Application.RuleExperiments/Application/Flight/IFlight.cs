using Domain.RuleExperiments.Attributes.StateMachine;

namespace Application.RuleExperiments.Application.Flight
{
	[StateMachineAttribute]
	public interface IFlight
	{
		[StartStateAttribute("Search")]
		void Start();

		[StateAttribute("Result")]
		void Search();

		[ResetStateAttribute]
		void Reset();

		[StateAttribute("Basket")]
		void Result();

		[StateAttribute("Passenger")]
		void Basket();

		[StateAttribute("Book")]
		void Passenger();

		[StateAttribute("Summary")]
		void Book();

		[EndStateAttribute]
		void Summary();
	}
}

