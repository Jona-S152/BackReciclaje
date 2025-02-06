﻿using BackReciclaje.Model;
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

        public UserPuntos Login(UserLogin uLogin)
        {
            using (SqlConnection conn = new SqlConnection(_connectionStrings.Db_Connection))
            {
                conn.Open();

                string query = @"SELECT u.Cedula, u.NombreUsuario, p.* FROM UserLog u 
                                JOIN Puntos p ON u.Cedula = p.Usuario
                                WHERE Email = @Email AND Contraseña = @Contraseña";

                UserPuntos result = null;

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", uLogin.Email);
                    cmd.Parameters.AddWithValue("@Contraseña", uLogin.Contraseña);

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
