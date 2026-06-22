USE SGES;
GO


/*==================================================
DATA: ADMINISTRADORES
==================================================*/

INSERT INTO Administrador
(
    idAdmin,
    nombreAdmin,
    emailAdmin,
    passwordHash
)
VALUES
(1,'Carlos Mendoza','carlos.admin@sges.com','12345'),
(2,'Laura Torres','laura.admin@sges.com','admin123');
GO



/*==================================================
DATA: PROGRAMAS
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
(1001,'Analisis y Desarrollo de Software',
'2026-01-15',
24,
'Tecnologo'),


(1002,'Diseño Grafico',
'2026-02-01',
18,
'Tecnico');
GO




/*==================================================
DATA: FICHAS
==================================================*/

INSERT INTO Fichas
(
    codigoFic,
    fechaIniFic,
    fechaFinFic,
    codigoProg
)
VALUES

(280001,
'2026-01-15',
'2028-01-15',
1001),


(280002,
'2026-02-01',
'2027-08-01',
1002);
GO




/*==================================================
DATA: APRENDICES
==================================================*/

INSERT INTO Aprendiz
(
    idApr,
    nombreApr,
    emailApr,
    passwordHash,
    edadApr,
    contactoApr,
    generoApr,
    codigoFic
)
VALUES 
(101, 'Juan Perez', 'juan.aprendiz@sges.com', '12345', 18, '3001112233', 'M', 280001),
(102, 'Maria Gomez', 'maria.aprendiz@sges.com', '12345', 20, '3004445566', 'F', 280001),
(103,'Pedro Ramirez', 'pedro.aprendiz@sges.com', '12345', 22, '3017778899', 'M', 280002),
(104, 'Daniela Sofia Rojas', 'daniela.rojas@email.com', 'DanyR2004', 22, '3109998877', 'F', 280001),
(105, 'Eduardo Jose Maestre', 'eduardo.maestre@email.com', 'EdMaestre#', 25, '3218887766', 'M', 280001),
(106, 'Fabiana Lucia Gomez', 'fabiana.gomez@email.com', 'FabiG2003', 23, '3057776655', 'F', 280001),
(107, 'Gabriel Alonso Rios', 'gabriel.rios@email.com', 'GaboRios!', 20, '3126665544', 'M', 280001),
(108, 'Helena Maria Vargas', 'helena.vargas@email.com', 'Hellen_V', 24, NULL, 'F', 280001),
(109, 'Ivan Dario Benitez', 'ivan.benitez@email.com', 'IvanD9988', 26, '3185554433', 'M', 280002),
(110, 'Jessica Paola Diaz', 'jessica.diaz@email.com', 'JessP@2026', 19, '3144443322', 'F', 280002),
(111, 'Kevin Alexander Ruiz', 'kevin.ruiz@email.com', 'KevRuiz9', 22, '3173332211', 'M', 280002),
(112, 'Laura Valentina Mora', 'laura.mora@email.com', 'LauraV_M', 20, '3012221100', 'F', 280002),
(113, 'Mateo Sebastian Gil', 'mateo.gil@email.com', 'MatGil88', 18, NULL, 'M', 280002),
(114, 'Natalia Ines Silva', 'natalia.silva@email.com', 'NatiSilva', 23, '3111110099', 'F', 280001),
(115, 'Oscar David Castro', 'oscar.castro@email.com', 'OscarC2026', 21, '3160009988', 'M', 280001),
(116, 'Paula Andrea Marin', 'paula.marin@email.com', 'PaulaM_99', 24, '3029998877', 'F', 280001),
(117, 'Ricardo Jose Guerra', 'ricardo.guerra@email.com', 'RichieG!', 25, '3048887766', 'M', 280001),
(118, 'Andres Felipe Ortiz', 'andres.ortiz@email.com', 'Clave123!', 19, '3005551122', 'M', 280001),
(119, 'Beatriz Elena Nuñez', 'beatriz.nunez@email.com', 'Betty_2026', 21, '3154443322', 'F', 280001),
(120, 'Camilo Andres Torres', 'camilo.torres@email.com', 'Milito99', 18, NULL, 'M', 280001)
GO




/*==================================================
DATA: EVENTOS
==================================================*/

INSERT INTO Eventos
(
nombreEvento,
tipoEvento,
modalidadEvento,
tipoInscrip,
cupoMaximo,
fechaHoraInicio,
fechaHoraFin,
idAdmin
)
VALUES


(
'Taller de Programacion Web',
'Educativo',
'Presencial',
'Grupal',
5,
'2026-07-10 08:00',
'2026-07-10 12:00',
1
),


(
'Torneo de Futbol',
'Deportivo',
'Presencial',
'Grupal',
16,
'2026-07-15 14:00',
'2026-07-15 18:00',
1
),


(
'Charla Inteligencia Artificial',
'Educativo',
'Virtual',
'Individual',
0,
'2026-08-01 10:00',
'2026-08-01 11:30',
2
),


(
'Festival Cultural',
'Cultural',
'Presencial',
'Grupal',
50,
'2026-08-20 09:00',
'2026-08-20 16:00',
2
);
GO




/*==================================================
DATA: GRUPOS
==================================================*/


INSERT INTO Grupos
(
nombreGrupo,
descripcion,
idEvento
)
VALUES

(
'Grupo A',
'Equipo principal del evento',
1
),

(
'Grupo B',
'Segundo grupo participante',
1
),

(
'Equipo Rojo',
'Participantes torneo futbol',
2
),

(
'Equipo Azul',
'Participantes torneo futbol',
2
);
GO




/*==================================================
DATA: INSCRIPCIONES
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
'2026-06-20',
101,
1,
1
),


(
'2026-06-20',
102,
1,
2
),


(
'2026-06-21',
103,
2,
3
),


(
'2026-06-22',
101,
3,
NULL
);
GO



/*==================================================
CONSULTAS DE PRUEBA
==================================================*/


-- Ver eventos con administrador

SELECT 
    E.nombreEvento,
    A.nombreAdmin
FROM Eventos E
INNER JOIN Administrador A
ON E.idAdmin = A.idAdmin;



-- Ver aprendices inscritos

SELECT
    AP.nombreApr,
    E.nombreEvento,
    I.fechaInscrip
FROM Inscripciones I
INNER JOIN Aprendiz AP
ON I.idApr = AP.idApr
INNER JOIN Eventos E
ON I.idEvento = E.idEvento;
GO