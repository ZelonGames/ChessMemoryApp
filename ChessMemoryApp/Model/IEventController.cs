using ChessMemoryApp.Model.ChessMoveLogic;
using ChessMemoryApp.Model.Lichess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model
{
    public interface IEventController
    {
        void SubscribeToEvents(params object[] subscribers);
    }
}
