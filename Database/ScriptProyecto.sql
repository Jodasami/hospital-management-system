/* ----------------------------------- Creacion de la BD ----------------------------------- */
CREATE DATABASE SistemaHospitales

USE SistemaHospitales

CREATE TABLE Hospital (
    IdHospital INT PRIMARY KEY IDENTITY,
    Nombre NVARCHAR(100),
    Direccion NVARCHAR(200),
    Telefono NVARCHAR(20)
);

CREATE TABLE Paciente (
    IdPaciente VARCHAR(20) PRIMARY KEY,
    Nombre NVARCHAR(50),
    Apellido NVARCHAR(50),
    FechaNacimiento DATE,
    Genero CHAR(1),
    Direccion NVARCHAR(200),
    Telefono NVARCHAR(20),
    IdHospital INT,
    FOREIGN KEY (IdHospital) REFERENCES Hospital(IdHospital)
);

CREATE TABLE Medico (
    IdMedico INT PRIMARY KEY IDENTITY,
    Nombre NVARCHAR(100),
    Especialidad NVARCHAR(100),
    Telefono NVARCHAR(20),
    Email NVARCHAR(100),
    IdHospital INT,
    FOREIGN KEY (IdHospital) REFERENCES Hospital(IdHospital)
);

CREATE TABLE Cita (
    IdCita INT PRIMARY KEY IDENTITY,
    Fecha DATE,
    Hora TIME,
    IdMedico INT,
    IdPaciente VARCHAR(20),
    IdHospital INT,
    Diagnostico NVARCHAR(500),
    FOREIGN KEY (IdMedico) REFERENCES Medico(IdMedico),
    FOREIGN KEY (IdPaciente) REFERENCES Paciente(IdPaciente),
    FOREIGN KEY (IdHospital) REFERENCES Hospital(IdHospital)
);

CREATE TABLE Tratamiento (
    IdTratamiento INT PRIMARY KEY IDENTITY,
    IdCita INT,
    CostoTotal DECIMAL(10,2),
    FOREIGN KEY (IdCita) REFERENCES Cita(IdCita)
);

CREATE TABLE Medicamento (
    IdMedicamento INT PRIMARY KEY IDENTITY,
    Nombre NVARCHAR(100),
    Descripcion NVARCHAR(200),
    CostoUnidad DECIMAL(10,2)
);

CREATE TABLE Medicamento_Hospital (
    IdHospital INT,
    IdMedicamento INT,
    CantidadDisponible INT,
    PRIMARY KEY (IdHospital, IdMedicamento),
    FOREIGN KEY (IdHospital) REFERENCES Hospital(IdHospital),
    FOREIGN KEY (IdMedicamento) REFERENCES Medicamento(IdMedicamento)
);

CREATE TABLE Tratamiento_Medicamento (
    IdTratamiento INT,
    IdMedicamento INT,
    Cantidad INT,
    PRIMARY KEY (IdTratamiento, IdMedicamento),
    FOREIGN KEY (IdTratamiento) REFERENCES Tratamiento(IdTratamiento),
    FOREIGN KEY (IdMedicamento) REFERENCES Medicamento(IdMedicamento)
);

CREATE TABLE Pago (
    IdPago INT PRIMARY KEY IDENTITY,
    IdPaciente VARCHAR(20),
    IdTratamiento INT,
    Fecha DATE,
    Monto DECIMAL(10,2),
    MetodoPago NVARCHAR(50),
    Pagado BIT,
    FOREIGN KEY (IdPaciente) REFERENCES Paciente(IdPaciente),
    FOREIGN KEY (IdTratamiento) REFERENCES Tratamiento(IdTratamiento)
);

CREATE TABLE Rol (
    IdRol INT PRIMARY KEY,
    NombreRol NVARCHAR(50) NOT NULL -- 1 Paciente, 2 Medico, 3 Admin;
);

CREATE TABLE Usuario (
    IdUsuario INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Contrasena NVARCHAR(255) NOT NULL,
    IdRol INT NOT NULL,
    IdPaciente VARCHAR(20) NULL,
    IdMedico INT NULL,
    FOREIGN KEY (IdRol) REFERENCES Rol(IdRol),
    FOREIGN KEY (IdPaciente) REFERENCES Paciente(IdPaciente),
    FOREIGN KEY (IdMedico) REFERENCES Medico(IdMedico)
);


/* +++++++++++++++++++++++++++++++++ FIN CREACION DE TABLAS +++++++++++++++++++++++++++++++++ */



/* ----------------------------------- Procedimientos Almacenados ----------------------------------- */

-- 1. Procedimiento almacenado para Obtener todos los pacientes atendidos por un médico
	-- en un hospital específico durante un periodo determinado:
CREATE PROCEDURE ObtenerPacientesPorMedico
    @IdMedico INT,
    @IdHospital INT,
    @FechaInicio DATE,
    @FechaFin DATE
AS
BEGIN
    BEGIN TRY
        SELECT DISTINCT p.IdPaciente, p.Nombre, p.Apellido
        FROM Cita c
        JOIN Paciente p ON c.IdPaciente = p.IdPaciente
        WHERE c.IdMedico = @IdMedico
          AND c.IdHospital = @IdHospital
          AND c.Fecha BETWEEN @FechaInicio AND @FechaFin;
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000), @ErrorSeverity INT, @ErrorState INT;
        SELECT 
            @ErrorMessage = ERROR_MESSAGE(),
            @ErrorSeverity = ERROR_SEVERITY(),
            @ErrorState = ERROR_STATE();
        RAISERROR('Error al obtener pacientes: %s', @ErrorSeverity, @ErrorState, @ErrorMessage);
    END CATCH
END;




-- 2. Procedimiento almacenado Obtener el inventario de medicamentos y las prescripciones realizadas
	-- en los últimos 30 días para un hospital específico:
CREATE PROCEDURE InventarioYPrescripciones
    @IdHospital INT
AS
BEGIN
    BEGIN TRY
        SELECT m.IdMedicamento, m.Nombre, mh.CantidadDisponible,
               SUM(tm.Cantidad) AS CantidadPrescritaUltimos30Dias
        FROM Medicamento m
        JOIN Medicamento_Hospital mh ON m.IdMedicamento = mh.IdMedicamento AND mh.IdHospital = @IdHospital
        LEFT JOIN Tratamiento_Medicamento tm ON m.IdMedicamento = tm.IdMedicamento
        LEFT JOIN Tratamiento t ON t.IdTratamiento = tm.IdTratamiento
        LEFT JOIN Cita c ON c.IdCita = t.IdCita AND c.Fecha >= DATEADD(DAY, -30, GETDATE()) AND c.IdHospital = @IdHospital
        GROUP BY m.IdMedicamento, m.Nombre, mh.CantidadDisponible;
    END TRY
    BEGIN CATCH
        -- Manejo de errores
        DECLARE @ErrorMessage NVARCHAR(4000), @ErrorSeverity INT, @ErrorState INT;
        SELECT 
            @ErrorMessage = ERROR_MESSAGE(),
            @ErrorSeverity = ERROR_SEVERITY(),
            @ErrorState = ERROR_STATE();
        
        RAISERROR('Error al obtener inventario y prescripciones: %s', @ErrorSeverity, @ErrorState, @ErrorMessage);
    END CATCH
END;



-- 3. Procedimiento almacenado Obtener el historial de pagos pendientes de un paciente:
CREATE PROCEDURE PagosPendientesPorPaciente
    @IdPaciente VARCHAR(20)
AS
BEGIN
    BEGIN TRY
        SELECT p.IdPago, p.Fecha, p.Monto, p.MetodoPago
        FROM Pago p
        WHERE p.IdPaciente = @IdPaciente AND p.Pagado = 0;
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000), @ErrorSeverity INT, @ErrorState INT;
        SELECT 
            @ErrorMessage = ERROR_MESSAGE(),
            @ErrorSeverity = ERROR_SEVERITY(),
            @ErrorState = ERROR_STATE();
        RAISERROR('Error al obtener pagos pendientes: %s', @ErrorSeverity, @ErrorState, @ErrorMessage);
    END CATCH
END;




-- 4. Procedimiento Almacenado para Registrar una Nueva Cita y Actualizar la Disponibilidad del Médico:
CREATE PROCEDURE RegistrarNuevaCita
    @Fecha DATE,
    @Hora TIME,
    @IdMedico INT,
    @IdPaciente VARCHAR(20),
    @IdHospital INT,
    @Diagnostico NVARCHAR(500) = NULL
AS
BEGIN
    BEGIN TRY
        IF EXISTS (
            SELECT 1
            FROM Cita
            WHERE Fecha = @Fecha AND Hora = @Hora AND IdMedico = @IdMedico
        )
        BEGIN
            RAISERROR('El médico ya tiene una cita asignada en esa fecha y hora.', 16, 1);
            RETURN;
        END;

        INSERT INTO Cita (Fecha, Hora, IdMedico, IdPaciente, IdHospital, Diagnostico)
        VALUES (@Fecha, @Hora, @IdMedico, @IdPaciente, @IdHospital, @Diagnostico);

        PRINT 'Cita registrada exitosamente.';
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000), @ErrorSeverity INT, @ErrorState INT;
        SELECT 
            @ErrorMessage = ERROR_MESSAGE(),
            @ErrorSeverity = ERROR_SEVERITY(),
            @ErrorState = ERROR_STATE();
        RAISERROR('Error al registrar la cita: %s', @ErrorSeverity, @ErrorState, @ErrorMessage);
    END CATCH
END;



-- 5. Procedimiento Almacenado para Calcular el Total de Pagos Realizados por un Paciente en un Rango de Fechas
CREATE PROCEDURE TotalPagosRealizadosPorPaciente
    @IdPaciente VARCHAR(20),
    @FechaInicio DATE,
    @FechaFin DATE
AS
BEGIN
    BEGIN TRY
        SELECT SUM(Monto) AS TotalPagado
        FROM Pago
        WHERE IdPaciente = @IdPaciente
          AND Fecha BETWEEN @FechaInicio AND @FechaFin
          AND Pagado = 1;
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000), @ErrorSeverity INT, @ErrorState INT;
        SELECT 
            @ErrorMessage = ERROR_MESSAGE(),
            @ErrorSeverity = ERROR_SEVERITY(),
            @ErrorState = ERROR_STATE();
        RAISERROR('Error al calcular el total de pagos realizados: %s', @ErrorSeverity, @ErrorState, @ErrorMessage);
    END CATCH
END;
/* +++++++++++++++++++++++++++++++++ FIN PROCEDIMIENTOS ALMACENADOS (SP) +++++++++++++++++++++++++++++++++ */



/* ----------------------------------- Triggers ----------------------------------- */

-- Trigger para Controlar el Stock de Medicamentos:
CREATE TRIGGER tr_ActualizarStockMedicamento
ON Tratamiento_Medicamento
AFTER INSERT
AS
BEGIN
    DECLARE @IdMedicamento INT, @Cantidad INT, @IdTratamiento INT, @IdHospital INT;

    SELECT TOP 1
        @IdMedicamento = i.IdMedicamento,
        @Cantidad = i.Cantidad,
        @IdTratamiento = i.IdTratamiento
    FROM INSERTED i;

    SELECT @IdHospital = c.IdHospital
    FROM Tratamiento t
    JOIN Cita c ON t.IdCita = c.IdCita
    WHERE t.IdTratamiento = @IdTratamiento;

    IF EXISTS (
        SELECT * FROM Medicamento_Hospital
        WHERE IdMedicamento = @IdMedicamento AND IdHospital = @IdHospital AND CantidadDisponible >= @Cantidad
    )
    BEGIN
        UPDATE Medicamento_Hospital
        SET CantidadDisponible = CantidadDisponible - @Cantidad
        WHERE IdMedicamento = @IdMedicamento AND IdHospital = @IdHospital;
    END
    ELSE
    BEGIN
        RAISERROR('Stock insuficiente para este medicamento.', 16, 1);
        ROLLBACK;
    END
END;



-- Trigger para crear un usuario automáticamente cuando se agrega un nuevo MÉDICO
CREATE TRIGGER tr_CrearUsuarioMedico
ON Medico
AFTER INSERT
AS
BEGIN
    INSERT INTO Usuario (Username, Contrasena, IdRol, IdMedico)
    SELECT 
        LOWER(LEFT(Nombre, 3) + CAST(IdMedico AS NVARCHAR)) AS Username,
        CAST(IdMedico AS NVARCHAR), -- contraseña: IdMedico
        2, -- IdRol = 2 para Médico
        IdMedico
    FROM INSERTED;
END;




-- Trigger para crear un usuario automáticamente cuando se agrega un nuevo PACIENTE
CREATE TRIGGER tr_CrearUsuarioPaciente
ON Paciente
AFTER INSERT
AS
BEGIN
    INSERT INTO Usuario (Username, Contrasena, IdRol, IdPaciente)
    SELECT 
        LOWER(LEFT(Nombre, 3) + IdPaciente) AS Username,
        IdPaciente, -- contraseña = IdPaciente
        1, -- IdRol = 1 para Paciente
        IdPaciente
    FROM INSERTED;
END;


-- Trigger para almacenar los pagos en las tablas correspondientes
CREATE TRIGGER tr_CrearPagoPorTratamiento
ON Tratamiento
AFTER INSERT
AS
BEGIN
    INSERT INTO Pago (IdPaciente, IdTratamiento, Fecha, Monto, MetodoPago, Pagado)
    SELECT 
        p.IdPaciente,
        i.IdTratamiento,
        GETDATE(), -- Fecha actual
        i.CostoTotal,
        'Pendiente',
    FROM INSERTED i
    JOIN Cita c ON i.IdCita = c.IdCita
    JOIN Paciente p ON p.IdPaciente = c.IdPaciente;
END;

/* +++++++++++++++++++++++++++++++++ FIN TRIGGERS +++++++++++++++++++++++++++++++++ */



/* ----------------------------------- Inserts ----------------------------------- */


------------------------ Insertar roles ------------------------
INSERT INTO Rol (IdRol, NombreRol) VALUES (1, 'Paciente');

INSERT INTO Rol (IdRol, NombreRol) VALUES (2, 'Medico');

INSERT INTO Rol (IdRol, NombreRol) VALUES (3, 'Admin');

--********************************** FIN ROLES **********************************--


------------------------ Insert Usuarios ------------------------
--INSERT INTO Usuario (Username, Contrasena, IdRol, IdPaciente)
--VALUES ('jodasami', '1234', 1, '118160452');

--INSERT INTO Usuario (Username, Contrasena, IdRol, IdMedico)
--VALUES ('Dr.Ana', '1234', 2, 1);

INSERT INTO Usuario (Username, Contrasena, IdRol)
VALUES ('Admin', 'admin', 3);

--********************************** FIN USUARIOS **********************************--

------------------------ Insertar Hospitales ------------------------
INSERT INTO Hospital (Nombre, Direccion, Telefono) VALUES 
('Hospital Max Peralta', 'Cartago Centro, Cartago', '2222-1111');

INSERT INTO Hospital (Nombre, Direccion, Telefono) VALUES 
('Hospital San Juan de Dios', 'San José Centro, San José', '2222-2222');

--********************************** FIN HOSPITALES **********************************--



------------------------ Insertar Medicos ------------------------
INSERT INTO Medico (Nombre, Especialidad, Telefono, Email, IdHospital) VALUES 
('Dra. Ana López', 'Oncología', '8888-1234', 'an.lopez@sanjuandios.com', 2)

INSERT INTO Medico (Nombre, Especialidad, Telefono, Email, IdHospital) VALUES 
('Dra. Laura Ruiz', 'Ginecología', '8888-9876', 'la.ruiz@sanjuandios.com', 2);

INSERT INTO Medico (Nombre, Especialidad, Telefono, Email, IdHospital) VALUES 
('Dr. Carlos Pérez', 'Cardiología', '8888-5678', 'carlos.perez@maxperalta.com', 1)

INSERT INTO Medico (Nombre, Especialidad, Telefono, Email, IdHospital) VALUES 
('Dra. Cintia Pacheco', 'Dermatología', '8777-5555', 'cintia.pacheco@maxperalta.com', 1);

--********************************** FIN MEDICOS **********************************--



------------------------ Insert Pacientes ------------------------
INSERT INTO Paciente (IdPaciente,Nombre, Apellido, FechaNacimiento, Genero, Direccion, Telefono, IdHospital) VALUES 
('305310304','Yandari', 'Garita', '2001-04-3', 'F', 'Llanos Santa Lucía, Cartago', '7111-2222', 1);

INSERT INTO Paciente (IdPaciente,Nombre, Apellido, FechaNacimiento, Genero, Direccion, Telefono, IdHospital) VALUES 
('305670123', 'María', 'Rodríguez', '1985-03-15', 'F', 'Cartago Centro', '7222-3333', 1);

INSERT INTO Paciente (IdPaciente,Nombre, Apellido, FechaNacimiento, Genero, Direccion, Telefono, IdHospital) VALUES 
('309850456', 'Carlos', 'López', '1978-11-22', 'M', 'Cartago Oriental', '7333-4444', 1);

INSERT INTO Paciente (IdPaciente,Nombre, Apellido, FechaNacimiento, Genero, Direccion, Telefono, IdHospital) VALUES 
('302340789', 'Ana', 'Martínez', '1995-07-30', 'F', 'Cartago Occidental', '7444-5555', 1);

INSERT INTO Paciente (IdPaciente,Nombre, Apellido, FechaNacimiento, Genero, Direccion, Telefono, IdHospital) VALUES 
('306781234', 'Luis', 'Hernández', '1982-09-14', 'M', 'Tejar', '7555-6666', 1);

INSERT INTO Paciente (IdPaciente,Nombre, Apellido, FechaNacimiento, Genero, Direccion, Telefono, IdHospital) VALUES 
('118160452', 'Josué', 'Sánchez', '2001-07-08', 'M', 'San José Centro', '8501-6592', 2);

INSERT INTO Paciente (IdPaciente,Nombre, Apellido, FechaNacimiento, Genero, Direccion, Telefono, IdHospital) VALUES 
('104562890', 'Pedro', 'García', '1998-04-18', 'M', 'Rohmonser', '7777-8888', 2);

INSERT INTO Paciente (IdPaciente,Nombre, Apellido, FechaNacimiento, Genero, Direccion, Telefono, IdHospital) VALUES 
('101234567', 'Laura', 'Fernández', '1989-08-25', 'F', 'San José Oeste', '7888-9999', 2);

INSERT INTO Paciente (IdPaciente,Nombre, Apellido, FechaNacimiento, Genero, Direccion, Telefono, IdHospital) VALUES 
('103456789', 'Jorge', 'Díaz', '1965-01-30', 'M', 'Escazú Centro', '7999-0000', 2);

INSERT INTO Paciente (IdPaciente,Nombre, Apellido, FechaNacimiento, Genero, Direccion, Telefono, IdHospital) VALUES 
('102345678', 'Marta', 'Sánchez', '1970-06-12', 'F', 'Santa Ana', '7000-1111', 2);

--********************************** FIN PACIENTES **********************************--



------------------------ Insert Citas ------------------------
INSERT INTO Cita (Fecha, Hora, IdMedico, IdPaciente, IdHospital, Diagnostico) VALUES 
('2025-04-10', '08:00', 3, '305310304', 1, 'Chequeo general');

INSERT INTO Cita (Fecha, Hora, IdMedico, IdPaciente, IdHospital, Diagnostico) VALUES 
('2025-04-10', '09:00', 1, '118160452', 2, 'Consulta de rutina');

INSERT INTO Cita (Fecha, Hora, IdMedico, IdPaciente, IdHospital, Diagnostico) VALUES 
('2025-04-11', '10:30', 2, '104562890', 2, 'Control ginecológico');

INSERT INTO Cita (Fecha, Hora, IdMedico, IdPaciente, IdHospital, Diagnostico) VALUES 
('2025-04-11', '11:30', 4, '306781234', 1, 'Revisión dermatológica');

INSERT INTO Cita (Fecha, Hora, IdMedico, IdPaciente, IdHospital, Diagnostico) VALUES 
('2025-04-11', '16:30', 4, '306781234', 1, 'Revisión dermatológica');

--********************************** FIN CITAS **********************************--



------------------------ Insert Medicamentos ------------------------
INSERT INTO Medicamento (Nombre, Descripcion, CostoUnidad) VALUES 
('Paracetamol', 'Alivio del dolor y fiebre', 1000);

INSERT INTO Medicamento (Nombre, Descripcion, CostoUnidad) VALUES 
('Amoxicilina', 'Antibiótico penicilínico', 4000);

INSERT INTO Medicamento (Nombre, Descripcion, CostoUnidad) VALUES 
('Ibuprofeno', 'Antiinflamatorio no esteroideo', 1250);

INSERT INTO Medicamento (Nombre, Descripcion, CostoUnidad) VALUES 
('Loratadina', 'Antihistamínico para alergias', 3000);

--********************************** FIN MEDICAMENTOS **********************************--



------------------------ Insert stock Medicamentos ------------------------
INSERT INTO Medicamento_Hospital (IdHospital, IdMedicamento, CantidadDisponible) VALUES 
(1, 1, 200); -- Paracetamol en Max Peralta

INSERT INTO Medicamento_Hospital (IdHospital, IdMedicamento, CantidadDisponible) VALUES 
(1, 3, 150); -- Ibuprofeno en Max Peralta

INSERT INTO Medicamento_Hospital (IdHospital, IdMedicamento, CantidadDisponible) VALUES 
(2, 2, 180); -- Amoxicilina en San Juan

INSERT INTO Medicamento_Hospital (IdHospital, IdMedicamento, CantidadDisponible) VALUES 
(2, 4, 120); -- Loratadina en San Juan

--********************************** FIN STOCK MEDICAMENTOS **********************************--



------------------------ Insert tratamiento ------------------------
INSERT INTO Tratamiento (IdCita, CostoTotal) VALUES 
(1, 5000.00);

INSERT INTO Tratamiento (IdCita, CostoTotal) VALUES 
(2, 3000.00);

INSERT INTO Tratamiento (IdCita, CostoTotal) VALUES 
(3, 4500.00);

INSERT INTO Tratamiento (IdCita, CostoTotal) VALUES 
(4, 3500.00);

--********************************** FIN TRATAMIENTO **********************************--



------------------------ Insert Medicamentos aplicados en tratamientos ------------------------
INSERT INTO Tratamiento_Medicamento (IdTratamiento, IdMedicamento, Cantidad) VALUES 
(1, 1, 2); -- Paracetamol

INSERT INTO Tratamiento_Medicamento (IdTratamiento, IdMedicamento, Cantidad) VALUES 
(2, 2, 1); -- Amoxicilina

INSERT INTO Tratamiento_Medicamento (IdTratamiento, IdMedicamento, Cantidad) VALUES 
(3, 4, 2); -- Loratadina

INSERT INTO Tratamiento_Medicamento (IdTratamiento, IdMedicamento, Cantidad) VALUES 
(4, 3, 1); -- Ibuprofeno

--********************************** FIN TRATAMIENTOS APLICADOS **********************************--



------------------------ Insert Pagos ------------------------
INSERT INTO Pago (IdPaciente, IdTratamiento, Fecha, Monto, MetodoPago, Pagado) VALUES 
('305310304', 1, '2025-04-10', 5000.00, 'Efectivo', 1);

INSERT INTO Pago (IdPaciente, IdTratamiento, Fecha, Monto, MetodoPago, Pagado) VALUES 
('118160452', 2, '2025-04-10', 3000.00, 'Tarjeta', 1);

INSERT INTO Pago (IdPaciente, IdTratamiento, Fecha, Monto, MetodoPago, Pagado) VALUES 
('104562890', 3, '2025-04-11', 4500.00, 'Transferencia', 0);

INSERT INTO Pago (IdPaciente, IdTratamiento, Fecha, Monto, MetodoPago, Pagado) VALUES 
('306781234', 4, '2025-04-11', 3500.00, 'Efectivo', 1);

--********************************** FIN PAS **********************************--







/* +++++++++++++++++++++++++++++++++ FIN INSERTS +++++++++++++++++++++++++++++++++ */


/* ----------------------------------- PRUEBAS DE LOS SP ----------------------------------- */

/*
Ver pacientes atendidos por la Dra. Laura Ruiz (IdMedico = 2) en el Hospital San Juan de Dios (IdHospital = 2),
del 2025-04-01 al 2025-04-30:
*/
EXEC ObtenerPacientesPorMedico 
    @IdMedico = 2, 
    @IdHospital = 2, 
    @FechaInicio = '2025-04-01', 
    @FechaFin = '2025-04-30';



/*
Consultar inventario y prescripciones para el Hospital Max Peralta (IdHospital = 1):
*/
EXEC InventarioYPrescripciones @IdHospital = 1;



/*
Ver pagos pendientes
*/
EXEC PagosPendientesPorPaciente @IdPaciente = '104562890';-- PEDRO GARCÍA 104562890, ESTE SÍ DEBE AL

EXEC PagosPendientesPorPaciente @IdPaciente = '118160452';-- JOSUE SANCHEZ 118160452, ESTE NO DEBE NADA



/*
Registrar una cita con el Dr. Carlos Pérez (IdMedico = 3), 
para el paciente Ana Martínez (IdPaciente = '302340789') en el Hospital Max Peralta (IdHospital = 1):
*/
EXEC RegistrarNuevaCita 
    @Fecha = '2025-04-15', 
    @Hora = '14:00', 
    @IdMedico = 3, 
    @IdPaciente = '302340789', 
    @IdHospital = 1, 
    @Diagnostico = 'Revisión de presión arterial';



/*
Calcular total de pagos realizados por Josué Sánchez (IdPaciente = '118160452') entre el 2025-01-01 y 2025-04-30:
*/
EXEC TotalPagosRealizadosPorPaciente 
    @IdPaciente = '118160452',
    @FechaInicio = '2025-01-01',
    @FechaFin = '2025-04-30';

/* +++++++++++++++++++++++++++++++++ FIN PRUEBAS SP +++++++++++++++++++++++++++++++++ */

/* ----------------------------------- Recuperabilidad ----------------------------------- */
ALTER DATABASE SistemaHospitales SET RECOVERY FULL;
BACKUP DATABASE SistemaHospitales TO DISK = 'C:\SistemaHospitales_Backups\Backup.bak';

RESTORE DATABASE SistemaHospitales FROM DISK = 'C:\SistemaHospitales_Backups\Backup.bak' WITH RECOVERY;
/* +++++++++++++++++++++++++++++++++ FIN RECUPERABILIDAD +++++++++++++++++++++++++++++++++ */
