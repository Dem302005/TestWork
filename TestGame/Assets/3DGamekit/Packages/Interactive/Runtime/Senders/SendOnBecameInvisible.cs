namespace Gamekit3D.GameCommands
{
    public class SendOnBecameInvisible : SendGameCommand
    {
        private void OnBecameInvisible()
        {
            Send();
        }
    }
}