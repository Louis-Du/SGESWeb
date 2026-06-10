
CREATE DATABASE SGES;
GO

USE SGES;
GO

/*==================================================
TABLA: USUARIO
==================================================*/

CREATE TABLE Usuario
(
    idUser INT NOT NULL PRIMARY KEY,

    nombreUser VARCHAR(50) NOT NULL,

    emailUser VARCHAR(100) NOT NULL,

    passwordHash VARCHAR(255) NOT NULL,

    tipoUser VARCHAR(20) NOT NULL,

    CONSTRAINT UQ_Usuario_Email
        UNIQUE(emailUser)
);
GO

/*==================================================
TABLA: EVENTOS
==================================================*/

CREATE TABLE Eventos
(
    idEvento INT IDENTITY(1,1) NOT NULL PRIMARY KEY,

    nombreEvento VARCHAR(100) NOT NULL,

    tipoEvento VARCHAR(20) NOT NULL,

    fechaHoraInicio DATETIME2(0) NOT NULL,

    fechaHoraFin DATETIME2(0) NOT NULL,

    idUser INT NOT NULL,

    CONSTRAINT FK_Eventos_Usuario FOREIGN KEY(idUser) REFERENCES Usuario(idUser),

    CONSTRAINT CK_Eventos_Fechas CHECK(fechaHoraFin > fechaHoraInicio),

    CONSTRAINT CK_Eventos_Tipo CHECK ( 
    tipoEvento IN (
            'Educativo',
            'Deportivo',
            'Social',
            'Cultural'
        )
    )
);
GO

/*==================================================
TABLA: PROGRAMAS
==================================================*/

CREATE TABLE Programas
(
    codigoProg INT NOT NULL PRIMARY KEY,

    nombreProg VARCHAR(50) NOT NULL,

    fechaIniProg DATE NOT NULL,

    duracionProg INT NOT NULL,

    nivelProg VARCHAR(15) NOT NULL

);
GO

/*==================================================
TABLA: FICHAS
==================================================*/

CREATE TABLE Fichas
(
    codigoFic INT NOT NULL PRIMARY KEY,

    fechaIniFic DATE NOT NULL,

    fechaFinFic DATE NOT NULL,

    codigoProg INT NOT NULL,

    CONSTRAINT FK_Fichas_Programas FOREIGN KEY(codigoProg) REFERENCES Programas(codigoProg)

);
GO

/*==================================================
TABLA: APRENDIZ
==================================================*/

CREATE TABLE Aprendiz
(
    idApr INT NOT NULL PRIMARY KEY,

    nombreApr VARCHAR(50) NOT NULL,

    edadApr INT NOT NULL,

    emailApr VARCHAR(100) NOT NULL,

    contactoApr VARCHAR(15) NOT NULL,

    generoApr CHAR(1) NOT NULL,

    codigoFic INT NOT NULL,

    idUser INT NOT NULL,

    CONSTRAINT FK_Aprendiz_Fichas FOREIGN KEY(codigoFic) REFERENCES Fichas(codigoFic),

    CONSTRAINT FK_Aprendiz_Usuario FOREIGN KEY(idUser) REFERENCES Usuario(idUser),

    CONSTRAINT CK_Aprendiz_Genero CHECK(generoApr IN ('M','F'))

);
GO

/*==================================================
TABLA: GRUPOS
==================================================*/

CREATE TABLE Grupos
(
    idGrupo INT IDENTITY(1,1) NOT NULL PRIMARY KEY,

    nombreGrupo VARCHAR(20) NOT NULL

);
GO

/*==================================================
TABLA: INSCRIPCIONES
==================================================*/

CREATE TABLE Inscripciones
(
    idInscrip INT IDENTITY(1,1) NOT NULL PRIMARY KEY,

    fechaInscrip DATE NOT NULL,

    modalidadInscrip VARCHAR(15) NOT NULL,

    idApr INT NOT NULL,

    idEvento INT NOT NULL,

    idGrupo INT NULL,

    CONSTRAINT FK_Inscripciones_Aprendiz FOREIGN KEY(idApr) REFERENCES Aprendiz(idApr),

    CONSTRAINT FK_Inscripciones_Eventos FOREIGN KEY(idEvento) REFERENCES Eventos(idEvento),

    CONSTRAINT FK_Inscripciones_Grupos FOREIGN KEY(idGrupo) REFERENCES Grupos(idGrupo),

    CONSTRAINT CK_Inscripciones_Modalidad CHECK ( 
    modalidadInscrip IN (
                'Virtual',
                'Presencial'
            )
        ),

    CONSTRAINT UQ_Inscripcion_Aprendiz_Evento UNIQUE(idApr, idEvento)

);
GO

/*==================================================
ÍNDICES
==================================================*/

CREATE INDEX IX_Eventos_FechaInicio
ON Eventos(fechaHoraInicio);
GO

CREATE INDEX IX_Inscripciones_Evento
ON Inscripciones(idEvento);
GO

CREATE INDEX IX_Inscripciones_Aprendiz
ON Inscripciones(idApr);
GO
