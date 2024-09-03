namespace Splenduel.Core.Game.Model
{
    public class Player
    {
        public string Name { get; set; }
        public Guid Id { get; set; }

        public Player(string name, Guid id)
        {
            Name = name;
            Id = id;
        }
    }
}
