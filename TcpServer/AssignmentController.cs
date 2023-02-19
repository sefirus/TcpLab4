using Core;

namespace TcpServer;

public class AssignmentController: ControllerBase
{
    [ControllerMethod("start")]
    public Message Start(Message request)
    {
        return new Message();
    }
}