USE SGES;
GO

/*==================================================
  PROGRAMAS
==================================================*/
INSERT INTO Programas VALUES (101, 'Análisis de Datos',      '2024-01-15', 24, 'Tecnólogo');
INSERT INTO Programas VALUES (102, 'Desarrollo de Software', '2024-02-01', 18, 'Técnico');
INSERT INTO Programas VALUES (103, 'Ciberseguridad',         '2024-03-10', 12, 'Especialización');
GO

/*==================================================
  FICHAS (sin hora, ahora son DATE))
==================================================*/

INSERT INTO Fichas VALUES (26701, '2024-01-20', '2026-01-20', 101);
INSERT INTO Fichas VALUES (26702, '2024-02-05', '2026-08-05', 102);
INSERT INTO Fichas VALUES (26703, '2024-03-15', '2026-03-15', 103);
GO


/*==================================================
  USUARIOS — Administradores
==================================================*/
INSERT INTO Usuario VALUES (1, 'Carlos Ruiz',   'carlosruiz@gmail.com', 'Admin12*', 'Administrador');
INSERT INTO Usuario VALUES (2, 'Ana Martínez',  'anamartines@gmail.com','Pass789#', 'Administrador');
INSERT INTO Usuario VALUES (3, 'Luis Peña',     'luispena@gmail.com',   'Sena2024', 'Administrador');
GO

/*==================================================
  USUARIOS — Aprendices
  (cada aprendiz necesita un registro en Usuario)
==================================================*/
INSERT INTO Usuario VALUES (4, 'Carlos Ramirez',  'carlosr@gmail.com',   'pass123',  'Aprendiz');
INSERT INTO Usuario VALUES (5, 'Laura Gomez',     'laurag@gmail.com',    'clave456', 'Aprendiz');
INSERT INTO Usuario VALUES (6, 'Andres Torres',   'andrest@gmail.com',   'abc789',   'Aprendiz');
INSERT INTO Usuario VALUES (7, 'Sofia Martinez',  'sofijd@gmail.com',    'xyz123',   'Aprendiz');
INSERT INTO Usuario VALUES (8, 'Diego Fernandez', 'diegdx@outlook.com',  'pass789',  'Aprendiz');
INSERT INTO Usuario VALUES (9, 'Valentina Lopez', 'vale2d9@outlook.com', 'clave321', 'Aprendiz');
GO

/*==================================================
  APRENDIZ (depende de Fichas y Usuario)
  Columnas: idApr, nombreApr, edadApr, emailApr,
            contactoApr, generoApr, codigoFic, idUser
==================================================*/
INSERT INTO Aprendiz VALUES (4, 'Carlos Ramirez',  20, 'carlosr@gmail.com',   '3001234567', 'M', 26701, 4);
INSERT INTO Aprendiz VALUES (5, 'Laura Gomez',     22, 'laurag@gmail.com',    '3019876543', 'F', 26702, 5);
INSERT INTO Aprendiz VALUES (6, 'Andres Torres',   19, 'andrest@gmail.com',   '3024567890', 'M', 26703, 6);
INSERT INTO Aprendiz VALUES (7, 'Sofia Martinez',  21, 'sofijd@gmail.com',    '3031234567', 'F', 26701, 7);
INSERT INTO Aprendiz VALUES (8, 'Diego Fernandez', 23, 'diegdx@outlook.com',  '3049876543', 'M', 26702, 8);
INSERT INTO Aprendiz VALUES (9, 'Valentina Lopez', 20, 'vale2d9@outlook.com', '3054567890', 'F', 26703, 9);
GO

/*==================================================
  EVENTOS (depende de Usuario)
  - idEvento es IDENTITY, no se inserta manual
  - fechas en 2026 para que el programa las acepte
  - tipoEvento solo acepta: Educativo, Deportivo, Social, Cultural
==================================================*/
INSERT INTO Eventos (nombreEvento, tipoEvento, modalidadEvento, tipoInscrip, fechaHoraInicio, fechaHoraFin, idUser)
VALUES
('Conferencia Tech',  'Educativo', 'Virtual',    'Individual', '2026-08-10 09:00:00', '2026-08-10 10:30:00', 1),
('Hackathon SENA',    'Educativo', 'Presencial', 'Grupal',     '2026-09-15 08:00:00', '2026-09-15 20:00:00', 2),
('Feria de Empleo',   'Social',    'Presencial', 'Individual', '2026-10-20 10:00:00', '2026-10-20 13:00:00', 1),
('Torneo de Futbol',  'Deportivo', 'Presencial', 'Grupal',     '2026-11-05 14:00:00', '2026-11-05 17:00:00', 3),
('Festival Cultural', 'Cultural',  'Presencial', 'Individual', '2026-12-01 10:00:00', '2026-12-01 18:00:00', 2);
GO

-- GRUPOS
INSERT INTO Grupos (nombreGrupo) VALUES ('Grupo 1');
INSERT INTO Grupos (nombreGrupo) VALUES ('Grupo 2');
INSERT INTO Grupos (nombreGrupo) VALUES ('Grupo 3');
GO

-- Eventos 1,3,5 son Individual → idGrupo NULL
-- Eventos 2,4   son Grupal     → idGrupo asignado
INSERT INTO Inscripciones (fechaInscrip, idApr, idEvento, idGrupo)
VALUES
('2026-07-01', 4, 1, NULL),
('2026-07-02', 5, 2, 1),
('2026-07-03', 6, 3, NULL),
('2026-07-04', 7, 4, 2),
('2026-07-05', 8, 5, NULL),
('2026-07-06', 9, 2, 1);
GO

/*RESETEAR IDENTITY DE EVENTOS PARA QUE EMPIECE EN 1*/
DBCC CHECKIDENT ('Eventos', RESEED, 0);

/*==================================================
  VERIFICACIÓN
==================================================*/
SELECT * FROM Programas;
SELECT * FROM Fichas;
SELECT * FROM Usuario;
SELECT * FROM Aprendiz;
SELECT * FROM Eventos;
GO