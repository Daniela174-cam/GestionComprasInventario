
CREATE TABLE TipoMovCaja (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(50),
    tipo VARCHAR(50)
);

CREATE TABLE MovCaja (
    id SERIAL PRIMARY KEY,
    fecha DATE,
    tipoMov_id INT REFERENCES TipoMovCaja(id),
    valor NUMERIC,
    concepto TEXT,
    tercero_id INT
);


CREATE TABLE Planes (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(30),
    fechaInicio DATE,
    fechaFin DATE,
    dcto NUMERIC
);


CREATE TABLE PlanProducto (
    plan_id INT REFERENCES Planes(id),
    producto_id INT,
    PRIMARY KEY (plan_id, producto_id)
);


CREATE TABLE Compras (
    id SERIAL PRIMARY KEY,
    terceroProv_id INT,
    fecha DATE,
    terceroEmp_id INT,
    docCompra VARCHAR(50)
);


CREATE TABLE Detalle_Compra (
    id SERIAL PRIMARY KEY,
    fecha DATE,
    producto_id INT,
    cantidad INT,
    valor NUMERIC,
    compra_id INT REFERENCES Compras(id)
);


CREATE TABLE Facturacion (
    id SERIAL PRIMARY KEY,
    fechaResolucion DATE,
    numInicio INT,
    numFinal INT,
    factActual INT
);


CREATE TABLE Venta (
    fact_id INT PRIMARY KEY,
    fecha DATE,
    terceroEm_id INT,
    terceroCli_id INT,
    fecha2 DATE
);


CREATE TABLE Detalle_Venta (
    id SERIAL PRIMARY KEY,
    fact_id INT REFERENCES Venta(fact_id),
    producto_id INT,
    cantidad INT,
    valor NUMERIC
);




CREATE TABLE Tipo_Documentos (
    id SERIAL PRIMARY KEY,
    descripcion VARCHAR(50)
);


CREATE TABLE Tipo_Terceros (
    id SERIAL PRIMARY KEY,
    descripcion VARCHAR(50)
);


CREATE TABLE Ciudad (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(50),
    region_id INT
);


CREATE TABLE Terceros (
    id VARCHAR(20) PRIMARY KEY,
    nombre VARCHAR(50),
    apellidos VARCHAR(50),
    email VARCHAR(80) UNIQUE,
    tipoDoc_id INT REFERENCES Tipo_Documentos(id),
    tipoTercero_id INT REFERENCES Tipo_Terceros(id),
    ciudad_id INT REFERENCES Ciudad(id)
);


CREATE TABLE Tercero_Telefonos (
    id SERIAL PRIMARY KEY,
    numero VARCHAR(20),
    tipo VARCHAR(20),
    tercero_id VARCHAR(20) REFERENCES Terceros(id)
);


CREATE TABLE Proveedor (
    id SERIAL PRIMARY KEY,
    tercero_id VARCHAR(20) REFERENCES Terceros(id),
    dcto DOUBLE PRECISION,
    diaPago INT
);


CREATE TABLE EPS (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(50)
);

CREATE TABLE ARL (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(50)
);


CREATE TABLE Empleado (
    id SERIAL PRIMARY KEY,
    tercero_id VARCHAR(20) REFERENCES Terceros(id),
    fechaIngreso DATE,
    salarioBase DOUBLE PRECISION,
    eps_id INT REFERENCES EPS(id),
    arl_id INT REFERENCES ARL(id)
);

CREATE TABLE Cliente (
    id SERIAL PRIMARY KEY,
    tercero_id VARCHAR(20) REFERENCES Terceros(id),
    fechaNac DATE,
    fechaUCompra DATE
);

CREATE TABLE Productos (
    id VARCHAR(20) PRIMARY KEY,
    nombre VARCHAR(50),
    stock INT,
    stockMin INT,
    stockMax INT,
    createdAt DATE,
    updatedAt DATE,
    barcode VARCHAR(50) UNIQUE
);

CREATE TABLE Productos_Proveedor (
    tercero_id VARCHAR(20) REFERENCES Terceros(id),
    producto_id VARCHAR(20) REFERENCES Productos(id),
    PRIMARY KEY (tercero_id, producto_id)
);



CREATE TABLE Pais (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(50)
);


CREATE TABLE Region (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(50),
    pais_id INT REFERENCES Pais(id)
);

CREATE TABLE Empresa (
    id VARCHAR(20) PRIMARY KEY,
    nombre VARCHAR(50),
    ciudad_id INT REFERENCES Ciudad(id),
    fecha_reg DATE
);
