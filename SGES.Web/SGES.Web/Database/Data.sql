USE SGES;
GO

/*==================================================
 USUARIOS
==================================================*/

INSERT INTO Usuario
(
    idUser,
    nombreUser,
    emailUser,
    passwordHash,
    tipoUser
)
VALUES
(1, 'Admin Principal', 'admin@sges.com', '123456', 'Administrador'),
(2, 'Carlos Perez', 'carlos@sges.com', '123456', 'Aprendiz'),
(3, 'Maria Lopez', 'maria@sges.com', '123456', 'Aprendiz'),
(4, 'Juan Torres', 'juan@sges.com', '123456', 'Aprendiz');

INSERT INTO Usuario
(
    idUser,
    nombreUser,
    emailUser,
    passwordHash,
    tipoUser
)
VALUES
(5, 'Ana Gomez', 'ana@sges.com', '123456', 'Aprendiz'),
(6, 'Sofia Ramirez', 'sofia@sges.com', '123456', 'Aprendiz'),
(7, 'Andres Martinez', 'andres@sges.com', '123456', 'Aprendiz');

GO

/*RESETEAR IDENTITY DE EVENTOS PARA QUE EMPIECE EN 1*/
DBCC CHECKIDENT ('Eventos', RESEED, 0);

/*==================================================
PROGRAMAS
==================================================*/

INSERT INTO Programas
(
    codigoProg,
    nombreProg,
    fechaIniProg,
    duracionProg,
    nivelProg
)
VALUES
(1001,'ADSO','2026-01-15',24,'Tecnologo'),
(1002,'Multimedia','2026-02-01',18,'Tecnico');

GO


/*==================================================
 FICHAS
==================================================*/

INSERT INTO Fichas
(
    codigoFic,
    fechaIniFic,
    fechaFinFic,
    codigoProg
)
VALUES
(2678901,'2026-01-15','2028-01-15',1001),
(2678902,'2026-02-01','2027-08-01',1002);

GO


/*==================================================
 APRENDICES
==================================================*/

INSERT INTO Aprendiz
(
    idApr,
    nombreApr,
    edadApr,
    emailApr,
    contactoApr,
    generoApr,
    codigoFic,
    idUser
)
VALUES
(
    1,
    'Carlos Perez',
    20,
    'carlos@sges.com',
    '3001112233',
    'M',
    2678901,
    2
),
(
    2,
    'Maria Lopez',
    22,
    'maria@sges.com',
    '3004445566',
    'F',
    2678901,
    3
),
(
    3,
    'Juan Torres',
    19,
    'juan@sges.com',
    '3017778899',
    'M',
    2678902,
    4
);

INSERT INTO Aprendiz
(
    idApr,
    nombreApr,
    edadApr,
    emailApr,
    contactoApr,
    generoApr,
    codigoFic,
    idUser
)

VALUES
(
    4,
    'Ana Gomez',
    21,
    'ana@sges.com',
    '3012223344',
    'F',
    2678902,
    5
),


(
    5,
    'Sofia Ramirez',
    23,
    'sofia@sges.com',
    '3015556677',
    'F',
    2678902,
    6
),
(
    6,
    'Andres Martinez',
    24,
    'andres@sges.com',
    '3018889900',
    'M',
    2678902,
    7
);

GO


/*==================================================
 GRUPOS
==================================================*/

INSERT INTO Grupos
(
    nombreGrupo,
    descripcion
)
VALUES
(
    'Grupo A',
    'Aprendices ADSO mañana'
),
(
    'Grupo B',
    'Aprendices ADSO tarde'
);

GO


/*==================================================
 EVENTOS
==================================================*/

DBCC CHECKIDENT ('Eventos', RESEED, 0);

INSERT INTO Eventos
(
    nombreEvento,
    tipoEvento,
    modalidadEvento,
    tipoInscrip,
    cupoMaximo,
    fechaHoraInicio,
    fechaHoraFin,
    idUser
)
VALUES
(
    'Introduccion a SQL Server',
    'Educativo',
    'Presencial',
    'Grupal',
    30,              -- cupoMaximo obligatorio para Grupal
    '2026-07-10 08:00',
    '2026-07-10 12:00',
    1
),
(
    'Torneo Deportivo SENA',
    'Deportivo',
    'Presencial',
    'Grupal',
    25,              -- cupoMaximo obligatorio para Grupal
    '2026-07-15 14:00',
    '2026-07-15 17:00',
    1
),
(
    'Charla de Inteligencia Artificial',
    'Educativo',
    'Virtual',
    'Individual',
    0,               -- Individual siempre va en 0
    '2026-07-20 09:00',
    '2026-07-20 11:00',
    1
);
GO


/*==================================================
 INSCRIPCIONES
==================================================*/

INSERT INTO Inscripciones
(
    fechaInscrip,
    idApr,
    idEvento,
    idGrupo
)
VALUES
(
    GETDATE(),
    1,
    1,
    1
),
(
    GETDATE(),
    2,
    1,
    1
),
(
    GETDATE(),
    3,
    2,
    2
);

GO

SELECT * FROM Usuario;
SELECT * FROM Programas;
SELECT * FROM Fichas;
SELECT * FROM Aprendiz;
SELECT * FROM Grupos;
SELECT * FROM Eventos;
SELECT * FROM Inscripciones;


