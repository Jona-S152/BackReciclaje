using BackReciclaje.Model;
using Microsoft.Extensions.Options;
using System.Data.SqlClient;

namespace BackReciclaje.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ConnectionStrings _connectionStrings;

        public UserRepository(IOptions<ConnectionStrings> conexion)
        {
            _connectionStrings = conexion.Value;
        }

        public bool CreateUser(User user)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionStrings.Db_Connection))
                {
                    conn.Open();

                    string query = @"INSERT INTO UserLog VALUES (@Cedula, @NombreCompleto, @Email, @Telefono, @NombreUsuario, @Contraseña)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Cedula", user.Cedula);
                        cmd.Parameters.AddWithValue("@NombreCompleto", user.NombreCompleto);
                        cmd.Parameters.AddWithValue("@Email", user.Email);
                        cmd.Parameters.AddWithValue("@Telefono", user.Telefono);
                        cmd.Parameters.AddWithValue("@NombreUsuario", user.NombreUsuario);
                        cmd.Parameters.AddWithValue("@Contraseña", user.Contraseña);

                        cmd.ExecuteNonQuery();
                        conn.Close();
                        return true;
                    }
                }
            }catch (Exception ex)
            {
                return false;
            }
        }

        public bool UserExist(string cedula)
        {
            using (SqlConnection conn = new SqlConnection(_connectionStrings.Db_Connection))
            {
                conn.Open();

                string query = @"SELECT 
	                                CASE
		                                WHEN (SELECT COUNT(1) FROM UserLog WHERE Cedula = @Cedula) > 0
			                                THEN 1
			                                ELSE 0
	                                END";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Cedula", cedula);

                    bool result = (bool)cmd.ExecuteScalar();

                    conn.Close();

                    return result;
                }
            }
        }
    }
}
