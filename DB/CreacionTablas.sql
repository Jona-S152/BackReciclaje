CREATE TABLE UserLog(
	Cedula VARCHAR(10) PRIMARY KEY,
	--Id INT PRIMARY KEY IDENTITY(1,1),
	NombreCompleto VARCHAR(100),
	Email VARCHAR(200),
	Teléfono VARCHAR(10),
	NombreUsuario VARCHAR(40),
	Contraseña VARCHAR(30)
)
GO

CREATE TABLE Puntos(
	Id INT PRIMARY KEY IDENTITY(1,1),
	Usuario VARCHAR(10) FOREIGN KEY REFERENCES UserLog(Cedula),
	CantidadBasura INT,
	PuntosObtenidos INT,
	FechaRegistro DATETIME,
)
GO