namespace Gamekit3D.GameCommands
{
    public class SendOnBecameVisible : SendGameCommand
    {
        private void OnBecameVisible()
        {
            Send();
        }
    }
}