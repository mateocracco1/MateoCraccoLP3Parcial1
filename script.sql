--  creación
CREATE DATABASE tienda_online;
GO

USE tienda_online;
GO

-- categorias
CREATE TABLE categorias (
    idCategoria INT IDENTITY(1,1) PRIMARY KEY,
    descripcion VARCHAR(50) NOT NULL
);
GO

-- productos 
CREATE TABLE productos (
    idProducto INT IDENTITY(1,1) PRIMARY KEY,
    nombre VARCHAR(50) NOT NULL,
    precio DECIMAL(10, 2) NOT NULL,
    idCategoria INT NOT NULL,
    CONSTRAINT FK_productos_categorias FOREIGN KEY (idCategoria) 
        REFERENCES categorias(idCategoria)
);
GO

--  ejemplos de inserts 
INSERT INTO categorias (descripcion) VALUES ('Tecnología');
INSERT INTO categorias (descripcion) VALUES ('Hogar');
GO


INSERT INTO productos (nombre, precio, idCategoria) VALUES ('Teclado Mecánico', 45000.00, 1);
INSERT INTO productos (nombre, precio, idCategoria) VALUES ('Lámpara de Escritorio', 15000.00, 2);
GO