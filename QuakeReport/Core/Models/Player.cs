namespace QuakeReport.ParserCore.Models
{
    public class Player
    {
        public string? Name { get; set; }
        public int Score { get; set; }
        private int _id;

        public Player()
        {

        }

        public Player(string name ,int id)
        {
            Name = name;
            _id = id;
        }

        public void SetId(int id)
        {
            _id = id;
        }

        public int GetId()
        {
            return _id;
        }
    }
}

