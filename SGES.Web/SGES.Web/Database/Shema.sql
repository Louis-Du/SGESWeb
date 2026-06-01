CREATE DATABASE SGES;
GO

USE SGES;
GO

IF OBJECT_ID('dbo.Inscripciones', 'U') IS NOT NULL DROP TABLE dbo.Inscripciones;
IF OBJECT_ID('dbo.Grupos', 'U') IS NOT NULL DROP TABLE dbo.Grupos;
IF OBJECT_ID('dbo.Aprendiz', 'U') IS NOT NULL DROP TABLE dbo.Aprendiz;
IF OBJECT_ID('dbo.Eventos', 'U') IS NOT NULL DROP TABLE dbo.Eventos;
IF OBJECT_ID('dbo.Fichas', 'U') IS NOT NULL DROP TABLE dbo.Fichas;
IF OBJECT_ID('dbo.Programas', 'U') IS NOT NULL DROP TABLE dbo.Programas;
IF OBJECT_ID('dbo.Usuario', 'U') IS NOT NULL DROP TABLE dbo.Usuario;
GO

CREATE TABLE Usuario
(
    idUser INT IDENTITY(1,1) PRIMARY KEY,
    nombreUser VARCHAR(50) NOT NULL,
    emailUser VARCHAR(100) NOT NULL,
    contraseñaUser VARCHAR(8) NOT NULL,
    tipoUser VARCHAR(20) NOT NULL
);

CREATE TABLE Programas
(
    codigoProg INT PRIMARY KEY,
    nombreProg VARCHAR(50) NOT NULL,
    fechaIniProg DATE NOT NULL,
    fechaFinProg DATE NOT NULL,
    nivelProg VARCHAR(20) NOT NULL,
    duracionProg INT NOT NULL
);

CREATE TABLE Fichas
(
    codigoFic INT PRIMARY KEY,
    fechaIniFic DATE NOT NULL,
    fechaFinFic DATE NOT NULL,
    codigoProg INT NOT NULL,
    CONSTRAINT FK_Ficha_Programa FOREIGN KEY (codigoProg) REFERENCES Programas(codigoProg)
);

CREATE TABLE Eventos
(
    idEvento INT IDENTITY(1,1) PRIMARY KEY,
    nombreEvento VARCHAR(100) NOT NULL,
    tipoEvento VARCHAR(50) NOT NULL,
    diaEvento DATE NOT NULL,
    fechaHoraInicio DATETIME2(0) NOT NULL,
    fechaHoraFin DATETIME2(0) NOT NULL,
    idUser INT NOT NULL,
    CONSTRAINT FK_Evento_Usuario FOREIGN KEY (idUser) REFERENCES Usuario(idUser),
    CONSTRAINT CK_Evento_Fechas CHECK (fechaHoraFin > fechaHoraInicio)
);

CREATE TABLE Aprendiz
(
    idApr INT IDENTITY(1,1) PRIMARY KEY,
    nombreApr VARCHAR(50) NOT NULL,
    edadApr INT NOT NULL,
    emailApr VARCHAR(50) NOT NULL,
    contactoApr NUMERIC NOT NULL,
    nombreUser VARCHAR(50) NOT NULL,
    emailUser VARCHAR(100) NOT NULL,
    contraseñaUser VARCHAR(8) NOT NULL,
    tipoUser VARCHAR(20) NOT NULL,
    generoApr CHAR(1) NOT NULL,
    codigoFic INT NOT NULL,
    CONSTRAINT FK_Aprendiz_Ficha FOREIGN KEY (codigoFic) REFERENCES Fichas(codigoFic)
);

CREATE TABLE Grupos
(
    idGrupo INT IDENTITY(1,1) PRIMARY KEY,
    nombreGrupo VARCHAR(50) NOT NULL
);

CREATE TABLE Inscripciones
(
    idInscrip INT IDENTITY(1,1) PRIMARY KEY,
    fechaInscrip DATE NOT NULL,
    modalidadInscrip VARCHAR(10) NOT NULL,
    idApr INT NOT NULL,
    idEvento INT NOT NULL,
    idGrupo INT NULL,
    CONSTRAINT FK_Inscripcion_Aprendiz FOREIGN KEY (idApr) REFERENCES Aprendiz(idApr),
    CONSTRAINT FK_Inscripcion_Evento FOREIGN KEY (idEvento) REFERENCES Eventos(idEvento),
    CONSTRAINT FK_Inscripcion_Grupo FOREIGN KEY (idGrupo) REFERENCES Grupos(idGrupo)
);
GO