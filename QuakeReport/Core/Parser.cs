using System.Text.RegularExpressions;
using QuakeReport.ParserCore.Enums;
using QuakeReport.ParserCore.Interfaces;

namespace QuakeReport.ParserCore
{
    public class Parser : IParse
	{
        private readonly IStateMachine _stateMachine;
        private readonly IGameInfoManager _gameInfoManager;
        private readonly string gameNamePattern = @"\\gamename\\(.*?)\\";
        private readonly string commandPattern = @"\d+:\d+ (.*?):";
        private readonly string killDataPattern = @"Kill:\s(\d+)\s(\d+)\s(\d+):";
        private readonly string playerIdPattern = @":\s(\d+)\sn\\";
        private readonly string playerNamePattern = @"n\\(.*?)\\t";

        public Parser(IStateMachine stateMachine, IGameInfoManager gameInfoManager)
		{
            _stateMachine = stateMachine;
            _gameInfoManager = gameInfoManager;
        }

        public bool ParseLogLine(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            Match match = Regex.Match(str, commandPattern);

            string command = string.Empty;
            if (match.Success)
            {
                command = match.Groups[1].Value.Trim();//init command
            }

            if (_stateMachine.CurrentState == (int)GameStatus.ShutdownGame)
            {
                if (command.Contains("InitGame"))
                {
                    //get game name
                    match = Regex.Match(str, gameNamePattern);

                    if (match.Success)
                    {
                        string gameName = match.Groups[1].Value;
                        _stateMachine.SetStatus(GameStatus.InitGame);
                        _gameInfoManager.InitGame(gameName);

                    }
                }
            }
            else
            {
                if (command.Contains("Kill"))
                {
                    //init Kill
                    match = Regex.Match(str, killDataPattern);

                    int sourcePlayerId, targetPlayerId, weaponId;
                    if (match.Success && match.Groups.Count == 4)
                    {
                        sourcePlayerId = int.Parse(match.Groups[1].Value);
                        targetPlayerId = int.Parse(match.Groups[2].Value);
                        weaponId = int.Parse(match.Groups[3].Value);

                        _gameInfoManager.Kill(sourcePlayerId, targetPlayerId, weaponId);
                    }
                }
                else if (command.Contains("ClientUserinfoChanged"))
                {
                    //init player
                    int playerId = 0;
                    match = Regex.Match(str, playerIdPattern);

                    if (match.Success && match.Groups.Count >= 2)
                        playerId = int.Parse(match.Groups[1].Value);

                    match = Regex.Match(str, playerNamePattern);
                    string playerName = string.Empty;

                    if (match.Success && match.Groups.Count >= 2)
                        playerName = match.Groups[1].Value;

                    _gameInfoManager.AddPlayer(playerName,playerId);
                }
                else if (command.Contains("ShutdownGame"))
                {
                    //Shutdown game
                    _stateMachine.SetStatus(GameStatus.ShutdownGame);
                    _gameInfoManager.ShutdownGame();
                }
            }

            return true;
        }
    }
}

