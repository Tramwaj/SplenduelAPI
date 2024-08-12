using Splenduel.Core.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splenduel.Core.Game.Model
{
    internal class CreateGameResponse : DefaultResponse
    {
        public GameState GameState { get; set; }
        public CreateGameResponse(bool success, object obj) : base(success, obj)
        {
            if (obj != null)
            {
                GameState = obj as GameState;
            }
        }
    }
}
