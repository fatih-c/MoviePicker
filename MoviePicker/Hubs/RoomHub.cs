using Microsoft.AspNetCore.SignalR;

namespace MoviePicker.Hubs
{
    public class RoomHub : Hub
    {
        public async Task JoinRoomGroup(string roomCode)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);
        }
    }
}