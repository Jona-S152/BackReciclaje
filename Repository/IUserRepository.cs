using BackReciclaje.Model;

namespace BackReciclaje.Repository
{
    public interface IUserRepository
    {
        public bool CreateUser(User user);
        public bool UserExist(string cedula);
        public UserPuntos Login(UserLogin uLogin);
    }
}
