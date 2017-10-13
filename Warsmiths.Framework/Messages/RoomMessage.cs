namespace Warsmiths.Server.Framework.Messages
{
    public class RoomMessage : IMessage
    {
        public RoomMessage(byte action)
        {
            Action = action;
        }

        public RoomMessage(byte action, object message)
            : this(action)
        {
            Message = message;
        }

        public object Message { get; }

        public byte Action { get; }
    }
}