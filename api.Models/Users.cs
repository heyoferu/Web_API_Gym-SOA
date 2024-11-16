using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace api.Models;

public class Users
{
    [Key]
    public string Username { get; set; }
    public string Password { get; set; }
    
    [JsonIgnore] 
    public byte[]? Salt { get; set; }
}