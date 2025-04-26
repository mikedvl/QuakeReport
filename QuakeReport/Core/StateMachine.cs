using QuakeReport.ParserCore.Enums;
using QuakeReport.ParserCore.Interfaces;

namespace QuakeReport.ParserCore
{
	public class StateMachine: IStateMachine
    {
		public StateMachine()
		{
		}

        private GameStatus _currentState;

        public GameStatus CurrentState
        {
            get
            {
                return _currentState;
            }
        }

        public void SetStatus(GameStatus state)
        {
            _currentState = state;
        }
    }
}

