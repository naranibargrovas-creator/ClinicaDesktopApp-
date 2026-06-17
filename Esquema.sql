-- Tabla Usaurio
CREATE TABLE Usuario (
    ID_Usuario INT PRIMARY KEY IDENTITY,
    Nombre NVARCHAR(20) NOT NULL,
    Apellidos NVARCHAR(50)NOT NULL,
    FechaNacimiento DATE,
    Telefono NVARCHAR(20) NOT NULL,
    Correo NVARCHAR(50)  UNIQUE,
    Rol NVARCHAR(15) NOT NULL,--administrdaor/ usuario normal
    ContrasenaHash NVARCHAR(500) NOT NULL,
    UltimoAcceso DATETIME ,
    Estado NVARCHAR(10) NOT NULL DEFAULT 'Activo',--usuario activo/inactivo
    Logueado BIT NOT NULL DEFAULT 0 --TRUE OR FALSE
   );

   -- Tabla Paciente
CREATE TABLE Paciente (
    ID_Paciente INT PRIMARY KEY IDENTITY,
    Nombre NVARCHAR(20) NOT NULL,
    Apellidos NVARCHAR(50) NOT NULL,
    FechaNacimiento DATE,
    Telefono NVARCHAR(20) NOT NULL,
    Correo NVARCHAR(50) NOT NULL,
    Sexo CHAR(1) NOT NULL,
    Direccion NVARCHAR(30) NOT NULL,
    TipoDoc NVARCHAR(20) NOT NULL,
    NumeroDoc NVARCHAR(20)  UNIQUE,
    FechaRegistro DATETIME DEFAULT GETDATE(),--cuando se registra el paciente en el istema
    UsuarioRegistrador NVARCHAR(100) NOT NULL,--el usuario que registra el paciente en el siistema
    FechaModificacion DATETIME DEFAULT GETDATE(),--fecha de modificacion del paciente en el sistema
    UsuarioModificador NVARCHAR(100) NOT NULL,-- usuario que modifica el paciente en el sistema
    Estado BIT NOT NULL DEFAULT 1--esta enferma recibe atencion,0 ya se dio de alta
   );

     -- Tabla Medico
CREATE TABLE Medico (
    ID_Medico INT PRIMARY KEY IDENTITY,
    Nombre  NVARCHAR(20) NOT NULL ,
    Apellidos  NVARCHAR(50) NOT NULL ,
    Correo  NVARCHAR(30) NOT NULL ,
    Direccion  NVARCHAR(50) NOT NULL ,
    FechaNacimiento date,
    Colegiatura NVARCHAR(20)  UNIQUE,
    DisponibilidadHorario NVARCHAR(20) NOT NULL ,
    Apodo  NVARCHAR(15) NOT NULL ,
    ContrasenaHash NVARCHAR(500) NOT NULL,
    TipoDoc  NVARCHAR(20) NOT NULL ,
    NumeroDoc  NVARCHAR(20) NOT NULL 
    );

    -- Tabla Cita
CREATE TABLE Cita (
    ID_Cita INT PRIMARY KEY IDENTITY,
    HoraInicio DATETIME default getdate(),
    HoraFin DATETIME,
    Motivo NVARCHAR(200) NOT NULL ,
    Estado NVARCHAR(20) NOT NULL DEFAULT 'Pendiente',
    CHECK (Estado IN ('Pendiente','Confirmada', 'Atendida','Cancelada', 'NoAsistio')),
    ID_Paciente INT NOT NULL  FOREIGN KEY REFERENCES Paciente(ID_Paciente),
    ID_Medico INT NOT NULL FOREIGN KEY REFERENCES Medico(ID_Medico),
    Id_Usuario INT NOT NULL  FOREIGN KEY REFERENCES Usuario(ID_Usuario)
);

-- Tabla ProveedorSeguro
CREATE TABLE ProveedorSeguro (
    ID_ProvSeguro INT NOT NULL  PRIMARY KEY IDENTITY,
    Nombre NVARCHAR(50) NOT NULL 
);


-- Tabla ProveeSeguroPaciente
CREATE TABLE ProveeSeguroPaciente (
    ID_Paciente INT NOT NULL,
    ID_ProvSeguro INT NOT NULL,
    FOREIGN KEY (ID_Paciente) REFERENCES Paciente(ID_Paciente),
    FOREIGN KEY (ID_ProvSeguro) REFERENCES ProveedorSeguro(ID_ProvSeguro),
    PRIMARY KEY(ID_Paciente, ID_ProvSeguro)
); 



-- Tabla Alergia
CREATE TABLE Alergia (
    ID_Alergia INT PRIMARY KEY IDENTITY,
    Descripcion NVARCHAR(200) NOT NULL,
    TipoAlergia NVARCHAR(30)NOT NULL,
    NivelRiesgo NVARCHAR(20) NOT NULL,
    Observaciones NVARCHAR(200),
    ID_Paciente INT FOREIGN KEY REFERENCES Paciente(ID_Paciente)
);



-- Tabla HistorialClinico
CREATE TABLE HistorialClinico (
    ID_HistorialClinico INT PRIMARY KEY IDENTITY,
    Observaciones NVARCHAR(100)NOT NULL,
    FechaCreacionHistorial DATETIME DEFAULT GETDATE(),
    Estado  NVARCHAR(100)NOT NULL,--
    Peso  DECIMAL(5,2),
    Altura DECIMAL(4,2),
    PresionArterial  NVARCHAR(100)NOT NULL,
    TipoSangre  NVARCHAR(5) NOT NULL,--O+/O-/A+/A-/B+/B-/AB+/AB-
    Antecedentes nvarchar(50) NOT NULL,
   -- FechaRegistro DATETIME DEFAULT GETDATE(),--cuando se registra el historiaalClinico en el istema
    UsuarioRegistrador NVARCHAR(100)NOT NULL,--el usuario que registra el historiaalClinico en el siistema
    FechaModificacion DATETIME DEFAULT GETDATE(),--fecha de modificacion del historiaalClinico en el sistema
    UsuarioModificador NVARCHAR(100)NOT NULL,-- usuario que modifica el historiaalClinico en el sistema
    ID_Cita INT NOT NULL FOREIGN KEY REFERENCES Cita(ID_Cita),
    ID_Paciente INT NOT NULL FOREIGN KEY REFERENCES Paciente(ID_Paciente),
   
);

-- Tabla Diagnostico
CREATE TABLE Diagnostico(
    ID_Diagnostico INT PRIMARY KEY IDENTITY,
    Descripcion NVARCHAR(200) NOT NULL,
    Fecha DATETIME  DEFAULT GETDATE(),
    CodigoCIE10 NVARCHAR(20) NOT NULL,--J00  Resfriado común/E11  Diabetes tipo 2/ I10  Hipertensión esencial
    Id_HistorialClinico INT NOT NULL FOREIGN KEY REFERENCES HistorialClinico(Id_HistorialClinico)
);

-- Tabla Tratamiento
CREATE TABLE Tratamiento (
    ID_Tratamiento INT PRIMARY KEY IDENTITY,
    TipoTratamiento NVARCHAR(50)NOT NULL,
    Descripcion NVARCHAR(50)NOT NULL,
    DuracionDias INT,
    FechaInicio DATETIME DEFAULT  GETDATE(),
    FechaFin DATETIME DEFAULT  GETDATE(),
    Indicaciones NVARCHAR(50)NOT NULL,
    Estado NVARCHAR(50)NOT NULL,
    FechaCreacion DATETIME DEFAULT GETDATE(),
    UsuarioCreador NVARCHAR(100)NOT NULL,
    FechaModificacion DATETIME DEFAULT GETDATE(),
    UsuarioModificador NVARCHAR(100)NOT NULL,
    ID_Diagnostico INT FOREIGN KEY REFERENCES Diagnostico(ID_Diagnostico)
);

-- Tabla CategoriaFarmacia
CREATE TABLE CategoriaFarmacia (
    ID_CategoriaFarmacia INT PRIMARY KEY IDENTITY,
    NombreCategoria NVARCHAR(100)NOT NULL,
    Presentacion NVARCHAR(100)NOT NULL,
    UnidadMedida NVARCHAR(50)NOT NULL,
    FechaVencimiento DATE NOT NULL
);

-- Tabla CategoriaReactivo
CREATE TABLE CategoriaReactivo (
    ID_CategoriaReactivo INT PRIMARY KEY IDENTITY,
    Nombre NVARCHAR(100) NOT NULL,
    TipoReactivo NVARCHAR(100)NOT NULL,
    CondicionesAlmacenamiento NVARCHAR(200)NOT NULL,
    Lote NVARCHAR(50) NOT NULL
);

 -- Tabla Insumo (Inventario)
CREATE TABLE Insumo (
    ID_Insumo INT PRIMARY KEY IDENTITY,
    Nombre NVARCHAR(100)NOT NULL,
    StockActual INT NOT NULL,
    StockMinimo INT NOT NULL,
    FechaEntradaAlmacen date NOT NULL,--cuando se ingresa producto fisico al alamcen
    FechaCreacionProducto  DATETIME default getdate() not null,--cuando llega un producto nuevo y se registra en el sistema
    UsuarioCreador NVARCHAR(20)NOT NULL,--el usuario que crea el producto en el siistema
    PrecioUnitario DECIMAL(10,2)NOT NULL,
    Codigo NVARCHAR(50) UNIQUE,
    Proveedor NVARCHAR(100) NOT NULL,--EL QUE ME PROVEE LOS INSUMOS
    --Estado NVARCHAR(100),--por definir
    FechaVencimiento DATE NOT NULL,
    FechaRegistro DATETIME DEFAULT GETDATE(),--cuando se registra el prodiucto en el istema
    UsuarioRegistrador NVARCHAR(100) NOT NULL,--el usuario que registra el producto en el siistema
    FechaModificacion DATETIME DEFAULT GETDATE(),--fecha de modificacion del producto en el sistema
    UsuarioModificador NVARCHAR(100) NOT NULL,-- usuario que modifica el producto en el sistema
    ID_CategoriaFarmacia INT NOT NULL FOREIGN KEY REFERENCES CategoriaFarmacia(ID_CategoriaFarmacia),
    ID_CategoriaReactivo INT NOT NULL FOREIGN KEY REFERENCES CategoriaReactivo(ID_CategoriaReactivo)
);

-- Tabla TratamientoInsumo
CREATE TABLE TratamientoInsumo(
    ID_TratamientoInsumo INT PRIMARY KEY IDENTITY,
    Dosis NVARCHAR(50)NOT NULL,
    Frecuencia NVARCHAR(100)NOT NULL,
    Cantidad FLOAT NOT NULL,
    ID_Tratamiento INT NOT NULL FOREIGN KEY REFERENCES Tratamiento(ID_Tratamiento),
    ID_Insumo INT NOT NULL FOREIGN KEY REFERENCES Insumo(ID_Insumo)
    );





-- Tabla Farmaceutico
CREATE TABLE Farmaceutico (
    ID_Farmaceutico INT PRIMARY KEY IDENTITY,
    Nombre NVARCHAR(20) NOT NULL,
    Apellidos NVARCHAR(50)NOT NULL,
    Correo NVARCHAR(30)NOT NULL,
    Direccion NVARCHAR(100)NOT NULL,
    ContrasenaHash NVARCHAR(500)NOT NULL,
    Apodo NVARCHAR(15)NOT NULL,
    FechaNacimiento DATE NOT NULL,
    Telefono NVARCHAR(20)NOT NULL,
    TipoDoc NVARCHAR(20)NOT NULL,
    NumeroDoc NVARCHAR(20)NOT NULL
);

--movimiento inventario

CREATE TABLE MovimientoInventario (
    ID_Movimiento INT PRIMARY KEY IDENTITY,
    TipoMovimiento NVARCHAR(50)NOT NULL,
    CantidadEntregada DECIMAL(10,2)NOT NULL,
    StockAnterior FLOAT NOT NULL,
    StockNuevo FLOAT NOT NULL,
    Fecha DATE NOT NULL,
    Motivo NVARCHAR(50) NOT NULL, --cunado hay cambios en el stok
    ID_Insumo INT NOT NULL FOREIGN KEY REFERENCES Insumo(ID_Insumo),
    ID_Farmaceutico INT NOT NULL FOREIGN KEY REFERENCES Farmaceutico(ID_Farmaceutico)
);

-- Tabla Laboratorista
CREATE TABLE Laboratorista (
    ID_Lab INT PRIMARY KEY IDENTITY,
    Nombre  NVARCHAR(20)NOT NULL,
    Apellidos  NVARCHAR(50)NOT NULL,
    Correo  NVARCHAR(30)NOT NULL,
    Direccion  NVARCHAR(50)NOT NULL,
    FechaNacimiento date NOT NULL,
    Apodo  NVARCHAR(15) NOT NULL,
    ContrasenaHash NVARCHAR(500)NOT NULL,
    TipoDoc  NVARCHAR(20)NOT NULL,
    NumeroDoc  NVARCHAR(20)NOT NULL
    );




-- Tabla OrdenLaboratorio
CREATE TABLE OrdenLaboratorio (
    ID_OrdenLab INT PRIMARY KEY IDENTITY,
    TipoExamen NVARCHAR(100)NOT NULL,
    FechaEmision DATETIME DEFAULT getdate(),
    Estado NVARCHAR(20)NOT NULL,--atendido,por atender
    ID_Lab INT NULL FOREIGN KEY REFERENCES Laboratorista(ID_Lab),
    ID_Medico INT NOT NULL FOREIGN KEY REFERENCES Medico(ID_Medico),
    ID_Paciente INT NOT NULL FOREIGN KEY REFERENCES Paciente(ID_Paciente)
);


-- Tabla ResultadoLaboratorio
CREATE TABLE ResultadoLaboratorio (
    ID_ResultadoLaboratorio INT PRIMARY KEY IDENTITY,
    ExamenRealizado NVARCHAR(100)NOT NULL,
    ValorObtenido NVARCHAR(100)NOT NULL,
    Firma NVARCHAR(20) NOT NULL,
    Fecha DATE NOT NULL ,
    RutaDocumento NVARCHAR(100)NOT NULL,
    EstadoResultado NVARCHAR(100)NOT NULL,
    FechaCreacion DATETIME DEFAULT GETDATE(),
    UsuarioCreador NVARCHAR(100)NOT NULL,
    FechaModificacion DATETIME DEFAULT GETDATE(),
    UsuarioModificador NVARCHAR(50)NOT NULL,
    ID_OrdenLab INT FOREIGN KEY REFERENCES OrdenLaboratorio(ID_OrdenLab)
);


-- Tabla DetalleResultado
CREATE TABLE DetalleResultado (
    ID_DetalleResultado INT PRIMARY KEY IDENTITY,
    Detalle  NVARCHAR(200) NOT NULL,
    Observaciones NVARCHAR(200)NOT NULL,
    ID_ResultadoLaboratorio INT NOT NULL FOREIGN KEY REFERENCES ResultadoLaboratorio(ID_ResultadoLaboratorio)
    );

-- Tabla LabInsumo
CREATE TABLE LabInsumo(
    ID_OrdenLab INT NOT NULL FOREIGN KEY REFERENCES OrdenLaboratorio(Id_OrdenLab),
    ID_Movimiento INT NOT NULL FOREIGN KEY REFERENCES MovimientoInventario(ID_Movimiento),
    CantidadPedida DECIMAL(10,2)NOT NULL,
    PRIMARY KEY(ID_OrdenLab, ID_Movimiento)
);


   
 -- Tabla Especialidad
CREATE TABLE Especialidad (
    ID_Especialidad INT PRIMARY KEY IDENTITY,
    NombreEspecialidad NVARCHAR(50)NOT NULL,
    Descripcion  NVARCHAR(200)NOT NULL,
    Estado BIT DEFAULT 0 NOT NULL
    );

 -- Tabla MedicoEspecialidad
CREATE TABLE MedicoEspecialidad (
   FechaAsignacion DATETIME DEFAULT getdate(),
   Es_jefe BIT DEFAULT 0 NOT NULL,
   ID_Especialidad INT NOT NULL FOREIGN KEY REFERENCES Especialidad(ID_Especialidad),
   ID_Medico INT NOT NULL FOREIGN KEY REFERENCES Medico(ID_Medico),
   PRIMARY KEY(ID_Medico, ID_Especialidad)
   );