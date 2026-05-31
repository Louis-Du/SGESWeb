USE SGES;
GO

INSERT INTO Programas
VALUES
(101, 'Análisis de Datos', '2024-01-15', 24, 'Tecnólogo'),
(102, 'Desarrollo de Software', '2024-02-01', 18, 'Técnico'),
(103, 'Ciberseguridad', '2024-03-10', 12, 'Especialización');

INSERT INTO Usuario (nombreUser, emailUser, contraseñaUser, tipoUser)
VALUES
('Carlos Ruiz', 'carlosruiz@gmail.com', 'Admin123','Administrador'),
('Ana Martinez', 'anamartinez@gmail.com', 'Admin456', 'Administrador'),
('Luis Peña', 'luispena@gmail.com', 'Admin789', 'Administrador');

INSERT INTO Fichas
VALUES
(26701, '2024-01-20', '2026-01-20', 101),
(26702, '2024-02-05', '2025-08-05', 102),
(26703, '2024-03-15', '2025-03-15', 103);

INSERT INTO Eventos
(nombreEvento, tipoEvento, diaEvento, fechaHoraInicio, fechaHoraFin, idUser)
VALUES
('Conferencia Tech', 'Educativo', '2026-06-10', '2026-06-10 09:00', '2026-06-10 11:00', 1),
('Hackathon SENA', 'Competencia', '2026-07-15', '2026-07-15 08:00', '2026-07-15 20:00', 2),
('Feria de Empleo', 'Laboral', '2026-08-20', '2026-08-20 10:00', '2026-08-20 14:00', 1);

INSERT INTO Aprendiz
(nombreApr, edadApr, emailApr, contactoApr, nombreUser, emailUser, contraseñaUser, tipoUser, generoApr, codigoFic)
VALUES
('Carlos Ramirez', 20, 'carlosr@gmail.com', 3001234567, 'Carlos Ramirez', 'carlosr@gmail.com', 'pass123', 'Aprendiz', 'M', 26701),
('Laura Gomez', 22, 'laurag@gmail.com', 3019876543, 'Laura Gomez', 'laurag@gmail.com', 'pass456', 'Aprendiz', 'F', 26702),
('Andres Torres', 19, 'andrest@gmail.com', 3024567890, 'Andres Torres', 'andrest@gmail.com', 'pass789', 'Aprendiz', 'M', 26703);

INSERT INTO Grupos
(nombreGrupo)
VALUES
('Grupo A'),
('Grupo B');

INSERT INTO Inscripciones
(fechaInscrip, modalidadInscrip, idApr, idEvento, idGrupo)
VALUES
(GETDATE(), 'Presencial', 1, 1, 1),
(GETDATE(), 'Virtual', 2, 2, 2);
GO