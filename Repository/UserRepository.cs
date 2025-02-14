using BackReciclaje.Model;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

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
                        cmd.Parameters.AddWithValue("@NombreCompleto", (object)user.NombreCompleto ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Email", (object)user.Email ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Telefono", (object)user.Telefono ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@NombreUsuario", user.NombreUsuario);
                        cmd.Parameters.AddWithValue("@Contraseña", user.Clave);

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

        public List<Ranking> GetTopToRanking()
        {
            using (SqlConnection conn = new SqlConnection(_connectionStrings.Db_Connection))
            {
                conn.Open();

                string query = @"SELECT TOP 3 U.Cedula, u.NombreUsuario, SUM(p.PuntosObtenidos) AS TotalPuntos FROM UserLog u 
                                LEFT JOIN Puntos p ON u.Cedula = p.Usuario
                                GROUP BY U.Cedula, u.NombreUsuario
                                ORDER BY TotalPuntos DESC";

                List<Ranking> result = new List<Ranking>();

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Ranking user = new Ranking();

                            user.Cedula = reader.IsDBNull(reader.GetOrdinal("Cedula")) ? string.Empty : reader.GetString(reader.GetOrdinal("Cedula"));
                            user.NombreUsuario = reader.IsDBNull(reader.GetOrdinal("NombreUsuario")) ? string.Empty : reader.GetString(reader.GetOrdinal("NombreUsuario"));
                            user.TotalPuntos = reader.IsDBNull(reader.GetOrdinal("TotalPuntos")) ? 0 : reader.GetInt32(reader.GetOrdinal("TotalPuntos"));

                            result.Add(user);
                        }
                    }
                }

                conn.Close();

                return result;
            }
        }

        public UserPuntos Login(UserLogin uLogin)
        {
            using (SqlConnection conn = new SqlConnection(_connectionStrings.Db_Connection))
            {
                conn.Open();

                string query = @"SELECT u.Cedula, u.NombreUsuario, p.* FROM UserLog u 
                                LEFT JOIN Puntos p ON u.Cedula = p.Usuario
                                WHERE Email = @Email AND Contraseña = @Contraseña";

                UserPuntos result = null;

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", uLogin.Email);
                    cmd.Parameters.AddWithValue("@Contraseña", uLogin.Clave);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result = new UserPuntos();

                            result.Cedula = reader.IsDBNull(reader.GetOrdinal("Cedula")) ? string.Empty : reader.GetString(reader.GetOrdinal("Cedula"));
                            result.NombreUsuario = reader.IsDBNull(reader.GetOrdinal("NombreUsuario")) ? string.Empty : reader.GetString(reader.GetOrdinal("NombreUsuario"));
                            result.Id = reader.IsDBNull(reader.GetOrdinal("Id")) ? 0 : reader.GetInt32(reader.GetOrdinal("Id"));
                            result.CantidadBasura = reader.IsDBNull(reader.GetOrdinal("CantidadBasura")) ? null : reader.GetInt32(reader.GetOrdinal("CantidadBasura"));
                            result.PuntosObtenidos = reader.IsDBNull(reader.GetOrdinal("PuntosObtenidos")) ? null : reader.GetInt32(reader.GetOrdinal("PuntosObtenidos"));
                            result.FechaRegistro = reader.IsDBNull(reader.GetOrdinal("FechaRegistro")) ? null : reader.GetDateTime(reader.GetOrdinal("FechaRegistro"));
                        }
                    }

                    conn.Close();

                    return result;
                }
            }
        }

        public bool SavePoints(Puntos points)
        {
            using (SqlConnection conn = new SqlConnection(_connectionStrings.Db_Connection))
            {
                conn.Open();
                string query = @"IF NOT EXISTS (SELECT 1 FROM UserLog WHERE Cedula = @UsuarioCedula)
                                            BEGIN
	                                            RAISERROR ('El usuario con cédula %s no se encuentra registrado.', 16, 1, @UsuarioCedula);
                                                RETURN;
                                            END

                                            IF NOT EXISTS (SELECT 1 FROM Puntos WHERE Usuario = @UsuarioCedula)
                                            BEGIN
                                                INSERT INTO Puntos VALUES (@UsuarioCedula, @CantidadBasura, (@CantidadBasura * 5), GETDATE())
                                            END
                                            ELSE
                                            BEGIN
	                                            IF (
		                                            SELECT 
			                                            CASE 
				                                            WHEN DATEPART(MINUTE, GETDATE() - pp.FechaRegistro) > 0
					                                            THEN CAST(1 AS BIT)
					                                            ELSE CAST(0 AS BIT)
			                                            END
		                                            FROM (
			                                            SELECT TOP 1 p.FechaRegistro, p.Usuario 
			                                            FROM Puntos p
			                                            WHERE Usuario = @UsuarioCedula
			                                            ORDER BY p.FechaRegistro DESC
		                                            ) AS pp) = 1
		                                            BEGIN
			                                            INSERT INTO Puntos VALUES (@UsuarioCedula, @CantidadBasura, (@CantidadBasura * 5), GETDATE())
		                                            END
	                                            ELSE
		                                            BEGIN
			                                            RAISERROR ('Error intentalo de nuevo después de 1 minuto.', 16, 1);
			                                            RETURN;
		                                            END
                                            END";
                
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UsuarioCedula", points.UsuarioCedula);
                    cmd.Parameters.AddWithValue("@CantidadBasura", points.CantidadBasura);

                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

                return true;
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
			                                THEN CAST(1 AS BIT)
			                                ELSE CAST(0 AS BIT)
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
