using api.Models;

namespace auth.Core.BL.Interfaces;

public interface IAuth
{
    Task<string?> Login(string username, string password);
    Task<bool> Register(string username, string password);
    
    Task<List<Users>> List();
    
    
}