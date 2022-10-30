using Microsoft.AspNetCore.SignalR;
using ToDoList.Models;

namespace ToDoList.Hubs
{
    public class ToDoListHub : Hub
    {
        public async Task ListOfToDosHasChanged()
        {
            await Clients.All.SendAsync("ListOfToDosHasChanged");
        }
    }
}
