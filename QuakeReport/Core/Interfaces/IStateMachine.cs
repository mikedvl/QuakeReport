
using QuakeReport.ParserCore.Enums;

namespace QuakeReport.ParserCore.Interfaces
{
    public interface IStateMachine
    {
        GameStatus CurrentState { get; }

        void SetStatus(GameStatus state);
    }
}

