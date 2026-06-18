CREATE DATABASE SGES;
GO

USE SGES;
GO

select * from Grupos
/*==================================================
TABLA: ADMINISTRADOR
==================================================*/

CREATE TABLE Administrador
(
    idAdmin INT NOT NULL PRIMARY KEY,

    nombreAdmin VARCHAR(50) NOT NULL,

    emailAdmin VARCHAR(100) NOT NULL,

    passwordHash VARCHAR(255) NOT NULL,


    CONSTRAINT UQ_Admin_Email
        UNIQUE(emailAdmin)
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


    CONSTRAINT FK_Fichas_Programa
    FOREIGN KEY(codigoProg)
    REFERENCES Programas(codigoProg)
);
GO



/*==================================================
TABLA: APRENDIZ
==================================================*/

CREATE TABLE Aprendiz
(
    idApr INT NOT NULL PRIMARY KEY,

    nombreApr VARCHAR(50) NOT NULL,

    emailApr VARCHAR(100) NOT NULL,

    passwordHash VARCHAR(255) NOT NULL,


    edadApr INT NOT NULL,

    contactoApr VARCHAR(15),

    generoApr CHAR(1) NOT NULL,


    codigoFic INT NOT NULL,


    CONSTRAINT UQ_Aprendiz_Email
    UNIQUE(emailApr),


    CONSTRAINT FK_Aprendiz_Ficha
    FOREIGN KEY(codigoFic)
    REFERENCES Fichas(codigoFic),


    CONSTRAINT CK_Aprendiz_Genero
    CHECK(generoApr IN ('M','F'))
);
GO



/*==================================================
TABLA: EVENTOS
==================================================*/

CREATE TABLE Eventos
(
    idEvento INT IDENTITY(1,1) PRIMARY KEY,


    nombreEvento VARCHAR(100) NOT NULL,


    tipoEvento VARCHAR(20) NOT NULL,


    modalidadEvento VARCHAR(15) NOT NULL,


    tipoInscrip VARCHAR(10) NOT NULL,


    cupoMaximo INT NOT NULL DEFAULT 0,


    fechaHoraInicio DATETIME2(0) NOT NULL,

    fechaHoraFin DATETIME2(0) NOT NULL,


    idAdmin INT NOT NULL,


    CONSTRAINT FK_Evento_Admin
    FOREIGN KEY(idAdmin)
    REFERENCES Administrador(idAdmin),


    CONSTRAINT CK_Evento_Fecha
    CHECK(fechaHoraFin > fechaHoraInicio),


    CONSTRAINT CK_Evento_Tipo
    CHECK(tipoEvento IN
    (
        'Educativo',
        'Deportivo',
        'Social',
        'Cultural'
    )),


    CONSTRAINT CK_Evento_Modalidad
    CHECK(modalidadEvento IN
    (
        'Virtual',
        'Presencial'
    )),


    CONSTRAINT CK_Evento_Inscripcion
    CHECK(tipoInscrip IN
    (
        'Individual',
        'Grupal'
    )),


    CONSTRAINT CK_Evento_Cupo
    CHECK
    (
        tipoInscrip='Individual'
        OR
        (tipoInscrip='Grupal' AND cupoMaximo>2)
    )
);
GO



/*==================================================
TABLA: GRUPOS
==================================================*/

CREATE TABLE Grupos
(
    idGrupo INT IDENTITY(1,1) PRIMARY KEY,


    nombreGrupo VARCHAR(20) NOT NULL,


    descripcion VARCHAR(100),


    idEvento INT NOT NULL,


    CONSTRAINT FK_Grupo_Evento
    FOREIGN KEY(idEvento)
    REFERENCES Eventos(idEvento)
);
GO



/*==================================================
TABLA: INSCRIPCIONES
==================================================*/

CREATE TABLE Inscripciones
(
    idInscrip INT IDENTITY(1,1) PRIMARY KEY,


    fechaInscrip DATE NOT NULL,


    idApr INT NOT NULL,


    idEvento INT NOT NULL,


    idGrupo INT NULL,



    CONSTRAINT FK_Inscripcion_Aprendiz
    FOREIGN KEY(idApr)
    REFERENCES Aprendiz(idApr),


    CONSTRAINT FK_Inscripcion_Evento
    FOREIGN KEY(idEvento)
    REFERENCES Eventos(idEvento),


    CONSTRAINT FK_Inscripcion_Grupo
    FOREIGN KEY(idGrupo)
    REFERENCES Grupos(idGrupo),


    CONSTRAINT UQ_Aprendiz_Evento
    UNIQUE(idApr,idEvento)
);
GO




/*==================================================
INDICES
==================================================*/


CREATE INDEX IX_Eventos_Fecha
ON Eventos(fechaHoraInicio);
GO


CREATE INDEX IX_Inscripciones_Evento
ON Inscripciones(idEvento);
GO


CREATE INDEX IX_Inscripciones_Aprendiz
ON Inscripciones(idApr);
GO