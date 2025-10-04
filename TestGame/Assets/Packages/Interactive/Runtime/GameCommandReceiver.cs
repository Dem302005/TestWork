using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit3D.GameCommands
{
    //Class used to call the proper GameCommandHandler subclass to a given GameCommandType received from a subclass of SendGameCommand
    public class GameCommandReceiver : MonoBehaviour
    {
        private readonly Dictionary<GameCommandType, List<Action>> handlers =
            new Dictionary<GameCommandType, List<Action>>();

        public void Receive(GameCommandType e)
        {
            List<Action> callbacks = null;
            if (handlers.TryGetValue(e, out callbacks))
                foreach (var i in callbacks)
                    i();
        }

        public void Register(GameCommandType type, GameCommandHandler handler)
        {
            List<Action> callbacks = null;
            if (!handlers.TryGetValue(type, out callbacks)) callbacks = handlers[type] = new List<Action>();
            callbacks.Add(handler.OnInteraction);
        }

        public void Remove(GameCommandType type, GameCommandHandler handler)
        {
            handlers[type].Remove(handler.OnInteraction);
        }
    }
}