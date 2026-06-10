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
  FICHAS (depende de Programas)
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
INSERT INTO Eventos (nombreEvento, tipoEvento, fechaHoraInicio,          fechaHoraFin,             idUser)
VALUES
('Conferencia Tech',  'Educativo', '2026-08-10 09:00:00', '2026-08-10 10:30:00', 1),
('Hackathon SENA',    'Educativo', '2026-09-15 08:00:00', '2026-09-15 20:00:00', 2),
('Feria de Empleo',   'Social',    '2026-10-20 10:00:00', '2026-10-20 13:00:00', 1),
('Torneo de Futbol',  'Deportivo', '2026-11-05 14:00:00', '2026-11-05 17:00:00', 3),
('Festival Cultural', 'Cultural',  '2026-12-01 10:00:00', '2026-12-01 18:00:00', 2);
GO

/*==================================================
  VERIFICACIÓN
==================================================*/
SELECT * FROM Programas;
SELECT * FROM Fichas;
SELECT * FROM Usuario;
SELECT * FROM Aprendiz;
SELECT * FROM Eventos;
GO